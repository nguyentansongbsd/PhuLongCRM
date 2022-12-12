using System;
using Newtonsoft.Json;

namespace PhuLongCRM.Models
{
	public class ContentActionReservationModel
	{
        [JsonProperty("Command", NullValueHandling = NullValueHandling.Ignore)]
        public string Command { get; set; }
        [JsonProperty("Parameters", NullValueHandling = NullValueHandling.Ignore)]
        public string Parameters { get; set; }
	}
	public class Paramster
	{
		public string action { get; set; }
		public string name { get; set; }
		public string value { get; set; }
	}
}

