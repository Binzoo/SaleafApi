using Microsoft.EntityFrameworkCore;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly AppDbContext _context;

        public PasswordResetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddResetCodeAsync(PasswordResetDTO reset)
        {
            _context.PasswordResetsCodes.Add(reset);
            await _context.SaveChangesAsync();
        }

        public Task<PasswordResetDTO> GetResetCodeAsync(string userId, string code)
        {
            return _context.PasswordResetsCodes.FirstOrDefaultAsync(p => p.UserId == userId && p.Code == code)!;
        }

        public async Task RemoveResetCodeAsync(PasswordResetDTO reset)
        {
            _context.PasswordResetsCodes.Remove(reset);
            await _context.SaveChangesAsync();
        }
    }
}
