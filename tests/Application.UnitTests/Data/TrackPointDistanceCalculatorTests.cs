using Heracles.Application.Tracks;
using Xunit;

namespace Heracles.Application.UnitTests.Data;

public sealed class TrackPointDistanceCalculatorTests
{
    [Fact]
    public void Calculate_ReturnsZeroForSameLocation()
    {
        var first = Point(51.5, -0.1);
        var second = Point(51.5, -0.1);

        var distance = TrackPointDistanceCalculator.Calculate(first, second);

        Assert.Equal(0, distance);
    }

    [Fact]
    public void Calculate_ReturnsExpectedDistance()
    {
        var first = Point(51.5000, -0.1000);
        var second = Point(51.5009, -0.1000);

        var distance = TrackPointDistanceCalculator.Calculate(first, second);

        Assert.InRange(distance, 0.099, 0.101);
    }

    private static TrackPoint Point(double latitude, double longitude)
    {
        return new TrackPoint
        {
            Latitude = latitude,
            Longitude = longitude
        };
    }
}