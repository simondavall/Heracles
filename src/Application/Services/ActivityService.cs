﻿using System;
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

        public async Task<bool> DeleteActivityAsync(Guid trackId)
        {
            return await _trackRepository.DeleteTrackAsync(trackId);
        }

        public async Task<Track> GetActivityAsync(Guid trackId)
        {
            return await _trackRepository.GetTrackAsync(trackId);
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

        public async Task<List<ActivityListItem>> GetActivitiesByDateAsync(DateTime startDate, Guid? trackId = null)
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
                    Year = track.Time.Year.ToString(),
                    IsSelected = trackId == track.Id
                };
                activitiesList.Add(activityListItem);
            }

            return activitiesList;
        }

        public async Task<IList<ActivityListMonth>> GetActivitiesSummaryByMonthsAsync(Track track)
        {
            var activityMonthlySummary = await _trackRepository.GetTrackSummaryByMonthsAsync();
            var activities = await GetActivitiesByDateAsync(track.Time, track.Id);

            var selectedYearMonth = track.Time.Year * 100 + track.Time.Month;

            foreach (var summaryItem in activityMonthlySummary)
            {
                if (summaryItem.ActivityYearMonth == selectedYearMonth)
                {
                    summaryItem.Activities = activities;
                    break;
                }
            }

            return activityMonthlySummary;
        }

        public async Task<Track> GetFirstEverActivityAsync()
        {
            return await _trackRepository.GetFirstEverActivityAsync();
        }

        public async Task<Track> GetMostRecentActivityAsync()
        {
            return await _trackRepository.GetMostRecentTrackAsync();
        }

        public async Task<(int rank, int count)> GetActivityRankAsync(Track track)
        {
            var (upperBounds, lowerBounds) = ActivityRanking.GetRankBounds(track);
            var tracksInRange = await _trackRepository.GetTracksInRangeAsync(upperBounds, lowerBounds, track.ActivityType);

            var rank = ActivityRanking.GetRank(track, tracksInRange);

            return (rank, tracksInRange.Length);
        }
    }
}
