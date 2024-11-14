using SeleafAPI.Models;

namespace SeleafAPI.Interfaces;

public interface IEventRegistration
{
    Task<EventRegistration> CreateEventRegistrationsAsync(EventRegistration donation);
    Task<List<EventRegistration>> GetEventRegistrationsAsync();
    Task<EventRegistration> GetEventRegistrationsByPaymentIdAsync(string paymentId);
    Task UpdateEventRegistrationsStatusAsync(string paymentId, bool isPaid);
    Task<EventRegistration> GetEventRegistrationsById(int id);
    Task<int> GetTotalEventRegistrationsCountAsync();
    Task<List<EventRegistration>> GetPaginatedEventRegistrations(int pageNumber, int pageSize);
    Task<bool> UserAlreadyRegisteredForEvent(string userId, int eventId);
}