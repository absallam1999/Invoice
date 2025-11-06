using invoice.Helpers;
using invoice.Core.Interfaces;

namespace invoice.Core.Entities
{
    public class BaseEntity : IEntity
    {
        public string Id { get; set; } = GenerateShortSequentialId();
        public DateTime CreatedAt { get; set; } = GetSaudiTime.Now();
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        private static string GenerateShortSequentialId()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var random = Guid.NewGuid().GetHashCode() & 0xFFFF;

            string base36 = Base36Encode(timestamp ^ random);

            return base36.Substring(0, Math.Min(8, base36.Length));
        }

        private static string Base36Encode(long value)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = "";
            while (value > 0)
            {
                result = chars[(int)(value % 36)] + result;
                value /= 36;
            }
            return result;
        }
    }
}
