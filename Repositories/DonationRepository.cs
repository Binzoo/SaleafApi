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
                .Where(d => d.isAnonymous == true)
                .ToListAsync();

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
    }
}