using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using PhuLongCRM.Helper;
using PhuLongCRM.Models;
using PhuLongCRM.Resources;
using Xamarin.Forms;

namespace PhuLongCRM.Views
{
    public partial class ConformOTPPage : ContentPage
    {
        FirebaseClient firebaseClient = new FirebaseClient("https://smsappcrm-default-rtdb.asia-southeast1.firebasedatabase.app/",
            new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult("kLHIPuBhEIrL6s3J6NuHpQI13H7M0kHjBRLmGEPF") });

        private List<OTPModel> OTPList { get; set; }
        public Action<bool> OnCompeleted;

        private string _phone;
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(nameof(Phone)); } }

        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(nameof(Email)); } }

        private string _sendTo;
        public string SendTo { get => _sendTo; set { _sendTo = value; OnPropertyChanged(nameof(SendTo)); } }
        private bool SenToEmail { get; set; }
        private OTPModel OTP { get; set; }

        private string fireBaseDb = "PhuLongOTPDb";

        private int _timeRemaing = 60;
        public int TimeRemaining { get => _timeRemaing; set { _timeRemaing = value; OnPropertyChanged(nameof(TimeRemaining)); } }

        public ConformOTPPage(string phone, string email = null, bool sentoemail = false)
        {
            InitializeComponent();
            this.BindingContext = this;
            Phone = phone;
            Email = email;
            SenToEmail = sentoemail;
            if (sentoemail)
                SendTo = email;
            else
                SendTo = phone;

            Init();
        }

        public async void Init()
        {
            bool SendSuccess = await SendOTP();
            if (SendSuccess)
            {
                OnCompeleted?.Invoke(true);
                this.SetTimeRemaining(new CancellationToken());
            }

            else
                OnCompeleted?.Invoke(false);
        }

        protected override bool OnBackButtonPressed()
        {
            this.SetOTPCanceled(this.OTPList.SingleOrDefault(x => x.Id == OTP.Id));
            return base.OnBackButtonPressed();
        }

        public async Task<bool> SendOTP()
        {
            try
            {
                string otpCode = OTPCode();
                OTP = new OTPModel()
                {
                    Id = Guid.NewGuid(),
                    Content = $"{otpCode} is your PhuLongCRM verification code.",
                    Phone = $"{Phone}",
                    OTPCode = otpCode,
                    IsSend = false,
                    IsLimitTime = false,
                    IsConfirm = false,
                    IsCanceled = false,
                    Date = DateTime.Now
                };

                var result = await firebaseClient.Child(fireBaseDb).PostAsync<OTPModel>(OTP);
                if (result != null)
                {
                    await this.LoadDataOTP();
                    if (SenToEmail)
                        await SenEmail();
                    return true;
                }
                else
                {
                    ToastMessageHelper.LongMessage("Lỗi. Không thể lưu dữ liệu vào Firebase.");
                    return false;
                }
            }
            catch (FirebaseException ex)
            {
                ToastMessageHelper.LongMessage(ex.Message);
                return false;
            }
        }

        private async Task LoadDataOTP()
        {
            OTPList = new List<OTPModel>();
            OTPList = (await firebaseClient.Child(fireBaseDb).OnceAsync<OTPModel>()).Select(item => new OTPModel()
            {
                key = item.Key,
                Id = item.Object.Id,
                Content = item.Object.Content,
                Phone = item.Object.Phone,
                OTPCode = item.Object.OTPCode,
                IsSend = item.Object.IsSend,
                IsLimitTime = item.Object.IsLimitTime,
                IsConfirm = item.Object.IsConfirm,
                IsCanceled = item.Object.IsCanceled,
                Date = item.Object.Date
            }).ToList();
        }

        private string OTPCode()
        {
            return new Random().Next(1000, 9999).ToString();
        }

        private void MainEntry_Unfocused(object sender, FocusEventArgs e)
        {
            mainEntry.Focus();
        }

        private async void ConfirmOTP_Clicked(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            string ConformCode = Code1.Text + Code2.Text + Code3.Text + Code4.Text;
            ConformCode = ConformCode.Replace(" ", "");
            if (OTPList.Any(x => x.OTPCode == ConformCode && x.Phone == Phone && x.IsConfirm == false && x.IsLimitTime == false && x.IsCanceled == false))
            {
                if (ForgotPassWordPage.NeedRefreshForm.HasValue) ForgotPassWordPage.NeedRefreshForm = true;
                //await DeleteRecordFirebase();
                SetOTPConfirm();
                await Navigation.PopAsync();
                LoadingHelper.Hide();
            }
            else
            {
                LoadingHelper.Hide();
                ToastMessageHelper.ShortMessage(Language.ma_xac_thuc_khong_dung);
            }
        }

        private async Task DeleteRecordFirebase()
        {
            var key = OTPList.OrderByDescending(x => x.Date).FirstOrDefault(x => x.Phone == Phone).key;
            await firebaseClient.Child(fireBaseDb).Child(key).DeleteAsync();
        }

        public void SetEnableButtonConform()
        {
            if (string.IsNullOrWhiteSpace(Code1.Text)
                || string.IsNullOrWhiteSpace(Code2.Text)
                || string.IsNullOrWhiteSpace(Code3.Text)
                || string.IsNullOrWhiteSpace(Code4.Text)
                )
            {
                BtnConform.IsEnabled = false;
            }
            else
            {
                BtnConform.IsEnabled = true;
            }
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            this.SetOTPCanceled(this.OTPList.SingleOrDefault(x => x.Id == OTP.Id));
            await Navigation.PopAsync();
        }

        private async void Resend_Tapped(object sender, EventArgs e)
        {
            LoadingHelper.Show();
            mainEntry.Text = "";
            await this.SendOTP();
            this.CancelOldOTP();
            this.lblTimeRemaining.IsVisible = true;
            this.lblOTPExpired.IsVisible = false;
            using (var cancellationtokenresource = new CancellationTokenSource())
            {
                if (this.TimeRemaining < 60 && TimeRemaining > 0)
                {
                    cancellationtokenresource.Cancel();
                }
                this.SetTimeRemaining(cancellationtokenresource.Token);
            }
            LoadingHelper.Hide();
        }

        public void mainEntry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            string text = e.NewTextValue?.Trim() ?? "";
            try
            {
                Code1.Text = text[0].ToString();
                Code1.IsVisible = true;
                ShowHideBoxView(Code1, false);
            }
            catch
            {
                Code1.Text = "";
                Code1.IsVisible = false;
                ShowHideBoxView(Code1, true);
            }

            try
            {
                Code2.Text = text[1].ToString();
                Code2.IsVisible = true;
                ShowHideBoxView(Code2, false);
            }
            catch
            {
                Code2.Text = "";
                Code2.IsVisible = false;
                ShowHideBoxView(Code2, true);
            }


            try
            {
                Code3.Text = text[2].ToString();
                Code3.IsVisible = true;
                ShowHideBoxView(Code3, false);
            }
            catch
            {
                Code3.Text = "";
                Code3.IsVisible = false;
                ShowHideBoxView(Code3, true);
            }

            try
            {
                Code4.Text = text[3].ToString();
                Code4.IsVisible = true;
                ShowHideBoxView(Code4, false);
            }
            catch
            {
                Code4.Text = "";
                Code4.IsVisible = false;
                ShowHideBoxView(Code4, true);
            }

            SetEnableButtonConform();

            if (BtnConform.IsEnabled)
            {
                ConfirmOTP_Clicked(BtnConform, EventArgs.Empty);
            }
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            mainEntry.Focus();
        }

        public void ShowHideBoxView(Label label, bool show)
        {
            Grid grid = label.Parent as Grid;
            BoxView boxView = grid.Children[0] as BoxView;
            boxView.IsVisible = show;
        }

        private void SetTimeRemaining(CancellationToken cancellationToken)
        {
            this.TimeRemaining = 60;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (cancellationToken != null && cancellationToken.IsCancellationRequested)
                {
                    return false;
                }
                this.TimeRemaining--;
                if (this.TimeRemaining == 0)
                {
                    this.lblTimeRemaining.IsVisible = false;
                    this.lblOTPExpired.IsVisible = true;
                    this.SetLimitTime();
                }
                return (Convert.ToBoolean(TimeRemaining));
            });
        }

        private async void SetLimitTime()
        {
            OTPModel _otp = this.OTPList.SingleOrDefault(x => x.Id == OTP.Id);
            await firebaseClient.Child("PhuLongOTPDb").Child(_otp.key).PutAsync(new OTPModel() { Id = _otp.Id, Content = _otp.Content, Phone = _otp.Phone, OTPCode = _otp.OTPCode, IsSend = _otp.IsSend, IsLimitTime = true, IsConfirm = _otp.IsConfirm, IsCanceled = _otp.IsCanceled, Date = _otp.Date });
            _otp.IsLimitTime = true;
        }

        private async void SetOTPCanceled(OTPModel _otp)
        {
            await firebaseClient.Child("PhuLongOTPDb").Child(_otp.key).PutAsync(new OTPModel() { Id = _otp.Id, Content = _otp.Content, Phone = _otp.Phone, OTPCode = _otp.OTPCode, IsSend = _otp.IsSend, IsLimitTime = _otp.IsLimitTime, IsConfirm = _otp.IsConfirm, IsCanceled = true, Date = _otp.Date });
            _otp.IsCanceled = true;
        }

        private async void SetOTPConfirm()
        {
            OTPModel _otp = this.OTPList.SingleOrDefault(x => x.Id == OTP.Id);
            await firebaseClient.Child("PhuLongOTPDb").Child(_otp.key).PutAsync(new OTPModel() { Id = _otp.Id, Content = _otp.Content, Phone = _otp.Phone, OTPCode = _otp.OTPCode, IsSend = _otp.IsSend, IsLimitTime = _otp.IsLimitTime, IsConfirm = true, IsCanceled = _otp.IsCanceled, Date = _otp.Date });
            _otp.IsConfirm = true;
        }

        private void CancelOldOTP()
        {
            this.OTPList.Where(x => x.Id != this.OTP.Id && x.Phone == this.OTP.Phone && x.IsConfirm == false && x.IsCanceled == false && x.IsLimitTime == false).ToList().ForEach(item =>
            {
                this.SetOTPCanceled(item);
            });
        }

        private async Task SenEmail()
        {
            SmtpClient client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = "phupx@bsdinsight.com",
                    Password = "kgakcewdhsqgxsfm"
                }
            };
            MailAddress from = new MailAddress("phupx@bsdinsight.com", "BSD Insight");
            MailAddress to = new MailAddress(Email, Phone); //songnt@bsdinsight.com
            string contentEmail = $"Mã OTP : {OTP.OTPCode} được gửi từ BSD Insight" +
                $"\n Nếu bạn không phải là người gửi yêu cầu này, vui lòng bỏ qua.";
            MailMessage mail = new MailMessage()
            {
                From = from,
                Subject = $"Mã OTP : {OTP.OTPCode}",
                Body = contentEmail
            };
            mail.To.Add(to);
            client.SendCompleted += Client_SendCompleted;
            await client.SendMailAsync(mail);
        }

        private void Client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ToastMessageHelper.ShortMessage(Language.thong_bao_that_bai + e.Error.Message);
                LoadingHelper.Hide();
            }
            else
            {
                ToastMessageHelper.ShortMessage(Language.thong_bao_thanh_cong);
                LoadingHelper.Hide();
            }
        }
    }
}
