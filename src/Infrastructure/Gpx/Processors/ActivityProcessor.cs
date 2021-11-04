using Heracles.Application.GpxTrackAggregate;

namespace Heracles.Infrastructure.Gpx.Processors
{
    public static class ActivityProcessor
    {
        public static ActivityType GetActivityType(TrackAggregate trackAggregate)
        {
            if (trackAggregate != null && !string.IsNullOrWhiteSpace(trackAggregate.Name))
            {
                // to do consider using reg from settings for matching, in order to extend to other gpx types
                // update: in fact the regex pattern should be part of the gpx setup, with a separate gpx template
                // for each type of import. E.g. Strava, Garmin, Runkeeper, etc
                if (trackAggregate.Name.Contains("Running")) return ActivityType.Running;
                if (trackAggregate.Name.Contains("Cycling")) return ActivityType.Cycling;
            }

            return ActivityType.Unknown;
        }
    }
}
 