using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
        private static string s1;
        private static string s2;

        static void Main()
        {
            Task taskA = Task.Factory.StartNew(() => MakeRequest(0, "http://diabetesinsider.com/wp-content/uploads/2014/11/bill-gates-1024x688.jpg"));
            taskA.Wait();
            Console.WriteLine("taskA has completed.");

            Task taskB = Task.Factory.StartNew(() => MakeRequest(1, "http://thefilmstage.com/wp-content/uploads/2011/10/SteveJobsBook.jpg"));
            taskB.Wait();
            Console.WriteLine("taskB has completed.");

            if (taskB.IsCompleted && taskA.IsCompleted)
                MakeRequest2();
            else
                Console.WriteLine("Timed out.");

           
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        static async void MakeRequest(int image, String url)
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
            byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\""+url+ "\"}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                String JSON = await response.Content.ReadAsStringAsync();
                FacialRec facialRec = new FacialRec(JSON);
          
                String faceID = facialRec.faceId;
                
                if (image == 0)
                {
                    s1 = faceID;
                    
                }
                else
                {
                    s2 = faceID;
                }
                Console.WriteLine(faceID);
                Debug.WriteLine(faceID);
 
               
            }

        }
        static async void MakeRequest2()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request parameters
            queryString["analyzesFaceLandmarks"] = "false";
            queryString["analyzesAge"] = "false";
            queryString["analyzesGender"] = "false";
            queryString["analyzesHeadPose"] = "false";

            // Request headers
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "5017563ebb2d4070b52e16d3869f2a7b");

            var uri = "https://api.projectoxford.ai/face/v0/verifications?" + queryString;

            HttpResponseMessage response;

            String build = "{" +
                    "\"faceId1\":" + "\"" + s1 + "\"" + "," +
                    "\"faceId2\":" + "\"" + s2 + "\"" +
                            "}";
            Console.WriteLine(build);

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(build);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                String JSON = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JSON);
                Debug.WriteLine(JSON);
            }

        }
    }
}
