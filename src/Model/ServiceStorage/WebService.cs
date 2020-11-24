using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using Model.Other;

namespace Model
{
    class WebService : Service
    {

        private readonly string url;
        private readonly string checkUrl;
        private HttpClient _client;

        public WebService(string url, string checkUrl = "api/products/isalive")
        {
            this.url = url;
            this.checkUrl = checkUrl;
            _client = createClient();
        }

        public override Status IsAlive()
        {
           
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
