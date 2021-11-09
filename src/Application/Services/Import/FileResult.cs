namespace Heracles.Application.Services.Import
{
    public class FileResult
    {
        public FileResult(string filename, string reason)
        {
            Filename = filename;
            Reason = reason;
        }

        public string Filename { get; init; }

        public string Reason { get; init; }
    }
}
