using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace QueryWorkitems0619
{
    class Program
    {
        static void Main(string[] args)
        {
            string orgName = "benclark0838";
            string PAT = "cepa4nqa3dwwaatmzenra6yl2dmrgxedmsipmjrxxenoy3gv6uea";

            Uri uri = new Uri($"https://dev.azure.com/{orgName}");
            string project = "Wingnit_2";
            VssBasicCredential credentials = new VssBasicCredential("", PAT);
            //create a wiql object and build our query
            Wiql wiql = new Wiql()
            {
                Query = "Select * " +
                "From WorkItems " +
                "Where [System.TeamProject] = '" + project + "' " +
                "And [System.State] <> 'Closed' " +
                "And [System.RelatedLinkCount] > '0'" +
                "Order By [State] Asc, [Changed Date] Desc"
            };
            //create instance of work item tracking http client
            using (WorkItemTrackingHttpClient workItemTrackingHttpClient = new WorkItemTrackingHttpClient(uri, credentials))
            {
                //execute the query to get the list of work items in the results
                WorkItemQueryResult workItemQueryResult = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).Result;
                //some error handling
                if (workItemQueryResult.WorkItems.Count() != 0)
                {
                    //need to get the list of our work item ids and put them into an array
                    List<int> list = new List<int>();

                    foreach (var item in workItemQueryResult.WorkItems)
                    {
                        list.Add(item.Id);
                    }
                    int[] arr = list.ToArray();
                    //build a list of the fields we want to see
                    string[] fields = new string[5];
                    fields[0] = "System.Id";
                    fields[1] = "System.Title";
                    fields[2] = "System.RelatedLinkCount";
                    fields[3] = "System.Description";
                    fields[4] = "Microsoft.VSTS.Scheduling.OriginalEstimate";
                    //get work items for the ids found in query
                    var workItems = workItemTrackingHttpClient.GetWorkItemsAsync(arr, fields, workItemQueryResult.AsOf).Result;
                    //loop though work items and write to console
                    foreach (var workItem in workItems)
                    {
                        foreach (var field in workItem.Fields)
                        {
                            Console.WriteLine("- {0}: {1}", field.Key, field.Value);
                        }
                    }

                    Console.ReadLine();
                }
            }
        }
    }
}