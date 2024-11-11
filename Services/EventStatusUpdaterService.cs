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
                if (TryParseDate(evt.StartDate, out DateTime parsedStartDate) &&
                    TryParseDate(evt.EndDate, out DateTime parsedEndDate))
                {
                    string newStatus = DetermineEventStatus(parsedStartDate, parsedEndDate);
                    if (evt.Status != newStatus)
                    {
                        evt.Status = newStatus;
                        await _eventRepository.UpdateAsync(evt); // Consider batch updating if this operation becomes slow for many events
                    }
                }
                else
                {
                    // Optionally log or handle the scenario where date parsing fails
                    Console.WriteLine($"Failed to parse dates for event: {evt.EventId}");
                }
            }
        }

        private string DetermineEventStatus(DateTime startDate, DateTime endDate)
        {
            var now = DateTime.UtcNow;
            if (now < startDate)
            {
                return "Upcoming";
            }
            else if (now >= startDate && now <= endDate)
            {
                return "In Progress";
            }
            else
            {
                return "Closed";
            }
        }

        private bool TryParseDate(string dateString, out DateTime date)
        {
            // Adjust date parsing logic to fit your expected date format
            return DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
    }
}
