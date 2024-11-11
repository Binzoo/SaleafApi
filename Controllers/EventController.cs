using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleafApi.Interfaces;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;
using System.Globalization;
using System.Threading.Tasks;
using System;

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

        public EventController(IRepository<Event> repository, IEvent eventRepo, IS3Service S3Service, IConfiguration configuration)
        {
            _repository = repository;
            _eventRepo = eventRepo;
            _S3Service = S3Service;
            _awsRegion = configuration["AWS_REGION"];
            _bucketName = configuration["AWS_BUCKET_NAME"];
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] EventDTO eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate that EndDateTime is not earlier than StartDateTime
            if (eventDto.EndDateTime < eventDto.StartDateTime)
            {
                return BadRequest("End date and time cannot be earlier than start date and time.");
            }

            // Determine event status based on StartDateTime and EndDateTime
            string status = DetermineEventStatus(eventDto.StartDateTime, eventDto.EndDateTime);

            string? eventImageUrl = null;

            // Handle file upload if provided
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

                    // Construct the URL for the uploaded image
                    eventImageUrl = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileName}";
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred while uploading the image: {ex.Message}");
                }
            }

            string formattedStartDate = eventDto.StartDateTime.ToString("dd MMMM yyyy");
            string formattedEndDate = eventDto.EndDateTime.ToString("dd MMMM yyyy");

            string startTime = eventDto.StartDateTime.ToString("HH:mm");
            string endTime = eventDto.EndDateTime.ToString("HH:mm");

            // Create and populate the Event entity
            var newEvent = new Event
            {
                EventName = eventDto.EventName,
                EventDescription = eventDto.EventDescription,
                Location = eventDto.Location,
                StartDate = formattedStartDate,
                EndDate = formattedEndDate,
                StartTime = startTime,
                EndTime = endTime,
                Status = status,
                EventPrice = eventDto.EventPrice,
                Publish = eventDto.Publish,
                EventImageUrl = eventImageUrl
            };

            // Save the new event to the database
            await _repository.AddAsync(newEvent);

            return Ok(new
            {
                message = "Event created successfully",
                Event = newEvent
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent([FromForm] EventDTO eventDto, int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate that EndDateTime is not earlier than StartDateTime
                if (eventDto.EndDateTime < eventDto.StartDateTime)
                {
                    return BadRequest("End date and time cannot be earlier than start date and time.");
                }

                // Retrieve the existing event
                var exist = await _repository.GetByIdAsync(id);

                if (exist == null)
                {
                    return NotFound("Event does not exist.");
                }

                // Determine event status based on StartDateTime and EndDateTime
                string status = DetermineEventStatus(eventDto.StartDateTime, eventDto.EndDateTime);

                // Handle file upload if provided
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

                        // Construct the URL for the uploaded image
                        var eventImageUrl = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileName}";
                        exist.EventImageUrl = eventImageUrl;
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"An error occurred while uploading the image: {ex.Message}");
                    }
                }
                string formattedStartDate = eventDto.StartDateTime.ToString("dd MMMM yyyy");
                string formattedEndDate = eventDto.EndDateTime.ToString("dd MMMM yyyy");

                string startTime = eventDto.StartDateTime.ToString("HH:mm");
                string endTime = eventDto.EndDateTime.ToString("HH:mm");

                // Update event properties
                exist.EventName = eventDto.EventName;
                exist.EventDescription = eventDto.EventDescription;
                exist.Location = eventDto.Location;
                exist.StartDate = formattedStartDate;
                exist.EndDate = formattedEndDate;
                exist.StartTime = startTime;
                exist.EndTime = endTime;
                exist.Status = status;
                exist.EventPrice = eventDto.EventPrice;
                exist.Publish = eventDto.Publish;

                // Save changes to the database
                await _repository.UpdateAsync(exist);
                return Ok(new
                {
                    message = "Event updated successfully"
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while updating the event: {e.Message}");
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
