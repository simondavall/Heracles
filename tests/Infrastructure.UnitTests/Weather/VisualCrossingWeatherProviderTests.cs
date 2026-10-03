using System.Net;
using System.Text;
using Heracles.Application.Configuration;
using Heracles.Application.Weather;
using Heracles.Infrastructure.Weather;
using Xunit;

namespace Heracles.Infrastructure.UnitTests.Weather;

public sealed class VisualCrossingWeatherProviderTests
{
    private const string Endpoint = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline";
    private static readonly DateTime RequestedTime = new(2024, 4, 22, 8, 23, 0, DateTimeKind.Utc);
    private static readonly DateTime ExpectedHour = new(2024, 4, 22, 8, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task GetHistoricalWeatherAsync_RequestsUtcHourlyMetricData()
    {
        Uri? requestedUri = null;
        using var client = CreateClient(ValidResponse(), uri => requestedUri = uri);
        var provider = CreateProvider(client);

        await provider.GetHistoricalWeatherAsync(51.5074, -0.1278, RequestedTime,
            TestContext.Current.CancellationToken);

        Assert.NotNull(requestedUri);
        Assert.Contains("/51.507400,-0.127800/2024-04-22T08:00:00/", requestedUri.AbsoluteUri);
        Assert.Contains("unitGroup=metric", requestedUri.Query);
        Assert.Contains("include=hours", requestedUri.Query);
        Assert.Contains("timezone=Z", requestedUri.Query);
        Assert.Contains("iconSet=icons2", requestedUri.Query);
        Assert.Contains("elements=datetime,datetimeEpoch,temp,feelslike,conditions,icon", requestedUri.Query);
        Assert.Contains("key=test-key", requestedUri.Query);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_SelectsMatchingEpochAndMapsObservation()
    {
        using var client = CreateClient(ValidResponse());
        var provider = CreateProvider(client);

        var result = await provider.GetHistoricalWeatherAsync(51.5074, -0.1278, RequestedTime,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(12.5, result.Temperature);
        Assert.Equal(11.2, result.FeelsLike);
        Assert.Equal("Partially cloudy", result.Conditions);
        Assert.Equal(WeatherCode.PartlyCloudyDay, result.WeatherCode);
        Assert.Equal(ExpectedHour, result.ObservationTimeUtc);
        Assert.Equal(DateTimeKind.Utc, result.ObservationTimeUtc.Kind);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_NormalizesUnspecifiedTimestampAsUtc()
    {
        Uri? requestedUri = null;
        using var client = CreateClient(ValidResponse(), uri => requestedUri = uri);
        var provider = CreateProvider(client);
        var unspecified = DateTime.SpecifyKind(RequestedTime, DateTimeKind.Unspecified);

        var result = await provider.GetHistoricalWeatherAsync(51.5074, -0.1278, unspecified,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(ExpectedHour, result.ObservationTimeUtc);
        Assert.Contains("2024-04-22T08:00:00", requestedUri!.AbsoluteUri);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_HandlesMidnightOnFollowingUtcDay()
    {
        var midnight = new DateTime(2024, 4, 23, 0, 10, 0, DateTimeKind.Utc);
        var epoch = new DateTimeOffset(new DateTime(2024, 4, 23, 0, 0, 0, DateTimeKind.Utc))
            .ToUnixTimeSeconds();
        var response = Response($$"""
        { "datetimeEpoch": {{epoch}}, "temp": 8.0, "feelslike": 6.5,
          "conditions": "Clear", "icon": "clear-day" }
        """);
        Uri? requestedUri = null;
        using var client = CreateClient(response, uri => requestedUri = uri);
        var provider = CreateProvider(client);

        var result = await provider.GetHistoricalWeatherAsync(51.5074, -0.1278, midnight,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(new DateTime(2024, 4, 23, 0, 0, 0, DateTimeKind.Utc), result.ObservationTimeUtc);
        Assert.Contains("2024-04-23T00:00:00", requestedUri!.AbsoluteUri);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_ReturnsNullWhenRequestedHourMissing()
    {
        var response = Response("""
            { "datetimeEpoch": 1713769200, "temp": 10.0, "icon": "cloudy" }
            """); // 2024-04-22 07:00 UTC
        using var client = CreateClient(response);
        var provider = CreateProvider(client);

        var result = await provider.GetHistoricalWeatherAsync(51.5074, -0.1278, RequestedTime,
            TestContext.Current.CancellationToken);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"days\":null}")]
    [InlineData("{\"days\":[]}")]
    [InlineData("{\"days\":[{}]}")]
    [InlineData("{\"days\":[{\"hours\":[]}]}")]
    public async Task GetHistoricalWeatherAsync_ReturnsNullForMissingObservations(string response)
    {
        using var client = CreateClient(response);
        var provider = CreateProvider(client);

        var result = await provider.GetHistoricalWeatherAsync(51.5074, -0.1278, RequestedTime,
            TestContext.Current.CancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_MapsUnknownIconToUnknown()
    {
        var response = Response($$"""
        { "datetimeEpoch": {{new DateTimeOffset(ExpectedHour).ToUnixTimeSeconds()}},
          "temp": 12.5, "feelslike": null, "conditions": "Unusual", "icon": "unexpected-icon" }
        """);
        using var client = CreateClient(response);
        var provider = CreateProvider(client);

        var result = await provider.GetHistoricalWeatherAsync(51.5074, -0.1278, RequestedTime,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(12.5, result.Temperature);
        Assert.Null(result.FeelsLike);
        Assert.Equal("Unusual", result.Conditions);
        Assert.Equal(WeatherCode.Unknown, result.WeatherCode);
    }

    [Fact]
    public async Task GetHistoricalWeatherAsync_ThrowsOnHttpFailure()
    {
        using var client = CreateClient("{}", statusCode: HttpStatusCode.ServiceUnavailable);
        var provider = CreateProvider(client);

        await Assert.ThrowsAsync<HttpRequestException>(() => provider.GetHistoricalWeatherAsync(
            51.5074, -0.1278, RequestedTime, TestContext.Current.CancellationToken));
    }

    private static string ValidResponse()
    {
        var previous = new DateTimeOffset(ExpectedHour.AddHours(-1)).ToUnixTimeSeconds();
        var selected = new DateTimeOffset(ExpectedHour).ToUnixTimeSeconds();
        var following = new DateTimeOffset(ExpectedHour.AddHours(1)).ToUnixTimeSeconds();
        return $$"""
        {
          "days": [
            { "hours": [
              { "datetimeEpoch": {{previous}}, "temp": 10.0, "feelslike": 9.0,
                "conditions": "Overcast", "icon": "cloudy" },
              { "datetimeEpoch": {{selected}}, "temp": 12.5, "feelslike": 11.2,
                "conditions": "Partially cloudy", "icon": "partly-cloudy-day" },
              { "datetimeEpoch": {{following}}, "temp": 14.0, "feelslike": 13.0,
                "conditions": "Clear", "icon": "clear-day" }
            ] }
          ]
        }
        """;
    }

    private static string Response(string hourJson) => $$"""
    { "days": [{ "hours": [{{hourJson}}] }] }
    """;

    private static VisualCrossingWeatherProvider CreateProvider(HttpClient client) =>
        new(client, new WeatherApiSettings("test-key", new Uri(Endpoint)));

    private static HttpClient CreateClient(string response, Action<Uri>? onRequest = null,
        HttpStatusCode statusCode = HttpStatusCode.OK) =>
        new(new FakeHttpMessageHandler(response, onRequest, statusCode));

    private sealed class FakeHttpMessageHandler(
        string response,
        Action<Uri>? onRequest,
        HttpStatusCode statusCode) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri is not null)
                onRequest?.Invoke(request.RequestUri);

            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(response, Encoding.UTF8, "application/json")
            });
        }
    }
}
