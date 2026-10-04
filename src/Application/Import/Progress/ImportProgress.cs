namespace Heracles.Application.Import.Progress;

public class ImportProgress
{
    private const decimal ProcessingWeight = 0.2M;
    private const decimal PersistenceWeight = 0.8M;

    private readonly Action<decimal>? _progress;

    private int _trackCount;
    private int _segmentCount;
    private int _pointCount;
    private int _totalRecords;

    private decimal _lastProgress;

    public ImportProgress(Action<decimal>? progress) {
        _progress = progress;
        TrackProgressMethod = ReportFileProgress;
    }

    public Action<decimal> TrackProgressMethod { get; private set; }

    public void UpdateWithProcessedFileData(ImportFilesResult result) {
        _trackCount = result.Tracks.Count;
        _segmentCount = result.TrackSegments.Count;
        _pointCount = result.TrackPoints.Count;

        _totalRecords = _trackCount + _segmentCount + _pointCount;
    }

    public void SetTrackingProgressMethod(ImportPhase phase) {
        TrackProgressMethod = phase switch {
            ImportPhase.FilesProcessing => ReportFileProgress,
            ImportPhase.TrackImport => ReportTrackProgress,
            ImportPhase.SegmentImport => ReportSegmentProgress,
            ImportPhase.PointsImport => ReportPointProgress,
            _ => TrackProgressMethod
        };
    }

    public void Complete() => Report(1M);

    private void ReportFileProgress(decimal progress) => Report(Normalize(progress) * ProcessingWeight);

    private void ReportTrackProgress(decimal progress) => ReportPersistenceProgress(progress, _trackCount, 0);

    private void ReportSegmentProgress(decimal progress) => ReportPersistenceProgress(progress, _segmentCount, _trackCount);

    private void ReportPointProgress(decimal progress) => ReportPersistenceProgress(progress, _pointCount, _trackCount + _segmentCount);

    private void ReportPersistenceProgress(decimal progress, int currentCount, int completedCount) {
        if (_totalRecords == 0)
            return;

        var processedRecords = completedCount + Normalize(progress) * currentCount;
        var persistenceProgress = processedRecords / _totalRecords;

        Report(ProcessingWeight + persistenceProgress * PersistenceWeight);
    }

    private static decimal Normalize(decimal progress) {
        // File-processing callbacks use fractions.
        // Bulk-insert callbacks may report percentages.
        if (progress > 1M) 
            progress /= 100M;

        return Math.Clamp(progress, 0M, 1M);
    }

    private void Report(decimal progress) {
        progress = Math.Clamp(progress, 0M, 1M);

        if (progress <= _lastProgress)
            return;
        
        _lastProgress = progress;
        _progress?.Invoke(progress);
    }
}