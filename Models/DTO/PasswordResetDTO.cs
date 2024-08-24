namespace SeleafAPI.Models.DTO
{
    public class PasswordResetDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
