using System;
using FluentAssertions;
using Heracles.Application.Extensions;
using NUnit.Framework;

namespace Heracles.Application.UnitTests.ExtensionTests
{
    class TimespanExtensionTests
    {
        [Test]
        public void ToFormattedString_2Hours2Min2Secs_ReturnsCorrectFormat()
        {
            var timespan = new TimeSpan(2, 2, 2);
            var result = timespan.ToFormattedString();

            result.Should().Be("02:02:02");
        }

        [Test]
        public void ToFormattedString_2Min2Secs_ReturnsCorrectFormat()
        {
            var timespan = new TimeSpan(0, 2, 2);
            var result = timespan.ToFormattedString();

            result.Should().Be("2:02");
        }

        [Test]
        public void ToFormattedString_24Hours2Min2Secs_ReturnsCorrectFormat()
        {
            var timespan = new TimeSpan(24, 2, 2);
            var result = timespan.ToFormattedString();

            result.Should().Be("24:02:02");
        }

        [Test]
        public void ToFormattedString_0Hours0Min0ecs_ReturnsCorrectFormat()
        {
            var timespan = new TimeSpan(0, 0, 0);
            var result = timespan.ToFormattedString();

            result.Should().Be("0:00");
        }

        [Test]
        public void ToFormattedString_36Hours2Min2Secs_ReturnsCorrectFormat()
        {
            var timespan = new TimeSpan(36, 2, 2);
            var result = timespan.ToFormattedString();

            result.Should().Be("36:02:02");
        }
    }
}
