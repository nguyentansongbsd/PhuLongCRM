using System;
namespace PhuLongCRM.IServices
{
    public interface IDownLoaderService
    {
        void DownloadFile(string url,string folder);
        event EventHandler<DownloadEventArgs> OnFileDownloaded;
    }
    public class DownloadEventArgs : EventArgs
    {
        public bool FileSaved = false;
        public DownloadEventArgs(bool fileSaved)
        {
            FileSaved = fileSaved;
        }
    }
}
