using System;
using FluentAssertions;
using Heracles.Application.Activities;
using Heracles.Application.Data;
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
            _tracks = [
                new Track {Pace = new TimeSpan(1), Name = string.Empty},
                new Track {Pace = new TimeSpan(2), Name = string.Empty},
                new Track {Pace = new TimeSpan(2), Name = string.Empty},
                new Track {Pace = new TimeSpan(3), Name = string.Empty},
                new Track {Pace = new TimeSpan(4), Name = string.Empty}
            ];
        }

        [Test]
        public void GetTrackRankCalc_FastestActivity_ReturnsOne()
        {
            var fastestTrack = new Track { Pace = new TimeSpan(1), Name = string.Empty };

            var rank = ActivityRanking.GetRank(fastestTrack, _tracks);

            rank.Should().Be(1);
        }

        [Test]
        public void GetTrackRankCalc_EqualSecondFastest_ReturnsTwo()
        {
            var secondFastest = new Track { Pace = new TimeSpan(2), Name = string.Empty };

            var rank = ActivityRanking.GetRank(secondFastest, _tracks);

            rank.Should().Be(2);
        }

        [Test]
        public void GetTrackRankCalc_Slowest_ReturnsFiveOutOfFive()
        {
            var slowest = new Track { Pace = new TimeSpan(4), Name = string.Empty };

            var rank = ActivityRanking.GetRank(slowest, _tracks);

            rank.Should().Be(5);
        }
    }
}
