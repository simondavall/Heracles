using System;
using FluentAssertions;
using Heracles.Application.TrackAggregate;
using Heracles.Infrastructure.Gpx.Processors;
using NUnit.Framework;

namespace Heracles.Infrastructure.UnitTests.Gpx.Processors
{
    [TestFixture]
    public class SpeedProcessorTests
    {
        private Track _track;

        [SetUp]
        public void BeforeEachTest()
        {
            _track = new Track
            {
                Duration = new TimeSpan(0, 0, 30, 0),
                Distance = 6
            };
        }

        [Test]
        public void GetAverageSpeed_ActivityTypeRunning_SpeedInKmPerHr()
        {
            var result = SpeedProcessor.GetAverageSpeed(_track);

            result.Should().Be(12d);
        }

        [Test]
        public void GetAverageSpeed_ActivityTypeCycling_SpeedInKmPerHr()
        {
            _track.Distance = 12;
            var result = SpeedProcessor.GetAverageSpeed(_track);

            result.Should().Be(24d);
        }

        [Test]
        public void GetAverageSpeed_DurationZero_ReturnsZeroAverageSpeed()
        {
            _track.Duration = TimeSpan.Zero;
            var result = SpeedProcessor.GetAverageSpeed(_track);

            result.Should().Be(0);
        }

        [Test]
        public void GetAverageSpeed_DistanceNegative_ReturnsZeroAverageSpeed()
        {
            _track.Distance = -10;
            var result = SpeedProcessor.GetAverageSpeed(_track);

            result.Should().Be(0);
        }
    }
}
