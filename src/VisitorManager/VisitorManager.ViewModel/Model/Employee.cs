using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisitorManager.ViewModel
{
    public class EmployeeDto
    {
        public string emp_id { get; set; }
        public string emp_NO { get; set; }
        public string emp_name { get; set; }
        public string dep_id { get; set; }
        public string emp_cardNO { get; set; }
        public string emp_tel { get; set; }
        public string emp_imgurl { get; set; }
        public bool emp_isdeleted { get; set; }

        public override string ToString()
        {
            return emp_name;
        }
    }
}
