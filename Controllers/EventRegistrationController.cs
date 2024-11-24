using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SeleafAPI.Data;
using SeleafAPI.Helper;
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
    private readonly AppDbContext _context;

    public EventRegistrationController(AppDbContext context, IPayment payment, IEventRegistration eventRegistration, IEvent __event)
    {
        _payment = payment;
        _eventRegistration = eventRegistration;
        _event = __event;
        _context = context;
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EventRegistration>> RegisterEvent(EventRegistrationDTO model)
    {
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var userId = User.FindFirst("userId")?.Value;

    // Begin a transaction to ensure atomic operations
  
        try
        {
            // Fetch the event with an exclusive lock to prevent concurrent modifications
            var chosenEvent = await _context.Events
                                            .Where(e => e.EventId == model.EventId)
                                            .FirstOrDefaultAsync();

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

            // Check event capacity if it's set
            if (chosenEvent.Capacity.HasValue)
            {
                if (chosenEvent.Capacity.Value < model.NumberOfParticipant)
                {
                    return BadRequest(new 
                    {
                        Status = "Error",
                        Message = $"Cannot register {model.NumberOfParticipant} participants. Only {chosenEvent.Capacity.Value} slots remaining."
                    });
                }

                // Decrement the capacity
                chosenEvent.Capacity -= model.NumberOfParticipant;

                // Update the event in the context
                _context.Events.Update(chosenEvent);
            }

            // Create a new EventRegistration
            var eventReg = new EventRegistration
            {
                UserId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                NumberOfParticipant = model.NumberOfParticipant,
                PhoneNumber = model.PhoneNumber,
                EventId = model.EventId,
                PacakageName = model.PackageName,
                Amount = model.Amount,
                IsPaid = false,
                Currency = model.Currency
            };

            // Add the registration to the context
            _context.EventRegistrations.Add(eventReg);

            // Save changes within the transaction
            await _context.SaveChangesAsync();
            
            // Initiate payment
            var redirectUrl = await _payment.InitiateCheckoutAsyncEvent(
                eventReg,
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
            d.PacakageName,
            d.Amount,
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

    [Authorize]
    [HttpGet("get-logged-register-event")]
    public async Task<IActionResult> GetRegisteredEvent()
    {
       var userid = User.FindFirst("userId")?.Value;
       var eventRegistartionOfUser = await _eventRegistration.GetAllEventRegistrationsByUserAsync(userid);
       return Ok(eventRegistartionOfUser);
    }

    [Authorize]
    [HttpGet("generate-qr-code")]
    public async Task<IActionResult> GenerateQRCodeForEventRegistration([FromQuery] int eventId)
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;
            var eventRegistrationOfUser =
                await _eventRegistration.GetEventRegistrationByUserIdEventIdAsync(eventId, userId);
            if (eventRegistrationOfUser == null)
            {
                return NotFound("Event registration not found.");
            }

            //string data = $"/EventRegistration/verify-ticket/{eventRegistrationOfUser.Id}";
            string data = EncryptionHelper.EncryptString(eventRegistrationOfUser.Id);
            byte[] qrCodeBytes;
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                qrCodeBytes = qrCode.GetGraphic(20);
            }

            return File(qrCodeBytes, "image/png");
        }
        catch(Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }
    
    
    [HttpGet("verify-ticket/{encryptText}")]
    public async Task<IActionResult> VerifyTask(string encryptText)
    {
        try
        {
            string decodedText = Uri.UnescapeDataString(encryptText);

            int id = int.Parse(EncryptionHelper.DecryptString(decodedText));
            var eventreg = await _eventRegistration.GetEventRegistrationsById(id);
            return Ok(eventreg);
        }
        catch (FormatException)
        {
            return BadRequest("The input is not a valid Base-64 string.");
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
        
    }
    
}