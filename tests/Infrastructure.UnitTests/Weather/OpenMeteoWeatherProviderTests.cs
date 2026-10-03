using System.Net;
using System.Text;
using Heracles.Application.Configuration;
using Heracles.Application.Weather;
using Heracles.Infrastructure.Weather;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Heracles.Infrastructure.UnitTests.Weather;

public sealed class OpenMeteoWeatherProviderTests
{
    private const string Endpoint = "https://archive-api.open-meteo.com/v1/archive/fake";

    [Fact]
    public async Task GetHistoricalWeatherAsync_RequestsGmtAndCorrectDate()
    {
        Uri? requestedUri = null;

        var provider = CreateProvider(ValidResponse(), uri => requestedUri = uri);
        var timestamp = new DateTime(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc);
        
        await provider.GetHistoricalWeatherAsync(
            51.5074,
            -0.1278,
            timestamp, 
            TestContext.Current.CancellationToken);
        
        Assert.NotNull(requestedUri);

        var query = requestedUri.Query;

        Assert.Contains("latitude=51.507400", query);
        Assert.Contains("longitude=-0.127800", query);
        Assert.Contains("start_date=2024-04-22", query);
        Assert.Contains("end_date=2024-04-22", query);
        Assert.Contains("timezone=GMT", query);
        Assert.Contains("hourly=weather_code,temperature_2m,apparent_temperature", query);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_SelectsMatchingHour()
    {
        var provider = CreateProvider(ValidResponse());
        var timestamp = new DateTime(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc);
        
        var result = await provider.GetHistoricalWeatherAsync(
            51.5074,
            -0.1278,
            timestamp, 
            TestContext.Current.CancellationToken);
        
        Assert.NotNull(result);
        Assert.Equal(12.5, result.Temperature);
        Assert.Equal(11.2, result.FeelsLike);
        Assert.Equal("Partly cloudy", result.Conditions);
        Assert.Equal(WeatherCode.PartlyCloudyDay, result.WeatherCode);
        Assert.Equal(new DateTime(2024, 4, 22, 8, 0, 0, DateTimeKind.Utc), result.ObservationTimeUtc);
        Assert.Equal(DateTimeKind.Utc, result.ObservationTimeUtc.Kind);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_DoesNotAssumeArrayPosition()
    {
        // The response deliberately contains only two hours,
        // neither of which occupies its normal 24-hour array index.

        const string response = """
        {
            "hourly": {
                "time": [
                    "2024-04-22T07:00",
                    "2024-04-22T08:00"
                ],
                "temperature_2m": [10.0, 12.5],
                "apparent_temperature": [9.0, 11.2],
                "weather_code": [3, 2]
            }
        }
        """;

        var provider = CreateProvider(response);
        var timestamp = new DateTime(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc);
        
        var result = await provider.GetHistoricalWeatherAsync(
            51.5074,
            -0.1278,
            timestamp, 
            TestContext.Current.CancellationToken);
        
        Assert.NotNull(result);
        Assert.Equal(12.5, result.Temperature);
        Assert.Equal("Partly cloudy", result.Conditions);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_ReturnsNullWhenHourMissing()
    {
        const string response = """
        {
            "hourly": {
                "time": ["2024-04-22T07:00"],
                "temperature_2m": [10.0],
                "apparent_temperature": [9.0],
                "weather_code": [3]
            }
        }
        """;

        var provider = CreateProvider(response);
        var timestamp = new DateTime(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc);
        
        var result = await provider.GetHistoricalWeatherAsync(
            51.5074,
            -0.1278,
            timestamp, 
            TestContext.Current.CancellationToken);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_ReturnsNullForEmptyObservation()
    {
        const string response = """
        {
            "hourly": {
                "time": ["2024-04-22T08:00"],
                "temperature_2m": [null],
                "apparent_temperature": [null],
                "weather_code": [null]
            }
        }
        """;

        var provider = CreateProvider(response);
        var timestamp = new DateTime(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc);
        
        var result = await provider.GetHistoricalWeatherAsync(
            51.5074,
            -0.1278,
            timestamp, 
            TestContext.Current.CancellationToken);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_AcceptsPartialObservation()
    {
        const string response = """
        {
            "hourly": {
                "time": ["2024-04-22T08:00"],
                "temperature_2m": [12.5],
                "apparent_temperature": [null],
                "weather_code": [2]
            }
        }
        """;

        var provider = CreateProvider(response);
        var timestamp = new DateTime(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc);
        
        var result = await provider.GetHistoricalWeatherAsync(
            51.5074,
            -0.1278,
            timestamp, 
            TestContext.Current.CancellationToken);
        
        Assert.NotNull(result);
        Assert.Equal(12.5, result.Temperature);
        Assert.Null(result.FeelsLike);
        Assert.Equal("Partly cloudy", result.Conditions);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_ReturnsUnknownForUnrecognisedCode()
    {
        const string response = """
        {
            "hourly": {
                "time": ["2024-04-22T08:00"],
                "temperature_2m": [12.5],
                "apparent_temperature": [11.2],
                "weather_code": [999]
            }
        }
        """;

        var provider = CreateProvider(response);
        var timestamp = new DateTime(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc);
        
        var result = await provider.GetHistoricalWeatherAsync(
            51.5074,
            -0.1278,
            timestamp, 
            TestContext.Current.CancellationToken);
        
        Assert.NotNull(result);
        Assert.Equal("Unknown", result.Conditions);
        Assert.Equal(WeatherCode.Unknown, result.WeatherCode);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_ReturnsNullForMismatchedArrays()
    {
        const string response = """
        {
            "hourly": {
                "time": [
                    "2024-04-22T07:00",
                    "2024-04-22T08:00"
                ],
                "temperature_2m": [10.0],
                "apparent_temperature": [9.0, 11.2],
                "weather_code": [3, 2]
            }
        }
        """;

        var provider = CreateProvider(response);
        var timestamp = new DateTime(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc);
        
        var result = await provider.GetHistoricalWeatherAsync(
            51.5074,
            -0.1278,
            timestamp, 
            TestContext.Current.CancellationToken);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_ThrowsOnHttpFailure()
    {
        var provider = CreateProvider("{}", statusCode: HttpStatusCode.ServiceUnavailable);
        var timestamp = new DateTime(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc);
        
        await Assert.ThrowsAsync<HttpRequestException>(
            () => provider.GetHistoricalWeatherAsync(
                51.5074,
                -0.1278,
                timestamp, 
                TestContext.Current.CancellationToken));
    }

    private static OpenMeteoWeatherProvider CreateProvider(string response, Action<Uri>? onRequest = null, 
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new FakeHttpMessageHandler(response, onRequest, statusCode);

        var httpClient = new HttpClient(handler);

        var settings = new WeatherApiSettings(string.Empty, new Uri(Endpoint));

        return new OpenMeteoWeatherProvider(
            httpClient,
            settings,
            NullLogger<OpenMeteoWeatherProvider>.Instance);
    }

    private static string ValidResponse() => """
    {
        "hourly": {
            "time": [
                "2024-04-22T07:00",
                "2024-04-22T08:00",
                "2024-04-22T09:00"
            ],
            "temperature_2m": [10.0, 12.5, 14.0],
            "apparent_temperature": [9.0, 11.2, 13.0],
            "weather_code": [3, 2, 0]
        }
    }
    """;

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;
        private readonly Action<Uri>? _onRequest;
        private readonly HttpStatusCode _statusCode;

        public FakeHttpMessageHandler(string response, Action<Uri>? onRequest, HttpStatusCode statusCode)
        {
            _response = response;
            _onRequest = onRequest;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri is not null)
                _onRequest?.Invoke(request.RequestUri);

            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_response, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }
}