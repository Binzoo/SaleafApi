using SeleafAPI.Data;

namespace SeleafAPI.Models;

public class ChatBotConversation
{
    public int Id { get; set; }
    public string BotResponse { get; set; }
    public string UserQuestion { get; set; }
    public DateTime DateCreated { get; set; } 
    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}