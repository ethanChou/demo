using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using ActiveXHost;

namespace ActiveXHostDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HostContainer container = new HostContainer();
            container.LoadPlugin("PluginTest");
            container.Initialize("UserLogin");
            container.Dock = DockStyle.Fill;
            ConfigParameter paras = new ConfigParameter();
            paras.Id = "2";
            paras.MethodName = "Hello";
            paras.Args = new object[] { @"E:\image\0.jpg", 1, true };

            container.Excute(paras.ToJson());

            var res = container.Get("MyName");
            Console.WriteLine(res);

             res = container.Get("MyAge");
            Console.WriteLine(res);

             res = container.Get("MyFlag");
            Console.WriteLine(res);

            res = container.Set("MyName", "richmars");
            Console.WriteLine(res);

            res = container.Get("MyName");
            Console.WriteLine(res);

            this.Controls.Add(container);
        }
    }
}
