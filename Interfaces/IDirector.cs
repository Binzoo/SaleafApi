using System;
using Microsoft.AspNetCore.Mvc;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;

namespace SeleafAPI.Interfaces;

public interface IDirector
{
    Task AddDirectorAsync(Director model);
    Task DeleteDirectorAsync(Director director);
    Task<Director> ViewDirectorByIDAsync(int id);
    Task<List<Director>> GetAllDirectorAsync();
    Task UpdateDirectorAsync(int id, DirectorDTO model);
    Task SaveChanges();
}
