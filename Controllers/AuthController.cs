using invoice.Core.DTO.Auth;
using invoice.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto) =>
            Ok(await _authService.RegisterAsync(dto));

        //[HttpPost("login")]
        //public async Task<IActionResult> Login1([FromBody] LoginDTO dto) =>
        //    Ok(await _authService.LoginAsync(dto));



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
                return BadRequest(result);
            if(!string.IsNullOrEmpty(result.Data.RefreshToken))
           
            SetRefreshTokenInCookIes(result.Data.RefreshToken, result.Data.RefreshTokenExpirtion);

            return Ok(result);
        }

      

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        //{
        //    var response = await _authService.LoginAsync(dto);

        //    if (!response.Success)
        //        return Unauthorized(response);

        //    var loginData = response.Data as LoginResultDTO;
        //    if (loginData == null)
        //        return Unauthorized(response);

        //    SetRefreshTokenInCookIes(
        //        loginData.RefreshToken,
        //        loginData.RefreshTokenExpirtion
        //    );



        //    return Ok(response);
        //}
        [HttpGet("refreshtoken")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies[key: "refreshToken"];
            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            SetRefreshTokenInCookIes(result.Data.RefreshToken, result.Data.RefreshTokenExpirtion);
            return Ok(result);  

        }

        [HttpPost("revokerefreshtoken")]
        public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RevokeRefreshTokenDTO dto)
        {
            var token = dto.RefreshToken ?? Request.Cookies[key: "refreshToken"];

            if (string.IsNullOrEmpty(token)) { 
                return BadRequest("token is required");
            }
            return Ok( await _authService.RevokeRefreshTokenAsync(token));
           
        }


        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _authService.UpdateUserAsync(dto, currentUserId));
        }

        [HttpPost("forget")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO dto) =>
            Ok(await _authService.ForgetPasswordAsync(dto));

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto) =>
            Ok(await _authService.ResetPasswordAsync(dto));

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _authService.ChangePasswordAsync(dto, currentUserId));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _authService.DeleteAccountAsync(id, currentUserId));
        }

        private void SetRefreshTokenInCookIes(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.AddHours(2),
                ////SameSite = SameSiteMode.Strict,     // lax if diffrant domain

            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}