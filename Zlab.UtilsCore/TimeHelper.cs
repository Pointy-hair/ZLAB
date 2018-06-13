using System;
using System.Collections.Generic;
using System.Text;

namespace Zlab.UtilsCore
{
    public class TimeHelper
    {
        public static long GetUnixTimeMilliseconds() 
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        public static long GetLocalTimeMilliseconds()
        {
            var utcms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return utcms + (long)DateTimeOffset.Now.Offset.TotalMilliseconds;
        }
        public static long GetUnixTimeSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        public static long GetLocalTimeSeconds()
        {
            var utcms = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return utcms + (long)DateTimeOffset.Now.Offset.TotalSeconds;
        }
    }
}
