using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Config
{
    public class OrgConfig
    {
        public const string VerApp = "PL.CRM.UAT - VerDemo - Ver 1.6.9";
        //public const string VerApp = "PL.CRM - Version 4.1";
        //public const string VerApp = "PL.CRM - UAT - Ver 1.2";

        public const string ApiUrl = "https://phulong.api.crm5.dynamics.com/api/data/v9.1";
        public const string Resource = "https://phulong.crm5.dynamics.com/";
        public const string ClientId = "1b53e0dd-04fb-495c-a8d0-3f26ebb84468"; //
        public const string ClientSecret = "SII7Q~z5TqyjVoBBIKNuSxDJabQhuFE_~i5HI"; //

        // For login by user crm
        public const string TeantId = "87bbdb08-48ba-4dbf-9c53-92ceae16c353";
        public const string ClientId_ForUserCRM = "a7544a58-b7bb-4553-9548-d56d1cfbec55";
        public const string ClientSecret_ForUserCRM = "1kO7Q~FQ_o6uhrthjqlaUWiSY-bkpViYBDBPu";
        public const string Redirect_Uri = "https://crm.phulong.com/";
        public const string Scope = "offline_access https://phulong.crm5.dynamics.com/.default";

        //sharepoint
        public const string GraphApiSites = "https://graph.microsoft.com/v1.0/sites/";
        public const string GraphApiDrive = "https://graph.microsoft.com/v1.0/drives/";
        public const string GraphReSource = "https://graph.microsoft.com";

        // Sharepoint Site PhuLong
        //public const string SP_SiteId = "245fb505-41a2-4630-923b-b233fdd09865";
        //public const string SP_UnitID = "3c197b5b-a1b2-46a1-8d6c-7e45c2b53a13";
        //public const string SP_UnitTypeID = "f351dad2-e44a-474a-84f5-12835186f2b5";
        //public const string SP_ProjectID = "128241a3-19be-410e-afab-7cc233fba735";
        //public const string SP_ContactID = "166b0309-79b1-414b-9613-ce3529e89642";

        //public const string Graph_UnitTypeID = "b!BbVfJKJBMEaSO7Iz_dCYZaTJYdWNCktOh_yG1we5P9LS2lHzSuRKR4T1EoNRhvK1";
        //public const string Graph_UnitID = "b!BbVfJKJBMEaSO7Iz_dCYZaTJYdWNCktOh_yG1we5P9Jbexk8sqGhRo1sfkXCtToT";
        //public const string Graph_ProjectID = "b!BbVfJKJBMEaSO7Iz_dCYZaTJYdWNCktOh_yG1we5P9KjQYISvhkOQa-rfMIz-6c1";
        //public const string Graph_ContactID = "b!BbVfJKJBMEaSO7Iz_dCYZaTJYdWNCktOh_yG1we5P9IJA2sWsXlLQZYTzjUp6JZC";

        // Sharepoint Site PhuLong-UAT
        public const string SP_SiteId = "74f8fad8-13c1-428e-adde-229434c6a37f";
        public const string SP_UnitID = "7dbce0b3-fa3e-4e3d-84c8-481f4db43368";
        public const string SP_UnitTypeID = "7d92454f-b545-40c8-8730-19721320c089";
        public const string SP_ProjectID = "bf77feea-e01d-4895-bf9b-f31eea66b9f4";
        public const string SP_ContactID = "50d7610e-e9b8-4078-aefe-1ffc133244ee";

        public const string Graph_UnitTypeID = "b!2Pr4dMETjkKt3iKUNMajf9S0toE_AwpNtuTnCMuoqsZPRZJ9RbXIQIcwGXITIMCJ";
        public const string Graph_UnitID = "b!2Pr4dMETjkKt3iKUNMajf9S0toE_AwpNtuTnCMuoqsaz4Lx9Pvo9ToTISB9NtDNo";
        public const string Graph_ProjectID = "b!2Pr4dMETjkKt3iKUNMajf9S0toE_AwpNtuTnCMuoqsbq_ne_HeCVSL-b8x7qZrn0";
        public const string Graph_ContactID = "b!2Pr4dMETjkKt3iKUNMajf9S0toE_AwpNtuTnCMuoqsYOYddQuOl4QK7-H_wTMkTu";

        // Info Realtime
        public const string LinkFireBase_RealTimeData = "https://phulong-aff10-default-rtdb.firebaseio.com/";
        public const string AuthToken = "VhuPY1prumruPs8Vgxuj1P1NIIsqnvzZ8tycOuIK";

        //Dong bo danh ba
        //public const string Lead_Topic = "B564BDFC-50E2-EC11-BB3D-00224859CF8A"; //Khách hàng tiềm năng APP / Org Dev
        public const string Lead_Topic = "120A7D95-345B-ED11-9561-0022485939B9"; //Khách hàng tiềm năng APP / Org TMP
        //public const string PermanentCountry = "451F9FE7-A409-ED11-82E5-00224859C42D"; //VietNam
        //public const string PermanentProvice = "32AFD120-A509-ED11-82E5-00224859C3BD"; //Ho Chi Minh
        //public const string PermanentDistrict = "A7E59458-A509-ED11-82E5-00224859CD05"; // Quan 1
        //public const string PermanentHouseNumber = "Đa Kao";
        //public const string PermanentAddress = "Quận 1, Hồ Chí Minh, Việt Nam";
    }
}
