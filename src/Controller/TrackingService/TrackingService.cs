using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Text;
using System.Threading;

namespace Controller
{
	public class TrackingService
	{
		public TrackingService()
		{

		}

		public bool CheckServices(string port)
        {
			HttpClient _client = createClient(port);
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

		private static HttpClient createClient(string port)
		{
			HttpClient client = new HttpClient();

			string baseUrl = "http://localhost:" + port + "/";

			client.BaseAddress = new Uri(baseUrl);
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));

			return client;
		}
	}
}
