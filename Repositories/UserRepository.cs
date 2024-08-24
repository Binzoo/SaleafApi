using Microsoft.AspNetCore.Identity;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;

namespace SeleafAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<AppUser> FindByNameAsync(string username) => _userManager.FindByNameAsync(username);

        public Task<AppUser> FindByIdAsync(string id) => _userManager.FindByIdAsync(id);

        public Task<AppUser> FindByEmailAsync(string email) => _userManager.FindByEmailAsync(email);

        public Task<IdentityResult> CreateAsync(AppUser user, string password) => _userManager.CreateAsync(user, password);

        public Task<IdentityResult> AddToRoleAsync(AppUser user, string role) => _userManager.AddToRoleAsync(user, role);

        public Task<IdentityResult> RemovePasswordAsync(AppUser user) => _userManager.RemovePasswordAsync(user);

        public Task<IdentityResult> AddPasswordAsync(AppUser user, string newPassword) => _userManager.AddPasswordAsync(user, newPassword);

        public Task<IList<string>> GetRolesAsync(AppUser user) => _userManager.GetRolesAsync(user);

        public IQueryable<AppUser> GetUsers() => _userManager.Users;
        public Task<bool> CheckPasswordAsync(AppUser user, string password) => _userManager.CheckPasswordAsync(user, password);

        public Task<IdentityResult> UpdateAsync(AppUser user) => _userManager.UpdateAsync(user);


    }
}
