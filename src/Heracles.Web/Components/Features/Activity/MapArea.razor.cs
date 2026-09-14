using Heracles.Application.TrackAggregate;
using Microsoft.AspNetCore.Components;

namespace Heracles.Web.Components.Features.Activity;

public partial class MapArea : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;
    
    [Parameter]
    public required Track Track { get; set; }

    private Guid? TrackId { get; set; }
    private string? SiteRootUrl { get; set; }

    protected override void OnParametersSet()
    {
        TrackId = Track.Id;
        SiteRootUrl = Navigation.BaseUri.TrimEnd('/');
    }
}