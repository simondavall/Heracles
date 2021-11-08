using FluentAssertions;
using Heracles.Application.TrackAggregate;
using Heracles.Infrastructure.Gpx.Processors;
using NUnit.Framework;

namespace Heracles.Infrastructure.UnitTests.Gpx.Processors
{
    [TestFixture]
    public class ActivityProcessorTests
    {
        [Test]
        public void GetActivityType_RunningName_ReturnsRunningActivityType()
        {
            var track = new Track {Name = "[Running 20/7/17 12:25 pm]"};
            var result = ActivityProcessor.GetActivityType(track);

            result.Should().Be(ActivityType.Running);
        }
        [Test]
        public void GetActivityType_CyclingName_ReturnsCyclingActivityType()
        {
            var track = new Track { Name = "[Cycling 9/7/17 7:53 am]]"};
            var result = ActivityProcessor.GetActivityType(track);

            result.Should().Be(ActivityType.Cycling);
        }
        [Test]
        public void GetActivityType_EmptyTrackName_ReturnsUnknownActivityType()
        {
            var track = new Track { Name = string.Empty};
            var result = ActivityProcessor.GetActivityType(track);

            result.Should().Be(ActivityType.Unknown);
        }
        [Test]
        public void GetActivityType_NullTrackName_ReturnsUnknownActivityType()
        {
            var result = ActivityProcessor.GetActivityType(null);

            result.Should().Be(ActivityType.Unknown);
        }
        [Test]
        public void GetActivityType_UnrecognizedTrackName_ReturnsUnknownActivityType()
        {
            var track = new Track { Name = "unrecognized-entry" };
            var result = ActivityProcessor.GetActivityType(track);

            result.Should().Be(ActivityType.Unknown);
        }
    }
}
