using System.ComponentModel.DataAnnotations;
using Heracles.Application.Data;

namespace Heracles.Application.Weather;

public class ActivityWeather
{
    public Guid TrackId { get; init; }
    public double? Temperature { get; init; }
    public double? FeelsLike { get; init; }
    [MaxLength(100)]
    public string? Conditions { get; init; }
    public WeatherCode? WeatherCode { get; init; }
    public DateTime ObservationTimeUtc { get; set; }
    
    public Track Track { get; init; } = null!;
}