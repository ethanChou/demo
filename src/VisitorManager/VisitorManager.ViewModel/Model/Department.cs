using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisitorManager.Model
{
    /// <summary>
    /// 单位
    /// </summary>
    public class Department
    {
        /// <summary>
        /// 
        /// </summary>
        public string dep_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string dep_parent_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ad_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string dep_NO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string dep_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool dep_isspacial { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool dep_isdeleted { get; set; }

        public override string ToString()
        {
            return dep_name;
        }
    }
}
