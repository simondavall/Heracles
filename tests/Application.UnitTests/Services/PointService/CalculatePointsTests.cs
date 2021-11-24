using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Heracles.Application.Entities.Points;
using Heracles.Application.Interfaces;
using Heracles.Application.TrackAggregate;
using NUnit.Framework;

namespace Heracles.Application.UnitTests.Services.PointService
{
    class CalculatePointsTests
    {
        private readonly IPointService _sut;
        private readonly Track _testTrack2Segments3Points;
        private readonly List<Point> _result;

        public CalculatePointsTests()
        {
            _sut = new Application.Services.Points.PointService();
            _testTrack2Segments3Points = new Track
            {
                //<trkpt lat = "51.361189000" lon="-0.346412000"><ele>29.5</ele><time>2021-11-17T16:17:56Z</time></trkpt>
                //<trkpt lat = "51.361053000" lon="-0.346040000"><ele>30.2</ele><time>2021-11-17T16:18:08Z</time></trkpt>
                //<trkpt lat = "51.360690000" lon="-0.345719000"><ele>31.3</ele><time>2021-11-17T16:18:23Z</time></trkpt>


                //<trkpt lat = "51.360023000" lon="-0.345009000"><ele>33.3</ele><time>2021-11-17T16:18:55Z</time></trkpt>
                //<trkpt lat = "51.359793000" lon="-0.344430000"><ele>33.7</ele><time>2021-11-17T16:19:10Z</time></trkpt>
                //<trkpt lat = "51.359893000" lon="-0.343620000"><ele>34.0</ele><time>2021-11-17T16:19:27Z</time></trkpt>

                TrackSegments = new List<TrackSegment>
                {
                    new() { TrackPoints = new List<TrackPoint>
                    {
                        new(){Elevation = 29.5, Time = DateTime.Parse("2021-11-17T16:17:56Z"), Latitude = 51.361189000, Longitude = -0.346412000}, 
                        new(){Elevation = 30.2, Time = DateTime.Parse("2021-11-17T16:18:08Z"), Latitude = 51.361053000, Longitude = -0.346040000}, 
                        new(){Elevation = 31.3, Time = DateTime.Parse("2021-11-17T16:18:23Z"), Latitude = 51.360690000, Longitude = -0.345719000}
                    } },
                    new() { TrackPoints = new List<TrackPoint>
                    {
                        new(){Elevation = 33.3, Time = DateTime.Parse("2021-11-17T16:18:55Z"), Latitude = 51.360023000, Longitude = -0.345009000}, 
                        new(){Elevation = 33.7, Time = DateTime.Parse("2021-11-17T16:19:10Z"), Latitude = 51.359793000, Longitude = -0.344430000}, 
                        new(){Elevation = 34.0, Time = DateTime.Parse("2021-11-17T16:19:27Z"), Latitude = 51.359893000, Longitude = -0.343620000}
                    } }
                }
            };
            _result = _sut.GetPoints(_testTrack2Segments3Points);
        }

        [Test]
        public void GetPoints_ReturnsListOfPoints()
        {
            _result.Should().BeOfType<List<Point>>();
        }

        [Test]
        public void GetPoints_FirstPoint_ReturnsTypeStart()
        {
            _result[0].Should().BeOfType<StartPoint>();
        }

        [Test]
        public void GetPoints_ThirdPoint_ReturnsTypePause()
        {
            _result[2].Should().BeOfType<PausePoint>();
        }

        [Test]
        public void GetPoints_FourthPoint_ReturnsTypeResume()
        {
            _result[3].Should().BeOfType<ResumePoint>();
        }

        [TestCaseSource(typeof(TestData), nameof(TestData.CheckReturnedPointType))]
        public string GetPoints_FourthPoint_ReturnsTypeResume(int index)
        {
            return _result[index].Type;
        }

        [Test]
        public void GetPoints_PausePoint_ReturnsZeroDeltaPause()
        {
            _result[2].DeltaPause.Should().Be(0);
        }

        [Test]
        public void GetPoints_ResumePoint_ReturnsNonZeroDeltaPause()
        {
            _result[3].DeltaPause.Should().Be(32_000);
        }

        [Test]
        public void GetPoints_ResumePoint_ReturnsZeroDeltaTime()
        {
            _result[3].DeltaTime.Should().Be(0);
        }
    }

    class TestData
    {
        public static IEnumerable CheckReturnedPointType
        {
            get
            {
                yield return new TestCaseData(0).Returns("StartPoint");
                yield return new TestCaseData(1).Returns("TrackPoint");
                yield return new TestCaseData(2).Returns("PausePoint");
                yield return new TestCaseData(3).Returns("ResumePoint");
                yield return new TestCaseData(4).Returns("TrackPoint");
                yield return new TestCaseData(5).Returns("EndPoint");
            }
        }
    }
}
