using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleafApi.Interfaces;
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
    private readonly IS3Service _S3Service;
    private readonly string _awsRegion;
    private readonly string _bucketName;

    public StudentProfileController(IUserRepository userRepository, IStudentProfileRepository studentProfileRepository , IS3Service S3Service, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _studentProfileRepository = studentProfileRepository;
        _S3Service = S3Service;
        _awsRegion = configuration["AWS_REGION"];
        _bucketName = configuration["AWS_BUCKET_NAME"];
    }

    [HttpGet("all-studentprofiles")]
    public async Task<IActionResult> GetAllStudentProfiles()
    {
        var profiles = await _studentProfileRepository.GetAllAsync();
       

        return Ok(profiles);
    }

    [HttpPost("create-profile")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> CreateStudentProfile([FromForm] StudentProfileDTO model)
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

        var fileName = $"studentProfilePic/{Guid.NewGuid()}-{model.ProfileImage.FileName}";



        string? imageUrl = null;
        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            using (var newMemoryStream = new MemoryStream())
            {
                await model.ProfileImage.CopyToAsync(newMemoryStream);
                await _S3Service.UploadFileAsync(newMemoryStream, fileName);
            }

            imageUrl = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileName}";

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
            ImageUrl = imageUrl 
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

        
        return Ok(studentProfile);
    }

    [HttpPut("update-profile")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> UpdateStudentProfile([FromForm] StudentProfileDTO model)
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

        // Update fields
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

        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            var fileName = $"directors/{Guid.NewGuid()}-{model.ProfileImage.FileName}";

            using (var newMemoryStream = new MemoryStream())
            {
                await model.ProfileImage.CopyToAsync(newMemoryStream);
                await _S3Service.UploadFileAsync(newMemoryStream, fileName);
            }
            // Get the URL of the uploaded image
            var s3Url = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileName}";

            studentProfile.ImageUrl = fileName;
        }

        await _studentProfileRepository.UpdateAsync(studentProfile);
        return Ok("Profile updated successfully.");
    }


    [HttpGet("paginated-studentprofiles")]
    public async Task<IActionResult> GetPaginatedStudentProfiles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        // Retrieve all profiles to count total items (consider optimizing this)
        var totalProfiles = await _studentProfileRepository.GetAllAsync();
        var totalItemsCount = totalProfiles.Count(); // Count total items

        // Get paginated profiles
        var paginatedProfiles = await _studentProfileRepository.GetPaginatedStudentProfilesAsync(pageNumber, pageSize);

        // Map the profiles to DTOs
      

        // Prepare the response
        var response = new
        {
            TotalItems = totalItemsCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalItemsCount / pageSize),
            Data = paginatedProfiles
        };

        return Ok(response);
    }


}