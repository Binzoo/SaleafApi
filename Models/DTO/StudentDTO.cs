namespace SeleafAPI.Models.DTO
{
    public class StudentDTO
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool IsStudent { get; set; }
        public bool IsVerified { get; set; }
    }
}
