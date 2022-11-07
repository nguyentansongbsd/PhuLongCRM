using System;
using System.Threading.Tasks;
using Firebase.Database;

namespace PhuLongCRM.Helper
{
    public class RealTimeHelper
    {
        public static FirebaseClient firebaseClient = new FirebaseClient(Config.OrgConfig.LinkFireBase_RealTimeData,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(Config.OrgConfig.AuthToken) });
    }
}
