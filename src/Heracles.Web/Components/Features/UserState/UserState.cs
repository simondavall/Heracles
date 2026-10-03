using Heracles.Application.Activities;

namespace Heracles.Web.Components.Features.UserState;

public sealed class UserState
{
    public bool? IsDarkMode { get; set; }
    public ActivityType? ActivityType { get; set; }
}