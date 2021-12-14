using System;
using System.IO;

namespace PhuLongCRM.Controls
{
    public interface IFileService
    {
        void SaveFile(string name, byte[] data, string location = "Download/Conasi");
        void OpenFile(string fileName, string location = "Download/Conasi");
    }
}
