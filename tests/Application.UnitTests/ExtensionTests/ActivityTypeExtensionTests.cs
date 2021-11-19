using FluentAssertions;
using Heracles.Application.Enums;
using Heracles.Application.Extensions;
using NUnit.Framework;

namespace Heracles.Application.UnitTests.ExtensionTests
{
    class ActivityTypeExtensionTests
    {
        [Test]
        public void GetImagePath_TypeRunning_DoesNotReturnNullOrEmpty()
        {
            const ActivityType activityType = ActivityType.Running;
            var result = activityType.GetImagePath();

            result.Should().NotBeNull();
            result.Should().NotBeNull();
        }

        [Test]
        public void GetTitleText_TypeRunning_ReturnsCorrectText()
        {
            const ActivityType activityType = ActivityType.Running;
            var result = activityType.GetTitleText();

            result.Should().Be("Run");
        }

        [Test]
        public void GetTitleText_TypeUnknown_ReturnsEmptyText()
        {
            const ActivityType activityType = ActivityType.Unknown;
            var result = activityType.GetTitleText();

            result.Should().Be(string.Empty);
        }
    }
}
