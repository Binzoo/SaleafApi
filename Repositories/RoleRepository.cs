using Microsoft.AspNetCore.Identity;
using SeleafAPI.Interfaces;

namespace SeleafAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public Task<bool> RoleExistsAsync(string role) => _roleManager.RoleExistsAsync(role);

        public Task<IdentityResult> CreateRoleAsync(IdentityRole role) => _roleManager.CreateAsync(role);
    }
}
