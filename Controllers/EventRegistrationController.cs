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

    public EventRegistrationController(AppDbContext context, IPayment payment)
    {
        _context = context;
        _payment = payment; 
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EventRegistration>> RegisterEvent(EventRegistrationDTO model)
    {
        var userId = User.FindFirst("userId")?.Value;
        var eventReg = new EventRegistration()
        {
            UserId = userId,
            EventId = model.EventId
        };
        
        await _context.EventRegistrations.AddAsync(eventReg);
        await _context.SaveChangesAsync();
        
        return Ok();
    }
    
    
}