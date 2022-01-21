using System;
using FluentAssertions;
using Heracles.Application.Services;
using Heracles.Application.TrackAggregate;
using NUnit.Framework;

namespace Heracles.Application.UnitTests.Services
{
    [TestFixture]
    public class ActivityRankingTests
    {
        private Track[] _tracks;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _tracks = new Track[]
            {
                new() {Pace = new TimeSpan(1)},
                new() {Pace = new TimeSpan(2)},
                new() {Pace = new TimeSpan(2)},
                new() {Pace = new TimeSpan(3)},
                new() {Pace = new TimeSpan(4)}
            };
        }

        [Test]
        public void GetTrackRankCalc_FastestActivity_ReturnsOne()
        {
            var fastestTrack = new Track { Pace = new TimeSpan(1) };

            var rank = ActivityRanking.GetRank(fastestTrack, _tracks);

            rank.Should().Be(1);
        }

        [Test]
        public void GetTrackRankCalc_EqualSecondFastest_ReturnsTwo()
        {
            var secondFastest = new Track { Pace = new TimeSpan(2) };

            var rank = ActivityRanking.GetRank(secondFastest, _tracks);

            rank.Should().Be(2);
        }

        [Test]
        public void GetTrackRankCalc_Slowest_ReturnsFiveOutOfFive()
        {
            var slowest = new Track { Pace = new TimeSpan(4) };

            var rank = ActivityRanking.GetRank(slowest, _tracks);

            rank.Should().Be(5);
        }
    }
}
