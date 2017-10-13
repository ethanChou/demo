using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisitorManager.Model
{
    public class Blacklist
    {
        public string bl_id { get; set; }
        public int bl_identify_type { get; set; }
        public string bl_identify_NO { get; set; }
        public string bl_reason { get; set; }
    }
}
