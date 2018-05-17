using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ActiveXHost
{
    [ComVisible(false)]
    public class HostMessage
    {
        public int code { get; set; }
        public object data { get; set; }
        public string msg { get; set; }

        public string eval { get; set; }

        public HostMessage(int code, object data, string msg = "")
        {
            this.code = code;
            this.data = data;
            this.msg = msg;
        }

        public HostMessage()
        {
            code = 0;
            data = null;
            msg = "";
            eval = "";
        }

        public static  HostMessage Empty()
        {
           return  new HostMessage(); 
        }

       
    }
}
