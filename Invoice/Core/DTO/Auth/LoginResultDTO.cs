using System.Text.Json.Serialization;

namespace invoice.Core.DTO.Auth
{
    public class LoginResultDTO
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string phoneNum { get; set; }
        public string? TabAccountId { get; set; }

        public string Token { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpirtion { get; set; }
    }
}
