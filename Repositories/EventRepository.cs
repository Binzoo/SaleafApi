using Microsoft.EntityFrameworkCore;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;

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
            .Where(e => e.Status == "Upcoming")
            .ToListAsync();
    }

    public async Task<List<Event>> GetAdminEvent()
    {
        return await _context.Events
            .ToListAsync();
    }
}