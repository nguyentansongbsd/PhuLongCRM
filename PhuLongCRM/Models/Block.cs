using PhuLongCRM.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PhuLongCRM.Models
{
    public class Block : BaseViewModel
    {
        public Guid bsd_blockid { get; set; }
        public string bsd_name { get; set; }

        private string _numChuanBiInBlock;
        public string NumChuanBiInBlock { get=>_numChuanBiInBlock; set { _numChuanBiInBlock = value;OnPropertyChanged(nameof(NumChuanBiInBlock)); } }

        private string _numSanSangInBlock;
        public string NumSanSangInBlock { get=>_numSanSangInBlock; set { _numSanSangInBlock = value;OnPropertyChanged(nameof(NumSanSangInBlock)); } }

        private string _numGiuChoInBlock;
        public string NumGiuChoInBlock { get=>_numGiuChoInBlock; set { _numGiuChoInBlock = value; OnPropertyChanged(nameof(NumGiuChoInBlock)); } }

        private string _numDatCocInBlock;
        public string NumDatCocInBlock { get=>_numDatCocInBlock; set { _numDatCocInBlock = value; OnPropertyChanged(nameof(NumDatCocInBlock)); } }

        private string _numDongYChuyenCoInBlock;
        public string NumDongYChuyenCoInBlock { get=>_numDongYChuyenCoInBlock; set { _numDongYChuyenCoInBlock = value;OnPropertyChanged(nameof(NumDongYChuyenCoInBlock)); } }

        private string _numDaDuTienCocInBlock;
        public string NumDaDuTienCocInBlock { get=>_numDaDuTienCocInBlock; set { _numDaDuTienCocInBlock = value;OnPropertyChanged(nameof(NumDaDuTienCocInBlock)); } }

        private string _numThanhToanDot1InBlock;
        public string NumThanhToanDot1InBlock { get=> _numThanhToanDot1InBlock; set { _numThanhToanDot1InBlock = value;OnPropertyChanged(nameof(NumThanhToanDot1InBlock)); } }

        private string _numDaBanInBlock;
        public string NumDaBanInBlock { get=>_numDaBanInBlock; set { _numDaBanInBlock = value;OnPropertyChanged(nameof(NumDaBanInBlock)); } }

        public IList<Floor> Floors { get; set; } = new ObservableCollection<Floor>();

    }
}
