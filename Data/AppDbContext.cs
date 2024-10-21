using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SaleafApi.Data;
using SaleafApi.Models;
using SeleafAPI.Models;
using SeleafAPI.Models.DTO;


namespace SeleafAPI.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<PasswordResetDTO> PasswordResetsCodes { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<DonorCertificateInfo> DonorCertificateInfos { get; set; }
        public DbSet<ManualPaymentDoc> ManualPaymentDocs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Donation>().Property(d => d.Amount).HasPrecision(18, 2);
        }
    }
}
