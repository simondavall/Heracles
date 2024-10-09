using System;

namespace Heracles.Application.Extensions
{
    public static class TimespanExtensions
    {
        public static string ToFormattedString(this TimeSpan span)
        {
            if (span.TotalHours >= 1)
            {
                return $"{span.TotalHours:#0}:{span.Minutes:00}:{span.Seconds:00}";
            }

            return $"{span.Minutes:#0}:{span.Seconds:00}";
        }
    }
}
