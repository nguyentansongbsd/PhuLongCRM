using System;
using System.Threading.Tasks;

namespace PhuLongCRM.IServices
{
    public interface IOpenFileService
    {
        Task OpenFile(string fileName, byte[] arr = null, string url = null, string folder = "Download/PhuLongFiles");
        Task OpenFilePdfFromUrl(string fileName, string url);
    }
}
