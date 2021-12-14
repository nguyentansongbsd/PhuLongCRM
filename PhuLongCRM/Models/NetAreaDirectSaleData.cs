using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class NetAreaDirectSaleData
    {
        public static List<NetAreaDirectSaleModel> NetAreaData()
        {
            return new List<NetAreaDirectSaleModel>()
            {
                new NetAreaDirectSaleModel("1","Dưới 60m2","60"),
                new NetAreaDirectSaleModel("2","60m2 -> 80m2","60","80"),
                new NetAreaDirectSaleModel("3","81m2 -> 100m2","81","100"),
                new NetAreaDirectSaleModel("4","101m2 -> 120m2","101","120"),
                new NetAreaDirectSaleModel("5","121m2 -> 150m2","121","150"),
                new NetAreaDirectSaleModel("6","151m2 -> 180m2","151","180"),
                new NetAreaDirectSaleModel("7","211m2 -> 240m2","211","240"),
                new NetAreaDirectSaleModel("8","241m2 -> 270m2","241","270"),
                new NetAreaDirectSaleModel("9","271m2 -> 300m2","271","300"),
                new NetAreaDirectSaleModel("10","301m2 -> 350m2","301","350"),
                new NetAreaDirectSaleModel("11","Trên 350m2",null,"350"),
            };
        }
        public static NetAreaDirectSaleModel GetNetAreaById(string Id)
        {
            return NetAreaData().SingleOrDefault(x => x.Id == Id);
        }
    }
    public class NetAreaDirectSaleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public NetAreaDirectSaleModel(string id,string name,string from = null,string to = null)
        {
            Id = id;
            Name = name;
            From = from;
            To = to;
        }
    }
}
