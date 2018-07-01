using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Zlab.DataCore
{
    public class ReturnResult
    {
        private static readonly uint successcode = 200;
        private static readonly uint failcode = 400;
        private static readonly string successstring = "success";
        private static readonly string failstring = "fail";


        public static string Success<T>(T data) where T : new()
        {
            var dto = new
            {
                code = successcode,
                msg = successstring,
                data
            };
            return dto.ToJson();
        }
        public static string Success()
        {
            var dto = new
            {
                code = successcode,
                msg = successstring,
                data = ""
            };
            return dto.ToJson();
        }
        public static string Success(string msg)
        {
            var dto = new
            {
                code = successcode,
                msg,
                data = ""
            };
            return dto.ToJson();
        }
        public static string Fail()
        {
            var dto = new
            {
                code = failcode,
                msg = failstring,
                data = ""
            };
            return dto.ToJson();
        }
        public static string Fail<T>(T data) where T : new()
        {
            var dto = new
            {
                code = failcode,
                msg = failstring,
                data
            };
            return dto.ToJson();
        }
        public static string Fail(string msg)
        {
            var dto = new
            {
                code = failcode,
                msg,
                data = ""
            };
            return dto.ToJson();
        }
    }
    public class ReturnResult<T>
    {
        public string code { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }

    public class ReturnMessage
    {
        public static readonly string json = "application/json";

        public static HttpResponseMessage Fail()
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(ReturnResult.Fail(), Encoding.UTF8, json),
            };
        }
        public static HttpResponseMessage Fail(string msg)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(ReturnResult.Fail(msg), Encoding.UTF8, json),
            };
        }

        public static HttpResponseMessage Success(string data)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(ReturnResult.Success(data), Encoding.UTF8, json),
            };
        }
        public static HttpResponseMessage Success<T>(T data) where T : new()
        {
            var content = ReturnResult.Success(data);
            return new HttpResponseMessage
            {
                Content = new StringContent(content, Encoding.UTF8, json),
            };
        }
        public static HttpResponseMessage Success()
        {
            var content = ReturnResult.Success();
            return new HttpResponseMessage
            {
                Content = new StringContent(content, Encoding.UTF8, json),
            };
        }
    }
}
