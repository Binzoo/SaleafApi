
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeleafAPI.Data;

namespace SeleafAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashBoardController  : ControllerBase
{
        private readonly AppDbContext _context;
        public DashBoardController(AppDbContext context)
        {
            _context = context;
        }
    
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetDashboardInfo()
        {
            var currentYear = DateTime.UtcNow.Year;
            var currentMonth = DateTime.UtcNow.Month;
            
            var allDonationAmount = await _context.Donations.Where(donation => donation.IsPaid).SumAsync(donation => donation.Amount);
            var totalEarningsMonth = await _context.Donations.Where(donation => donation.IsPaid && 
                                                                           donation.CreatedAt.Year == currentYear && 
                                                                           donation.CreatedAt.Month == currentMonth).SumAsync(donation => donation.Amount);
            var numberOfEvents = await _context.Events.CountAsync();
            var numberOfStudents = await _context.Users.Where(e => e.isStudent == true).CountAsync();
            
            var monthlyDonations = await _context.Donations
                .Where(donation => donation.IsPaid)
                .GroupBy(donation => new { donation.CreatedAt.Year, donation.CreatedAt.Month })
                .Select(group => new
                {
                    Year = group.Key.Year,
                    Month = group.Key.Month,
                    TotalEarnings = group.Sum(donation => donation.Amount)
                })
                .OrderBy(result => result.Year)
                .ThenBy(result => result.Month)
                .ToListAsync();
            
            var donationsTransactions = await _context.Donations.Take(30).ToListAsync();
    
            return Ok(new
            {
                AllDonationsAmount = allDonationAmount,
                TotalEarningsMonth = totalEarningsMonth,
                NumberOfEvents = numberOfEvents,
                NumberOfStudents = numberOfStudents,
                MonthlyDonations = monthlyDonations,
                DonationsTransactions = donationsTransactions
            });
    
    
        }
}