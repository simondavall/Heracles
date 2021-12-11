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
        public string DisplayFilesImported(int numberOfFiles)
        {
            return numberOfFiles == 1 ? "1 file" : numberOfFiles + " files";
        }
    }
}
