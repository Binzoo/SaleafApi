using SeleafAPI.Models.DTO;

namespace SeleafAPI.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task AddResetCodeAsync(PasswordResetDTO reset);
        Task<PasswordResetDTO> GetResetCodeAsync(string userId, string code);
        Task RemoveResetCodeAsync(PasswordResetDTO reset);
    }
}
