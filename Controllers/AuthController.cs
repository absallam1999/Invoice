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
            var user = new ApplicationUser
            {
                UserName = register.UserName,
                Email = register.Email,
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
            else
            {
                return BadRequest(new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User registration failed.",
                    Data = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {

            var user = await userManager.FindByEmailAsync(login.Email);
            if (user != null)
            {
                var result = await userManager.CheckPasswordAsync(user, login.Password);
                if (result)
                {
                    var token = GenerateJwtToken(user);
                    return Ok(new GeneralResponse<object>
                    {
                        Success = true,
                        Message = "User loggedin successfully.",
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

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByIdAsync(dto.Id);
            if (user.UserName == null)
                return NotFound(new { Message = "User not found." });

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(new { Errors = updateResult.Errors });

            return Ok(new { Message = "User updated successfully." });
        }

        [AllowAnonymous]
        [HttpPost("forget")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            return Ok(new { Message = "Password reset token generated.", Token = token });
        }

        [AllowAnonymous]
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            var result = await userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new { Errors = result.Errors });

            return Ok(new { Message = "Password reset successful." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var userId = User.FindFirstValue(id);
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { Errors = result.Errors });

            return Ok(new { Message = "Account deleted successfully." });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
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
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}