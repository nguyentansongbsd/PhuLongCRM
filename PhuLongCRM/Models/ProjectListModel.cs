
using System;
using System.Collections.Generic;
using System.Text;

namespace PhuLongCRM.Models
{
    public class ProjectListModel 
    {
        public string bsd_projectid { get; set; }
        public string bsd_projectcode { get; set; }       
        public string bsd_name { get; set; }
        public Decimal? bsd_landvalueofproject { get; set; }
        public DateTime? bsd_esttopdate { get; set; }
        public DateTime? bsd_acttopdate { get; set; }

        public string statuscode { get; set; }
        public bool bsd_queueproject { get; set; }
        public string bsd_projecttype { get; set; }
        public string bsd_address { get; set; }
        public string bsd_projectslogo { get; set; }

        public string bsd_queueproject_format { get { return BoolToStringData.GetStringByBool(bsd_queueproject); } }
        public StatusCodeModel StatusCode { get=> ProjectStatusCodeData.GetProjectStatusCodeById(statuscode); }
        public OptionSet ProjectType { get => ProjectTypeData.GetProjectType(bsd_projecttype); }
        public string projectLogo { get {
                if (string.IsNullOrWhiteSpace(bsd_projectslogo))
                {
                    return bsd_name;
                }
                else
                {
                    return bsd_projectslogo;
                }
            } }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ProjectListModel)) return false;
            var model = (ProjectListModel)obj;
            return this.bsd_projectid == model.bsd_projectid;
        }

        public override int GetHashCode()
        {
            return this.bsd_projectid.GetHashCode();
        }
    }
}
