using invoice.Core.DTO;
using invoice.Core.DTO.Auth;

namespace invoice.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<GeneralResponse<object>> RegisterAsync(RegisterDTO dto);
        Task<GeneralResponse<LoginResultDTO>> LoginAsync(LoginDTO dto);
        Task<GeneralResponse<object>> UpdateUserAsync(UpdateUserDTO dto, string currentUserId);
        Task<GeneralResponse<object>> ForgetPasswordAsync(ForgetPasswordDTO dto);
        Task<GeneralResponse<object>> ResetPasswordAsync(ResetPasswordDTO dto);
        Task<GeneralResponse<object>> ChangePasswordAsync(ChangePasswordDTO dto, string userId);
        Task<GeneralResponse<object>> DeleteAccountAsync(string id, string currentUserId);
        Task<GeneralResponse<LoginResultDTO>> RefreshTokenAsync(string token);
        Task<GeneralResponse<object>> RevokeRefreshTokenAsync(string token);


    }
}
