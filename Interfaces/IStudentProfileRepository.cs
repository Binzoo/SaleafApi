using SeleafAPI.Models;
using SeleafAPI.Repositories;

namespace SeleafAPI.Interfaces;

public interface IStudentProfileRepository
{
    Task<StudentProfile?> GetByUserIdAsync(string userId);
    Task CreateAsync(StudentProfile profile);
    Task UpdateAsync(StudentProfile profile);
    Task<IEnumerable<StudentProfile>> GetAllAsync(); 
    
    Task<List<StudentProfile>> GetThreeStudentProfilesAsync();
}