using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;

namespace SeleafAPI.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly AppDbContext _context;
    
    public SkillRepository(AppDbContext context)
    {
        _context  = context;
    }
    
    public async Task DeleteByStudentProfileIdAsync(string studentProfileId)
    {
        var existingItems = _context.Skills.Where(s => s.StudentProfileId == studentProfileId).ToList();
        _context.Skills.RemoveRange(existingItems);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Skill> skills)
    {
        _context.Skills.AddRange(skills);
        await _context.SaveChangesAsync();
    }
}