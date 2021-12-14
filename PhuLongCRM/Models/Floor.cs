using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace PhuLongCRM.Models
{
    public class Floor 
    {
        public Guid bsd_floorid { get; set; }
        public string bsd_name { get; set; }
        
        public List<Unit> Units { get; set; } = new List<Unit>();

        public string NumChuanBiInFloor { get; set; }
        public string NumSanSangInFloor { get; set; }
        public string NumGiuChoInFloor { get; set; }
        public string NumDatCocInFloor { get; set; }
        public string NumDongYChuyenCoInFloor { get; set; }
        public string NumDaDuTienCocInFloor { get; set; }
        public string NumThanhToanDot1InFloor { get; set; }
        public string NumDaBanInFloor { get; set; }
    }
}
