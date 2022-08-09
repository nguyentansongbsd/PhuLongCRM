using System;
using System.IO;
using System.Threading.Tasks;

namespace PhuLongCRM.IServices
{
	public interface IPDFSaveAndOpen
	{
		Task SaveAndView(string fileName, String contentType, MemoryStream stream, PDFOpenContext context);
	}
	public enum PDFOpenContext
	{
		InApp,
		ChooseApp
	}
}

