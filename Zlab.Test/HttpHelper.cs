using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Zlab.Test
{
    public class HttpHelper
    {
        public static async Task<string> PostAsync(string url, string body, bool isurl = false)
        {
            var contenttype = "application/json";
            if (isurl) contenttype = "application/x-www-form-urlencoded";
            var content = new StringContent(body, Encoding.UTF8, contenttype);
            var hc = new HttpClient();
            var response = await hc.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }
        public static async Task<string> GetAsync(string url)
        {
            var hc = new HttpClient();
            return await hc.GetStringAsync(url);
        }

        public async static Task<String> HttpPost(string url, string body, List<Cookie> cookies, bool isurl = false)
        {
            var contenttype = "application/json";
            if (isurl) contenttype = "application/x-www-form-urlencoded";
            var content = new StringContent(body, Encoding.UTF8, contenttype);
            var handler = new HttpClientHandler()
            {
                UseCookies = true,
            };
            if (cookies != null && cookies.Any())
                foreach (var cookie in cookies)
                {
                    handler.CookieContainer.Add(cookie);
                }
            HttpClient hc = new HttpClient(handler);

            var response = await hc.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }
        public async static Task<string> HttpGet(string url, List<Cookie> cookies)
        {
            var handler = new HttpClientHandler()
            {
                UseCookies = true,
            };
            if (cookies != null && cookies.Any())
                foreach (var cookie in cookies)
                {
                    handler.CookieContainer.Add(cookie);
                }

            var hc = new HttpClient(handler);
            var response = await hc.GetStringAsync(url);
            return response;
        }
    }
}
