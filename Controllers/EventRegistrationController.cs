using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Controllers;



[ApiController]
[Route("[controller]")]
public class EventRegistrationController : ControllerBase
{
    private readonly IPayment _payment;
    private readonly IEventRegistration _eventRegistration;
    private readonly IEvent _event;

    public EventRegistrationController(AppDbContext context, IPayment payment, IEventRegistration eventRegistration, IEvent __event)
    {
        _payment = payment;
        _eventRegistration = eventRegistration;
        _event = __event;
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EventRegistration>> RegisterEvent(EventRegistrationDTO model)
    {
        var userId = User.FindFirst("userId")?.Value;

        // Fetch the event and its packages
        var chosenEvent = await _event.GetEventById(model.EventId);

        if (chosenEvent == null)
        {
            return NotFound(new { Status = "Error", Message = "Event not found." });
        }

        
        // Check if the user has already registered for this package
        bool userRegistered = await _eventRegistration.UserAlreadyRegisteredForPackage(userId!, model.EventId, model.PackageName);

        if (userRegistered)
        {
            return BadRequest(new 
            {
                Status = "Error",
                Message = "You have already registered for this package."
            });
        }
        
        // Create a new EventRegistration
        var eventReg = new EventRegistration
        {
            UserId = userId,
            FristName = model.FristName,
            LastName = model.LastName,
            NumberOfParticipant = model.NumberOfParticipant,
            PhoneNumber = model.PhoneNumber,
            EventId = model.EventId,
            PacakageName = model.PackageName,
            Amount = model.Amount,
            IsPaid = false,
            Currency = model.Currency
        };

        try
        {
            // Initiate payment
            var redirectUrl = await _payment.InitiateCheckoutAsyncEvent(
                await _eventRegistration.CreateEventRegistrationsAsync(eventReg),
                model.CancelUrl!, model.SuccessUrl!, model.FailureUrl!
            );
            return Ok(new { redirectUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing the registration: {ex.Message}");
        }
    }

    
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetDonations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var totalItems = await _eventRegistration.GetTotalEventRegistrationsCountAsync(); 

        var donations = await _eventRegistration.GetPaginatedEventRegistrations(pageNumber, pageSize); 

        var model = donations.Select(d => new
        {
            UserName = d.User?.UserName ?? "Unknown",
            FirstName = d.User?.FirstName ?? "Unknown",
            LastName = d.User?.LastName ?? "Unknown",
            d.PaymentId,
            d.Event.EventName,
            d.RegistrationDate,
            d.IsPaid,
        }).ToList();

        var response = new
        {
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
            Data = model
        };

        return Ok(response);
    }
    
    
}