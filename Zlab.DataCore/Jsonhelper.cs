using Newtonsoft.Json;

namespace Zlab.DataCore
{
    public static class JsonHelper
    {
        public static string ToJson<T>(this T data)
        {
            return  JsonConvert.SerializeObject(data);
        }
        public static T ToObj<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
