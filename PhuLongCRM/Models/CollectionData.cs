using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace PhuLongCRM.Models
{
    public class CollectionData
    {
        public string Id { get; set; }
        public string MediaSourceId { get; set; }
        public string ImageSource { get; set; }
        public string ImageSourceBase64 { get; set; }
        public SharePointType SharePointType { get; set; }
        public int Index { get; set; }
        public string UrlPdfFile { get; set; }
        public string PdfName { get; set; }
        public string IconFile { get => SharePointType == SharePointType.Pdf ? "\uf1c1" : SharePointType == SharePointType.Docx ? "\uf1c2" : null; }
        public Color ColorFile { get => SharePointType == SharePointType.Docx ? Color.FromHex("#1961BE") : SharePointType == SharePointType.Pdf ? Color.Red : Color.Black; }
        public string FileName { get; set; }

        public CollectionData()
        { }
    }
    public enum SharePointType
    {
        Video,
        Image,
        Pdf,
        Docx
    }
}
