using System;
using System.Collections.Generic;
using System.Linq;

namespace PhuLongCRM.Models
{
    public class ProjectStatusCodeData
    {
        public static List<StatusCodeModel> ProjectStatusCodeDatas()
        {
            return new List<StatusCodeModel>()
            {
                new StatusCodeModel("1","Active","#14A184"),//Active
                new StatusCodeModel("861450002","Publish","#2FCC71"),//Publish
                new StatusCodeModel("861450001","Unpublish","#808080"),//Unpublish
                new StatusCodeModel("2","Inactive","#D90825")//Inactive
            };
        }
        public static StatusCodeModel GetProjectStatusCodeById(string id)
        {
            return ProjectStatusCodeDatas().Single(x => x.Id == id);
        }
    }
}
