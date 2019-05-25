using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 侧翻台.winfrom
{
    public partial class USER : Form
    {
        public USER()
        {
            InitializeComponent();
        }
        config con = new config();
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "admin")
            {
                if (textBox2.Text == "admin")
                {
                    con.admin = config.user.admin;
                  
                    this.Close();
                }
            }
            if (textBox1.Text == "user")
            {
                if (textBox2.Text == "user")
                {
                    con.admin = config.user.user;
                 
                    this.Close();
                }

            }
        }
    }
}
