namespace Heracles.Web.Components.Features.ActivityDetails;

public sealed class ActivityDetailsInteractionService
{
    public event Action<int?>? TrackPointChanged;

    public int? TrackPointId { get; private set; }

    public void Select(int trackPointId) {
        if (TrackPointId == trackPointId)
            return;

        TrackPointId = trackPointId;
        TrackPointChanged?.Invoke(trackPointId);
    }

    public void Clear() {
        if (TrackPointId is null)
            return;

        TrackPointId = null;
        TrackPointChanged?.Invoke(null);
    }
}