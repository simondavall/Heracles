using System.ComponentModel.DataAnnotations;
using Heracles.Application.Activities;

namespace Heracles.Application.Data
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
}
