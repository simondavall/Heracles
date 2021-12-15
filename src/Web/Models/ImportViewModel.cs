using System.Collections.Generic;
using Heracles.Application.Services.Import;

namespace Heracles.Web.Models
{
    public class ImportViewModel
    {
        public ImportViewModel()
        {
            FilesFailed = new List<FileResult>();
        }
        public int FilesImported { get; set; }
        public IList<FileResult> FilesFailed { get; set; }
        public SubNavigationViewModel SubNavigationViewModel { get; set; }
        public bool ImportExecuted { get; set; }
        public string FormatDisplay(int numberOfFiles)
        {
            return numberOfFiles == 1 ? "1 file" : numberOfFiles + " files";
        }

        public string FormatSuccessFormatSuccessfullyImported()
        {
            if (TotalFilesImported > 1)
            {
                return FilesImported + "/" + TotalFilesImported + " files";
            }

            return FilesImported + "/" + TotalFilesImported + " file";
        }

        private int TotalFilesImported => FilesImported + FilesFailed.Count;
    }
}
