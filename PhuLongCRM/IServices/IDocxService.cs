using System;
using System.Threading.Tasks;

namespace PhuLongCRM.IServices
{
    public interface IDocxService
    {
        Task OpenDocxFile(string url,string fileType);
    }
}
