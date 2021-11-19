using System;
using Heracles.Application.Enums;
using Heracles.Application.TrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class CaloriesProcessor
    {
        internal static int GetCaloriesBurned(Track track)
        {
            if (track.Duration <= TimeSpan.Zero)
            {
                return 0; // do not set calories for negative duration
            }
            //todo - improvements can be made here. met needs to be created by the activity based on the speed.
            // met is the measure of calorie burn rate for a given activity
            double met = 11; // default set as met for running at 6.7mph (9 min miles)
            const double weight = 90;
            var duration = track.Duration;

            if (track.ActivityType == ActivityType.Cycling)
            {
                met = 10; // this is the met for cycling at between 14 - 15.5 mph
            }

            var calories = Rounding(CalculateCalories(met, duration.TotalMinutes, weight));

            return Convert.ToInt32(calories);
        }

        private static double Rounding(double value)
        {
            return Math.Round(value, MidpointRounding.AwayFromZero);
        }

        private static double CalculateCalories(double met, double minutes, double weight)
        {
            return 0.0175 * met * minutes * weight;
        }
    }
}
