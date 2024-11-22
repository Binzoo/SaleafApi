using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Interfaces;

public interface IEvent 
{
    Task<List<Event>> GetUserEvent();
    Task<List<EventWithPackagesDTO>> GetAdminEvent();
    
    Task<List<EventWithPackagesDTO>> GetThreeLatestEvent();
    Task<Event> GetEventById(int id);

}