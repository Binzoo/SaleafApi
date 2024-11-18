using Microsoft.AspNetCore.Identity;
using SaleafApi.Data;
using SeleafAPI.Data;

namespace SeleafAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser> FindByNameAsync(string username);
        Task<AppUser> FindByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(AppUser user, string password);
        Task<IdentityResult> AddToRoleAsync(AppUser user, string role);
        Task<IdentityResult> RemovePasswordAsync(AppUser user);
        Task<IdentityResult> AddPasswordAsync(AppUser user, string newPassword);
        Task<IList<string>> GetRolesAsync(AppUser user);
        IQueryable<AppUser> GetUsers();
        Task<bool> CheckPasswordAsync(AppUser user, string password);
        Task<IdentityResult> UpdateAsync(AppUser user);
        Task<AppUser> FindByIdAsync(string id);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken token);

        Task SaveRefreshTokenAsync(RefreshToken token);
        
        Task<IdentityResult> DeleteUserAsync(string userId);


    }
}
