using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;


namespace Model
{
    class WebService : Service
    {
        string url;

        public WebService(string url)
        {
            this.url = url;
        }

        public override bool IsAlive()
        {
            HttpClient _client = createClient();
            try
            {
                HttpResponseMessage response = _client.GetAsync("api/products/isalive").Result;
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                return false;
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
