using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Config
{
    public class OrgConfig
    {
        public const string VerApp = "PL.CRM - VerDemo - Ver 3.2 - Song ngữ";
        //public const string VerApp = "PL.CRM - Version 3.0";

        public const string ApiUrl = "https://phulongdev.api.crm5.dynamics.com/api/data/v9.1";
        public const string Resource = "https://phulongdev.crm5.dynamics.com/";
        public const string ClientId = "1b53e0dd-04fb-495c-a8d0-3f26ebb84468"; //
        public const string ClientSecret = "SII7Q~z5TqyjVoBBIKNuSxDJabQhuFE_~i5HI"; //

        // For login by user crm
        public const string TeantId = "87bbdb08-48ba-4dbf-9c53-92ceae16c353";
        public const string ClientId_ForUserCRM = "a7544a58-b7bb-4553-9548-d56d1cfbec55";
        public const string ClientSecret_ForUserCRM = "1kO7Q~FQ_o6uhrthjqlaUWiSY-bkpViYBDBPu";
        public const string Redirect_Uri = "https://crm.phulong.com/";
        public const string Scope = "https://phulongdev.crm5.dynamics.com/.default";

        //public const string TeantId = "1958ace9-e5ba-4d51-b458-cca319ff9b4f";
        //public const string ClientId_ForUserCRM = "1d2267b7-2d9d-4b75-a45d-7531fe7b9494";
        //public const string ClientSecret_ForUserCRM = "ZpH8Q~XRbEoDfJjPgGnBV3OwEVkiMBGNJJwhraRs";
        //public const string Redirect_Uri = "https://facebook.com";
        //public const string Scope = "https://org957ed874.crm5.dynamics.com/.default";
    }
}
