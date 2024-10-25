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
        public DbSet<BursaryApplication> BursaryApplications { get; set; }
        public DbSet<FinancialDetails> FinancialDetails { get; set; }
        public DbSet<DependentInfo> DependentInfos { get; set; }
        public DbSet<PropertyDetails> PropertyDetails { get; set; }
        public DbSet<VehicleDetails> VehicleDetails { get; set; }
        public DbSet<LifeAssurancePolicy> LifeAssurancePolicies { get; set; }
        public DbSet<InvestmentDetails> InvestmentDetails { get; set; }
        public DbSet<OtherAsset> OtherAssets { get; set; }
        public DbSet<OtherLiability> OtherLiabilities { get; set; }

        public DbSet<BankAccountInfo> BankAccountInfo { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Donation>().Property(d => d.Amount).HasPrecision(18, 2);
            // Configure one-to-many relationships
            builder.Entity<BursaryApplication>()
                .HasMany(b => b.Dependents)
                .WithOne(d => d.BursaryApplication)
                .HasForeignKey(d => d.BursaryApplicationId);

            builder.Entity<FinancialDetails>()
                        .HasOne(fd => fd.BursaryApplication)
                        .WithMany(b => b.FinancialDetailsList)
                        .HasForeignKey(fd => fd.BursaryApplicationId)
                        .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BursaryApplication>()
                .HasMany(b => b.FixedProperties)
                .WithOne(p => p.BursaryApplication)
                .HasForeignKey(p => p.BursaryApplicationId);

            builder.Entity<BursaryApplication>()
                .HasMany(b => b.Vehicles)
                .WithOne(v => v.BursaryApplication)
                .HasForeignKey(v => v.BursaryApplicationId);

            builder.Entity<BursaryApplication>()
                .HasMany(b => b.LifeAssurancePolicies)
                .WithOne(l => l.BursaryApplication)
                .HasForeignKey(l => l.BursaryApplicationId);

            builder.Entity<BursaryApplication>()
                .HasMany(b => b.Investments)
                .WithOne(i => i.BursaryApplication)
                .HasForeignKey(i => i.BursaryApplicationId);

            builder.Entity<BursaryApplication>()
                .HasMany(b => b.OtherAssets)
                .WithOne(o => o.BursaryApplication)
                .HasForeignKey(o => o.BursaryApplicationId);

            builder.Entity<BursaryApplication>()
                .HasMany(b => b.OtherLiabilities)
                .WithOne(o => o.BursaryApplication)
                .HasForeignKey(o => o.BursaryApplicationId);
        }
    }
}
