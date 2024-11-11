﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SeleafAPI.Data;

#nullable disable

namespace SeleafAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("SaleafApi.Data.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("boolean");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("SaleafApi.Models.BankAccountInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountNo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Branch")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BranchCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("BankAccountInfo");
                });

            modelBuilder.Entity("SaleafApi.Models.DonorCertificateInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("AppUserId")
                        .HasColumnType("text");

                    b.Property<string>("IdentityNoOrCompanyRegNo")
                        .HasColumnType("text");

                    b.Property<string>("IncomeTaxNumber")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.ToTable("DonorCertificateInfos");
                });

            modelBuilder.Entity("SaleafApi.Models.ManualPaymentDoc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Amount")
                        .HasColumnType("double precision");

                    b.Property<bool>("Checked")
                        .HasColumnType("boolean");

                    b.Property<string>("DocUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ReferenceNumber")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ManualPaymentDocs");
                });

            modelBuilder.Entity("SeleafAPI.Data.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("isStudent")
                        .HasColumnType("boolean");

                    b.Property<bool>("isVerified")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("SeleafAPI.Models.BursaryApplication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AppUserId")
                        .HasColumnType("text");

                    b.Property<decimal>("ApproximateFundingRequired")
                        .HasColumnType("numeric");

                    b.Property<string>("ContactNumber")
                        .HasColumnType("text");

                    b.Property<decimal>("ContingentLiabilities")
                        .HasColumnType("numeric");

                    b.Property<decimal>("CreditCardDebts")
                        .HasColumnType("numeric");

                    b.Property<string>("DateOfBirth")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DeclarationDate")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DeclarationSignedBy")
                        .HasColumnType("text");

                    b.Property<string>("DegreeOrDiploma")
                        .HasColumnType("text");

                    b.Property<int>("DependentsAtPreSchool")
                        .HasColumnType("integer");

                    b.Property<int>("DependentsAtSchool")
                        .HasColumnType("integer");

                    b.Property<int>("DependentsAtUniversity")
                        .HasColumnType("integer");

                    b.Property<string>("DisabilityExplanation")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<decimal>("EquipmentValue")
                        .HasColumnType("numeric");

                    b.Property<decimal>("FurnitureAndFittingsValue")
                        .HasColumnType("numeric");

                    b.Property<string>("Grade11SubjectsAndResultsUrl")
                        .HasColumnType("text");

                    b.Property<string>("Grade12SubjectsAndResultsUrl")
                        .HasColumnType("text");

                    b.Property<bool>("HasDisabilities")
                        .HasColumnType("boolean");

                    b.Property<bool>("HasSensitiveMatters")
                        .HasColumnType("boolean");

                    b.Property<string>("HobbiesAndInterests")
                        .HasColumnType("text");

                    b.Property<string>("HomePhysicalAddress")
                        .HasColumnType("text");

                    b.Property<string>("HomePostalAddress")
                        .HasColumnType("text");

                    b.Property<decimal>("IncomeTaxDebts")
                        .HasColumnType("numeric");

                    b.Property<string>("InstitutionAppliedFor")
                        .HasColumnType("text");

                    b.Property<bool>("IntendsToStudyFurther")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsOfLebaneseOrigin")
                        .HasColumnType("boolean");

                    b.Property<decimal>("JewelleryValue")
                        .HasColumnType("numeric");

                    b.Property<string>("LeadershipRoles")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NameOfInstitution")
                        .HasColumnType("text");

                    b.Property<string>("NameOfSchool")
                        .HasColumnType("text");

                    b.Property<decimal>("Overdrafts")
                        .HasColumnType("numeric");

                    b.Property<string>("PlaceOfBirth")
                        .HasColumnType("text");

                    b.Property<string>("ReasonForStudyFieldChoice")
                        .HasColumnType("text");

                    b.Property<string>("SAIDNumber")
                        .HasColumnType("text");

                    b.Property<string>("SelfDescription")
                        .HasColumnType("text");

                    b.Property<string>("SportsAndCulturalActivities")
                        .HasColumnType("text");

                    b.Property<string>("StudentNumber")
                        .HasColumnType("text");

                    b.Property<string>("Surname")
                        .HasColumnType("text");

                    b.Property<string>("TertiarySubjectsAndResultsUrl")
                        .HasColumnType("text");

                    b.Property<decimal>("TotalOfAssetsAndLiabilities")
                        .HasColumnType("numeric");

                    b.Property<decimal>("UnsecuredLoans")
                        .HasColumnType("numeric");

                    b.Property<string>("WhySelectYou")
                        .HasColumnType("text");

                    b.Property<int>("YearCommencedInstitution")
                        .HasColumnType("integer");

                    b.Property<int>("YearCommencedSchool")
                        .HasColumnType("integer");

                    b.Property<string>("YearOfStudyAndCommencement")
                        .HasColumnType("text");

                    b.Property<int>("YearToBeCompletedInstitution")
                        .HasColumnType("integer");

                    b.Property<int>("YearToBeCompletedSchool")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.ToTable("BursaryApplications");
                });

            modelBuilder.Entity("SeleafAPI.Models.DTO.PasswordResetDTO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PasswordResetsCodes");
                });

            modelBuilder.Entity("SeleafAPI.Models.DependentInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<int>("BursaryApplicationId")
                        .HasColumnType("integer");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("InstitutionName")
                        .HasColumnType("text");

                    b.Property<string>("RelationshipToApplicant")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BursaryApplicationId");

                    b.ToTable("DependentInfos");
                });

            modelBuilder.Entity("SeleafAPI.Models.Director", b =>
                {
                    b.Property<int>("DirectorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("DirectorId"));

                    b.Property<string>("DirectorDescription")
                        .HasColumnType("text");

                    b.Property<string>("DirectorImage")
                        .HasColumnType("text");

                    b.Property<string>("DirectorLastName")
                        .HasColumnType("text");

                    b.Property<string>("DirectorName")
                        .HasColumnType("text");

                    b.HasKey("DirectorId");

                    b.ToTable("Directors");
                });

            modelBuilder.Entity("SeleafAPI.Models.Donation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("Amount")
                        .HasPrecision(18, 2)
                        .HasColumnType("double precision");

                    b.Property<string>("AppUserId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("boolean");

                    b.Property<string>("PaymentId")
                        .HasColumnType("text");

                    b.Property<bool>("isAnonymous")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.ToTable("Donations");
                });

            modelBuilder.Entity("SeleafAPI.Models.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EventId"));

                    b.Property<string>("EndDate")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EndTime")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EventDescription")
                        .HasColumnType("text");

                    b.Property<string>("EventImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("EventName")
                        .HasColumnType("text");

                    b.Property<double>("EventPrice")
                        .HasColumnType("double precision");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<bool>("Publish")
                        .HasColumnType("boolean");

                    b.Property<string>("StartDate")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StartTime")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("EventId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("SeleafAPI.Models.FinancialDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BursaryApplicationId")
                        .HasColumnType("integer");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<decimal>("GrossMonthlyIncome")
                        .HasColumnType("numeric");

                    b.Property<string>("MaritalStatus")
                        .HasColumnType("text");

                    b.Property<string>("Occupation")
                        .HasColumnType("text");

                    b.Property<decimal>("OtherIncome")
                        .HasColumnType("numeric");

                    b.Property<string>("Role")
                        .HasColumnType("text");

                    b.Property<string>("SAIDNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BursaryApplicationId");

                    b.ToTable("FinancialDetails");
                });

            modelBuilder.Entity("SeleafAPI.Models.InvestmentDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BursaryApplicationId")
                        .HasColumnType("integer");

                    b.Property<string>("Company")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<decimal>("MarketValue")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("BursaryApplicationId");

                    b.ToTable("InvestmentDetails");
                });

            modelBuilder.Entity("SeleafAPI.Models.LifeAssurancePolicy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BursaryApplicationId")
                        .HasColumnType("integer");

                    b.Property<string>("Company")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<decimal>("SurrenderValue")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("BursaryApplicationId");

                    b.ToTable("LifeAssurancePolicies");
                });

            modelBuilder.Entity("SeleafAPI.Models.OtherAsset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BursaryApplicationId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<decimal>("Value")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("BursaryApplicationId");

                    b.ToTable("OtherAssets");
                });

            modelBuilder.Entity("SeleafAPI.Models.OtherLiability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<int>("BursaryApplicationId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BursaryApplicationId");

                    b.ToTable("OtherLiabilities");
                });

            modelBuilder.Entity("SeleafAPI.Models.PropertyDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BursaryApplicationId")
                        .HasColumnType("integer");

                    b.Property<string>("DatePurchased")
                        .HasColumnType("text");

                    b.Property<string>("ErfNoTownship")
                        .HasColumnType("text");

                    b.Property<decimal>("MunicipalValue")
                        .HasColumnType("numeric");

                    b.Property<string>("PhysicalAddress")
                        .HasColumnType("text");

                    b.Property<decimal>("PresentValue")
                        .HasColumnType("numeric");

                    b.Property<decimal>("PurchasePrice")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("BursaryApplicationId");

                    b.ToTable("PropertyDetails");
                });

            modelBuilder.Entity("SeleafAPI.Models.VehicleDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BursaryApplicationId")
                        .HasColumnType("integer");

                    b.Property<string>("MakeModelYear")
                        .HasColumnType("text");

                    b.Property<decimal>("PresentValue")
                        .HasColumnType("numeric");

                    b.Property<string>("RegistrationNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BursaryApplicationId");

                    b.ToTable("VehicleDetails");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SeleafAPI.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SeleafAPI.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SeleafAPI.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SeleafAPI.Data.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SaleafApi.Data.RefreshToken", b =>
                {
                    b.HasOne("SeleafAPI.Data.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SaleafApi.Models.DonorCertificateInfo", b =>
                {
                    b.HasOne("SeleafAPI.Data.AppUser", "AppUser")
                        .WithMany()
                        .HasForeignKey("AppUserId");

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("SeleafAPI.Models.BursaryApplication", b =>
                {
                    b.HasOne("SeleafAPI.Data.AppUser", "AppUser")
                        .WithMany()
                        .HasForeignKey("AppUserId");

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("SeleafAPI.Models.DependentInfo", b =>
                {
                    b.HasOne("SeleafAPI.Models.BursaryApplication", "BursaryApplication")
                        .WithMany("Dependents")
                        .HasForeignKey("BursaryApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BursaryApplication");
                });

            modelBuilder.Entity("SeleafAPI.Models.Donation", b =>
                {
                    b.HasOne("SeleafAPI.Data.AppUser", "AppUser")
                        .WithMany("Donations")
                        .HasForeignKey("AppUserId");

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("SeleafAPI.Models.FinancialDetails", b =>
                {
                    b.HasOne("SeleafAPI.Models.BursaryApplication", "BursaryApplication")
                        .WithMany("FinancialDetailsList")
                        .HasForeignKey("BursaryApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BursaryApplication");
                });

            modelBuilder.Entity("SeleafAPI.Models.InvestmentDetails", b =>
                {
                    b.HasOne("SeleafAPI.Models.BursaryApplication", "BursaryApplication")
                        .WithMany("Investments")
                        .HasForeignKey("BursaryApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BursaryApplication");
                });

            modelBuilder.Entity("SeleafAPI.Models.LifeAssurancePolicy", b =>
                {
                    b.HasOne("SeleafAPI.Models.BursaryApplication", "BursaryApplication")
                        .WithMany("LifeAssurancePolicies")
                        .HasForeignKey("BursaryApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BursaryApplication");
                });

            modelBuilder.Entity("SeleafAPI.Models.OtherAsset", b =>
                {
                    b.HasOne("SeleafAPI.Models.BursaryApplication", "BursaryApplication")
                        .WithMany("OtherAssets")
                        .HasForeignKey("BursaryApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BursaryApplication");
                });

            modelBuilder.Entity("SeleafAPI.Models.OtherLiability", b =>
                {
                    b.HasOne("SeleafAPI.Models.BursaryApplication", "BursaryApplication")
                        .WithMany("OtherLiabilities")
                        .HasForeignKey("BursaryApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BursaryApplication");
                });

            modelBuilder.Entity("SeleafAPI.Models.PropertyDetails", b =>
                {
                    b.HasOne("SeleafAPI.Models.BursaryApplication", "BursaryApplication")
                        .WithMany("FixedProperties")
                        .HasForeignKey("BursaryApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BursaryApplication");
                });

            modelBuilder.Entity("SeleafAPI.Models.VehicleDetails", b =>
                {
                    b.HasOne("SeleafAPI.Models.BursaryApplication", "BursaryApplication")
                        .WithMany("Vehicles")
                        .HasForeignKey("BursaryApplicationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BursaryApplication");
                });

            modelBuilder.Entity("SeleafAPI.Data.AppUser", b =>
                {
                    b.Navigation("Donations");
                });

            modelBuilder.Entity("SeleafAPI.Models.BursaryApplication", b =>
                {
                    b.Navigation("Dependents");

                    b.Navigation("FinancialDetailsList");

                    b.Navigation("FixedProperties");

                    b.Navigation("Investments");

                    b.Navigation("LifeAssurancePolicies");

                    b.Navigation("OtherAssets");

                    b.Navigation("OtherLiabilities");

                    b.Navigation("Vehicles");
                });
#pragma warning restore 612, 618
        }
    }
}
