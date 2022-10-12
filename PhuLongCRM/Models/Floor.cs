using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace PhuLongCRM.Models
{
    public class Floor : BaseViewModel
    {
        public Guid bsd_floorid { get; set; }
        public string bsd_name { get; set; }
        //public List<Unit> Units { get; set; } = new List<Unit>();
        public ObservableCollection<Unit> Units { get; set; } = new ObservableCollection<Unit>();
        //private ObservableCollection<Unit> _units;
        //public ObservableCollection<Unit> Units { get => _units; set { _units = value; OnPropertyChanged(nameof(Units)); } }

        public int _numChuanBiInFloor;
        public int NumChuanBiInFloor { get => _numChuanBiInFloor; set { _numChuanBiInFloor = value; OnPropertyChanged(nameof(NumChuanBiInFloor)); } }

        public int _umSanSangInFloor;
        public int NumSanSangInFloor { get => _umSanSangInFloor; set { _umSanSangInFloor = value; OnPropertyChanged(nameof(NumSanSangInFloor)); } }

        public int _numBookingInFloor;
        public int NumBookingInFloor { get => _numBookingInFloor; set { _numBookingInFloor = value; OnPropertyChanged(nameof(NumBookingInFloor)); } }

        public int _numGiuChoInFloor;
        public int NumGiuChoInFloor { get => _numGiuChoInFloor; set { _numGiuChoInFloor = value; OnPropertyChanged(nameof(NumGiuChoInFloor)); } }

        public int _numDatCocInFloor;
        public int NumDatCocInFloor { get => _numDatCocInFloor; set { _numDatCocInFloor = value; OnPropertyChanged(nameof(NumDatCocInFloor)); } }

        public int _numDongYChuyenCoInFloor;
        public int NumDongYChuyenCoInFloor { get => _numDongYChuyenCoInFloor; set { _numDongYChuyenCoInFloor = value; OnPropertyChanged(nameof(NumDongYChuyenCoInFloor)); } }

        public int _numDaDuTienCocInFloor;
        public int NumDaDuTienCocInFloor { get => _numDaDuTienCocInFloor; set { _numDaDuTienCocInFloor = value; OnPropertyChanged(nameof(NumDaDuTienCocInFloor)); } }

        public int _numOptionInFloor;
        public int NumOptionInFloor { get => _numOptionInFloor; set { _numOptionInFloor = value; OnPropertyChanged(nameof(NumOptionInFloor)); } }

        public int _numThanhToanDot1InFloor;
        public int NumThanhToanDot1InFloor { get => _numThanhToanDot1InFloor; set { _numThanhToanDot1InFloor = value; OnPropertyChanged(nameof(NumThanhToanDot1InFloor)); } }

        public int _numSignedDAInFloor;
        public int NumSignedDAInFloor { get => _numSignedDAInFloor; set { _numSignedDAInFloor = value; OnPropertyChanged(nameof(NumSignedDAInFloor)); } }

        public int _numQualifiedInFloor;
        public int NumQualifiedInFloor { get => _numQualifiedInFloor; set { _numQualifiedInFloor = value; OnPropertyChanged(nameof(NumQualifiedInFloor)); } }

        public int _numDaBanInFloor;
        public int NumDaBanInFloor { get => _numDaBanInFloor; set { _numDaBanInFloor = value; OnPropertyChanged(nameof(NumDaBanInFloor)); } }

        public int _totalUnitInFloor;
        public int TotalUnitInFloor { get => _totalUnitInFloor; set { _totalUnitInFloor = value; OnPropertyChanged(nameof(TotalUnitInFloor)); } }

        private bool _iShow;
        public bool iShow { get => _iShow; set { _iShow = value; OnPropertyChanged(nameof(iShow)); } }
    }
}
