using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Veggie.APISystem {
    public static class APIConnection {

        public static HttpClient WebApliClient = new HttpClient();

        static APIConnection() {
            WebApliClient.BaseAddress = new Uri("https://localhost:44396/api");
            WebApliClient.DefaultRequestHeaders.Clear();
            WebApliClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
