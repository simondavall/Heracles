using System;
using Newtonsoft.Json;

namespace Heracles.Application.Entities
{
    public class ActivityListItem
    {
        public string Month { get; set; }
        public string Distance { get; set; }
        public string DayOfMonth { get; set; }
        public string Year { get; set; }
        [JsonProperty("activity_id")]
        public Guid ActivityId { get; set; }

        public string DistanceUnits { get; set; }
        public string ElapsedTime { get; set; }
        public bool Live { get; set; }
        public string MainText { get; set; }

        public string MonthNum { get; set; }
        public string Type { get; set; }
        public string Username { get; set; }

    }
}
