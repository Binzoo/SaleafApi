using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeleafAPI.Interfaces;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorController : ControllerBase
    {
        private readonly IRepository<Director> _director;
        public DirectorController(IRepository<Director> director)
        {
            _director = director;
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

        [Authorize(Roles = "admin")]
        [HttpPost("add-director")]
        public async Task<IActionResult> AddDirector([FromBody] DirectorDTO model)
        {
            var director = new Director
            {
                DirectorName = model.DirectorName,
                DirectorLastName = model.DirectorLastName,
                DirectorImage = model.DirectorImage,
                DirectorDescription = model.DirectorDescription
            };

            await _director.AddAsync(director);
            return Ok("Director Added");
        }

        [Authorize(Roles = "admin")]
        [HttpPut("update-director")]
        public async Task<IActionResult> UpdateDirector(int id, [FromBody] DirectorDTO directorDTO)
        {
            var director = await _director.GetByIdAsync(id);

            if (director == null)
            {
                return NotFound();
            }

            director.DirectorName = directorDTO.DirectorName;
            director.DirectorLastName = directorDTO.DirectorLastName;
            director.DirectorImage = directorDTO.DirectorImage;
            director.DirectorDescription = directorDTO.DirectorDescription;

            await _director.UpdateAsync(director);
            return Ok("Director has been updated successfully.");
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDirector(int id)
        {
            var director = await _director.GetByIdAsync(id);
            if (director == null)
            {
                return NotFound("Director Not found.");
            }

            await _director.DeleteAsync(director);
            return Ok("Director has been deleted successfully.");
        }

    }
}
