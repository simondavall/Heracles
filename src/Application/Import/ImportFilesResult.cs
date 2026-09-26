using System.Collections.Generic;
using Heracles.Application.Data;

namespace Heracles.Application.Import
{
    public record ImportFilesResult
    {
        public List<Track> Tracks { get; } = [];
        public List<TrackSegment> TrackSegments { get; } = [];
        public List<TrackPoint> TrackPoints { get; } = [];
        public List<FileResult> ImportedFiles { get; } = [];
        public List<FileResult> FailedFiles { get; } = [];
    }

    public record FileResult(string Filename, string Reason);

}
