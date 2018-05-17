using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PluginTest
{
    public partial class UserLogin : UserControl
    {
        public UserLogin()
        {
            InitializeComponent();
            MyName = "zhouxianming";
        }

        public object MyName { get; set; }

        public int MyAge { get; set; }

        public bool MyFlag { get; set; }

        public string Hello(string id, int numb, bool flag)
        {
            listView1.Items.Add(string.Format("id:{0},numb:{1},flag:{2}", id, numb, flag));
            if (CustumEventFired != null)
            {
                CustumEventFired.Invoke(id, "CustumEventFired", "OK");
                //CustumEventFired(id, "CustumEventFired", "OK");
            }
            return string.Format("id:{0},numb:{1},flag:{2}", id, numb, flag);
        }

        public delegate void TestEventHandler(string id, string evt, string data);

        public event TestEventHandler CustumEventFired;
    }
}
