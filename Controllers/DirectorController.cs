using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleafApi.Interfaces;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

[Route("api/[controller]")]
[ApiController]
public class DirectorController : ControllerBase
{
    private readonly IRepository<Director> _director;
    private readonly IS3Service _S3Service;
    private readonly string _awsRegion;
    private readonly string _bucketName;

    public DirectorController(IRepository<Director> director, IS3Service S3Service, IConfiguration configuration)
    {
        _director = director;
        _S3Service = S3Service;
        _awsRegion = configuration["AWS_REGION"];  
        _bucketName = configuration["AWS_BUCKET_NAME"]; 
    }

    [HttpGet("get-all-director")]
    public async Task<IActionResult> GetAll()
    {
        var directors = await _director.GetAllAsync();
        if (directors == null || !directors.Any())
        {
            return NotFound("No directors found.");
        }
        return Ok(directors);
    }

    [HttpGet("get-director-by-id")]
    public async Task<IActionResult> GetDirectorByIdAsync(int id)
    {
        var director = await _director.GetByIdAsync(id);
        if (director == null)
        {
            return NotFound("No director found.");
        }
        return Ok(director);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("add-director")]
    public async Task<IActionResult> AddDirector([FromForm] DirectorDTO model)
    {
        if (model.DirectorImage == null || model.DirectorImage.Length == 0)
        {
            return BadRequest("Image file is required.");
        }

        var fileName = $"directors/{Guid.NewGuid()}-{model.DirectorImage.FileName}"; 

        try
        {
            // Upload the image to S3
            using (var newMemoryStream = new MemoryStream())
            {
                await model.DirectorImage.CopyToAsync(newMemoryStream);
                await _S3Service.UploadFileAsync(newMemoryStream, fileName);  
            }

            // Construct the URL of the uploaded image
            var s3Url = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileName}";

            // Save the director info with the S3 URL in the database
            var director = new Director
            {
                DirectorName = model.DirectorName,
                DirectorLastName = model.DirectorLastName,
                DirectorImage = s3Url, 
                DirectorDescription = model.DirectorDescription
            };

            await _director.AddAsync(director);  
            return Ok("Director added and image uploaded to S3.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update-director/{id}")]
    public async Task<IActionResult> UpdateDirector(int id, [FromForm] DirectorDTO model)
    {
        var director = await _director.GetByIdAsync(id);

        if (director == null)
        {
            return NotFound("Director not found.");
        }

        if (model.DirectorImage != null && model.DirectorImage.Length > 0){
            // Define the file name or key for S3
            var fileName = $"directors/{Guid.NewGuid()}-{model.DirectorImage.FileName}";

            try
            {
                // Upload the new image to S3
                using (var newMemoryStream = new MemoryStream())
                {
                    await model.DirectorImage.CopyToAsync(newMemoryStream);
                    await _S3Service.UploadFileAsync(newMemoryStream, fileName);
                }
                // Get the URL of the uploaded image
                var s3Url = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileName}";
                // Update the director's image URL
                director.DirectorImage = s3Url;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading image.");
            }
        }

        // Update the other director fields
        director.DirectorName = model.DirectorName;
        director.DirectorLastName = model.DirectorLastName;
        director.DirectorDescription = model.DirectorDescription;

        await _director.UpdateAsync(director);
        return Ok("Director has been updated successfully.");
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDirector(int id)
    {
        var director = await _director.GetByIdAsync(id);
        if (director == null)
        {
            return NotFound("Director not found.");
        }

        await _director.DeleteAsync(director);
        return Ok("Director has been deleted successfully.");
    }
}

