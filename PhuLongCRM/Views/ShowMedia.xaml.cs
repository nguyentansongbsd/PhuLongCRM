using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhuLongCRM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowMedia : ContentPage
    {
        public Action<bool> OnCompleted;
        private string mediaSourceId { get; set; }
        private string folderId { get; set; }
        public ShowMedia(string FolderId, string MediaSourceId)
        {
            InitializeComponent();
            folderId = FolderId;
            mediaSourceId = MediaSourceId;
            Init();
        }

        private async void Init()
        {
            if (videoView != null)
            {
                var result = await CrmHelper.RetrieveImagesSharePoint<GrapDownLoadUrlModel>($"{folderId}/items/{mediaSourceId}/driveItem");
                if (result != null)
                {
                    string url = result.MicrosoftGraphDownloadUrl;
                    videoView.Source = url;
                    OnCompleted?.Invoke(true);
                }
                else
                {
                    OnCompleted?.Invoke(false);
                }
            }
        }
    }
}