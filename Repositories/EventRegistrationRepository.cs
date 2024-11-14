using Microsoft.EntityFrameworkCore;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;

namespace SeleafAPI.Repositories;

public class EventRegistrationRepository : IEventRegistration
{
       private readonly AppDbContext _context;
        public EventRegistrationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventRegistration>> GetEventRegistrationsAsync()
        {
            var eventRegistration = await _context.EventRegistrations
                .Include(d => d.User)
                .ToListAsync();

            return eventRegistration;
        }
        
        public async Task<List<EventRegistration>> GetPaginatedEventRegistrations(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
    
            
            int skip = (pageNumber - 1) * pageSize;

            
            return await _context.EventRegistrations
                .OrderBy(d => d.RegistrationDate).Include(d => d.User)
                .Include(d => d.Event)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> UserAlreadyRegisteredForEvent(string userId, int eventId)
        {
            var user = await _context.EventRegistrations.
                Where(e => e.UserId == userId).
                Where(e => e.EventId == eventId).
                Where(e => e.IsPaid == true)
                .FirstOrDefaultAsync();

            if (user is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<EventRegistration> GetEventRegistrationsById(int id)
        {
            var eventRegistration = await _context.EventRegistrations.FindAsync(id);
            if (eventRegistration == null)
            {
                return null;
            }
            return eventRegistration;
        }

        public async Task<EventRegistration> CreateEventRegistrationsAsync(EventRegistration eventRegistration)
        {
            await _context.EventRegistrations.AddAsync(eventRegistration);
            await _context.SaveChangesAsync();
            return eventRegistration;
        }

        public async Task<EventRegistration> GetEventRegistrationsByPaymentIdAsync(string paymentId)
        {
            return await _context.EventRegistrations
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.PaymentId == paymentId);
        }

        public async Task UpdateEventRegistrationsStatusAsync(string paymentId, bool isPaid)
        {
            var EventRegistrations = await GetEventRegistrationsByPaymentIdAsync(paymentId);
            if (EventRegistrations != null)
            {
                EventRegistrations.IsPaid = isPaid;
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<int> GetTotalEventRegistrationsCountAsync()
        {
            return await _context.EventRegistrations.CountAsync(); 
        }
}