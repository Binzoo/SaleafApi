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
    private readonly AppDbContext _context;
    private readonly IPayment _payment;
    private readonly IEventRegistration _eventRegistration;

    public EventRegistrationController(AppDbContext context, IPayment payment, IEventRegistration eventRegistration)
    {
        _context = context;
        _payment = payment;
        _eventRegistration = eventRegistration;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EventRegistration>> RegisterEvent(EventRegistrationDTO model)
    {
        var userId = User.FindFirst("userId")?.Value;
        var eventReg = new EventRegistration()
        {
            UserId = userId,
            EventId = model.EventId,
            IsPaid = false
        };
        try
        {
            var redirectUrl = await _payment.InitiateCheckoutAsyncEvent(await _eventRegistration.CreateEventRegistrationsAsync(eventReg),
                    model.CancelUrl!, model.SuccessUrl!, model.FailureUrl!);
            return Ok(new { redirectUrl });
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Error processing the Event: {ex.Message}");
        }
    }
    
    
}