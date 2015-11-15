using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NerfLockWindowsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class FacialRec
    {
        public FacialRec(string json)
        {
            json = @"{ ""data"" : " + json + "}";
            JObject jObject = JObject.Parse(json);
            var results = jObject["data"];
            foreach (var result in results)
            {
                faceId = (string)result["faceId"];
            }
        }

        public string faceId { get; set; }
    }

    public class Report {

        public string match {get; set;}
        public string confidence { get; set; }

        public Report(string json)
        {
            json = @"{ ""data"" : " + json + "}";
            JObject jObject = JObject.Parse(json);
            var results = jObject["data"];
            match = (string) results["isIdentical"];
            confidence = (string) results["confidence"];           
        }



    }

    public partial class MainWindow : Window
    {

        private static string s1;
        private static string s2;
        private static IFirebaseConfig config;
        private static IFirebaseClient client;
        private static string check;

        public MainWindow()
        {
            InitializeComponent();
            config = new FirebaseConfig
            {
                AuthSecret = "5mEJmENtkosiLf6Dd37yjc4RKXxONSsRuiNWKRJV",
                BasePath = "https://foreveralone.firebaseio.com/"
            };

            client = new FirebaseClient(config);
            Debug.WriteLine("Hell");
            getValue();
            working();
   
        }

        public async void working()
        {
            while (true)
            {
                FirebaseResponse response = await client.GetAsync("screenshot");
                String value = response.ResultAs<string>();
                Debug.WriteLine(value);

                    if (check != null && check.ToLower() != value.ToLower())
                    {
                        Task taskA = Task.Factory.StartNew(() => MakeRequest(0, "https://foreveralone.blob.core.windows.net/picture/myblog.jpg"));
                        taskA.Wait();
                        Console.WriteLine("taskA has completed.");
                        int time = 1000;
                        Thread.Sleep(time);

                        Task taskB = Task.Factory.StartNew(() => MakeRequest(1, "https://scontent-lax3-1.xx.fbcdn.net/hphotos-xpf1/t31.0-8/882115_161761973982107_1639044371_o.jpg"));
                        taskB.Wait();
                        Console.WriteLine("taskB has completed.");

                        Thread.Sleep(time);

                        if (taskB.IsCompleted && taskA.IsCompleted)
                            MakeRequest2();
                        else
                            Console.WriteLine("Timed out.");

                        check = value;

                    }

            }
        }

        public async void getValue()
        {
  
                FirebaseResponse response = await client.GetAsync("screenshot");

                string ben = response.ResultAs<string>();
                Debug.WriteLine("WTF: " + ben);
                check = ben;

        }

        async void MakeRequest(int image, String url)
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
            byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"" + url + "\"}");

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
        async void MakeRequest2()
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
            //Console.WriteLine(build);

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(build);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
                String JSON = await response.Content.ReadAsStringAsync();

                Console.WriteLine(JSON);
                Debug.WriteLine(JSON);
                MakeRequest3(JSON);

            }


        }

        public async void MakeRequest3(String json)
        {
            Report report = new Report(json);
            Debug.WriteLine(report.match + " " + report.confidence);         
            SetResponse response = await client.SetAsync("match", report.match );
            SetResponse response1 = await client.SetAsync("confidence", report.confidence);

            //FirebaseResponse response = await client.UpdateAsync(key, value);

        }
    }
}
