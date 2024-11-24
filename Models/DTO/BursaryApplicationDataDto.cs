using System;
using SaleafApi.Models.DTO;

namespace SeleafAPI.Models.DTO;

public class BursaryApplicationDataDto
{
    public string? AppUserId { get; set; }
    // Applicant Details
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

    // Academic History (URLs)
    public string? NameOfInstitution { get; set; }
    public int YearCommencedInstitution { get; set; }
    public int YearToBeCompletedInstitution { get; set; }
    public string? TertiarySubjectsAndResultsUrl { get; set; }

    public string? NameOfSchool { get; set; }
    public int YearCommencedSchool { get; set; }
    public int YearToBeCompletedSchool { get; set; }
    public string? Grade12SubjectsAndResultsUrl { get; set; }
    public string? Grade11SubjectsAndResultsUrl { get; set; }

    // Additional Information
    public string? LeadershipRoles { get; set; }
    public string? SportsAndCulturalActivities { get; set; }
    public string? HobbiesAndInterests { get; set; }
    public string? ReasonForStudyFieldChoice { get; set; }
    public string? SelfDescription { get; set; }
    public bool IntendsToStudyFurther { get; set; }
    public string? WhySelectYou { get; set; }
    public bool HasSensitiveMatters { get; set; }

    // Financial Details
    public ICollection<FinancialDetailsDto> FinancialDetailsList { get; set; } = new List<FinancialDetailsDto>();

    // Dependents
    public int DependentsAtPreSchool { get; set; }
    public int DependentsAtSchool { get; set; }
    public int DependentsAtUniversity { get; set; }
    public ICollection<DependentInfoDto> Dependents { get; set; } = new List<DependentInfoDto>();

    // Assets and Liabilities
    public ICollection<PropertyDetailsDto> FixedProperties { get; set; } = new List<PropertyDetailsDto>();
    public ICollection<VehicleDetailsDto> Vehicles { get; set; } = new List<VehicleDetailsDto>();
    public ICollection<LifeAssurancePolicyDto> LifeAssurancePolicies { get; set; } = new List<LifeAssurancePolicyDto>();
    public ICollection<InvestmentDetailsDto> Investments { get; set; } = new List<InvestmentDetailsDto>();
    public decimal JewelleryValue { get; set; }
    public decimal FurnitureAndFittingsValue { get; set; }
    public decimal EquipmentValue { get; set; }
    public ICollection<OtherAssetDto> OtherAssets { get; set; } = new List<OtherAssetDto>();

    // Liabilities
    public decimal Overdrafts { get; set; }
    public decimal UnsecuredLoans { get; set; }
    public decimal CreditCardDebts { get; set; }
    public decimal IncomeTaxDebts { get; set; }
    public ICollection<OtherLiabilityDto> OtherLiabilities { get; set; } = new List<OtherLiabilityDto>();
    public decimal ContingentLiabilities { get; set; } // Suretyships

    // Totals
    public decimal TotalOfAssetsAndLiabilities { get; set; }

    // Declaration
    public string? DeclarationSignedBy { get; set; }
    public DateTime DeclarationDate { get; set; }
}