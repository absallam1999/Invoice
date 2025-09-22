namespace invoice.Core.DTO.Auth
{
    public class LoginResultDTO
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string phoneNum { get; set; }
        public string Token { get; set; }
    }
}
