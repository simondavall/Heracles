using Heracles.Application.Enums;
using Heracles.Application.TrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class ActivityProcessor
    {
        public static ActivityType GetActivityType(Track track)
        {
            if (track != null && !string.IsNullOrWhiteSpace(track.Name))
            {
                // to do consider using reg from settings for matching, in order to extend to other gpx types
                // update: in fact the regex pattern should be part of the gpx setup, with a separate gpx template
                // for each type of import. E.g. Strava, Garmin, Runkeeper, etc
                if (track.Name.Contains("Running")) return ActivityType.Running;
                if (track.Name.Contains("Cycling")) return ActivityType.Cycling;
            }

            return ActivityType.Unknown;
        }
    }
}
 