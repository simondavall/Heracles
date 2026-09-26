namespace Heracles.Application.Import
{
    public static class ImportServiceStrings
    {
        public const string DuplicateTrackRecord = "Duplicate track record";
        public const string FileCouldNotBeProcessed = "File could not be processed";
        public const string FailedToSaveImportedFiles = "Failed to save imported files to db. Check logs for more details.";
        public const string ImportSuccess = "Success";
        public const string IncorrectFileExtension = "Incorrect file extension. Not .gpx file";
        public const string NoTrackSegmentsFound = "Imported file badly formed. Could not read/find <trgseg> segment tags";
        public const string NoTrackPointsFound = "Imported file badly formed. At lease one <trgseg> tag did not contain <trkpt> track point tags";
        public const string NoTrackFound = "No track found";
    }
}
