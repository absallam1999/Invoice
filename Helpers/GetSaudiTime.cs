namespace invoice.Helpers
{
    public static class GetSaudiTime
    {
        public static DateTime Now()
        {
            try
            {
                TimeZoneInfo saTimeZone = null;

                string[] timeZoneIds = {
                    "Arab Standard Time",           // Windows
                    "Asia/Riyadh",                  // Linux/macOS
                    "Asia/Kuwait",                  // Alternative
                    "Asia/Bahrain",                 // Alternative
                    "Asia/Qatar"                    // Alternative
                };

                foreach (var timeZoneId in timeZoneIds)
                {
                    try
                    {
                        saTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                        break;
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        continue;
                    }
                }

                if (saTimeZone == null)
                {
                    return DateTime.UtcNow.AddHours(3);
                }

                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, saTimeZone);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting Saudi time: {ex.Message}. Falling back to UTC+3.");
                return DateTime.UtcNow.AddHours(3);
            }
        }

        public static DateTime ConvertToSaudiTime(DateTime dateTime)
        {
            try
            {
                TimeZoneInfo saTimeZone = null;

                string[] timeZoneIds = {
                    "Arab Standard Time",
                    "Asia/Riyadh",
                    "Asia/Kuwait",
                    "Asia/Bahrain",
                    "Asia/Qatar"
                };

                foreach (var timeZoneId in timeZoneIds)
                {
                    try
                    {
                        saTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                        break;
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        continue;
                    }
                }

                if (saTimeZone == null)
                {
                    return dateTime.Kind == DateTimeKind.Utc
                        ? dateTime.AddHours(3)
                        : dateTime.ToUniversalTime().AddHours(3);
                }

                if (dateTime.Kind == DateTimeKind.Utc)
                {
                    return TimeZoneInfo.ConvertTimeFromUtc(dateTime, saTimeZone);
                }
                else
                {
                    return TimeZoneInfo.ConvertTime(dateTime, saTimeZone);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting to Saudi time: {ex.Message}. Falling back to UTC+3.");
                return dateTime.Kind == DateTimeKind.Utc
                    ? dateTime.AddHours(3)
                    : dateTime.ToUniversalTime().AddHours(3);
            }
        }

        public static DateTime Today()
        {
            return Now().Date;
        }

        public static DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }

        public static bool IsDuringBusinessHours(DateTime? dateTime = null)
        {
            var time = dateTime ?? Now();
            return time.Hour >= 9 && time.Hour < 17; // 9 AM to 5 PM
        }

        public static DateTime StartOfDay(DateTime? date = null)
        {
            var saudiDate = date ?? Today();
            return new DateTime(saudiDate.Year, saudiDate.Month, saudiDate.Day, 0, 0, 0, DateTimeKind.Local);
        }

        public static DateTime EndOfDay(DateTime? date = null)
        {
            var saudiDate = date ?? Today();
            return new DateTime(saudiDate.Year, saudiDate.Month, saudiDate.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        public static DateTime AddBusinessDays(DateTime date, int days)
        {
            var result = date;
            var direction = days < 0 ? -1 : 1;
            var remainingDays = Math.Abs(days);

            while (remainingDays > 0)
            {
                result = result.AddDays(direction);
                if (result.DayOfWeek != DayOfWeek.Friday && result.DayOfWeek != DayOfWeek.Saturday)
                {
                    remainingDays--;
                }
            }

            return result;
        }

        public static string ToSaudiTimeString(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            var saudiTime = ConvertToSaudiTime(dateTime);
            return saudiTime.ToString(format);
        }

        public static TimeSpan GetTimeUntilNextBusinessHour()
        {
            var now = Now();
            var nextBusinessHour = now;

            if (now.Hour >= 17)
            {
                nextBusinessHour = AddBusinessDays(now.Date.AddDays(1), 0).AddHours(9);
            }
            else if (now.Hour < 9)
            {
                nextBusinessHour = now.Date.AddHours(9);
            }
            else
            {
                return TimeSpan.Zero;
            }

            return nextBusinessHour - now;
        }
    }
}