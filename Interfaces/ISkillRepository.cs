using SeleafAPI.Models;

namespace SeleafAPI.Interfaces;

public interface ISkillRepository
{
    Task DeleteByStudentProfileIdAsync(string studentProfileId);
    Task AddRangeAsync(IEnumerable<Skill> skills);
}