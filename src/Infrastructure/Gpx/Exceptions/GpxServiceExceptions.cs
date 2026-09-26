namespace Heracles.Infrastructure.Gpx.Exceptions
{
    public sealed class TrackCreationException : Exception
    {
        private const string CreateFailure = "GpxService Failed to create TrackAggregate for file {0}";
        public TrackCreationException(string fileName, Exception inner) 
            : base(string.Format(CreateFailure, fileName), inner) { }
    }
}
