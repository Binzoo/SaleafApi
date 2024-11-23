using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaleafApi.Interfaces;
using SeleafAPI.Data;
using SeleafAPI.Models;

namespace SaleafApi.Repositories
{
    public class DonationRepository : IDonation
    {
        private readonly AppDbContext _context;
        public DonationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Donation>> GetDonations()
        {
            var donations = await _context.Donations
                .Include(d => d.AppUser)
                .ToListAsync();

            return donations;
        }
        
        public async Task<List<Donation>> GetPaginatedDonations(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
    
            
            int skip = (pageNumber - 1) * pageSize;

            
            return await _context.Donations
                .OrderBy(d => d.CreatedAt).Include(d => d.AppUser) 
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Donation>> GetAllLoggedInUserDonations(string userId)
        {
            var donations = await _context.Donations.Include(d => d.AppUser)
                .Where(d => d.AppUserId == userId).Where(i => i.IsPaid == true).ToListAsync();
            if (donations == null)
            {
                return null; 
            }

            return donations;
        }


        public async Task<Donation> GetDonationById(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                return null;
            }
            return donation;
        }

        public async Task<Donation> CreateDonationAsync(Donation donation)
        {
            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();
            return donation;
        }

        public async Task<Donation> GetDonationByPaymentIdAsync(string paymentId)
        {
            return await _context.Donations
                .Include(d => d.AppUser)
                .FirstOrDefaultAsync(d => d.PaymentId == paymentId);
        }

        public async Task UpdateDonationStatusAsync(string paymentId, bool isPaid)
        {
            var donation = await GetDonationByPaymentIdAsync(paymentId);
            if (donation != null)
            {
                donation.IsPaid = isPaid;
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task UpdateDonationStatusPOPAsync(int referenceNo, bool isPaid)
        {
            var donation = await GetDonationById(referenceNo);
            if (donation != null)
            {
                donation.IsPaid = isPaid;
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<int> GetTotalDonationsCountAsync()
        {
            return await _context.Donations.CountAsync(); 
        }

    }
}