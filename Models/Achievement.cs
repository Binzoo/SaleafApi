using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Build.Framework;

namespace SeleafAPI.Models;

public class Achievement
{
    public int Id { get; set; }

    [Required]
    public string AchievementName { get; set; }

    [ForeignKey("StudentProfile")]
    public string StudentProfileId { get; set; } 

    public StudentProfile StudentProfile { get; set; } 
}