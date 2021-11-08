using System;
using FluentAssertions;
using Heracles.Application.TrackAggregate;
using Heracles.Infrastructure.Gpx.Processors;
using NUnit.Framework;

namespace Heracles.Infrastructure.UnitTests.Gpx.Processors
{
    public class CaloriesProcessorTests
    {
        [Test]
        public void GetCaloriesBurned_PositiveDuration_ReturnsGreaterThanZeroCalories()
        {
            var track = new Track
            {
                ActivityType = ActivityType.Running,
                Duration = TimeSpan.FromMinutes(30)
            };

            var result = CaloriesProcessor.GetCaloriesBurned(track);

            result.Should().BeGreaterThan(0);
        }

        [Test]
        public void GetCaloriesBurned_ZeroDuration_ReturnsZeroCalories()
        {
            var track = new Track
            {
                ActivityType = ActivityType.Running,
                Duration = TimeSpan.Zero
            };

            var result = CaloriesProcessor.GetCaloriesBurned(track);

            result.Should().Be(0);
        }

        [Test]
        public void GetCaloriesBurned_NegativeDuration_ReturnsZeroCalories()
        {
            var track = new Track
            {
                ActivityType = ActivityType.Running,
                Duration = TimeSpan.MinValue
            };

            var result = CaloriesProcessor.GetCaloriesBurned(track);

            result.Should().Be(0);
        }

        [Test]
        public void GetCaloriesBurned_RunningCaloriesGreaterThanCyclingCalories()
        {
            //todo - this test highlights an issue. Cycling at a high rate should have a higher calorie burn.
            // but because one value is used, cycling will always be less than running regardless of effort.
            // Need to change the values to depend on work rate.
            var trackRunning = new Track
            {
                ActivityType = ActivityType.Running,
                Duration = TimeSpan.FromMinutes(30)
            };
            var trackCycling = new Track
            {
                ActivityType = ActivityType.Cycling,
                Duration = TimeSpan.FromMinutes(30)
            };

            var resultRunning = CaloriesProcessor.GetCaloriesBurned(trackRunning);
            var resultCycling = CaloriesProcessor.GetCaloriesBurned(trackCycling);

            resultRunning.Should().BeGreaterThan(resultCycling);
        }
    }
}
