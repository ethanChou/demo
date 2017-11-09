using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisitorManager.ViewModel
{
    public enum VisitorStatus
    {
        /// <summary>
        /// 正在访问
        /// </summary>
        In = 1,
        /// <summary>
        /// 已经离开
        /// </summary>
        Out = 2,
        /// <summary>
        /// 逾期
        /// </summary>
        Expire = 3,
        /// <summary>
        /// 暂存待办
        /// </summary>
        Pause = 4,
        /// <summary>
        /// 手续合法，等待提交进入
        /// </summary>
        Waiting = 5
    }

    /// <summary>
    /// 来访者信息
    /// </summary>
    public class VisitorDto : ViewModelBase
    {
        public string vt_id { get; set; }
        public string vt_name { get; set; }
        public bool vt_sex { get; set; }

        /// <summary>
        /// 1.身份证，2员工卡，3其他
        /// </summary>
        public int vt_identify_type { get; set; }
        public string vt_identify_NO { get; set; }
        public string vt_identify_imgurl { get; set; }
        public string vt_address { get; set; }
        public string vt_imgurl { get; set; }
        public string tmpcard_id { get; set; }

        /// <summary>
        /// 0 临时卡，1 正式卡
        /// </summary>
        public int tmpcard_type { get; set; }
        public DateTime vt_in_time { get; set; }
        public DateTime vt_out_time { get; set; }

        /// <summary>
        /// 1.正在访问，2.已经离开，3逾期未还
        /// </summary>
        public VisitorStatus vt_status { get; set; }

        /// <summary>
        /// 访问单位id,可为空
        /// </summary>
        public string vt_visit_department_id { get; set; }


        /// <summary>
        /// 被访问人员id,可为空
        /// </summary>
        public string vt_visit_employee_id { get; set; }
     

        /// <summary>
        /// 来访单编号
        /// </summary>
        public string vt_visitinglist_id { get; set; }


        public VisitorDto Clone()
        {
            VisitorDto v = new VisitorDto();
            v.vt_id = vt_id;
            v.vt_address = vt_address;
            v.vt_identify_imgurl = vt_identify_imgurl;
            v.vt_identify_NO = vt_identify_NO;
            v.vt_identify_type = vt_identify_type;
            v.vt_imgurl = vt_imgurl;
            v.vt_in_time = vt_in_time;
            v.vt_name = vt_name;
            v.vt_out_time = vt_out_time;
            v.vt_sex = vt_sex;
            v.vt_status = vt_status;
            v.vt_visit_department = vt_visit_department;
            v.vt_visit_department_id = vt_visit_department_id;
            v.vt_visit_employee = vt_visit_employee;
            v.vt_visit_employee_id = vt_visit_employee_id;
            v.vt_visitinglist_id = vt_visitinglist_id;
            v.tmpcard_id = tmpcard_id;
            v.tmpcard_type = tmpcard_type;
            return v;
        }

        public void NoticeAll()
        {
            NotifyChange("vt_identify_NO");
            NotifyChange("vt_sex");
            NotifyChange("vt_identify_type");
            NotifyChange("vt_address");
            NotifyChange("vt_identify_imgurl");

            NotifyChange("vt_imgurl");

            NotifyChange("vt_name");
            NotifyChange("tmpcard_id");
            NotifyChange("tmpcard_type");
            NotifyChange("vt_status");

            NotifyChange("vt_visit_department");
            NotifyChange("vt_visit_department_id");
            NotifyChange("vt_visit_employee");
            NotifyChange("vt_visit_employee_id");
        }
    }
}
