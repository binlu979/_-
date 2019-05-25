using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using spMODBUS;
using System.Threading;

namespace 侧翻台
{
    public partial class Set : Form
    {
        Thread t_xunshi;
        public Set()
        {
            InitializeComponent();
          
        }

        #region 设置称重
        private void button6_Click_1(object sender, EventArgs e)
        {
            t_xunshi = new Thread(info);
            t_xunshi.Start();
        }
        bool run = true;
        private void info()
        {
            while (run)
            {

                if (plc.port_open)
                {
                    ushort[] a = PLC.mk_03;
                    ushort[] b = PLC.mk_02;

                    if (a != null)
                    {
                        try
                        {
                            this.Invoke((EventHandler)delegate//线程锁放置线程之间干扰
                            {
                                if (a != null && b != null)
                                {
                                    lableyg1.Text = ((float)a[0] * 0.0321f).ToString();
                                    lableyg2.Text = ((float)a[1] * 0.0321f).ToString();
                                    lableyg3.Text = ((float)b[0] * 0.0321f).ToString();
                                    lableyg4.Text = ((float)b[1] * 0.0321f).ToString();
                                }
                            });
                        }
                        catch
                        {

                        }
                        

                        Thread.Sleep(100);
                    }
                }
                else
                {//.Suspend()


                }


            }

        }
        #region 油缸上升
        private void button1_Click(object sender, EventArgs e)
        {
            /*
            if (trackBar1.Value == 0)
            {
                MessageBox.Show("速度必须大于0");
                return;
            }
          */


            List<PLC.my_yougang> yougang = new List<PLC.my_yougang>();


            foreach (Control c in panel2.Controls)
            {
                if (c is CheckBox && ((CheckBox)c).Checked == true)
                {

                    if (int.Parse(c.Tag.ToString()) == 0)
                    {

                        yougang.Add(PLC.my_yougang.youguang1);
                    }
                    else if (int.Parse(c.Tag.ToString()) == 1)
                    {
                        yougang.Add(PLC.my_yougang.yougang2);
                    }
                    else if (int.Parse(c.Tag.ToString()) == 2)
                    {
                        yougang.Add(PLC.my_yougang.yougang3);
                    }
                    else if (int.Parse(c.Tag.ToString()) == 3)
                    {
                        yougang.Add(PLC.my_yougang.yougang4);
                    }

                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {/*
            if (trackBar1.Value == 0)
            {
                MessageBox.Show("速度必须大于0");
                return;
            }*/
            List<PLC.my_yougang> yougang = new List<PLC.my_yougang>();


            foreach (Control c in panel2.Controls)
            {
                if (c is CheckBox && ((CheckBox)c).Checked == true)
                {


                    if (int.Parse(c.Tag.ToString()) == 0)
                    {

                        yougang.Add(PLC.my_yougang.youguang1);
                    }
                    else if (int.Parse(c.Tag.ToString()) == 1)
                    {
                        yougang.Add(PLC.my_yougang.yougang2);
                    }
                    else if (int.Parse(c.Tag.ToString()) == 2)
                    {
                        yougang.Add(PLC.my_yougang.yougang3);
                    }
                    else if (int.Parse(c.Tag.ToString()) == 3)
                    {
                        yougang.Add(PLC.my_yougang.yougang4);
                    }

                }
            }
            plc.on_fanqi(true, -trackBar1.Value);
            // modbus.Write_D(9, 1, 1655);
        }

        private void button2_Click(object sender, EventArgs e)

        {
            plc.my_yeyazhan_open = false;
            plc.on_fanqi(false);



        }
        private void button4_Click(object sender, EventArgs e)
        {
            plc.on_fanqi(false);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            plc.my_yeyazhan_open = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("注意必须485报错后才可点击", "系统提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {
                plc.red_dzc_open();
                return;
            }

            else
            {
                return;
            }

        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar1.Value.ToString();
        }
        #endregion
        #endregion

        PLC plc = new PLC();
    
        bmodbus modbus = new bmodbus();

      
       
        private void Set_FormClosing(object sender, FormClosingEventArgs e)
        {
            run = false;
        }
        #region 设置称重
        private void button7_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add(64);
            timer1.Start();
        }
        ad ad = new ad();
        int id = 0;
        config con = new config();

        private void timer1_Tick(object sender, EventArgs e)
        {
            float[][] mod_cz = ad.mod_value;


            for(int i=0; i < 8; i++)
            {
              
                for (int a = 0; a < 8; a++)
                {
                    if (ad.mod_value[i] == null)
                    {
                        return;
                    }
                    dataGridView1.Rows[id].Cells[3].Value = ad.mod_value[i][a].ToString();
                    dataGridView1.Rows[id].Cells[4].Value = con.red_jiaozheng(id);
                    dataGridView1.Rows[id].Cells[5].Value = ad.mod_value[i][a]-con.red_jiaozheng(id);
                    dataGridView1.Rows[id].Cells[1].Value = a.ToString();
                    dataGridView1.Rows[id].Cells[0].Value = (i+1).ToString();
                    id++;
                }
               //
            }
            id = 0;
            // dt = mod_cz;
         
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int myid=0;
            con.write_jiaozheng(1, 30);
            float[][] mod_cz = ad.mod_value;

            try
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int a = 0; a < 8; a++)
                    {
                        //  dataGridView1.Rows[id].Cells[3].Value = ad.mod_value[i][a].ToString();
                        con.write_jiaozheng(myid, (int)ad.mod_value[i][a]);
                        myid++;
                    }
                    //
                }

                MessageBox.Show("矫正成功","系统提示");
            }
            catch
            {
                MessageBox.Show("写入数据失败", "系统提示");

            }
          
        }




        #endregion

        #region 通讯设置在
        private void tongxun()
        {
            txt_tx_com.Text = con.ck_com;
            txt_tx_bt.Text = con.tx_Baud;
            txt_tx_jiaoyan.Text = con.tx_jiaoyanwei;
            txt_tx_stop.Text = con.tx_stop;
            txt_tx_time.Text = con.tx_outtime;
            txt_tx_sjc.Text = con.tx_sjc;
        }

        #endregion

        private void Set_Load(object sender, EventArgs e)
        {
            tongxun();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            if ((int)con.admin >0)
            {
                MessageBox.Show("此项危险请使用管理员账号登录！");
                return;
            }
            try
            {
                con.ck_com = txt_tx_com.Text;
                con.tx_Baud = txt_tx_bt.Text;
                con.tx_jiaoyanwei = txt_tx_jiaoyan.Text;
                con.tx_outtime = txt_tx_time.Text;
                con.tx_sjc = txt_tx_sjc.Text;
                con.tx_stop = txt_tx_stop.Text;
                MessageBox.Show("写入成功");
            }
            catch
            {
                MessageBox.Show("写入失败!");
            }
           
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            txt_tx_com.Text = con.ck_com;
            txt_tx_bt.Text = con.tx_Baud;
            txt_tx_jiaoyan.Text = con.tx_jiaoyanwei;
            txt_tx_stop.Text = con.tx_stop;
            txt_tx_time.Text = con.tx_outtime;
            txt_tx_sjc.Text = con.tx_sjc;
        }

        private void qatCustomizePanel1_Load(object sender, EventArgs e)
        {

        }
    }
}
