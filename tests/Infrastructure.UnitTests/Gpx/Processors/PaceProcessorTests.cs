using System;
using FluentAssertions;
using Heracles.Application.TrackAggregate;
using Heracles.Infrastructure.Gpx.Processors;
using NUnit.Framework;

namespace Heracles.Infrastructure.UnitTests.Gpx.Processors
{
    [TestFixture]
    public class PaceProcessorTests
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
        public void GetAveragePace_Duration30Distance6_Returns5()
        {
            var result = PaceProcessor.GetAveragePace(_track);

            result.Minutes.Should().Be(5);
        }

        [Test]
        public void GetAveragePace_Duration30Distance12_Returns2mins30secs()
        {
            _track.Distance = 12;
            var result = PaceProcessor.GetAveragePace(_track);

            result.Minutes.Should().Be(2);
            result.Seconds.Should().Be(30);
        }

        [Test]
        public void GetAveragePace_TimespanZero_ReturnsTimespanZero()
        {
            _track.Duration = TimeSpan.Zero;
            var result = PaceProcessor.GetAveragePace(_track);

            result.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void GetAveragePace_NegativeTimespan_ReturnsTimespanZero()
        {
            _track.Duration = TimeSpan.MinValue;
            var result = PaceProcessor.GetAveragePace(_track);

            result.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void GetAveragePace_DistanceZero_ReturnsTimespanZero()
        {
            _track.Distance = 0;
            var result = PaceProcessor.GetAveragePace(_track);

            result.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void GetAveragePace_DistanceNegative_ReturnsTimespanZero()
        {
            _track.Distance = -10;
            var result = PaceProcessor.GetAveragePace(_track);

            result.Should().Be(TimeSpan.Zero);
        }
    }
}
