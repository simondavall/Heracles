using Heracles.Application.Data;
using Heracles.Application.Weather;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Heracles.Application.UnitTests.Weather;

public sealed class WeatherServiceTests
{
    private static readonly DateTime StartTime =
        new(2024, 4, 22, 7, 58, 0, DateTimeKind.Utc);

    private const double StartLatitude = 51.5074;
    private const double StartLongitude = -0.1278;

    [Fact]
    public async Task GetActivityWeatherAsync_ReturnsCachedWeather() {
        var track = CreateTrack();
        var cachedWeather = new ActivityWeather { TrackId = track.Id, Temperature = 12.5, FeelsLike = 11.2, Conditions = "Partly cloudy" };
        var repository = new FakeWeatherRepository { CachedWeather = cachedWeather };
        var provider = new FakeWeatherProvider();
        var service = CreateService(repository, provider);

        var result = await service.GetActivityWeatherAsync(track, TestContext.Current.CancellationToken);

        Assert.Same(cachedWeather, result);

        Assert.Equal(1, repository.GetCount);
        Assert.Equal(0, repository.SaveCount);
        Assert.Equal(0, provider.CallCount);
    }

    [Fact]
    public async Task GetActivityWeatherAsync_CacheMissRetrievesAndPersistsWeather() {
        var track = CreateTrack();
        var repository = new FakeWeatherRepository();
        var provider = new FakeWeatherProvider {
            Observation = new WeatherObservation(12.5, 11.2, "Partly cloudy")
        };
        var service = CreateService(repository, provider);

        var result = await service.GetActivityWeatherAsync(track, TestContext.Current.CancellationToken);

        Assert.NotNull(result);

        Assert.Equal(track.Id, result.TrackId);
        Assert.Equal(12.5, result.Temperature);
        Assert.Equal(11.2, result.FeelsLike);
        Assert.Equal("Partly cloudy", result.Conditions);

        Assert.Equal(1, repository.GetCount);
        Assert.Equal(1, repository.SaveCount);
        Assert.Equal(1, provider.CallCount);

        Assert.Same(result, repository.SavedWeather);
    }

    [Fact]
    public async Task GetActivityWeatherAsync_UsesStartingLocationAndMidpointTime() {
        // The activity starts at 07:58 and lasts 50 minutes.
        // Its midpoint is therefore 08:23.

        var track = CreateTrack(startTime: StartTime, duration: TimeSpan.FromMinutes(50));

        // Add a later point at a different location.
        // Its coordinates must not affect the weather request.

        track.TrackSegments[0].TrackPoints.Add(new TrackPoint { Seq = 2, Latitude = 52.0000, Longitude = -1.0000, Time = StartTime.AddMinutes(25) });

        var repository = new FakeWeatherRepository();
        var provider = new FakeWeatherProvider {
            Observation = new WeatherObservation(12.5, 11.2, "Partly cloudy")
        };
        var service = CreateService(repository, provider);

        await service.GetActivityWeatherAsync(track, TestContext.Current.CancellationToken);

        Assert.Equal(1, provider.CallCount);
        Assert.Equal(StartLatitude, provider.RequestedLatitude);
        Assert.Equal(StartLongitude, provider.RequestedLongitude);
        Assert.Equal(new DateTime(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc), provider.RequestedTimestamp);
    }

    [Fact]
    public async Task GetActivityWeatherAsync_MidpointCanCrossMidnight() {
        var startTime = new DateTime(2024, 4, 22, 23, 50, 0, DateTimeKind.Utc);
        var track = CreateTrack(startTime: startTime, duration: TimeSpan.FromMinutes(40));
        var repository = new FakeWeatherRepository();
        var provider = new FakeWeatherProvider {
            Observation = new WeatherObservation(8.0, 6.5, "Clear sky")
        };
        var service = CreateService(repository, provider);

        await service.GetActivityWeatherAsync(track, TestContext.Current.CancellationToken);

        Assert.Equal(new DateTime(2024, 4, 23, 0, 10, 0, DateTimeKind.Utc), provider.RequestedTimestamp);
    }

    [Fact]
    public async Task GetActivityWeatherAsync_SelectsFirstPointBySequence() {
        // Deliberately insert segments and points out of order.

        var track = CreateTrack();
        track.TrackSegments.Clear();
        track.TrackSegments.Add(
            new TrackSegment {
                Seq = 2, TrackPoints = [new TrackPoint { Seq = 1, Latitude = 53.0, Longitude = -2.0, Time = StartTime.AddMinutes(20) }]
            });

        track.TrackSegments.Add(
            new TrackSegment {
                Seq = 1,
                TrackPoints = [
                    new TrackPoint { Seq = 2, Latitude = 52.0, Longitude = -1.0, Time = StartTime.AddMinutes(5) },
                    new TrackPoint { Seq = 1, Latitude = StartLatitude, Longitude = StartLongitude, Time = StartTime }
                ]
            });

        var repository = new FakeWeatherRepository();

        var provider = new FakeWeatherProvider {
            Observation = new WeatherObservation(12.5, 11.2, "Partly cloudy")
        };

        var service = CreateService(repository, provider);

        await service.GetActivityWeatherAsync(track, TestContext.Current.CancellationToken);

        Assert.Equal(StartLatitude, provider.RequestedLatitude);
        Assert.Equal(StartLongitude, provider.RequestedLongitude);

        Assert.Equal(
            StartTime.Add(track.Duration / 2),
            provider.RequestedTimestamp);
    }
    
