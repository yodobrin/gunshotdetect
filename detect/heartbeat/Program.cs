using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Text;

namespace heartbeat
{
    class Program
    {
         private const string URL = "https://gunshotdetectfunctions.azurewebsites.net/api/Heartbeat?code=wAtzCbAJnMgwsGfC2gSVXiljzzFnus6qslQJGUBiYqFIWL1utZ4SGw==";
        
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            // client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string payload = "{\"DeviceId\": 5,\"DeviceLocation\": \"38.8951 , -77.0364\"}";

            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");


            while(true)
            {
                Thread.Sleep(60000);
                
                HttpResponseMessage response = client.PostAsync(new Uri(URL),c).Result;  
            }
            
            
        }
    }
}
