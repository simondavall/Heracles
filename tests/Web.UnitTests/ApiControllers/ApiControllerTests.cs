using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Heracles.Application.Entities;
using Heracles.Application.Interfaces;
using Heracles.Web.Api;
using Moq;
using NUnit.Framework;

namespace Heracles.Web.UnitTests.ApiControllers
{
    class ApiControllerTests
    {
        private readonly Mock<IActivityService> _mockTrackService;
        private readonly ActivityController _sut;

        public ApiControllerTests()
        {
            _mockTrackService = new Mock<IActivityService>();
            _sut = new ActivityController(_mockTrackService.Object);
        }

        [Ignore("To be corrected")]
        [Test]
        public async Task ActivityListByDate_ReturnsData()
        {
            var activityId = Guid.Parse("fe0bd75d-aaef-4de7-9f8f-9135eb636955");
            _mockTrackService.Setup(x => x.GetActivitiesByDate(It.IsAny<DateTime>(), null))
                .ReturnsAsync(new List<ActivityListItem>()
            {
                new()
                {
                    ActivityId = activityId,
                    Year = "2021",
                    DayOfMonth = "31",
                    Distance = "3.59",
                    Month = "Oct"
                }
            });
            var result = await _sut.ActivityListByDate(null, new DateTime(2021, 10, 1), new DateTime(2021, 10, 31));

            result.Should().BeOfType<string>();
            result.Should()
                .Be(
                    "{\"activities\":{\"2021\":{\"Oct\":[{\"month\":\"Oct\",\"distance\":\"3.59\",\"dayOfMonth\":\"31\",\"year\":\"2021\",\"activity_id\":fe0bd75d-aaef-4de7-9f8f-9135eb636955}]}}}");
        }
    }
}
