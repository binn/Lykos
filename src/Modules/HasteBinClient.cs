﻿// https://gist.github.com/b5e14d0c36f5a972060655b1aa875dbf
// CODE HERE COMES FROM ABOVE GIST. MAY BE MODIFIED.
// All rights belong to original creator, and not the author of this software.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lykos.Modules
{
    public class HasteBinClient
    {
        private static HttpClient _httpClient;
        private string _baseUrl;

        static HasteBinClient()
        {
            _httpClient = new HttpClient();
        }

        public HasteBinClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<HasteBinResult> Post(string content)
        {
            string fullUrl = _baseUrl;
            if (!fullUrl.EndsWith("/"))
            {
                fullUrl += "/";
            }
            string postUrl = $"{fullUrl}documents";

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(postUrl));
            request.Content = new StringContent(content);
            HttpResponseMessage result = await _httpClient.SendAsync(request);

            if (result.IsSuccessStatusCode)
            {
                string json = await result.Content.ReadAsStringAsync();
                HasteBinResult hasteBinResult = JsonConvert.DeserializeObject<HasteBinResult>(json);

                if (hasteBinResult?.Key != null)
                {
                    hasteBinResult.FullUrl = $"{fullUrl}{hasteBinResult.Key}";
                    hasteBinResult.IsSuccess = true;
                    hasteBinResult.StatusCode = 200;
                    return hasteBinResult;
                }
            }

            return new HasteBinResult()
            {
                FullUrl = fullUrl,
                IsSuccess = false,
                StatusCode = (int)result.StatusCode
            };
        }
    }

    // Define other methods and classes here
    public class HasteBinResult
    {
        public string Key { get; set; }
        public string FullUrl { get; set; }
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
    }
}
