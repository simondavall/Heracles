using System.Collections.Generic;
using Heracles.Application.TrackAggregate;

namespace Heracles.Application.Services.Import
{
    public class ImportFilesResult
    {
        public List<Track> Tracks { get; } = new();
        public List<TrackSegment> TrackSegments { get; } = new();
        public List<TrackPoint> TrackPoints { get; } = new();
        public List<FileResult> ImportedFiles { get; } = new();
        public List<FileResult> FailedFiles { get; } = new();
    }
}
