using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Diagnostics;


public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string name = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
        .Value;

    /*Start*/
    string uri = "http://zipcloud.ibsnet.co.jp/api/search?zipcode=" + name;
    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
    webReq.ContentType = "application./json";
    webReq.Method = "POST";

    HttpWebResponse res = (HttpWebResponse)webReq.GetResponse();
    ResultAddress info;
    using (res)
    {
        using (var resStream = res.GetResponseStream())
        {
            var serializer = new DataContractJsonSerializer(typeof(ResultAddress));
            info = (ResultAddress)serializer.readObject(resStream);
        }
    }
    name = info.address1;
    /*End*/

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    //name = name ?? data?.name;

    return name == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
        : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
}


public class ResultAddress
{
    public string address1 { get; set; }
    public string address2 { get; set; }
    public string address3 { get; set; }
    public string kana1 { get; set; }
    public string kana2 { get; set; }
    public string kana3 { get; set; }
    public int prefcode { get; set; }
    public int zipcode { get; set; }
}