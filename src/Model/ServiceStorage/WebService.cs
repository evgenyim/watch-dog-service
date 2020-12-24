using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Model.Other;

namespace Model
{
    public class WebService : Service
    {

        public string checkUrl { get; set; }
        private HttpClient _client;

        public WebService() { }

        public WebService(string url, string checkUrl = "api/products/isalive", int timeCheck=10)
        {
            this.url = url;
            this.checkUrl = checkUrl;
            this.timeCheck = timeCheck;
            _client = createClient();
        }

        public override Status IsAlive()
        {
            if (_client is null)
                _client = createClient();
           
            try
            {
                HttpResponseMessage response = _client.GetAsync(this.checkUrl).Result;
                bool status = response.IsSuccessStatusCode;
                return new WebStatus(status, url);
            }
            catch (Exception e)
            {
                Logger.Error($"Error occured in WebService.IsAlive()", e);
                return new WebStatus(false, url);
            }
        }

        private HttpClient createClient()
        {
            HttpClient client = new HttpClient();

            string baseUrl = url;

            try
            {

                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }
            catch (Exception e)
            {
                Logger.Error($"Error occured in WebService.IsAlive()", e);
            }
            return client;
        }

    }
}
