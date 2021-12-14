using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class PriceDirectSaleData
    {
        public static List<PriceDirectSaleModel> PriceData()
        {
            return new List<PriceDirectSaleModel>()
            {
                new PriceDirectSaleModel("1","Dưới 1 tỷ","1000000000"),
                new PriceDirectSaleModel("2","1 tỷ -> 2 tỷ","1000000000","2000000000"),
                new PriceDirectSaleModel("3","2 tỷ -> 5 tỷ","2000000000","5000000000"),
                new PriceDirectSaleModel("4","5 tỷ -> 10 tỷ","5000000000","10000000000"),
                new PriceDirectSaleModel("5","10 tỷ -> 20 tỷ","10000000000","20000000000"),
                new PriceDirectSaleModel("6","20 tỷ -> 50 tỷ","20000000000","50000000000"),
                new PriceDirectSaleModel("7","50 tỷ trở lên",null,"50000000000")
            };
        }
        public static PriceDirectSaleModel GetPriceById(string Id)
        {
            return PriceData().SingleOrDefault(x => x.Id == Id);
        }
    }
    public class PriceDirectSaleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public PriceDirectSaleModel(string id,string name, string from =null, string to = null)
        {
            Id = id;
            Name = name;
            From = from;
            To = to;
        }
    }
}
