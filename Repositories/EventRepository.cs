using Microsoft.EntityFrameworkCore;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Repositories;

public class EventRepository : IEvent
{
    private readonly AppDbContext _context;

    public EventRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Event>> GetUserEvent()
    {
        return await _context.Events
            .Where(e => e.Status == "Upcoming" && e.Publish == true).Include(e => e.Packages)
            .ToListAsync();
    }



    public async Task<List<EventWithPackagesDTO>> GetAdminEvent()
    {
        return await _context.Events
            .Select(e => new EventWithPackagesDTO
            {
                EventId = e.EventId,
                EventName = e.EventName,
                EventDescription = e.EventDescription,
                Location = e.Location,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Publish = e.Publish,
                EventImageUrl = e.EventImageUrl,
                Status = e.Status,
                Packages = e.Packages.Select(p => new PackageDTO
                {
                    PackageName = p.PackageName,
                    PackagePrice = p.PackagePrice
                }).ToList()
            })
            .ToListAsync();
    }


    public async Task<List<EventWithPackagesDTO>> GetThreeLatestEvent()
    {
        return await _context.Events
            .Where(e => e.Status == "Upcoming")
            .OrderBy(e => e.StartDate) // Optional: Ensure the latest events are in order
            .Take(3)
            .Select(e => new EventWithPackagesDTO
            {
                EventId = e.EventId,
                EventName = e.EventName,
                EventDescription = e.EventDescription,
                Location = e.Location,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Publish = e.Publish,
                EventImageUrl = e.EventImageUrl,
                Status = e.Status,
                Packages = e.Packages.Select(p => new PackageDTO
                {
                    PackageName = p.PackageName,
                    PackagePrice = p.PackagePrice
                }).ToList()
            })
            .ToListAsync();
    }


    public async Task<Event> GetEventById(int id)
    {
        var eventbyid = await _context.Events
            .Include(e => e.Packages)
            .Where(e => e.EventId == id).FirstOrDefaultAsync();
        if (eventbyid == null)
        {
            return null;
        }
        return eventbyid;
    }
}