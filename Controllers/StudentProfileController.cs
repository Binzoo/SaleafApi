using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentProfileController : ControllerBase
{
        private readonly IUserRepository _userRepository;
        private readonly IStudentProfileRepository _studentProfileRepository;

        public StudentProfileController(IUserRepository userRepository, IStudentProfileRepository studentProfileRepository)
        {
            _userRepository = userRepository;
            _studentProfileRepository = studentProfileRepository;
        }
        
        [HttpGet("all-studentprofiles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStudentProfiles()
        {
            var profiles = await _studentProfileRepository.GetAllAsync();
            var profileDtos = profiles.Select(sp => new StudentProfileDTO
            {

                FirstName = sp.FirstName,
                LastName = sp.LastName,
                Skills = sp.Skills,
                Achievements = sp.Achievements,
                Year = sp.Year,
                IsFinalYear = sp.IsFinalYear,
                Bio = sp.Bio,
                GraduationDate = sp.GraduationDate,
                University = sp.University,
                Degree = sp.Degree,
                OnlineProfile = sp.OnlineProfile
            }).ToList();

            return Ok(profileDtos);
        }

        [HttpPost("create-profile")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateStudentProfile([FromBody] StudentProfileDTO model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data provided.");
            }

            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
            {
                return BadRequest("First Name and Last Name are required.");
            }

            if (string.IsNullOrWhiteSpace(model.Year))
            {
                return BadRequest("Year is required.");
            }

            if (model.GraduationDate != null && model.GraduationDate < DateTime.UtcNow)
            {
                return BadRequest("Graduation date cannot be in the past.");
            }

            // Get User ID from token
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User Id is required.");
            }

            var existingUser = await _userRepository.FindByIdAsync(userId);
            if (existingUser == null)
            {
                return BadRequest("User does not exist.");
            }

            if (!existingUser.isStudent)
            {
                return BadRequest("The specified user is not a student.");
            }

            if (!existingUser.isVerified)
            {
                return BadRequest("Student must be verified before creating a profile.");
            }

            var existingProfile = await _studentProfileRepository.GetByUserIdAsync(existingUser.Id);
            if (existingProfile != null)
            {
                return BadRequest("Student profile already exists.");
            }

            // Create a new StudentProfile
            var studentProfile = new StudentProfile
            {
                Id = existingUser.Id,
                AppUserId = existingUser.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Skills = model.Skills,
                Achievements = model.Achievements,
                Year = model.Year,
                IsFinalYear = model.IsFinalYear,
                Bio = model.Bio,
                GraduationDate = model.GraduationDate,
                University = model.University,
                Degree = model.Degree,
                OnlineProfile = model.OnlineProfile,
            };

            await _studentProfileRepository.CreateAsync(studentProfile);

            return Ok("Profile created successfully.");
        }
        
        
        [HttpGet("get-logged-user-profile")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst("userId")?.Value;
            var userprofile = await _studentProfileRepository.GetByUserIdAsync(userId);
            if (userprofile == null)
            {
                return BadRequest("User Profile does not exist.");
            }

            return Ok(userprofile);
        }
        
        
        [HttpGet("profile/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!user.isStudent || !user.isVerified)
            {
                return BadRequest("User is not a verified student and does not have a public profile.");
            }

            var studentProfile = await _studentProfileRepository.GetByUserIdAsync(user.Id);
            if (studentProfile == null)
            {
                return NotFound("Student profile not found.");
            }

            var profileDto = new StudentProfileDTO
            {
                // FirstName and LastName come from the StudentProfile, not AppUser
                FirstName = studentProfile.FirstName,
                LastName = studentProfile.LastName,
                Skills = studentProfile.Skills,
                Achievements = studentProfile.Achievements,
                Year = studentProfile.Year,
                IsFinalYear = studentProfile.IsFinalYear,
                Bio = studentProfile.Bio,
                GraduationDate = studentProfile.GraduationDate,
                University = studentProfile.University,
                Degree = studentProfile.Degree,
                OnlineProfile = studentProfile.OnlineProfile
            };

            return Ok(profileDto);
        }

        [HttpGet("get-three-studnet-profiles")]
        public async Task<IActionResult> GetThreeStudentProfiles()
        {
            var threeStudentProfiles = await _studentProfileRepository.GetThreeStudentProfilesAsync();
            return Ok(threeStudentProfiles);
        }
        
        
        [HttpPut("update-profile")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UpdateStudentProfile([FromBody] StudentProfileDTO model)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not found.");
            }

            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null || !user.isStudent)
            {
                return BadRequest("User not authorized to update profile.");
            }

            var studentProfile = await _studentProfileRepository.GetByUserIdAsync(userId);
            if (studentProfile == null)
            {
                return NotFound("Student profile not found.");
            }

            // Lightweight Validation - Check GraduationDate if provided
            if (model.GraduationDate != null && model.GraduationDate < DateTime.UtcNow)
            {
                return BadRequest("Graduation date cannot be in the past.");
            }

            // Update fields (only update if values are provided)
            studentProfile.FirstName = string.IsNullOrWhiteSpace(model.FirstName) ? studentProfile.FirstName : model.FirstName;
            studentProfile.LastName = string.IsNullOrWhiteSpace(model.LastName) ? studentProfile.LastName : model.LastName;
            studentProfile.Skills = model.Skills ?? studentProfile.Skills;
            studentProfile.Achievements = model.Achievements ?? studentProfile.Achievements;
            studentProfile.Year = model.Year ?? studentProfile.Year;
            studentProfile.IsFinalYear = model.IsFinalYear;
            studentProfile.Bio = model.Bio ?? studentProfile.Bio;
            studentProfile.GraduationDate = model.GraduationDate ?? studentProfile.GraduationDate;
            studentProfile.University = model.University ?? studentProfile.University;
            studentProfile.Degree = model.Degree ?? studentProfile.Degree;
            studentProfile.OnlineProfile = model.OnlineProfile ?? studentProfile.OnlineProfile;

            await _studentProfileRepository.UpdateAsync(studentProfile);
            return Ok("Profile updated successfully.");
        }
}