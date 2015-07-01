using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BasicVSOClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var username = "technizerindia";
            var password = "technizer@1";

            //Enable methods as required from below
            GetWorkItem(username, password, 7);
            //CreateWorkItem(username, password);
            //GetWorkItems(baseUrl, username, password);
            //UpdateWorkItem(username, password);
        }

        static async void GetWorkItem(string username, string password, int WiId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", username, password))));
                    string Url = "https://technizerindia.visualstudio.com/defaultcollection/_apis/wit/workitems?id=1&api-version=1.0";
                    using (HttpResponseMessage response = client.GetAsync(Url).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        WorkItemDetails wiDetails = JsonConvert.DeserializeObject<WorkItemDetails>(responseBody);
                        Console.WriteLine("Work Item ID: \t" + wiDetails.id);
                        foreach (KeyValuePair<string, string> fld in wiDetails.fields)
                        {
                            Console.WriteLine(fld.Key + ":\t" + fld.Value);
                        }
                    }
                }
            }
            catch
            {

            }
        }
        public static async void  GetWorkItems(string baseUrl, string username, string password)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", username, password))));
                    string Url = baseUrl + "_apis/wit/queries?$depth=1&api-version=1.0";
                    using (HttpResponseMessage response = client.GetAsync(Url).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        ApiCollection<QueryFolder> queryFolders = (ApiCollection<QueryFolder>)Newtonsoft.Json.JsonConvert.DeserializeObject<ApiCollection<QueryFolder>>(responseBody);
                        foreach (QueryFolder queryFolder in queryFolders.Value)
                        {
                            if (queryFolder.name == "My Queries" && queryFolder.hasChildren)
                            {
                                string GetQueryUrl = baseUrl + "_apis/wit/queries/" + queryFolder.path + "/All PBIs?api-version=1.0";
                                using (HttpResponseMessage QueryResponse = client.GetAsync(GetQueryUrl).Result)
                                {
                                    QueryResponse.EnsureSuccessStatusCode();
                                    string QueryResponseBody = await QueryResponse.Content.ReadAsStringAsync();
                                    Query TargetQuery = (Query)JsonConvert.DeserializeObject(QueryResponseBody, typeof(Query));
                                    string GetWorkItemsUrl = baseUrl + "_apis/wit/wiql/" + TargetQuery.id + "?api-version=1.0";
                                    using (HttpResponseMessage WorkItemsResponse = client.GetAsync(GetWorkItemsUrl).Result)
                                    {
                                        WorkItemsResponse.EnsureSuccessStatusCode();
                                        string WorkItemResponseBody = await WorkItemsResponse.Content.ReadAsStringAsync();
                                        WorkItemQueryResult QueryResult = (WorkItemQueryResult)JsonConvert.DeserializeObject<WorkItemQueryResult>(WorkItemResponseBody);
                                        WorkItem[] workItems = QueryResult.workItems;
                                        if (workItems.Length > 0)
                                        {
                                            foreach(WorkItem wi in workItems)
                                            {
                                                baseUrl = "https://technizerindia.visualstudio.com/defaultcollection/";
                                                string GetWorkItemDetailsUrl = baseUrl + "_apis/wit/workitems?ids=" + wi.id +"&api-version=1.0";
                                                using (HttpResponseMessage WorkItemDetailsResponse = client.GetAsync(GetWorkItemDetailsUrl).Result)
                                                {
                                                    
                                                    WorkItemDetailsResponse.EnsureSuccessStatusCode();
                                                    string WorkItemDetailsResponseBody = await WorkItemDetailsResponse.Content.ReadAsStringAsync();
                                                    ApiCollection<WorkItemDetails> wiDetailsCollection = (ApiCollection<WorkItemDetails>)JsonConvert.DeserializeObject<ApiCollection<WorkItemDetails>>(WorkItemDetailsResponseBody);
                                                    foreach (WorkItemDetails wiDetails in wiDetailsCollection.Value)
                                                    {
                                                        foreach(KeyValuePair<string, string> fld in wiDetails.fields)
                                                        {
                                                            Console.WriteLine(fld.Key + ":" + fld.Value);
                                                        }
                                                    }
                                                    Console.ReadLine();
                                                }
                                            }
                                        }
                                    }
                                }                                 
                            }
                        }
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
        public static async void CreateWorkItem(string username, string password)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json-patch+json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", username, password))));
                    
                    WorkItemPostData wiPostData = new WorkItemPostData();
                    wiPostData.op = "add";
                    wiPostData.path = "/fields/System.Title";
                    wiPostData.value = "Employee edits other employees profile";
                    List<WorkItemPostData> wiPostDataArr = new List<WorkItemPostData> { wiPostData };
                    string wiPostDataString = JsonConvert.SerializeObject(wiPostDataArr);
                    HttpContent wiPostDataContent = new StringContent(wiPostDataString, Encoding.UTF8, "application/json-patch+json");

                    string Url = "https://technizerindia.visualstudio.com/DefaultCollection/TimeS/_apis/wit/workitems/$Product%20Backlog%20Item?api-version=1.0";
                    
                    using (HttpResponseMessage response = client.PatchAsync(Url, wiPostDataContent).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string ResponseContent = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
        public static async void UpdateWorkItem(string username, string password)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json-patch+json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", username, password))));

                    WorkItemPostData wiPostData = new WorkItemPostData();
                    wiPostData.op = "replace";
                    wiPostData.path = "/fields/System.Title";
                    wiPostData.value = "Employee views own profile in browser based application";
                    List<WorkItemPostData> wiPostDataArr = new List<WorkItemPostData> { wiPostData };
                    string wiPostDataString = JsonConvert.SerializeObject(wiPostDataArr);
                    HttpContent wiPostDataContent = new StringContent(wiPostDataString, Encoding.UTF8, "application/json-patch+json");

                    string Url = "https://technizerindia.visualstudio.com/DefaultCollection/_apis/wit/workitems/1?api-version=1.0";

                    using (HttpResponseMessage response = client.PatchAsync(Url, wiPostDataContent).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        string ResponseContent = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }

    public static class HttpClientExtensions
    {
        public async static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };
            return await client.SendAsync(request);
        }

    }
    public class QueryFolder
    {
        public string id;
        public string name;
        public string path;
        public bool isFolder;
        public bool hasChildren;
        public QueryFolder[] children;
        public bool isPublic;
        public QueryLink _link;
        public string url;
    }
    public class ApiCollection<T>
    {
        [JsonProperty(PropertyName = "value")]
        public IEnumerable<T> Value { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
    }
    public class QueryLink
    {
        public self _self;
        public html _html;
        public parent _parent;

    }
    public class Query
    {
        public string id;
        public string name;
        public string path;
        public bool isFolder;
        public bool hasChildren;
        public QueryFolder[] children;
        public bool isPublic;
        public string url;
    }
    public class self
    {
        public string href;
    }
    public class html
    {
        public string href;
    }
    public class parent
    {
        public string href;
    }

    public class Column
    {
        public string referenceName;
        public string name;
        public string url;
    }
    public class WorkItem
    {
        public string id;
        public string Url;
    }
    public class WorkItemQueryResult
    {
        public string queryType;
        public string asOf;
        public Column[] columns;
        public WorkItem[] workItems;
    }
    public class Fields
    {
        public string name;
        public string value;
    }
    public class WorkItemDetails
    {
        public string id;
        public string rev;
        public IDictionary<string, string> fields;
        public string Url;
    }
    public class WorkItemPostData
    {
        public string op;
        public string path;
        public string value;
    }
}
