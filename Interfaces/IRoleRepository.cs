using Microsoft.AspNetCore.Identity;

namespace SeleafAPI.Interfaces
{
    public interface IRoleRepository
    {
        Task<bool> RoleExistsAsync(string role);
        Task<IdentityResult> CreateRoleAsync(IdentityRole role);
    }
}
