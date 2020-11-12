using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;

namespace Model
{
    class WebService : Service
    {

        private readonly string url;
        private readonly string checkUrl;
        private readonly int timeCheck;
        private DateTime lastChecked;
        private HttpClient _client;
        private bool lastStatus;

        public WebService(string url, string checkUrl = "api/products/isalive", int timeCheck = 10)
        {
            this.url = url;
            this.checkUrl = checkUrl;
            this.timeCheck = timeCheck;
            lastChecked = DateTime.MinValue;
            lastStatus = false;
            _client = createClient();
        }

        public override Status IsAlive()
        {
            DateTime now = DateTime.Now;

            TimeSpan diff = now - lastChecked;
            if (diff.TotalSeconds < timeCheck && lastChecked != null)
                return new WebStatus(lastStatus, url);
            try
            {
                HttpResponseMessage response = _client.GetAsync(this.checkUrl).Result;
                lastStatus = response.IsSuccessStatusCode;
                lastChecked = DateTime.Now;
                return new WebStatus(lastStatus, url);
            }
            catch (Exception e)
            {
               Console.WriteLine($"Error occured in WebService.IsAlive(): {e.Message}");
                lastStatus = false;
                lastChecked = DateTime.Now;
                return new WebStatus(false, url);
            }
        }

        private HttpClient createClient()
        {
            HttpClient client = new HttpClient();

            string baseUrl = this.url;

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

    }
}
