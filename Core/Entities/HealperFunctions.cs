namespace invoice.Core.Entities
{
    public class HelperFunctions
    {


        public static DateTime GetSaudiTime()
        {
            var saTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, saTimeZone);
        }
    }
}
