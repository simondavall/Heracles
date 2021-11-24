using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Heracles.Application.Entities.Points;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using NUnit.Framework;

namespace Heracles.Application.UnitTests.Services.PointService
{
    class CreatePointServiceTests
    {
        private readonly IPointService _sut;
        private readonly Track _testTrack3Points;
        private readonly Track _testTrack3Segments3Points;

        public CreatePointServiceTests()
        {
            _sut = new Application.Services.Points.PointService();
            
            _testTrack3Points = new Track
            {
                TrackSegments = new List<TrackSegment>
                {
                    new() { TrackPoints = new List<TrackPoint> { new(), new(), new() } }
                }
            };

            _testTrack3Segments3Points = new Track
            {
                TrackSegments = new List<TrackSegment>
                {
                    new() { TrackPoints = new List<TrackPoint> { new(), new(), new(), new() } },
                    new() { TrackPoints = new List<TrackPoint> { new(), new(), new(), new() } },
                    new() { TrackPoints = new List<TrackPoint> { new(), new(), new() } }
                }
            };
        }

        [Test]
        public void GetPoints_ReturnsListOfPoints()
        {
            var result = _sut.GetPoints(_testTrack3Points);

            result.Should().BeOfType<List<Point>>();
        }

        [Test]
        public void GetPoints_TrackWithThreePoints_ReturnsListOfThreePoints()
        {
            var result = _sut.GetPoints(_testTrack3Points);

            result.Count.Should().Be(3);
        }

        [TestCaseSource(typeof(TestData), nameof(TestData.OneSegmentsThreePointsReturnsCorrectType))]
        public string GetPoints_TrackWithThreePoints_ReturnsCorrectPointType(int index)
        {
            var result = _sut.GetPoints(_testTrack3Points);

             return result[index].Type;
        }

        [TestCaseSource(typeof(TestData), nameof(TestData.ThreeSegmentsFourPointsReturnsCorrectType))]
        public string GetPoints_TrackWith3Segments3Points_ReturnsCorrectPointType(int index)
        {
            var result = _sut.GetPoints(_testTrack3Segments3Points);

            return result[index].Type;
        }

        class TestData
        {
            public static IEnumerable ThreeSegmentsFourPointsReturnsCorrectType
            {
                get
                {
                    yield return new TestCaseData(0).Returns("StartPoint");
                    yield return new TestCaseData(1).Returns("TrackPoint");
                    yield return new TestCaseData(2).Returns("TrackPoint");
                    yield return new TestCaseData(3).Returns("PausePoint");
                    yield return new TestCaseData(4).Returns("ResumePoint");
                    yield return new TestCaseData(5).Returns("TrackPoint");
                    yield return new TestCaseData(6).Returns("TrackPoint");
                    yield return new TestCaseData(7).Returns("PausePoint");
                    yield return new TestCaseData(8).Returns("ResumePoint");
                    yield return new TestCaseData(9).Returns("TrackPoint");
                    yield return new TestCaseData(10).Returns("EndPoint");
                }
            }

            public static IEnumerable OneSegmentsThreePointsReturnsCorrectType
            {
                get
                {
                    yield return new TestCaseData(0).Returns("StartPoint");
                    yield return new TestCaseData(1).Returns("TrackPoint");
                    yield return new TestCaseData(2).Returns("EndPoint");
                }
            }
        }
    }
}
