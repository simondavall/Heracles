using System.ComponentModel.DataAnnotations;

namespace Heracles.Application.Data;

public class ActivityWeather
{
    public Guid TrackId { get; set; }
    public double? Temperature { get; set; }
    public double? FeelsLike { get; set; }
    public double? Humidity { get; set; }
    public double? Pressure { get; set; }
    [MaxLength(200)]
    public string? Conditions { get; set; }
    [MaxLength(100)]
    public string? Icon { get; set; }
    public DateTime ObservationTimeUtc { get; set; }
    
    public Track Track { get; set; } = null!;
}