using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SeleafAPI.Data;
using SeleafAPI.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SeleafAPI.Interfaces;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SaleafApi.Models.DTO;
using SaleafApi.Data;

namespace SeleafAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailService;
        private readonly AppDbContext _context;

        public AccountController(IUserRepository userRepository, IRoleRepository roleRepository, IPasswordResetRepository passwordResetRepository, 
            IConfiguration configuration, IEmailSender emailSender, AppDbContext context)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordResetRepository = passwordResetRepository;
            _configuration = configuration;
            _emailService = emailSender;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                // Check if the email already exists
                var userEmail = await _userRepository.FindByEmailAsync(model.Email);
                if (userEmail != null)
                {
                    return BadRequest("Email already exists.");
                }
                // Create a new user
                var user = new AppUser
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    isStudent = model.isStudent
                };
                user.UserName = model.Email;

                // create the user
                var result = await _userRepository.CreateAsync(user, model.Password);

                // Assign role based on whether the user is a student
                if (!model.isStudent)
                {
                    result = await _userRepository.AddToRoleAsync(user, "Sponsor");
                }
                else
                {
                    result = await _userRepository.AddToRoleAsync(user, "Student");
                }

                // If the user creation and role assignment succeeded
                if (result.Succeeded)
                {
                    // Fetch roles and remove duplicates
                    var userRoles = await _userRepository.GetRolesAsync(user);
                    var distinctRoles = userRoles.Distinct().ToList();

                    // Create claims for the JWT
                    var authClaims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email!), 
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                            new Claim("userId", user.Id), 
                        };

                    // Add distinct roles to the claims
                    authClaims.AddRange(distinctRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                    // Generate JWT token
                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                            SecurityAlgorithms.HmacSha256)
                    );

                    // Return the token and a success message
                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), message = "User registered successfully" });
                }

                // If user creation or role assignment failed, return the errors
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 status code with the error message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                // Check if the email already exists
                var userEmail = await _userRepository.FindByEmailAsync(model.Email);
                if (userEmail != null)
                {
                    return BadRequest("Email already exists.");
                }
                // Create a new user
                var user = new AppUser
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                };
                user.UserName = model.Email;

                // create the user
                var result = await _userRepository.CreateAsync(user, model.Password);
                
                result = await _userRepository.AddToRoleAsync(user, "Admin");
                
                // If the user creation and role assignment succeeded
                if (result.Succeeded)
                {
                    // Fetch roles and remove duplicates
                    var userRoles = await _userRepository.GetRolesAsync(user);
                    var distinctRoles = userRoles.Distinct().ToList();

                    // Create claims for the JWT
                    var authClaims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email!), 
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                            new Claim("userId", user.Id), 
                        };

                    // Add distinct roles to the claims
                    authClaims.AddRange(distinctRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                    // Generate JWT token
                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                            SecurityAlgorithms.HmacSha256)
                    );

                    // Return the token and a success message
                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), message = "Admin registered successfully" });
                }

                // If user creation or role assignment failed, return the errors
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 status code with the error message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userRepository.FindByEmailAsync(model.Email);
            if (user != null && await _userRepository.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userRepository.GetRolesAsync(user);
                var distinctRoles = userRoles.Distinct().ToList();

                // Create JWT token claims
                var authClaims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email!), // Username claim
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token ID claim
                        new Claim("userId", user.Id), // User ID claim
                    };

                // Add distinct roles to the claims
                authClaims.AddRange(distinctRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                // Generate JWT access token
                var accessToken = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"], // Optional, if you have an audience
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                        SecurityAlgorithms.HmacSha256)
                );

                // Generate refresh token
                var refreshToken = GenerateRefreshToken();

                // Save refresh token to the database
                var refreshTokenEntry = new RefreshToken
                {
                    Token = refreshToken,
                    ExpiryDate = DateTime.UtcNow.AddDays(7), // Refresh token expires after 7 days
                    UserId = user.Id,
                    IsRevoked = false
                };

                await _userRepository.SaveRefreshTokenAsync(refreshTokenEntry);

                // Return both access token and refresh token
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(accessToken),
                    refreshToken = refreshToken
                });
            }
            return Unauthorized();
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO model)
        {
            // Validate the refresh token
            var storedToken = await _userRepository.GetRefreshTokenAsync(model.RefreshToken);
            if (storedToken == null || storedToken.ExpiryDate <= DateTime.UtcNow || storedToken.IsRevoked)
            {
                return Unauthorized("Invalid or expired refresh token");
            }

            // Find the user associated with the refresh token
            var user = await _userRepository.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return Unauthorized("User not found");
            }

            // Generate new access token
            var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("userId", user.Id),
                };

            var userRoles = await _userRepository.GetRolesAsync(user);
            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var accessToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                claims: authClaims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256)
            );

            // Optionally, issue a new refresh token
            var newRefreshToken = GenerateRefreshToken();
            storedToken.Token = newRefreshToken;
            storedToken.ExpiryDate = DateTime.Now.AddDays(7);
            await _userRepository.UpdateRefreshTokenAsync(storedToken);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(accessToken),
                refreshToken = newRefreshToken
            });
        }


        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] string role)
        {
            if (!await _roleRepository.RoleExistsAsync(role))
            {
                var result = await _roleRepository.CreateRoleAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    return Ok(new { message = "Role added successfully" });
                }

                return BadRequest(result.Errors);
            }

            return BadRequest("Role already exists");
        }

        [HttpPost("assign-role/{userId}")]
        public async Task<IActionResult> AssignRole(string userId, [FromBody] UserRoleDTO model)
        {
            
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var bursayUser = await _context.BursaryApplications.Where(e => e.Email == user.Email).FirstOrDefaultAsync();
            if (bursayUser == null)
            {
                return BadRequest("User not found");
            }                
            if (bursayUser.AppUserId == null)
            {
                bursayUser.AppUserId = user.Id; 
            }
            await _context.SaveChangesAsync();
            
            user.isVerified = true;
            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }
            var result = await _userRepository.AddToRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                if (user.isStudent && user.isVerified)
                {
                    await _emailService.SendEmailAsync(user.Email!, "Verified at Saleaf as a student.", "Congratulation you have been verify as a studnet at Saleaf.");
                }
                return Ok(new { message = "Role assigned successfully" });
            }
            return BadRequest(result.Errors);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-unverify-student")]
        public IActionResult GetUnverifyStudent()
        {
            var unverifyStudent = _userRepository.GetUsers().Where(u => u.isStudent && !u.isVerified).Select(u => new StudentDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                UserName = u.UserName,
                IsStudent = u.isStudent,
                IsVerified = u.isVerified
            }).ToList();

            if (unverifyStudent.Any())
            {
                return Ok(unverifyStudent);
            }
            return NotFound("No unverified students found.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var user = await _userRepository.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Ok("If an account with your email was found, we've sent a code to it.");
            }
            var code = new Random().Next(100000, 999999).ToString();

            var resetEntry = new PasswordResetDTO
            {
                UserId = user.Id,
                Code = code,
                ExpiryTime = DateTime.UtcNow.AddMinutes(15)
            };

            await _passwordResetRepository.AddResetCodeAsync(resetEntry);
            await SendPasswordResetCodeByEmail(user.Email!, code);

            return Ok("Password reset code sent to your email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> RestPassword([FromBody] ResetPasswordDTO model)
        {
            var user = await _userRepository.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var isValidCode = await ValidateResetCodeAsync(user.Id, model.Code!);
            if (!isValidCode)
            {
                return BadRequest("Invalid or Expired Code");
            }

            var removePasswordResult = await _userRepository.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return BadRequest(removePasswordResult.Errors);
            }

            var addPasswordResult = await _userRepository.AddPasswordAsync(user, model.NewPassword!);
            if (!addPasswordResult.Succeeded)
            {
                return BadRequest(addPasswordResult.Errors);
            }

            return Ok("Password has been reset successfully.");
        }

        private async Task<bool> ValidateResetCodeAsync(string userId, string submittedCode)
        {
            var resetEntry = await _passwordResetRepository.GetResetCodeAsync(userId, submittedCode);
            if (resetEntry != null && resetEntry.ExpiryTime > DateTime.UtcNow)
            {
                await _passwordResetRepository.RemoveResetCodeAsync(resetEntry);
                return true;
            }
            return false;
        }

        private async Task SendPasswordResetCodeByEmail(string email, string code)
        {
            string subject = "Your Password Reset Code";
            string message = $"Your password reset code is: {code}. This code will expire in 15 minutes.";
            await _emailService.SendEmailAsync(email, subject, message);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

    }

}
