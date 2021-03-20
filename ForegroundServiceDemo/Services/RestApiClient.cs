using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ForegroundServiceDemo.Services
{
    public class RestApiClient
    {
        private const string _endPoint = "urldelServicio";

        private readonly Lazy<HttpClient> clientHolder = new Lazy<HttpClient>();
        private HttpClient Client => clientHolder.Value;
        public RestApiClient()
        {
            try
            {
                Client.Timeout = TimeSpan.FromMinutes(10);
            }
            catch (Exception e)
            {
                //Crashes.TrackError(e);
            }
        }
    }
}
