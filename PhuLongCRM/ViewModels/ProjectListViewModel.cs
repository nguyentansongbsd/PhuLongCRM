using System;
using PhuLongCRM.Models;
using PhuLongCRM.Settings;
using Xamarin.Forms;

namespace PhuLongCRM.ViewModels
{
    public class ProjectListViewModel : ListViewBaseViewModel2<ProjectListModel>
    {
        public string Keyword { get; set; }

        public ProjectListViewModel()
        {
            PreLoadData = new Command(() =>
            {
                //Khi Search thi lay nhung du an publish, active, unpublish
                string filter = string.Empty;
                if (string.IsNullOrWhiteSpace(Keyword))
                {
                    filter = $@"<filter type='and'>
                                    <condition attribute='statuscode' operator='eq' value='861450002' />
                                </filter>";
                }
                else
                {
                    filter = $@"<filter type='and'>
                                  <filter type='or'>
                                    <condition attribute='bsd_name' operator='like' value='%25{Keyword}%25'/>
                                    <condition attribute='bsd_projectcode' operator='like' value='%25{Keyword}%25' />
                                  </filter>
                                  <condition attribute='statuscode' operator='in'>
                                    <value>1</value>
                                    <value>861450001</value>
                                    <value>861450002</value>
                                  </condition>
                                </filter>";
                }

                EntityName = "bsd_projects";
                FetchXml = $@"<fetch version='1.0' count='15' page='{Page}' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bsd_project'>
                                    <attribute name='bsd_projectid'/>
                                    <attribute name='bsd_projectcode'/>
                                    <attribute name='bsd_name'/>
                                    <attribute name='bsd_projectslogo'/>
                                    <attribute name='statuscode' />
                                    <attribute name='bsd_queueproject'/>
                                    <attribute name='bsd_projecttype'/>
                                    <attribute name='bsd_address' />
                                    <attribute name='createdon' />
                                    <order attribute='bsd_name' descending='false' />
                                    {filter}
                                  </entity>
                            </fetch>";
            });
        }
    }
}
