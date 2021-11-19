using Heracles.Application.Attributes;

namespace Heracles.Application.Enums
{
    public enum ActivityType
    {
        [ActivityTitleImage("../images/icon-running.png")]
        [ActivityTitleText("")]
        Unknown,
        [ActivityTitleImage("../images/icon-running.png")]
        [ActivityTitleText("Run")]
        Running,
        [ActivityTitleImage("../images/icon-cycling.png")]
        [ActivityTitleText("Bike Ride")]
        Cycling
    }
}
