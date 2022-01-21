using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heracles.Application.Entities;
using Heracles.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Heracles.Web.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _trackService;

        public ActivityController(IActivityService trackService)
        {
            _trackService = trackService;
        }

        [HttpGet]
        public async Task<string> ActivityListByDate(string username, DateTime startDate, DateTime endDate)
        {
            var activities = await _trackService.GetActivitiesByDateAsync(startDate);
            if (activities is null || activities.Count == 0)
            {
                return string.Empty;
            }

            return FormatToJson(activities);
        }

        [HttpGet]
        public async Task<ActivityInfo> GetActivityInfo(string trackId)
        {
            if (!Guid.TryParse(trackId, out var trackGuid))
            {
                throw new ArgumentException("trackId must be a valid Guid.");
            }

            return await _trackService.GetActivityInfoAsync(trackGuid);
        }

        [HttpGet]
        public async Task<bool> Delete(string trackId)
        {
            if (!Guid.TryParse(trackId, out var trackGuid))
            {
                return false;
            }

            return await _trackService.DeleteActivityAsync(trackGuid);
        }

        private static string FormatToJson(IReadOnlyList<ActivityListItem> activities)
        {
            var jArray = JArray.FromObject(activities,
                new JsonSerializer { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var jResult = new JObject
            {
                ["activities"] = new JObject
                {
                    [activities[0].Year] = new JObject
                    {
                        [activities[0].Month] = jArray
                    }
                }
            };

            return jResult.ToString(Formatting.None);
        }
    }
}
