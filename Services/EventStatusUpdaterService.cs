using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SeleafAPI.Services
{

        public class EventStatusUpdaterService
        {
            private readonly IRepository<Event> _eventRepository;
            public EventStatusUpdaterService(IRepository<Event> eventRepository)
            {
                _eventRepository = eventRepository;
            }
            public async Task UpdateEventStatuses()
            {
                var events = await _eventRepository.GetAllAsync();
                foreach (var evt in events)
                {
                    if (DateTime.TryParseExact(evt.StartDate, "dd MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStartDate) &&
                        DateTime.TryParseExact(evt.EndDate, "dd MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedEndDate))
                    {
                        string newStatus = DetermineEventStatus(parsedStartDate, parsedEndDate);
                        if (evt.Status != newStatus)
                        {
                            evt.Status = newStatus;
                            await _eventRepository.UpdateAsync(evt);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to parse dates for event: {evt.EventId}");
                    }
                }
            }

            private string DetermineEventStatus(DateTime startDate, DateTime endDate)
            {
                var now = DateTime.UtcNow;
                return now < startDate ? "Upcoming" :
                    now <= endDate ? "In Progress" : "Closed";
            }
        }
}
