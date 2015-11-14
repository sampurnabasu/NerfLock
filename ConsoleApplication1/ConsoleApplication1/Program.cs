using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CSHttpClientSample
{
    public class FacialRec
    {
        public FacialRec(string json)
        {
            json = @"{ ""data"" : " + json + "}";
            JObject jObject = JObject.Parse(json);
           var results = jObject["data"];
            foreach( var result in results)
            {
                faceId = (string)result["faceId"];
            }
        }

        public string faceId { get; set; }
    }
   
    static class Program
    {
        static void Main()
        {
            MakeRequest(); ;
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        static async void MakeRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers


            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "5017563ebb2d4070b52e16d3869f2a7b");
            
            // Request parameters
            queryString["analyzesFaceLandmarks"] = "false";
            queryString["analyzesAge"] = "false";
            queryString["analyzesGender"] = "false";
            queryString["analyzesHeadPose"] = "false";
            var uri = "https://api.projectoxford.ai/face/v0/detections?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"http://diabetesinsider.com/wp-content/uploads/2014/11/bill-gates-1024x688.jpg\"}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                String JSON = await response.Content.ReadAsStringAsync();
                FacialRec facialRec = new FacialRec(JSON);
                String faceID = facialRec.faceId;
                Console.WriteLine(faceID);
                
            }

        }
    }
}
