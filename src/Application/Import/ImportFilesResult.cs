using System.Collections.Generic;
using Heracles.Application.Data;
//using Heracles.Application.Services.Import;

namespace Heracles.Application.Import
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
