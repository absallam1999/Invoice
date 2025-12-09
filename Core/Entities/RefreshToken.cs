using invoice.Helpers;
using Microsoft.EntityFrameworkCore;
namespace invoice.Core.Entities
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }

        public DateTime Expration { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expration;

        public DateTime CreateOn { get; set; } = GetSaudiTime.Now();
        public DateTime? RevokedOn { get; set; }
        public bool IsActive => RevokedOn == null && !IsExpired;
           }
}
