namespace Heracles.Application.Resources
{
    public static class ImportServiceStrings
    {
        public static string DuplicateTrackRecord = "Duplicate track record.";
        public static string FileCouldNotBeProcessed = "File could not be processed.";
        public static string FailedToSaveImportedFiles = "Failed to save imported files to db. Check logs for more details.";
        public static string ImportSuccess = "Success";
        public static string IncorrectFileExtension = "Incorrect file extension. Not .gpx file.";
        public static string NoTrackSegmentsFound = "Imported file badly formed. Could not read/find <trgseg> segment tags";
        public static string NoTrackPointsFound = "Imported file badly formed. At lease one <trgseg> tag did not contain <trkpt> track point tags.";

    }
}
