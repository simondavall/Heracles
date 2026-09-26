using Newtonsoft.Json;

namespace Heracles.Application.Activities;

     public class ActivityListItem
     {
         public required string Month { get; set; }
         public required string Distance { get; set; }
         public required string DayOfMonth { get; set; }
         public required string Year { get; set; }
         [JsonProperty("activity_id")]
         public required Guid ActivityId { get; set; }

         public required string DistanceUnits { get; set; }
         public required string ElapsedTime { get; set; }
         public required bool Live { get; set; }
         public required string MainText { get; set; }

         public required string MonthNum { get; set; }
         public required string Type { get; set; }
         public required string Username { get; set; }
         public required bool IsSelected { get; set; }
     }
     
     public class ActivityListMonth
     {
         public int ActivityYearMonth { get; set; }
         public int Count { get; set; }
         public List<ActivityListItem> Activities { get; set; } = [];
     }
     
     public class ActivityListYear
     {
         public int ActivityYear { get; set; }
         public int Count { get; set; }
     }