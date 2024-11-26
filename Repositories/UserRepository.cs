using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaleafApi.Data;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;

namespace SeleafAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public UserRepository(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public Task<AppUser> FindByNameAsync(string username) => _userManager.FindByNameAsync(username)!;

        public Task<AppUser> FindByIdAsync(string id) => _userManager.FindByIdAsync(id)!;

        public Task<AppUser> FindByEmailAsync(string email) => _userManager.FindByEmailAsync(email)!;

        public Task<IdentityResult> CreateAsync(AppUser user, string password) => _userManager.CreateAsync(user, password);

        public Task<IdentityResult> AddToRoleAsync(AppUser user, string role) => _userManager.AddToRoleAsync(user, role);

        public Task<IdentityResult> RemovePasswordAsync(AppUser user) => _userManager.RemovePasswordAsync(user);

        public Task<IdentityResult> AddPasswordAsync(AppUser user, string newPassword) => _userManager.AddPasswordAsync(user, newPassword);

        public Task<IList<string>> GetRolesAsync(AppUser user) => _userManager.GetRolesAsync(user);

        public IQueryable<AppUser> GetUsers() => _userManager.Users;
        public Task<bool> CheckPasswordAsync(AppUser user, string password) => _userManager.CheckPasswordAsync(user, password);

        public Task<IdentityResult> UpdateAsync(AppUser user) => _userManager.UpdateAsync(user);

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }

       public async Task<IdentityResult> DeleteUserAsync(string userId)
{
    var strategy = _context.Database.CreateExecutionStrategy();

    return await strategy.ExecuteAsync(async () =>
    {
        // Begin transaction
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Find the user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            // Delete related RefreshTokens
            var refreshTokens = _context.RefreshTokens.Where(rt => rt.UserId == user.Id);
            _context.RefreshTokens.RemoveRange(refreshTokens);

            // Delete related DonorCertificateInfos
            var donorCertificateInfos = _context.DonorCertificateInfos.Where(dci => dci.AppUserId == user.Id);
            _context.DonorCertificateInfos.RemoveRange(donorCertificateInfos);

            // Delete related BursaryApplications and their dependent entities
            var bursaryApplications = await _context.BursaryApplications
                .Include(ba => ba.Dependents)
                .Include(ba => ba.FinancialDetailsList)
                .Include(ba => ba.FixedProperties)
                .Include(ba => ba.Vehicles)
                .Include(ba => ba.Investments)
                .Include(ba => ba.LifeAssurancePolicies)
                .Include(ba => ba.OtherAssets)
                .Include(ba => ba.OtherLiabilities)
                .Where(ba => ba.AppUserId == user.Id)
                .ToListAsync();

            foreach (var application in bursaryApplications)
            {
                _context.DependentInfos.RemoveRange(application.Dependents);
                _context.FinancialDetails.RemoveRange(application.FinancialDetailsList);
                _context.PropertyDetails.RemoveRange(application.FixedProperties);
                _context.VehicleDetails.RemoveRange(application.Vehicles);
                _context.InvestmentDetails.RemoveRange(application.Investments);
                _context.LifeAssurancePolicies.RemoveRange(application.LifeAssurancePolicies);
                _context.OtherAssets.RemoveRange(application.OtherAssets);
                _context.OtherLiabilities.RemoveRange(application.OtherLiabilities);

                _context.BursaryApplications.Remove(application);
            }

            // Delete related Donations
            var donations = _context.Donations.Where(d => d.AppUserId == user.Id);
            _context.Donations.RemoveRange(donations);

            // Delete related EventRegistrations
            var eventRegistrations = _context.EventRegistrations.Where(er => er.UserId == user.Id);
            _context.EventRegistrations.RemoveRange(eventRegistrations);

            // Delete related StudentProfiles
            var studentProfiles = _context.StudentProfiles.Where(sp => sp.AppUserId == user.Id);
            _context.StudentProfiles.RemoveRange(studentProfiles);

            // Delete related PasswordResets
            var passwordResets = _context.PasswordResetsCodes.Where(pr => pr.UserId == user.Id);
            _context.PasswordResetsCodes.RemoveRange(passwordResets);

            // Delete IdentityUserClaims
            var userClaims = _context.UserClaims.Where(uc => uc.UserId == user.Id);
            _context.UserClaims.RemoveRange(userClaims);

            // Delete IdentityUserLogins
            var userLogins = _context.UserLogins.Where(ul => ul.UserId == user.Id);
            _context.UserLogins.RemoveRange(userLogins);

            // Delete IdentityUserTokens
            var userTokens = _context.UserTokens.Where(ut => ut.UserId == user.Id);
            _context.UserTokens.RemoveRange(userTokens);

            // Delete IdentityUserRoles
            var userRoles = _context.UserRoles.Where(ur => ur.UserId == user.Id);
            _context.UserRoles.RemoveRange(userRoles);

            // Save changes to remove related data
            await _context.SaveChangesAsync();

            // Finally, delete the user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                // Handle errors
                return result;
            }

            // Commit transaction
            await transaction.CommitAsync();

            return IdentityResult.Success;
        }
        catch (Exception)
        {
            // Rollback transaction
            await transaction.RollbackAsync();
            return IdentityResult.Failed(new IdentityError { Description = "An error occurred while deleting the user." });
        }
    });
}

        public async Task<IEnumerable<AppUser>> GetUsersInRoleAsync(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }

public async Task SaveRefreshTokenAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }
        
        

    }
}
