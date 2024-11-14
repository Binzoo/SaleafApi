using SeleafAPI.Models;

namespace SeleafAPI.Interfaces;

public interface IEvent 
{
    Task<List<Event>> GetUserEvent();
    Task<List<Event>> GetAdminEvent();
    
    Task<List<Event>> GetThreeLatestEvent();
    Task<Event> GetEventById(int id);
}