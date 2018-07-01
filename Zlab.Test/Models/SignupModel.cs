using System;
using System.Collections.Generic;
using System.Text;

namespace Zlab.Test
{
    public class SignupModel
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string code { get; set; }
    }
    public class SigninModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string devicemodel { get; set; }
        public string devicename { get; set; }
    }
}
