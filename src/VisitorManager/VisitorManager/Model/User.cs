using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisitorManager.Model
{
    public class User
    {
        public string user_id { get; set; }

        public string user_name { get; set; }

        public string user_pass { get; set; }

        public DateTime user_logintime { get; set; }

        public DateTime user_loginouttime { get; set; }

        public DateTime user_isdeleted { get; set; }
    }


}
