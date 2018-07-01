
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Storage.Streams;
using System;

namespace Zlab.UWP.View.Helpers
{
    public class HttpHelper
    {
        private static readonly string JsonType = "application/json";
        private static readonly string UrlType = "application/x-www-form-urlencoded";
        public static async Task<string> PostAsync(string url, string body, bool isurl=false)
        {
            var contentType = isurl ? UrlType : JsonType;
            var content = new HttpStringContent(body, UnicodeEncoding.Utf8, contentType);
            var hc = new HttpClient();
            var response = await hc.PostAsync(new Uri(url), content);
            return await response.Content.ReadAsStringAsync();
        }
        public static async Task<string> GetAsync(string url)
        {
            var hc = new HttpClient();
            return await hc.GetStringAsync(new Uri(url));
        }

    }
}
