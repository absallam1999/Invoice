using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using invoice.Core.DTO;
using invoice.Core.DTO.Auth;
using invoice.Core.Entities;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using invoice.Helpers;


namespace invoice.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<GeneralResponse<object>> RegisterAsync(RegisterDTO dto)
        {
            var user = new ApplicationUser { UserName = dto.UserName, Email = dto.Email ,PhoneNumber=dto.PhoneNumber };
            var userDB = await _userManager.FindByEmailAsync(dto.Email);
            if (userDB != null)
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "This email is already registered.",
                    Data = null,
                };
            }
            var result = await _userManager.CreateAsync(user, dto.Password);

            return result.Succeeded
                ? new GeneralResponse<object>
                {
                    Success = true,
                    Message = "User registered successfully.",
                    Data = user.Id,
                }
                : new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User registration failed.",
                    Data = result.Errors,
                };
        }

        public async Task<GeneralResponse<LoginResultDTO>> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "Invalid login attempt.",
                    Data = null,
                };

            var token = GenerateJwtToken(user);
            RefreshToken refreshToken;
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                 refreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);

            }
            else
            {
                refreshToken = GenerateRefreshToken();
                  

                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);

            }
            return new GeneralResponse<LoginResultDTO>
            {
                Success = true,
                Message = "Login successful.",

                Data = new LoginResultDTO
                {
                    Token = token,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpirtion = refreshToken.Expration,
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    phoneNum = user.PhoneNumber,
                    TabAccountId = user.TabAccountId

                }
            };
            }

        public async Task<GeneralResponse<object>> UpdateUserAsync(
            UpdateUserDTO dto,
            string currentUserId
        )
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null)
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null,
                };

            if (user.Id != currentUserId)
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Not authorized to update this user.",
                    Data = null,
                };

            user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? new GeneralResponse<object>
                {
                    Success = true,
                    Message = "User updated successfully.",
                    Data = null,
                }
                : new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Error updating user.",
                    Data = result.Errors,
                };
        }

        public async Task<GeneralResponse<object>> ForgetPasswordAsync(ForgetPasswordDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null,
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return new GeneralResponse<object>
            {
                Success = true,
                Message = "Password reset token generated.",
                Data = token,
            };
        }

        public async Task<GeneralResponse<object>> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null,
                };

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            return result.Succeeded
                ? new GeneralResponse<object>
                {
                    Success = true,
                    Message = "Password reset successfully.",
                    Data = null,
                }
                : new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Error resetting password.",
                    Data = result.Errors,
                };
        }

        public async Task<GeneralResponse<object>> ChangePasswordAsync(
            ChangePasswordDTO dto,
            string userId
        )
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null,
                };

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword
            );
            return result.Succeeded
                ? new GeneralResponse<object>
                {
                    Success = true,
                    Message = "Password changed successfully.",
                    Data = null,
                }
                : new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Error changing password.",
                    Data = result.Errors,
                };
        }

        public async Task<GeneralResponse<object>> DeleteAccountAsync(
            string id,
            string currentUserId
        )
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null,
                };

            if (user.Id != currentUserId)
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Not authorized to delete this user.",
                    Data = null,
                };

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded
                ? new GeneralResponse<object>
                {
                    Success = true,
                    Message = "Account deleted successfully.",
                    Data = null,
                }
                : new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Error deleting user.",
                    Data = result.Errors,
                };
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
               expires: DateTime.Now.AddMinutes(15),  //GetSaudiTime.Now()
               //   expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
       public async Task<GeneralResponse<LoginResultDTO>> RefreshTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u=>u.RefreshTokens.Any(t=>t.Token==token));
            if(user == null)
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "Invalid token.",
                    Data = null,
                };
            }
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                return new GeneralResponse<LoginResultDTO>
                {
                    Success = false,
                    Message = "inactive token.",
                    Data = null,
                };

            }
            refreshToken.RevokedOn= GetSaudiTime.Now();
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = GenerateJwtToken(user);

            return new GeneralResponse<LoginResultDTO>
            {
                Success = true,
                Message = "Login successful.",

                Data = new LoginResultDTO
                {
                    Token = jwtToken,
                    RefreshToken = newRefreshToken.Token,
                    RefreshTokenExpirtion = newRefreshToken.Expration,
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    phoneNum = user.PhoneNumber,
                    TabAccountId = user.TabAccountId

                }
            };
        }
            public async Task<GeneralResponse<object>> RevokeRefreshTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "Invalid token.",
                    Data = null,
                };
            }
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                return new GeneralResponse<object>
                {
                    Success = false,
                    Message = "inactive token.",
                    Data = null,
                };

            }

            refreshToken.RevokedOn = GetSaudiTime.Now();
            await _userManager.UpdateAsync(user);
            return new GeneralResponse<object>
            {
                Success = true,
                Message = "RefreshToken is revoked",
                Data = null,
            };

        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return  new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expration = GetSaudiTime.Now().AddDays(10)
                };
            }
        }

    }
}
