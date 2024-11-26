using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleafApi.Interfaces;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IRepository<Event> _repository;
        private readonly IEvent _eventRepo;
        private readonly IS3Service _S3Service;
        private readonly string _awsRegion;
        private readonly string _bucketName;
        public readonly AppDbContext _context; 

        public EventController(IRepository<Event> repository, IEvent eventRepo, IS3Service S3Service, IConfiguration configuration ,  AppDbContext context)
        {
            _repository = repository;
            _eventRepo = eventRepo;
            _S3Service = S3Service;
            _awsRegion = configuration["AWS_REGION"];
            _bucketName = configuration["AWS_BUCKET_NAME"];
            _context = context;
        }
            
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] EventDTO eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (eventDto.EndDateTime < eventDto.StartDateTime)
            {
                return BadRequest("End date and time cannot be earlier than start date and time.");
            }

            string status = DetermineEventStatus(eventDto.StartDateTime, eventDto.EndDateTime);

            string? eventImageUrl = null;

            if (eventDto.EventImageFile != null && eventDto.EventImageFile.Length > 0)
            {
                var fileName = $"events/{Guid.NewGuid()}-{eventDto.EventImageFile.FileName}";
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await eventDto.EventImageFile.CopyToAsync(memoryStream);
                        await _S3Service.UploadFileAsync(memoryStream, fileName);
                    }
                    eventImageUrl = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileName}";
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred while uploading the image: {ex.Message}");
                }
            }

            var newEvent = new Event
            {
                EventName = eventDto.EventName,
                EventDescription = eventDto.EventDescription,
                Location = eventDto.Location,
                StartDate = eventDto.StartDateTime.ToString("dd MMMM yyyy"),
                EndDate = eventDto.EndDateTime.ToString("dd MMMM yyyy"),
                StartTime = eventDto.StartDateTime.ToString("HH:mm"),
                EndTime = eventDto.EndDateTime.ToString("HH:mm"),
                Status = status,
                Publish = eventDto.Publish,
                EventImageUrl = eventImageUrl,
                Capacity = eventDto.Capacity,  
                Packages = eventDto.Packages.Select(p => new Package
                {
                    PackageName = p.PackageName,
                    PackagePrice = p.PackagePrice,
                }).ToList()
            };

            await _repository.AddAsync(newEvent);

            return Ok(new { message = "Event created successfully", Event = newEvent });
        }


      [Authorize(Roles = "Admin")]
[HttpPut("{id}")]
public async Task<IActionResult> UpdateEvent([FromForm] EventDTO eventDto, int id)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var existingEvent = await _repository.GetByIdAsync(id);
    if (existingEvent == null)
    {
        return NotFound("Event does not exist.");
    }

    if (eventDto.EndDateTime < eventDto.StartDateTime)
    {
        return BadRequest("End date and time cannot be earlier than start date and time.");
    }

    string status = DetermineEventStatus(eventDto.StartDateTime, eventDto.EndDateTime);

    if (eventDto.EventImageFile != null && eventDto.EventImageFile.Length > 0)
    {
        var fileName = $"events/{Guid.NewGuid()}-{eventDto.EventImageFile.FileName}";
        try
        {
            using (var memoryStream = new MemoryStream())
            {
                await eventDto.EventImageFile.CopyToAsync(memoryStream);
                await _S3Service.UploadFileAsync(memoryStream, fileName);
            }
            existingEvent.EventImageUrl = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileName}";
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while uploading the image: {ex.Message}");
        }
    }

    existingEvent.EventName = eventDto.EventName;
    existingEvent.EventDescription = eventDto.EventDescription;
    existingEvent.Location = eventDto.Location;
    existingEvent.StartDate = eventDto.StartDateTime.ToString("dd MMMM yyyy");
    existingEvent.EndDate = eventDto.EndDateTime.ToString("dd MMMM yyyy");
    existingEvent.StartTime = eventDto.StartDateTime.ToString("HH:mm");
    existingEvent.EndTime = eventDto.EndDateTime.ToString("HH:mm");
    existingEvent.Status = status;
    existingEvent.Publish = eventDto.Publish;
    existingEvent.Capacity = eventDto.Capacity;

    // Remove all existing packages for the event
    var packagesToRemove = _context.Packages.Where(p => p.EventId == id).ToList();
    _context.Packages.RemoveRange(packagesToRemove);
    await _context.SaveChangesAsync();

    // Add new packages from DTO
    foreach (var packageDto in eventDto.Packages)
    {
        existingEvent.Packages.Add(new Package
        {
            PackageName = packageDto.PackageName,
            PackagePrice = packageDto.PackagePrice
        });
    }

    await _repository.UpdateAsync(existingEvent);

    return Ok(new { message = "Event updated successfully", Event = existingEvent });
}




        
        [HttpGet("get-three-latest-events")]
        public async Task<IActionResult> ThreeLatestEvents()
        {
            var threeEvents = await _eventRepo.GetThreeLatestEvent();
            return Ok(threeEvents);
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

        private string DetermineEventStatus(DateTime eventStartDateTime, DateTime eventEndDateTime)
        {
            var now = DateTime.UtcNow;
            if (now < eventStartDateTime)
            {
                return "Upcoming";
            }
            else if (now >= eventStartDateTime && now <= eventEndDateTime)
            {
                return "In Progress";
            }
            else
            {
                return "Closed";
            }
        }
    }
}
