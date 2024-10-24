using System;
using SeleafAPI.Data;


namespace SeleafAPI.Models
{
    public class BursaryApplication
    {
        public int Id { get; set; }

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

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

        // Academic History (S3 URLs)
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

        public ICollection<FinancialDetails> FinancialDetailsList { get; set; } = new List<FinancialDetails>();
        // Dependents
        public int DependentsAtPreSchool { get; set; }
        public int DependentsAtSchool { get; set; }
        public int DependentsAtUniversity { get; set; }
        public ICollection<DependentInfo> Dependents { get; set; } = new List<DependentInfo>();

        // Assets and Liabilities
        public ICollection<PropertyDetails> FixedProperties { get; set; } = new List<PropertyDetails>();
        public ICollection<VehicleDetails> Vehicles { get; set; } = new List<VehicleDetails>();
        public ICollection<LifeAssurancePolicy> LifeAssurancePolicies { get; set; } = new List<LifeAssurancePolicy>();
        public ICollection<InvestmentDetails> Investments { get; set; } = new List<InvestmentDetails>();
        public decimal JewelleryValue { get; set; }
        public decimal FurnitureAndFittingsValue { get; set; }
        public decimal EquipmentValue { get; set; }
        public ICollection<OtherAsset> OtherAssets { get; set; } = new List<OtherAsset>();

        // Liabilities
        public decimal Overdrafts { get; set; }
        public decimal UnsecuredLoans { get; set; }
        public decimal CreditCardDebts { get; set; }
        public decimal IncomeTaxDebts { get; set; }
        public ICollection<OtherLiability> OtherLiabilities { get; set; } = new List<OtherLiability>();
        public decimal ContingentLiabilities { get; set; } // Suretyships

        // Totals
        public decimal TotalOfAssetsAndLiabilities { get; set; }

        // Declaration  
        public string? DeclarationSignedBy { get; set; }
        public DateTime DeclarationDate { get; set; }
    }

}

