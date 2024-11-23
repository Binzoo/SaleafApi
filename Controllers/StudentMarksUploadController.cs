using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleafApi.Interfaces;
using SeleafAPI.Data;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentMarksUploadController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IS3Service _S3Service;
    private readonly string _awsRegion;
    private readonly string _bucketName;

    public StudentMarksUploadController(AppDbContext context, IS3Service S3Service, IConfiguration configuration)
    {
        _context = context;
        _S3Service = S3Service;
        _awsRegion = configuration["AWS_REGION"];
        _bucketName = configuration["AWS_BUCKET_NAME"];
    }

    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<IActionResult> UploadStudentMarks([FromForm] StudentMarksUploadDTO model)
    {
        var userId = User.FindFirst("userId")?.Value;

        // Validate model
        if (model == null || model.File == null || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Type))
        {
            return BadRequest("Invalid input data. Ensure all fields are properly filled.");
        }

        var currentDate = DateTime.Now;
        string period = GetUploadPeriod(currentDate);

        if (string.IsNullOrEmpty(period))
        {
            return BadRequest("Uploads are only allowed during specified periods (1st to 9th of July or December).");
        }

        string folderName = period == "July" ? "julyReports" : "decemberReports";
        string fileName = $"{folderName}/{Guid.NewGuid()}-{model.File.FileName}";

        try
        {
            // Upload the file to S3 and save to the database
            var fileUrl = await UploadToS3Async(model.File, fileName);
            await SaveStudentMarkAsync(userId, model, fileUrl);

            return Ok($"{period} Marks Added Successfully.");
        }
        catch (Exception ex)
        {
            // Log error details (implement a logger for production)
            Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    
    
    [Authorize(Roles = "Student")]
    [HttpGet("can-upload")]
    public IActionResult CanUpload()
    {
        var userId = User.FindFirst("userId")?.Value;

        // Get the current date
        var currentDate = DateTime.Now;

        // Check if the current date is within the allowed upload period
        if (!IsUploadPeriod(currentDate))
        {
            return Ok(false); 
        }

        // Check if the user has already uploaded for the current period
        var hasUploaded = _context.StudentMarksUploads.Any(mark =>
            mark.AppUserId == userId &&
            mark.UploadDate.Month == currentDate.Month &&
            mark.UploadDate.Year == currentDate.Year);

        if (hasUploaded)
        {
            return Ok(false); 
        }

        // User is eligible to upload
        return Ok(true);
    }
    
    
    [Authorize(Roles = "Admin")]
    [HttpGet("uploaded-documents")]
    public IActionResult GetUploadedDocuments([FromQuery] int? month, [FromQuery] int? year)
    {
        // Default to current month and year if not specified
        var currentDate = DateTime.Now;
        var filterMonth = month ?? currentDate.Month;
        var filterYear = year ?? currentDate.Year;

        // Query uploaded student marks
        var uploads = _context.StudentMarksUploads
            .Where(mark => mark.UploadDate.Month == filterMonth && mark.UploadDate.Year == filterYear)
            .Select(mark => new
            {
                mark.Id,
                mark.Name,
                mark.Type,
                mark.FileUrl,
                mark.UploadDate,
                mark.AppUserId
            })
            .ToList();

        return Ok(uploads);
    }
    
    
    [Authorize(Roles = "Admin")]
    [HttpGet("uploads/paginated")]
    public IActionResult GetUploadsPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Page and pageSize must be greater than 0.");
        }

        // Total count
        var totalRecords = _context.StudentMarksUploads.Count();

        // Paginate results
        var uploads = _context.StudentMarksUploads
            .OrderByDescending(upload => upload.UploadDate) // Sort by latest
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(upload => new
            {
                upload.Id,
                upload.Name,
                upload.Type,
                upload.FileUrl,
                upload.UploadDate,
                upload.AppUserId
            })
            .ToList();

        // Response
        return Ok(new
        {
            totalRecords,
            totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            currentPage = page,
            uploads
        });
    }



    private bool IsUploadPeriod(DateTime date)
    {
        return (date.Month == 7 && date.Day >= 1 && date.Day <= 9) ||
               (date.Month == 12 && date.Day >= 1 && date.Day <= 9);
    }


    // Determine the upload period based on the current date
    private string GetUploadPeriod(DateTime date)
    {
        // Only allow uploads between 1st and 9th of July or December
        if (date.Month == 7 && date.Day >= 1 && date.Day <= 9)
            return "July";
        if (date.Month == 12 && date.Day >= 1 && date.Day <= 9)
            return "December";

        return null;
    }

    // Upload the file to S3 and return its URL
    private async Task<string> UploadToS3Async(IFormFile file, string fileName)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        await _S3Service.UploadFileAsync(memoryStream, fileName);

        return $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileName}";
    }

    // Save the student mark record to the database
    private async Task SaveStudentMarkAsync(string userId, StudentMarksUploadDTO model, string fileUrl)
    {
        var studentMark = new StudentMarksUpload
        {
            FileUrl = fileUrl,
            Name = model.Name,
            Type = model.Type,
            UploadDate = DateTime.UtcNow,
            AppUserId = userId
        };

        await _context.StudentMarksUploads.AddAsync(studentMark);
        await _context.SaveChangesAsync();
    }
}