    [Fact]
    public async Task GetActivityWeatherAsync_NoGpsPointsReturnsNull() {
        var track = CreateTrack();
        track.TrackSegments.Clear();
        
        var repository = new FakeWeatherRepository();
        var provider = new FakeWeatherProvider();
        var service = CreateService(repository, provider);

        var result = await service.GetActivityWeatherAsync(track, TestContext.Current.CancellationToken);

        Assert.Null(result);

        Assert.Equal(1, repository.GetCount);
        Assert.Equal(0, repository.SaveCount);
        Assert.Equal(0, provider.CallCount);
    }

    [Fact]
    public async Task GetActivityWeatherAsync_NoObservationDoesNotPersistWeather() {
        var track = CreateTrack();
        var repository = new FakeWeatherRepository();
        var provider = new FakeWeatherProvider { Observation = null };
        var service = CreateService(repository, provider);

        var result = await service.GetActivityWeatherAsync(track, TestContext.Current.CancellationToken);

        Assert.Null(result);

        Assert.Equal(1, provider.CallCount);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task GetActivityWeatherAsync_ProviderFailureReturnsNull() {
        var track = CreateTrack();
        var repository = new FakeWeatherRepository();
        var provider = new FakeWeatherProvider { Exception = new HttpRequestException("Weather API unavailable") };
        var service = CreateService(repository, provider);

        var result = await service.GetActivityWeatherAsync(track, TestContext.Current.CancellationToken);

        Assert.Null(result);

        Assert.Equal(1, provider.CallCount);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task GetActivityWeatherAsync_CancellationIsPropagated() {
        var track = CreateTrack();
        var repository = new FakeWeatherRepository();
        var provider = new FakeWeatherProvider { Exception = new OperationCanceledException() };
        var service = CreateService(repository, provider);

        using var cancellationTokenSource = new CancellationTokenSource();

        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.GetActivityWeatherAsync(
            track,
            cancellationTokenSource.Token));

        Assert.Equal(0, repository.SaveCount);
    }

    private static WeatherService CreateService(FakeWeatherRepository repository, FakeWeatherProvider provider) {
        return new WeatherService(repository, provider, NullLogger<WeatherService>.Instance);
    }

    private static Track CreateTrack(DateTime? startTime = null,
        TimeSpan? duration = null) {
        var time = startTime ?? StartTime;

        var track = new Track { Name = "Test activity", Time = time, Duration = duration ?? TimeSpan.FromMinutes(50) };

        track.TrackSegments.Add(
            new TrackSegment {
                TrackId = track.Id,
                Seq = 1,
                TrackPoints = [new TrackPoint { Seq = 1, Latitude = StartLatitude, Longitude = StartLongitude, Time = time }]
            });

        return track;
    }

    private sealed class FakeWeatherRepository : IWeatherRepository
    {
        public ActivityWeather? CachedWeather { get; init; }
        public ActivityWeather? SavedWeather { get; private set; }
        public int GetCount { get; private set; }
        public int SaveCount { get; private set; }

        public Task<ActivityWeather?> GetAsync(Guid trackId,
            CancellationToken cancellationToken = default) {
            GetCount++;

            return Task.FromResult(CachedWeather);
        }

        public Task SaveAsync(ActivityWeather weather,
            CancellationToken cancellationToken = default) {
            SaveCount++;

            SavedWeather = weather;

            return Task.CompletedTask;
        }
    }

    private sealed class FakeWeatherProvider : IWeatherProvider
    {
        public WeatherObservation? Observation { get; init; }
        public Exception? Exception { get; init; }
        public int CallCount { get; private set; }
        public double? RequestedLatitude { get; private set; }
        public double? RequestedLongitude { get; private set; }
        public DateTime? RequestedTimestamp { get; private set; }

        public Task<WeatherObservation?> GetHistoricalWeatherAsync(double latitude,
            double longitude,
            DateTime timestampUtc,
            CancellationToken cancellationToken = default) {
            CallCount++;

            RequestedLatitude = latitude;
            RequestedLongitude = longitude;
            RequestedTimestamp = timestampUtc;

            if (Exception is not null)
                throw Exception;

            return Task.FromResult(Observation);
        }
    }
}