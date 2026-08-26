using System;

namespace StudentServiceRequestSystem.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly TimeZoneInfo LocalTz;

        static DateTimeExtensions()
        {
            try
            {
                LocalTz = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            }
            catch
            {
                try
                {
                    LocalTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Dhaka");
                }
                catch
                {
                    try
                    {
                        LocalTz = TimeZoneInfo.CreateCustomTimeZone("BST", TimeSpan.FromHours(6), "Bangladesh Standard Time", "Bangladesh Standard Time");
                    }
                    catch
                    {
                        LocalTz = TimeZoneInfo.Local;
                    }
                }
            }
        }

        /// <summary>
        /// Converts any UTC or database DateTime to Bangladesh Standard Time (UTC+6).
        /// </summary>
        public static DateTime ToBangladeshTime(this DateTime dt)
        {
            var utc = dt.Kind switch
            {
                DateTimeKind.Utc => dt,
                DateTimeKind.Local => dt.ToUniversalTime(),
                _ => DateTime.SpecifyKind(dt, DateTimeKind.Utc)
            };

            return TimeZoneInfo.ConvertTimeFromUtc(utc, LocalTz);
        }

        public static string ToDisplayDate(this DateTime dt, string format = "dd MMM yyyy")
        {
            return dt.ToBangladeshTime().ToString(format);
        }

        public static string ToDisplayDateTime(this DateTime dt, string format = "dd MMM yyyy, hh:mm tt")
        {
            return dt.ToBangladeshTime().ToString(format);
        }

        public static string ToDisplayDateTime(this DateTime? dt, string format = "dd MMM yyyy, hh:mm tt")
        {
            return dt.HasValue ? dt.Value.ToDisplayDateTime(format) : string.Empty;
        }
    }
}
