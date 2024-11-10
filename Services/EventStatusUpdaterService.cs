using SeleafAPI.Interfaces;
using SeleafAPI.Models;

namespace SeleafAPI.Services;

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
            string newStatus = DetermineEventStatus(evt.StartDate, evt.EndDate);
            if (evt.Status != newStatus)
            {
                evt.Status = newStatus;
                await _eventRepository.UpdateAsync(evt);
            }
        }
    }
    private string DetermineEventStatus(DateTime startDate, DateTime endDate)
    {
        var now = DateTime.UtcNow;
        if (now < startDate)
        {
            return "Upcoming";
        }else if (now >= startDate && now <= endDate)
        {
            return "In Progress";
        }
        else
        {
            return "Closed";
        }
    }

}