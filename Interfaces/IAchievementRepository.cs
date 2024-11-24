using SeleafAPI.Models;

namespace SeleafAPI.Interfaces;

public interface IAchievementRepository
{
    Task DeleteByStudentProfileIdAsync(string studentProfileId);
    Task AddRangeAsync(IEnumerable<Achievement> skills);
}