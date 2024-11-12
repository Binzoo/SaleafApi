using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SaleafApi.Interfaces;
using SeleafAPI.Data;
using SeleafAPI.Models;
using SaleafApi.Models.DTO;
using SeleafAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SeleafAPI.Interfaces;


namespace SeleafAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BursaryApplicationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IS3Service _S3Service;
        private readonly string _awsRegion;
        private readonly string _bucketName;
        private readonly IUserRepository _userRepository;

        public BursaryApplicationController(AppDbContext context, IMapper mapper, IS3Service S3Service, IConfiguration configuration, IUserRepository userRepository)
        {
            _context = context;
            _mapper = mapper;
            _S3Service = S3Service;
            _awsRegion = configuration["AWS_REGION"];
            _bucketName = configuration["AWS_BUCKET_NAME"];
            _userRepository = userRepository;
        }

        // POST: api/BursaryApplication
       // [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromForm] BursaryApplicationFileUploadDto uploadDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.BursaryApplications.Any(b => b.Email == uploadDto.Email))
            {
                return BadRequest(new
                {
                    message = "Bursary application already exists with this email."
                });  
            }

            try
            {
                var userId = User.FindFirst("userId")?.Value;
                string tertaiary = "";
                string grade12 = "";
                string grade11 = "";

                if (uploadDto.TertiarySubjectsAndResultsFile != null)
                {
                    var fileKey = $"TertiarySubjectsAndResults/{Guid.NewGuid()}-{uploadDto.TertiarySubjectsAndResultsFile.FileName}";
                    using (var newMemoryStream = new MemoryStream())
                    {
                        await uploadDto.TertiarySubjectsAndResultsFile.CopyToAsync(newMemoryStream);
                        await _S3Service.UploadFileAsync(newMemoryStream, fileKey);
                    }
                    var tertiarySubjectsAndResultsUrl = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileKey}";
                    tertaiary = tertiarySubjectsAndResultsUrl;
                }

                // Handle Grade12SubjectsAndResultsFile
                if (uploadDto.Grade12SubjectsAndResultsFile != null)
                {
                    var fileKey = $"Grade12SubjectsAndResults/{Guid.NewGuid()}-{uploadDto.Grade12SubjectsAndResultsFile.FileName}";
                    using (var newMemoryStream = new MemoryStream())
                    {
                        await uploadDto.Grade12SubjectsAndResultsFile.CopyToAsync(newMemoryStream);
                        await _S3Service.UploadFileAsync(newMemoryStream, fileKey);
                    }
                    var grade12SubjectsAndResultsUrl = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileKey}";
                    grade12 = grade12SubjectsAndResultsUrl;
                }

                // Handle Grade11SubjectsAndResultsFile
                if (uploadDto.Grade11SubjectsAndResultsFile != null)
                {
                    var fileKey = $"Grade11SubjectsAndResults/{Guid.NewGuid()}-{uploadDto.Grade11SubjectsAndResultsFile.FileName}";
                    using (var newMemoryStream = new MemoryStream())
                    {
                        await uploadDto.Grade11SubjectsAndResultsFile.CopyToAsync(newMemoryStream);
                        await _S3Service.UploadFileAsync(newMemoryStream, fileKey);
                    }
                    var grade11SubjectsAndResultsUrl = $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{fileKey}";
                    grade11 = grade11SubjectsAndResultsUrl;
                }

                // Create the BursaryApplication object
                BursaryApplication bursary = new BursaryApplication
                {
                    AppUserId = userId,
                    Name = uploadDto.Name,
                    Surname = uploadDto.Surname,
                    DateOfBirth = uploadDto.DateOfBirth,
                    SAIDNumber = uploadDto.SAIDNumber,
                    PlaceOfBirth = uploadDto.PlaceOfBirth,
                    IsOfLebaneseOrigin = uploadDto.IsOfLebaneseOrigin,
                    HomePhysicalAddress = uploadDto.HomePhysicalAddress,
                    HomePostalAddress = uploadDto.HomePostalAddress,
                    ContactNumber = uploadDto.ContactNumber,
                    Email = uploadDto.Email,
                    HasDisabilities = uploadDto.HasDisabilities,
                    DisabilityExplanation = uploadDto.DisabilityExplanation,

                    // Studies Applied For
                    InstitutionAppliedFor = uploadDto.InstitutionAppliedFor,
                    DegreeOrDiploma = uploadDto.DegreeOrDiploma,
                    YearOfStudyAndCommencement = uploadDto.YearOfStudyAndCommencement,
                    StudentNumber = uploadDto.StudentNumber,
                    ApproximateFundingRequired = uploadDto.ApproximateFundingRequired,

                    // Academic History (S3 URLs)
                    NameOfInstitution = uploadDto.NameOfInstitution,
                    YearCommencedInstitution = uploadDto.YearCommencedInstitution,
                    YearToBeCompletedInstitution = uploadDto.YearToBeCompletedInstitution,
                    TertiarySubjectsAndResultsUrl = tertaiary,
                    NameOfSchool = uploadDto.NameOfSchool,
                    YearCommencedSchool = uploadDto.YearCommencedSchool,
                    YearToBeCompletedSchool = uploadDto.YearToBeCompletedSchool,
                    Grade12SubjectsAndResultsUrl = grade12,
                    Grade11SubjectsAndResultsUrl = grade11,

                    // Additional Information
                    LeadershipRoles = uploadDto.LeadershipRoles,
                    SportsAndCulturalActivities = uploadDto.SportsAndCulturalActivities,
                    HobbiesAndInterests = uploadDto.HobbiesAndInterests,
                    ReasonForStudyFieldChoice = uploadDto.ReasonForStudyFieldChoice,
                    SelfDescription = uploadDto.SelfDescription,
                    IntendsToStudyFurther = uploadDto.IntendsToStudyFurther,
                    WhySelectYou = uploadDto.WhySelectYou,
                    HasSensitiveMatters = uploadDto.HasSensitiveMatters,
                    
                    DependentsAtPreSchool = uploadDto.DependentsAtPreSchool,
                    DependentsAtSchool = uploadDto.DependentsAtSchool,
                    DependentsAtUniversity = uploadDto.DependentsAtUniversity,
                    
                    // Totals and Declaration
                    JewelleryValue = uploadDto.JewelleryValue,
                    FurnitureAndFittingsValue = uploadDto.FurnitureAndFittingsValue,
                    EquipmentValue = uploadDto.EquipmentValue,
                    Overdrafts = uploadDto.Overdrafts,
                    UnsecuredLoans = uploadDto.UnsecuredLoans,
                    CreditCardDebts = uploadDto.CreditCardDebts,
                    IncomeTaxDebts = uploadDto.IncomeTaxDebts,
                    ContingentLiabilities = uploadDto.ContingentLiabilities,
                    TotalOfAssetsAndLiabilities = uploadDto.TotalOfAssetsAndLiabilities,
                    DeclarationSignedBy = uploadDto.DeclarationSignedBy,
                    DeclarationDate = uploadDto.DeclarationDate,
                };

                await _context.BursaryApplications.AddAsync(bursary);
                await _context.SaveChangesAsync();

                foreach (var f in uploadDto.FinancialDetailsList)
                {
                    var financialDetail = new FinancialDetails
                    {
                        FullName = f.FullName,
                        SAIDNumber = f.SAIDNumber,
                        Occupation = f.Occupation,
                        MaritalStatus = f.MaritalStatus,
                        GrossMonthlyIncome = f.GrossMonthlyIncome,
                        OtherIncome = f.OtherIncome,
                        Role = f.Role,
                        BursaryApplicationId = bursary.Id
                    };
                    await _context.FinancialDetails.AddAsync(financialDetail);
                }

                // Dependents
                foreach (var d in uploadDto.Dependents)
                {
                    var dependent = new DependentInfo
                    {
                        FullName = d.FullName,
                        RelationshipToApplicant = d.RelationshipToApplicant,
                        Age = d.Age,
                        InstitutionName = d.InstitutionName,
                        BursaryApplicationId = bursary.Id
                    };  
                    await _context.DependentInfos.AddAsync(dependent);
                }

                // Fixed Properties
                foreach (var p in uploadDto.FixedProperties)
                {
                    var property = new PropertyDetails
                    {
                        PhysicalAddress = p.PhysicalAddress,
                        ErfNoTownship = p.ErfNoTownship,
                        DatePurchased = p.DatePurchased,
                        PurchasePrice = p.PurchasePrice,
                        MunicipalValue = p.MunicipalValue,
                        PresentValue = p.PresentValue,
                        BursaryApplicationId = bursary.Id
                    };
                    await _context.PropertyDetails.AddAsync(property);
                }

                // Vehicles
                foreach (var v in uploadDto.Vehicles)
                {
                    var vehicle = new VehicleDetails
                    {
                        MakeModelYear = v.MakeModelYear,
                        RegistrationNumber = v.RegistrationNumber,
                        PresentValue = v.PresentValue,
                        BursaryApplicationId = bursary.Id
                    };
                    await _context.VehicleDetails.AddAsync(vehicle);
                }

                // Life Assurance Policies
                foreach (var l in uploadDto.LifeAssurancePolicies)
                {
                    var lifePolicy = new LifeAssurancePolicy
                    {
                        Company = l.Company,
                        Description = l.Description,
                        SurrenderValue = l.SurrenderValue,
                        BursaryApplicationId = bursary.Id
                    };
                    await _context.LifeAssurancePolicies.AddAsync(lifePolicy);
                }

                // Investments
                foreach (var i in uploadDto.Investments)
                {
                    var investment = new InvestmentDetails
                    {
                        Company = i.Company,
                        Description = i.Description,
                        MarketValue = i.MarketValue,
                        BursaryApplicationId = bursary.Id
                    };
                    await _context.InvestmentDetails.AddAsync(investment);
                }

                // Other Assets
                foreach (var o in uploadDto.OtherAssets)
                {
                    var otherAsset = new OtherAsset
                    {
                        Description = o.Description,
                        Value = o.Value,
                        BursaryApplicationId = bursary.Id
                    };
                    await _context.OtherAssets.AddAsync(otherAsset);
                }

                // Other Liabilities
                foreach (var ol in uploadDto.OtherLiabilities)
                {
                    var otherLiability = new OtherLiability
                    {
                        Description = ol.Description,
                        Amount = ol.Amount,
                        BursaryApplicationId = bursary.Id
                    };
                    await _context.OtherLiabilities.AddAsync(otherLiability);
                }
                var userId1 = User.FindFirst("userId")?.Value;
                if (userId1 == null)
                {

                    var userexist = await _context.Users.Where(e => e.Email == uploadDto.Email).FirstOrDefaultAsync();

                    if (userexist == null)
                    {
                        var appuser = new AppUser()
                        {
                            Email = uploadDto.Email,
                            FirstName = uploadDto.Name,
                            LastName = uploadDto.Surname,
                            isStudent = true,
                            UserName = uploadDto.Email
                        };
                    
                        var result =  await _userRepository.CreateAsync(appuser, "P@ssword!1");
                        if (result.Succeeded)
                        {
                            await _context.SaveChangesAsync();
                            return Ok("Bursary Application and user has been Created");
                        }
                        else
                        {
                            return BadRequest(result.Errors);
                        }
                    }
                }
                
                await _context.SaveChangesAsync();
                return Ok("Bursary Application Created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading files: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Retrieve all bursary applications from the database
                var bursaryApplications = await _context.BursaryApplications
                                                        .Include(b => b.OtherLiabilities)
                                                        .Include(b => b.OtherAssets)
                                                        .Include(b => b.Dependents)
                                                        .Include(b => b.FinancialDetailsList)
                                                        .Include(b => b.FixedProperties)
                                                        .Include(b => b.Vehicles)
                                                        .Include(b => b.LifeAssurancePolicies)
                                                        .Include(b => b.Investments)
                                                        .ToListAsync();

                // Check if there are any records
                if (bursaryApplications == null || bursaryApplications.Count == 0)
                {
                    return Ok(new List<BursaryApplicationDataDto>()); 
                }

                // Map them to the BursaryApplicationDataDto
                var bursaryApplicationDtos = _mapper.Map<List<BursaryApplicationDataDto>>(bursaryApplications);

                // Return the list of applications
                return Ok(bursaryApplicationDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                // Retrieve the bursary application by its ID
                var bursaryApplication = await _context.BursaryApplications
                                                        .Include(b => b.OtherLiabilities)
                                                        .Include(b => b.OtherAssets)
                                                        .Include(b => b.Dependents)
                                                        .Include(b => b.FinancialDetailsList)
                                                        .Include(b => b.FixedProperties)
                                                        .Include(b => b.Vehicles)
                                                        .Include(b => b.LifeAssurancePolicies)
                                                        .Include(b => b.Investments)
                                                        .FirstOrDefaultAsync(b => b.Id == id);

                if (bursaryApplication == null)
                {
                    return NotFound($"Bursary application with ID {id} not found.");
                }
                var bursaryApplicationDto = _mapper.Map<BursaryApplicationDataDto>(bursaryApplication);
                return Ok(bursaryApplicationDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
