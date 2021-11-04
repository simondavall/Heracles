using System;
using Heracles.Application.TrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class CaloriesProcessor
    {
        internal static int GetCaloriesBurned(Track trackAggregate)
        {
            // met is the measure of calorie burn rate for a given activity
            double met = 11; // default set as met for running at 6.7mph (9 min miles)
            double weight = 90;
            TimeSpan duration = trackAggregate.Duration;

            if (trackAggregate.ActivityType == ActivityType.Cycling)
            {
                met = 10; // this is the met for cycling at between 14 - 15.5 mph
            }

            return Convert.ToInt32(Math.Round(CalculateCalories(met, duration.TotalMinutes, weight), MidpointRounding.AwayFromZero));

        }

        private static double CalculateCalories(double met, double minutes, double weight)
        {
            return 0.0175 * met * minutes * weight;
        }
    }
}
