using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;

namespace SeleafAPI.Repositories;

public class AchievementRepository : IAchievementRepository
{
    private readonly AppDbContext _context;
    
    public AchievementRepository(AppDbContext context)
    {
        _context  = context;
    }
    
    public async Task DeleteByStudentProfileIdAsync(string studentProfileId)
    {
        var existingItems = _context.Achievements.Where(a => a.StudentProfileId == studentProfileId).ToList();
        _context.Achievements.RemoveRange(existingItems);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Achievement> achievements)
    {
        _context.Achievements.AddRange(achievements);
        await _context.SaveChangesAsync();
    }
}