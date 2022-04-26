using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Config
{
    public class OrgConfig
    {
        public const string VerApp = "PL.CRM - VerDemo - Ver 2.9 - Song ngữ";
        //public const string VerApp = "PL.CRM - Version 2.0";

        public const string ApiUrl = "https://phulongdev.api.crm5.dynamics.com/api/data/v9.1";
        public const string Resource = "https://phulongdev.crm5.dynamics.com/";
        public const string ClientId = "1b53e0dd-04fb-495c-a8d0-3f26ebb84468";
        public const string ClientSecret = "SII7Q~z5TqyjVoBBIKNuSxDJabQhuFE_~i5HI"; //JmC7Q~Ege0KWrZMODL6yv_ExwFDdkINIXrsF8

        // For login by user crm
        public const string TeantId = "87bbdb08-48ba-4dbf-9c53-92ceae16c353";
        public const string ClientId_ForUserCRM = "1b53e0dd-04fb-495c-a8d0-3f26ebb84468";
        public const string ClientSecret_ForUserCRM = "SII7Q~z5TqyjVoBBIKNuSxDJabQhuFE_~i5HI";
        public const string Redirect_Uri = "https://sundiland.com";
        public const string Scope = "https://phulongdev.crm5.dynamics.com/.default";
    }
}
