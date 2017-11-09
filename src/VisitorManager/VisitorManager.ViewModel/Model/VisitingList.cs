using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisitorManager.ViewModel
{
    /// <summary>
    /// 来访单
    /// </summary>
    public class VisitingListDto
    {
        public string vtl_id { get; set; }

        public DateTime vtl_time { get; set; }

        public string vtl_tmp { get; set; }
        public string vtl_tmp1 { get; set; }
    }
}
