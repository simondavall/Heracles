using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Heracles.Application.Entities;
using Heracles.Application.Extensions;
using Heracles.Application.Interfaces;
using Heracles.Application.Specifications;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly ITrackRepository _trackRepository;
        private readonly IPointService _pointService;

        public ActivityService(ITrackRepository trackRepository, IPointService pointService)
        {
            _trackRepository = trackRepository;
            _pointService = pointService;
        }

        public async Task<ActivityInfo> GetActivityInfoAsync(Guid trackId)
        {
            var track = await _trackRepository.GetTrackAsync(trackId);
            var activityInfo = new ActivityInfo
            {
                Points = _pointService.GetPoints(track)
            };

            return activityInfo;
        }

        public async Task<List<ActivityListItem>> GetActivitiesByDate(DateTime startDate)
        {
            var firstOfMonth = new DateTime(startDate.Year, startDate.Month, 1);
            var firstOfNextMonth = firstOfMonth.AddMonths(1);
            var activitiesByDateSpec = new ActivitiesByDateSpec(firstOfMonth, firstOfNextMonth);
            var activities = await _trackRepository.ListAsync(activitiesByDateSpec);
            var activitiesList = new List<ActivityListItem>();
            foreach (var track in activities)
            {
                var activityListItem = new ActivityListItem
                {
                    ActivityId = track.Id,
                    DayOfMonth = track.Time.Day.ToString("D2"),
                    Distance = track.Distance.ToString("0.00"),
                    DistanceUnits = "km",
                    ElapsedTime = track.Duration.ToFormattedString(),
                    Live = false,
                    MainText = track.ActivityType.ToString(),
                    Month = track.Time.ToString("MMM", CultureInfo.InvariantCulture),
                    MonthNum = track.Time.Month.ToString("D2"),
                    Type = "CARDIO",
                    Username = "Unknown",
                    Year = track.Time.Year.ToString()
                };
                activitiesList.Add(activityListItem);
            }

            return activitiesList;
        }

        public async Task<IList<ActivityListMonth>> GetActivitiesSummaryByMonths()
        {
            return await _trackRepository.GetTrackSummaryByMonths();
        }

        public async Task<Track> GetMostRecentActivity()
        {
            return await _trackRepository.GetMostRecentTrack();
        }
    }
}
