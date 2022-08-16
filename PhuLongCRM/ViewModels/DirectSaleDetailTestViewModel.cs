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
        private int Page = 0;

        private int Size = 5;
        public ICommand LoadData
        {
            get => new Command(async () =>
            {
                IsBusy = true;
                await LoadFloor();
                IsBusy = false;
            });
        }

        private List<DirectSaleModel> _directSaleResult;
        public List<DirectSaleModel> DirectSaleResult { get => _directSaleResult; set { _directSaleResult = value; OnPropertyChanged(nameof(DirectSaleResult)); } }

        private DirectSaleSearchModel _filter;
        public DirectSaleSearchModel Filter { get => _filter; set { _filter = value; OnPropertyChanged(nameof(Filter)); } }

        private DirectSaleModel _block;
        public DirectSaleModel Block { get => _block; set { _block = value; OnPropertyChanged(nameof(Block)); } }

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
                var list = Block.listFloor.Take(Page * Size);
                foreach(var floor in list)
                {

                    Floors.Add(new Floor { bsd_name = floor.name, TotalUnitInFloor = int.Parse(floor.sumQty)});
                }
            }
        }
    }
}
