using System.Collections.Generic;

namespace Heracles.Web.Models
{
    public class ImportViewModel
    {
        public ImportViewModel()
        {
            FilesFailed = new List<FailedFile>();
        }
        public int FilesImported { get; set; }
        public ICollection<FailedFile> FilesFailed { get; set; }

        public string FormatDisplay(int numberOfFiles)
        {
            return numberOfFiles == 1 ? "1 file" : numberOfFiles + " files";
        }

        public bool ImportExecuted { get; set; }
    }

    public class FailedFile
    {
        public string Filename { get; set; }
        public string ErrorMessage { get; set; }
    }
}
