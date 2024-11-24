using System.Collections;
using Microsoft.EntityFrameworkCore;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;

namespace SeleafAPI.Repositories;

public class StudentProfileRepository : IStudentProfileRepository
{
    private readonly AppDbContext _context;

    public StudentProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StudentProfile?> GetByUserIdAsync(string userId)
    {
        return await _context.StudentProfiles.Include(e => e.Skills).Include(e => e.Achievements)
            .FirstOrDefaultAsync(sp => sp.AppUserId == userId);
    }
    public async Task<IEnumerable<StudentProfile>> GetAllAsync()
    {
        return await _context.StudentProfiles.Include(e => e.Skills).Include(e => e.Achievements)
            .ToListAsync(); 
    }

    public Task<List<StudentProfile>> GetThreeStudentProfilesAsync()
    {
        var threeStudentProfiles = _context.StudentProfiles
            .Include(e => e.Skills).Include(e => e.Achievements).Take(3).ToListAsync();
        return threeStudentProfiles;
    }


    public async Task<List<StudentProfile>> GetPaginatedStudentProfilesAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        int skip = (pageNumber - 1) * pageSize;

        return await _context.StudentProfiles.Include(e => e.Skills).Include(e => e.Achievements)
            .OrderBy(sp => sp.FirstName) 
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }


    public async Task CreateAsync(StudentProfile profile)
    {
        await _context.StudentProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(StudentProfile profile)
    {
        _context.StudentProfiles.Update(profile);
        await _context.SaveChangesAsync();
    }

   
}