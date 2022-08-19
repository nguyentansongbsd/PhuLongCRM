using Newtonsoft.Json;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class DirectSaleDetailTestViewModel : BaseViewModel
    {
        public int Page = 0;

        public int Size = 5;
        public ICommand LoadData
        {
            get => new Command(async () =>
            {
                LoadingHelper.Show();
                await LoadFloor();
                LoadingHelper.Hide();
            });
        }

        private List<DirectSaleModel> _directSaleResult;
        public List<DirectSaleModel> DirectSaleResult { get => _directSaleResult; set { _directSaleResult = value; OnPropertyChanged(nameof(DirectSaleResult)); } }

        private DirectSaleSearchModel _filter;
        public DirectSaleSearchModel Filter { get => _filter; set { _filter = value; OnPropertyChanged(nameof(Filter)); } }

        private DirectSaleModel _block;
        public DirectSaleModel Block { get => _block; set { _block = value; OnPropertyChanged(nameof(Block)); } }

        private Block _numUnitblock;
        public Block NumUniBlock { get => _numUnitblock; set { _numUnitblock = value; OnPropertyChanged(nameof(NumUniBlock)); } }

        private ObservableCollection<Floor> _floors;
        public ObservableCollection<Floor> Floors { get => _floors; set { _floors = value; OnPropertyChanged(nameof(Floors)); } }
        public DirectSaleDetailTestViewModel()
        {
            Floors = new ObservableCollection<Floor>();
        }
        public async Task LoadTotalDirectSale()
        {
            string json = JsonConvert.SerializeObject(Filter);
            var input = new
            {
                input = json
            };
            string body = JsonConvert.SerializeObject(input);
            CrmApiResponse result = await CrmHelper.PostData("/bsd_Action_DirectSale_GetTotalQty", body);
            if (result.IsSuccess == false && result.Content == null) return;

            string content = result.Content;
            ResponseAction responseActions = JsonConvert.DeserializeObject<ResponseAction>(content);
            DirectSaleResult = JsonConvert.DeserializeObject<List<DirectSaleModel>>(responseActions.output);
        }
        public async Task LoadFloor()
        {
            if(Block != null && Block.listFloor != null)
            {
                Page += 1;
                var list = Block.listFloor.Skip(Page*Size).Take(Size);
                foreach(var item in list)
                {
                    Floor floor = new Floor();
                    floor.bsd_floorid = Guid.Parse(item.ID);
                    floor.bsd_name = item.name;
                    var arrStatusInFloor = item.stringQty.Split(',');
                    floor.NumChuanBiInFloor = int.Parse(arrStatusInFloor[0]);
                    floor.NumSanSangInFloor = int.Parse(arrStatusInFloor[1]);
                    floor.NumBookingInFloor = int.Parse(arrStatusInFloor[2]);
                    floor.NumGiuChoInFloor = int.Parse(arrStatusInFloor[3]);
                    floor.NumDatCocInFloor = int.Parse(arrStatusInFloor[4]);
                    floor.NumDongYChuyenCoInFloor = int.Parse(arrStatusInFloor[5]);
                    floor.NumDaDuTienCocInFloor = int.Parse(arrStatusInFloor[6]);
                    floor.NumOptionInFloor = int.Parse(arrStatusInFloor[7]);
                    floor.NumThanhToanDot1InFloor = int.Parse(arrStatusInFloor[8]);
                    floor.NumSignedDAInFloor = int.Parse(arrStatusInFloor[9]);
                    floor.NumQualifiedInFloor = int.Parse(arrStatusInFloor[10]);
                    floor.NumDaBanInFloor = int.Parse(arrStatusInFloor[11]);
                    floor.TotalUnitInFloor = int.Parse(item.sumQty);
                    Floors.Add(floor);
                }
            }
        }
    }
}
