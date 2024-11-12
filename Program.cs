using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SeleafAPI.Data;
using SeleafAPI.Interfaces;
using SeleafAPI.Repositories;
using Amazon.S3;
using Amazon.Runtime;
using SeleafAPI.Mapping;
using SeleafAPI.Filters.SwaggerConfig;
using SeleafAPI.Services;
using Hangfire;
using Hangfire.PostgreSql;
using SaleafApi.Interfaces;
using SaleafApi.Repositories;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Retrieve AWS credentials and region from environment variables
var awsAccessKeyId = builder.Configuration["AWS_ACCESS_KEY_ID"];
var awsSecretAccessKey = builder.Configuration["AWS_SECRET_ACCESS_KEY"];
var awsRegion = builder.Configuration["AWS_REGION"];

// Register the AmazonS3 service
builder.Services.AddDefaultAWSOptions(new Amazon.Extensions.NETCore.Setup.AWSOptions
{
    Credentials = new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey),
    Region = Amazon.RegionEndpoint.GetBySystemName(awsRegion)
});
builder.Services.AddAWSService<IAmazonS3>();

// Register services
builder.Services.AddScoped<IS3Service, S3Service>();
builder.Services.AddHttpClient();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHangfire(config => 
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddHangfireServer();

builder.Services.AddScoped<EventStatusUpdaterService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.OperationFilter<FileUploadOperationFilter>();
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});



// Database Context Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"), npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    })
);

// Identity Configuration
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Authentication and JWT Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SponsorPolicy", policy => policy.RequireRole("Sponsor"));
    options.AddPolicy("StudentPolicy", policy => policy.RequireRole("Student"));
});

// Logging Configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Additional Services
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig!);
builder.Services.AddTransient<IEmailSender, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPayment, PaymentRepository>();
builder.Services.AddScoped<IDonation, DonationRepository>();
builder.Services.AddScoped<IPdf, PdfRepository>();
builder.Services.AddScoped<IEvent, EventRepository>();




// Application Build
var app = builder.Build();



// Apply pending migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        SeedAdminUser(userManager, roleManager).Wait();
    } catch (Exception ex)
    {
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}

// Configure HTTP pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard(); 
app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Hangfire Recurring Job Example
using (var scope = app.Services.CreateScope())
{
    var eventStatusUpdater = scope.ServiceProvider.GetRequiredService<EventStatusUpdaterService>();
    RecurringJob.AddOrUpdate(
        "update-event-statuses",
        () => eventStatusUpdater.UpdateEventStatuses(),
        "*/30 * * * *"); 
}

app.Run();


async Task SeedAdminUser(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
{
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    var adminUser = await userManager.FindByEmailAsync("superadmin@gmail.com");
    if (adminUser == null)
    {
        adminUser = new AppUser()
        {
            FirstName = "Super",
            LastName = "Admin",
            UserName = "superadmin@gmail.com",
            Email = "superadmin@gmail.com",
        };
        await userManager.CreateAsync(adminUser, "Admin@123"); 
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}
