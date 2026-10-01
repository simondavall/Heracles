using System.Globalization;
using Heracles.Application.Data;

namespace Heracles.Application.Activities;

public interface IActivityService
{
    Task<bool> DeleteActivityAsync(Guid trackId);
    Task<Track?> GetActivityAsync(Guid trackId);
    Task<List<ActivityListItem>> GetActivitiesByDateAsync(DateTime startDate, Guid? trackId = null, ActivityType? activityType = null);
    Task<IList<ActivityListMonth>> GetActivitiesSummaryByMonthsAsync(Track track, ActivityType? activityType = null);
    Task<IList<ActivityListYear>> GetActivitiesSummaryByYearAsync(Track track, ActivityType? activityType = null);
    Task<Track?> GetFirstEverActivityAsync();
    Task<Track?> GetMostRecentActivityAsync(ActivityType? activityType = null);
    Task<(int rank, int count)> GetActivityRankAsync(Track track);
}

public class ActivityService : IActivityService
{
    private readonly ITrackRepository _trackRepository;

    public ActivityService(ITrackRepository trackRepository) {
        _trackRepository = trackRepository;
    }

    public async Task<bool> DeleteActivityAsync(Guid trackId) {
        return await _trackRepository.DeleteTrackAsync(trackId);
    }

    public async Task<Track?> GetActivityAsync(Guid trackId) {
        return await _trackRepository.GetTrackAsync(trackId);
    }

    public async Task<List<ActivityListItem>> GetActivitiesByDateAsync(DateTime startDate, Guid? trackId = null, ActivityType? activityType = null) {
        var firstOfMonth = new DateTime(startDate.Year, startDate.Month, 1);
        var firstOfNextMonth = firstOfMonth.AddMonths(1);

        var activitiesList = new List<ActivityListItem>();

        var activities = await _trackRepository.GetTracksByDateRangeAsync(firstOfMonth, firstOfNextMonth, activityType);
        foreach (var track in activities) {
            var activityListItem = new ActivityListItem {
                ActivityId = track.Id,
                DayOfMonth = track.Time.Day.ToString("D2"),
                Distance = track.Distance.ToString("0.00"),
                DistanceUnits = "km",
                ElapsedTime = ToFormattedString(track.Duration),
                Live = false,
                MainText = track.ActivityType.ToString(),
                Month = track.Time.ToString("MMM", CultureInfo.InvariantCulture),
                MonthNum = track.Time.Month.ToString("D2"),
                Type = "CARDIO",
                Username = "Unknown",
                Year = track.Time.Year.ToString(),
                IsSelected = trackId == track.Id
            };

            activitiesList.Add(activityListItem);
        }

        return activitiesList;
    }

    public async Task<IList<ActivityListMonth>> GetActivitiesSummaryByMonthsAsync(Track track, ActivityType? activityType = null) {
        var activities = await GetActivitiesByDateAsync(track.Time, track.Id, activityType);
        var selectedYearMonth = track.Time.Year * 100 + track.Time.Month;

        var activityMonthlySummary = await _trackRepository.GetTrackSummaryByMonthsAsync(activityType);
        foreach (var summaryItem in activityMonthlySummary) {
            if (summaryItem.ActivityYearMonth == selectedYearMonth) {
                summaryItem.Activities = activities;
                break;
            }
        }

        return activityMonthlySummary;
    }

    public async Task<IList<ActivityListYear>> GetActivitiesSummaryByYearAsync(Track track, ActivityType? activityType = null) {
        return await _trackRepository.GetTrackSummaryByYearAsync(activityType);
    }

    public async Task<Track?> GetFirstEverActivityAsync() {
        return await _trackRepository.GetFirstEverActivityAsync();
    }

    public async Task<Track?> GetMostRecentActivityAsync(ActivityType? activityType = null) {
        return await _trackRepository.GetMostRecentTrackAsync(activityType);
    }

    public async Task<(int rank, int count)> GetActivityRankAsync(Track track) {
        var (upperBounds, lowerBounds) = ActivityRanking.GetRankBounds(track);
        var tracksInRange = await _trackRepository.GetTracksInRangeAsync(upperBounds, lowerBounds, track.ActivityType);

        var rank = ActivityRanking.GetRank(track, tracksInRange);

        return (rank, tracksInRange.Length);
    }

    private static string ToFormattedString(TimeSpan span) {
        return span.TotalHours >= 1
            ? $"{span.TotalHours:#0}:{span.Minutes:00}:{span.Seconds:00}"
            : $"{span.Minutes:#0}:{span.Seconds:00}";
    }
}