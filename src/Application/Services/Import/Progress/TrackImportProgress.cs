using System;
using Heracles.Application.Enums;
using Heracles.Application.Interfaces;

namespace Heracles.Application.Services.Import.Progress
{
    public class TrackImportProgress
    {
        private readonly IImportProgressService _progressService;
        private const decimal ProgressWeighting = 0.2M;
        private int _totalRecordsToImport;
        private int _trackCount;
        private int _trackSegmentCount;
        private int _trackPointCount;
        private int _filesProcessedCount;
        private decimal _percentageComplete;

        public TrackImportProgress(IImportProgressService progressService, Guid processId)
        {
            _progressService = progressService;
            ProcessId = processId;
            
            InitializeImportProgress(processId);
        }

        public Guid ProcessId { get; private set; }

        public Action<decimal> TrackProgressMethod;

        private void InitializeImportProgress(Guid processId)
        {
            ProcessId = processId;
            TrackProgressMethod = GetFilesProcessedProgress;
            _progressService.InitializeProgress(ProcessId);
        }

        public void UpdateWithProcessedFileData(ImportFilesResult importFilesResult)
        {
            _trackCount = importFilesResult.Tracks.Count;
            _trackSegmentCount = importFilesResult.TrackSegments.Count;
            _trackPointCount = importFilesResult.TrackPoints.Count;

            _totalRecordsToImport = _trackCount + _trackSegmentCount + _trackPointCount;

            _filesProcessedCount = (int)(_totalRecordsToImport * ProgressWeighting / (1 - ProgressWeighting));
            _totalRecordsToImport += _filesProcessedCount;

            _progressService.InitializeProgress(ProcessId);
        }

        public void SetTrackingProgressMethod(TrackImportMethod method)
        {
            TrackProgressMethod = method switch
            {
                TrackImportMethod.FilesProcessing => GetFilesProcessedProgress,
                TrackImportMethod.TrackImport => GetTracksProgress,
                TrackImportMethod.SegmentImport => GetTrackSegmentsProgress,
                TrackImportMethod.PointsImport => GetTrackPointsProgress,
                _ => TrackProgressMethod
            };
        }

        private void GetFilesProcessedProgress(decimal percentage)
        {
            _percentageComplete = percentage * ProgressWeighting;
            _progressService.UpdateProgress(ProcessId, _percentageComplete);
        }

        private void GetTracksProgress(decimal percentage)
        {
            _percentageComplete = (percentage * _trackCount + _filesProcessedCount) / _totalRecordsToImport;
            _progressService.UpdateProgress(ProcessId, _percentageComplete);
        }

        private void GetTrackSegmentsProgress(decimal percentage)
        {
            _percentageComplete = (percentage * _trackSegmentCount + _filesProcessedCount + _trackCount) / _totalRecordsToImport;
            _progressService.UpdateProgress(ProcessId, _percentageComplete);
        }

        private void GetTrackPointsProgress(decimal percentage)
        {
            _percentageComplete = (percentage * _trackPointCount + _filesProcessedCount + _trackCount + _trackSegmentCount) / _totalRecordsToImport;
            _progressService.UpdateProgress(ProcessId, _percentageComplete);
        }
    }
}
