using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Controllers;
[Route("api/[controller]")] 
[ApiController]
public class EventController : ControllerBase
{
    private readonly IRepository<Event> _repository;
    private readonly IEvent _eventRepo;
    
    public EventController(IRepository<Event> repository, IEvent eventRepo)
    {
        _repository = repository;
        _eventRepo = eventRepo;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] EventDTO eventDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (eventDto.EndDate < eventDto.StartDate)
        {
            return BadRequest("End date cannot be earlier than start date.");
        }
        
        string status = "";
        if (DateTime.UtcNow < eventDto.StartDate)
        {
            status = "Upcoming";
        }
        else if (DateTime.UtcNow >= eventDto.StartDate && DateTime.UtcNow <= eventDto.EndDate)
        {
            status = "In Progress";
        }
        else
        {
            status = "Closed";
        }
        
        var newEvent = new Event()
        {
            EventName = eventDto.EventName,
            EventDescription = eventDto.EventDescription,   
            StartDate =  eventDto.StartDate,
            EndDate = eventDto.EndDate,
            Status = status,
            EventPrice = eventDto.EventPrice,   
            Publish = eventDto.Publish
        };
        
        await _repository.AddAsync(newEvent);
        return Ok(new
        {
            message = "Event created successfully"
        });
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateEvent([FromBody] EventDTO eventDto, int id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (eventDto.EndDate < eventDto.StartDate)
            {
                return BadRequest("End date cannot be earlier than start date.");
            }

            var exist = await _repository.GetByIdAsync(id);

            if (exist == null)
            {
                return BadRequest("Event does not exist.");
            }

            string status = "";

            if (DateTime.UtcNow < eventDto.StartDate)
            {
                status = "Upcoming";
            }
            else if (DateTime.UtcNow >= eventDto.StartDate && DateTime.UtcNow <= eventDto.EndDate)
            {
                status = "In Progress";
            }
            else
            {
                status = "Closed";
            }

            exist.EventName = eventDto.EventName;
            exist.EventDescription = eventDto.EventDescription;
            exist.StartDate = eventDto.StartDate;
            exist.EndDate = eventDto.EndDate;
            exist.Status = status;
            exist.EventPrice = eventDto.EventPrice;
            exist.Publish = eventDto.Publish;

            await _repository.UpdateAsync(exist);
            return Ok(new
            {
                message = "Event updated successfully"
            });
        }
        catch (Exception e)
        {
            return BadRequest("An error occured while updating the event.");
        }
    }

    [HttpGet("userevents")]
    public async Task<IActionResult> GetUserEvent()
    {
        var events = await _eventRepo.GetUserEvent();
        return Ok(events);
    }
    
    [HttpGet("adminevents")]
    public async Task<IActionResult> GetAdminEvents()
    {
        var events = await _eventRepo.GetAdminEvent();
        return Ok(events);
    }
}