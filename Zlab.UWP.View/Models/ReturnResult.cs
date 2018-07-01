using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlab.UWP.View.Models
{
    public class ReturnResult<T>
    {
        public int code { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }
    public class ReturnResult
    {
        public static readonly int SuccessCode = 200;
        public static readonly int FailCode = 400;
    }
}
