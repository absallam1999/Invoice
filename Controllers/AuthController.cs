using invoice.DTO;
using invoice.DTO.Auth;
using invoice.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public AuthController(UserManager<ApplicationUser> _userManager, IConfiguration _configuration)
        {
            userManager = _userManager;
            configuration = _configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });

            var user = new ApplicationUser
            {
                UserName = register.UserName,
                Email = register.Email
            };
            var result = await userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                return Ok(new GeneralResponse<object>
                {
                    Success = true,
                    Message = "User registered successfully.",
                    Data = user.Id
                });
            }

            return BadRequest(new GeneralResponse<object>
            {
                Success = false,
                Message = "User registration failed.",
                Data = result.Errors
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });

            var user = await userManager.FindByEmailAsync(login.Email);
            if (user != null)
            {
                var isPasswordValid = await userManager.CheckPasswordAsync(user, login.Password);
                if (isPasswordValid)
                {
                    var token = GenerateJwtToken(user);
                    return Ok(new GeneralResponse<object>
                    {
                        Success = true,
                        Message = "User logged in successfully.",
                        Data = new LoginResultDTO
                        {
                            Token = token,
                            UserId = user.Id,
                            Email = user.Email
                        }
                    });
                }
            }

            return BadRequest(new GeneralResponse<object>
            {
                Success = false,
                Message = "Invalid login attempt.",
                Data = null
            });
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });

            var user = await userManager.FindByIdAsync(dto.Id);
            if (user == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null
                });

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Error updating user.",
                    Data = result.Errors
                });

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "User updated successfully.",
                Data = null
            });
        }

        [HttpPost("forget")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null
                });

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Password reset token generated.",
                Data = token
            });
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null
                });

            var result = await userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Error resetting password.",
                    Data = result.Errors
                });

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Password reset successfully.",
                Data = null
            });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = ModelState
                });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null
                });

            var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Error changing password.",
                    Data = result.Errors
                });

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Password changed successfully.",
                Data = null
            });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null
                });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user.Id != currentUserId)
                return Forbid();

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Error deleting user.",
                    Data = result.Errors
                });

            return Ok(new GeneralResponse<object>
            {
                Success = true,
                Message = "Account deleted successfully.",
                Data = null
            });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
           // new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),    
             new Claim(ClaimTypes.NameIdentifier, user.Id)

        };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}