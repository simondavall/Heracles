using System.Collections.Generic;
using Heracles.Application.Entities.Points;

namespace Heracles.Application.Entities
{
    public class ActivityInfo
    {
        public IList<Point> Points { get; set; }

        //TODO Maybe able to delete these. Implement Activity controller first.
        //public bool IsLive { get; set; }
        //public string StatsDuration { get; set; }
        //public string ActivityType { get; set; }
        //public string StatsPace { get; set; }
        //public string ActivityEntryType { get; set; }
    }
}
