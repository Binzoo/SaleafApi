using SeleafAPI.Models;

namespace SeleafAPI.Interfaces;

public interface IStudentProfileRepository
{
    Task<StudentProfile?> GetByUserIdAsync(string userId);
    Task CreateAsync(StudentProfile profile);
    Task UpdateAsync(StudentProfile profile);
    Task<IEnumerable<StudentProfile>> GetAllAsync(); 

    Task<List<StudentProfile>> GetThreeStudentProfilesAsync();
    Task<List<StudentProfile>> GetPaginatedStudentProfilesAsync(int pageNumber, int pageSize);
}