﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SeleafAPI.Data;
using SeleafAPI.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SeleafAPI.Interfaces;

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

        public AccountController(IUserRepository userRepository, IRoleRepository roleRepository, IPasswordResetRepository passwordResetRepository, IConfiguration configuration, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordResetRepository = passwordResetRepository;
            _configuration = configuration;
            _emailService = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var userEmail = await _userRepository.FindByEmailAsync(model.Email);
            if (userEmail != null)
            {
                return BadRequest("Email Already Exists.");
            }

            var user = new AppUser { UserName = model.Username, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, isStudent = model.isStudent };
            var result = await _userRepository.CreateAsync(user, model.Password);
            //if user un tick student or if isstudnet is false then user will automatically be a sponsor 
            if (!model.isStudent)
            {
                result = await _userRepository.AddToRoleAsync(user, "Sponsor");
            }

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully" });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userRepository.FindByNameAsync(model.Username);
            if (user != null && await _userRepository.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userRepository.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                   new Claim(JwtRegisteredClaimNames.Sub, user.UserName!), // This holds the username
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token ID
                   new Claim("userId", user.Id), // Use a custom claim for the user ID
                   new Claim(ClaimTypes.Role, "admin") // Example role
                };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256));

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return Unauthorized();
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

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleDTO model)
        {
            var user = await _userRepository.FindByNameAsync(model.Username);
            if (user == null)
            {
                return BadRequest("User not found");
            }
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
                    await _emailService.SendEmailAsync(user.Email!, "Verified at Salef as a student.", "Congratulation you have been verify as a studnet at ");
                }
                return Ok(new { message = "Role assigned successfully" });
            }
            return BadRequest(result.Errors);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("get-unverify-student")]
        public IActionResult GetUnverifyStudent()
        {
            var unverifyStudent = _userRepository.GetUsers().Where(u => u.isStudent && !u.isVerified).Select(u => new StudentDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName!,
                Email = u.Email!,
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
            var user = await _userRepository.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var isValidCode = await ValidateResetCodeAsync(user.Id, model.Code);
            if (!isValidCode)
            {
                return BadRequest("Invalid or Expired Code");
            }

            var removePasswordResult = await _userRepository.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return BadRequest(removePasswordResult.Errors);
            }

            var addPasswordResult = await _userRepository.AddPasswordAsync(user, model.NewPassword);
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
    }

}