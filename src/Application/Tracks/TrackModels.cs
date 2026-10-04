using System.ComponentModel.DataAnnotations;
using Heracles.Application.Activities;

namespace Heracles.Application.Tracks
{
    public class Track
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public ActivityType ActivityType { get; set; } = ActivityType.Unknown;
        public double Elevation { get; set; }
        public int Calories { get; set; }
        public TimeSpan Pace { get; set; } = TimeSpan.Zero;
        public double Speed { get; set; }

        public IList<TrackSegment> TrackSegments { get; set; } = [];
    }
    
    public class TrackSegment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Seq { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public double Elevation { get; set; }
        public int Calories { get; set; }
        public IList<TrackPoint> TrackPoints { get; set; } = [];
        public Guid TrackId { get; set; }
    }
    
    public class TrackPoint
    {
        public int Id { get; set; }
        public int Seq { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public Guid TrackSegmentId { get; set; }
    }
}
