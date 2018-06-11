using System;
using System.Collections.Generic;
using System.Text;

namespace Zlab.UtilsCore
{
    public class GuidHelper
    {
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString().Replace("-","");
        }
    }
}
