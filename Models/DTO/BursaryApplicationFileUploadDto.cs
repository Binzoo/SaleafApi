using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SaleafApi.Models.DTO
{
    public class BursaryApplicationFileUploadDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? SAIDNumber { get; set; }
        public string? PlaceOfBirth { get; set; }
        public bool IsOfLebaneseOrigin { get; set; }
        public string? HomePhysicalAddress { get; set; }
        public string? HomePostalAddress { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public bool HasDisabilities { get; set; }
        public string? DisabilityExplanation { get; set; }

        // Studies Applied For
        public string? InstitutionAppliedFor { get; set; }
        public string? DegreeOrDiploma { get; set; }
        public string? YearOfStudyAndCommencement { get; set; }
        public string? StudentNumber { get; set; }
        public decimal ApproximateFundingRequired { get; set; }

        // Academic History (Files)
        public string? NameOfInstitution { get; set; }
        public int YearCommencedInstitution { get; set; }
        public int YearToBeCompletedInstitution { get; set; }
        public IFormFile? TertiarySubjectsAndResultsFile { get; set; }

        public string? NameOfSchool { get; set; }
        public int YearCommencedSchool { get; set; }
        public int YearToBeCompletedSchool { get; set; }
        public IFormFile? Grade12SubjectsAndResultsFile { get; set; }
        public IFormFile? Grade11SubjectsAndResultsFile { get; set; }

        // Additional Information
        public string? LeadershipRoles { get; set; }
        public string? SportsAndCulturalActivities { get; set; }
        public string? HobbiesAndInterests { get; set; }
        public string? ReasonForStudyFieldChoice { get; set; }
        public string? SelfDescription { get; set; }
        public bool IntendsToStudyFurther { get; set; }
        public string? WhySelectYou { get; set; }
        public bool HasSensitiveMatters { get; set; }

        // Collections as JSON strings
        [FromForm(Name = "financialDetailsList")]
        public string? FinancialDetailsListJson { get; set; }

        [FromForm(Name = "dependents")]
        public string? DependentsJson { get; set; }

        [FromForm(Name = "fixedProperties")]
        public string? FixedPropertiesJson { get; set; }

        [FromForm(Name = "vehicles")]
        public string? VehiclesJson { get; set; }

        [FromForm(Name = "lifeAssurancePolicies")]
        public string? LifeAssurancePoliciesJson { get; set; }

        [FromForm(Name = "investments")]
        public string? InvestmentsJson { get; set; }

        [FromForm(Name = "otherAssets")]
        public string? OtherAssetsJson { get; set; }

        [FromForm(Name = "otherLiabilities")]
        public string? OtherLiabilitiesJson { get; set; }

        // Computed collection properties
        public ICollection<FinancialDetailsDto> FinancialDetailsList =>
            !string.IsNullOrEmpty(FinancialDetailsListJson)
                ? JsonSerializer.Deserialize<List<FinancialDetailsDto>>(FinancialDetailsListJson) ?? new List<FinancialDetailsDto>()
                : new List<FinancialDetailsDto>();

        public ICollection<DependentInfoDto> Dependents =>
            !string.IsNullOrEmpty(DependentsJson)
                ? JsonSerializer.Deserialize<List<DependentInfoDto>>(DependentsJson) ?? new List<DependentInfoDto>()
                : new List<DependentInfoDto>();

        public ICollection<PropertyDetailsDto> FixedProperties =>
            !string.IsNullOrEmpty(FixedPropertiesJson)
                ? JsonSerializer.Deserialize<List<PropertyDetailsDto>>(FixedPropertiesJson) ?? new List<PropertyDetailsDto>()
                : new List<PropertyDetailsDto>();

        public ICollection<VehicleDetailsDto> Vehicles =>
            !string.IsNullOrEmpty(VehiclesJson)
                ? JsonSerializer.Deserialize<List<VehicleDetailsDto>>(VehiclesJson) ?? new List<VehicleDetailsDto>()
                : new List<VehicleDetailsDto>();

        public ICollection<LifeAssurancePolicyDto> LifeAssurancePolicies =>
            !string.IsNullOrEmpty(LifeAssurancePoliciesJson)
                ? JsonSerializer.Deserialize<List<LifeAssurancePolicyDto>>(LifeAssurancePoliciesJson) ?? new List<LifeAssurancePolicyDto>()
                : new List<LifeAssurancePolicyDto>();

        public ICollection<InvestmentDetailsDto> Investments =>
            !string.IsNullOrEmpty(InvestmentsJson)
                ? JsonSerializer.Deserialize<List<InvestmentDetailsDto>>(InvestmentsJson) ?? new List<InvestmentDetailsDto>()
                : new List<InvestmentDetailsDto>();

        public ICollection<OtherAssetDto> OtherAssets =>
            !string.IsNullOrEmpty(OtherAssetsJson)
                ? JsonSerializer.Deserialize<List<OtherAssetDto>>(OtherAssetsJson) ?? new List<OtherAssetDto>()
                : new List<OtherAssetDto>();

        public ICollection<OtherLiabilityDto> OtherLiabilities =>
            !string.IsNullOrEmpty(OtherLiabilitiesJson)
                ? JsonSerializer.Deserialize<List<OtherLiabilityDto>>(OtherLiabilitiesJson) ?? new List<OtherLiabilityDto>()
                : new List<OtherLiabilityDto>();

        // Values
        public decimal JewelleryValue { get; set; }
        public decimal FurnitureAndFittingsValue { get; set; }
        public decimal EquipmentValue { get; set; }
        public decimal Overdrafts { get; set; }
        public decimal UnsecuredLoans { get; set; }
        public decimal CreditCardDebts { get; set; }
        public decimal IncomeTaxDebts { get; set; }
        public decimal ContingentLiabilities { get; set; }
        public decimal TotalOfAssetsAndLiabilities { get; set; }

        // Declaration
        public string? DeclarationSignedBy { get; set; }
        public DateTime DeclarationDate { get; set; }
    }
}