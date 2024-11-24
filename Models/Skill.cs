using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace SeleafAPI.Models;

public class Skill
{
    public int Id { get; set; }
    public string SkillName { get; set; }
    
    [ForeignKey("StudentProfile")]
    public string StudentProfileId { get; set; } // Foreign key property

    public StudentProfile StudentProfile { get; set; } // Navigation property
}