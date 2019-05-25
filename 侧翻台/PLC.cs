using System;
using System.Collections.Generic;
using System.Windows.Forms;
using spMODBUS;
using System.Threading;

namespace 侧翻台
{
    public delegate void ERROR(bool err, string message,int err_id); //声明一个委托错误
    public delegate void jiaodu();
    public partial class PLC : Form
    {
        
        //处理异常
        private delegate void ChildThreadExceptionHandler(string message);
        private event ChildThreadExceptionHandler ChildThreadException;
        public event jiaodu st_jiaodu;//声明一个事件
        protected virtual void OnChildThreadException(string message)
        {
            if (ChildThreadException != null)
                ChildThreadException(message);
        }
        //处理异常
        bmodbus modbus = new bmodbus();
        bmodbus modbus2 = new bmodbus();
        public Thread t_red_io;
        
        bool cg;
        //bool port_open;
        public static ushort[] mk_02;// = new List<ushort[]>() ;
        public static ushort[] mk_03;
        public List<ushort[]> ad = new List<ushort[]>(7);
        /// <summary>
        /// 定义油缸角度
        /// </summary>
        public Double[] _jiaodu1=new Double[2];
        public Double[] _jiaodu2 = new Double[2];
        /// <summary>
        /// 心跳复位
        /// </summary>
        public bool xintiao
        {
           set { Write_plc_m(106, true); }
        }

        public Double[] jiaodu1 
        {
            get { return _jiaodu1; }
        }
        public Double[] jiaodu2
        {
            get { return _jiaodu2; }
        }
        /// <summary>
        /// 定义动作的油缸
        /// </summary>
        public bool yg_dz1 = false;
        public bool yg_dz2 = false;
        public bool yg_dz3 = false;
        public bool yg_dz4 = false;
        #region 液压站
        #region 定义设置项
        /// <summary>
        /// 定义翻起的平台
        /// </summary>
        public static my_fanqi _my_pingtai;
        public enum my_fanqi
        {
            shuoyou = 0,
            pingban1 = 1,
            pingban2 = 2,

        }
        public my_fanqi my_pingtai
        {
            get { return _my_pingtai; }
            set { _my_pingtai = value; }
        }
        #region 打开关闭液压站
        static bool _my_yeyazhan_open;
        public   bool my_yeyazhan_open
        {
            get { return yyz_on(); }
            set { _my_yeyazhan_open = yyzqd(value,1); }

        }
        private bool yyz_on()
        {
            //Read_plc_m
            if (Read_plc_m((ushort)plc_m.yyz_dianji1)|| Read_plc_m((ushort)plc_m.yyz_dianji2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        /// <summary>
        /// 定义油缸
        /// </summary>
        public enum my_yougang
        {
            
            youguang1=110,
            yougang2 =120,
            yougang3 =130,
            yougang4 =140,
        }
        #endregion
        #region 翻起动作
        public void on_fanqi(bool qd)
        {

            on_fanqi(false, 0);
        }
        public bool 平台起落;
        ushort sd;
        /// <summary>
        /// 设备运动中
        /// </summary>
        private static bool _dz_run;
        /// <summary>
        /// 设备运动中
        /// </summary>
        public static bool dz_run
        {
            get { return _dz_run; }
            set { _dz_run = value; }
        }
        private static bool _sb_tiaoshi=false;
        public static bool sb_tiaoshi
        {
            get { return _sb_tiaoshi; }
            set { _sb_tiaoshi = value; }

        }
        /// <summary>
        /// 油缸动作
        /// </summary>
        /// <param name="qd">启动</param>
        /// <param name="yg">油缸</param>
        /// <param name="value">速度</param>
        public void on_fanqi(bool qd, int value)
        {

            if (qd)
            {
                if (Form1.ERROR)
                {
                    MessageBox.Show("设备故障，请消除后再运行");
                }
                if (!my_yeyazhan_open)
                {
                    DialogResult dr = MessageBox.Show("液压站未启动，请先启动液压站?", "系统提示", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.OK)//如果点击“确定”按钮
                    {
                        my_yeyazhan_open = true;
                        return;
                    }

                    else
                    {
                        return;
                    }

                }

                sd = (ushort)(value * 320 + 32000);//溢流阀速度换算;
                if (sd > 32000)
                {
                    平台起落 = true;
                }
                else { 平台起落 = false; }
 

                if (my_pingtai == my_fanqi.shuoyou||_sb_tiaoshi)
                {
                    write_plc_d((ushort)my_yougang.youguang1, sd);
                    write_plc_d((ushort)my_yougang.yougang2, sd);
                    write_plc_d((ushort)my_yougang.yougang3, sd);
                    write_plc_d((ushort)my_yougang.yougang4, sd);

                    write_plc_m(110, true);//溢流阀打开

                }
                else if (my_pingtai == my_fanqi.pingban1)
                {
                    write_plc_d((ushort)my_yougang.youguang1, sd);
                    write_plc_d((ushort)my_yougang.yougang2, sd);
                    write_plc_d((ushort)my_yougang.yougang3, sd);
                    write_plc_d((ushort)my_yougang.yougang4, sd);
                    write_plc_m((ushort)plc_m.yyz_yiliufa1, true);//溢流阀打开
                }
                else if (my_pingtai == my_fanqi.pingban2)
                {
                    write_plc_d((ushort)my_yougang.youguang1, sd);
                    write_plc_d((ushort)my_yougang.yougang2, sd);
                    write_plc_d((ushort)my_yougang.yougang3, sd);
                    write_plc_d((ushort)my_yougang.yougang4, sd);
                    write_plc_m((ushort)plc_m.yyz_yiliufa2, true);//溢流阀打开
                }

                _dz_run = true;//设备运动
            }
            else
            {
                _sb_tiaoshi = false;
                _dz_run = false;//设备翻起停止
                sd = 32000;//溢流阀速度换算;
                write_plc_d((int)my_yougang.youguang1, sd);
                write_plc_d((int)my_yougang.yougang2, sd);
                write_plc_d((int)my_yougang.yougang3, sd);
                write_plc_d((int)my_yougang.yougang4, sd);
                yg_dz1 = false;
                yg_dz2 = false;
                yg_dz3 = false;
                yg_dz4 = false;
                write_plc_m(110, false);//溢流阀关闭
                write_plc_m((ushort)plc_m.yyz_yiliufa2, false);//溢流阀关闭
                write_plc_m((ushort)plc_m.yyz_yiliufa1, false);//溢流阀关闭
                                                               // run = false;
            }

        }
      
        #endregion

        #region 定义PLC的M点
        /// <summary>
        /// 定义PLC的M点
        /// </summary>
        public enum plc_m
        {
            /// <summary>
            /// 液压电机1
            /// </summary>
            yyz_dianji1=101,
            /// <summary>
            /// 液压电机2
            /// </summary>
            yyz_dianji2=100,
            /// <summary>
            /// 1#溢流阀
            /// </summary>
            yyz_yiliufa1=105,
            /// <summary>
            /// 2#溢流阀
            /// </summary>
            yyz_yiliufa2=104,
            fan=103,
            jiare=102,

        }
        #endregion
       
        #region 液压站启动
        /// <summary>
        /// 液压站启动
        /// </summary>
        /// <param name="value">启动还是关闭</param>
        public  bool yyzqd(bool value,int cg)
        {
            //return true;
            if (value)
            {
               
                if (my_pingtai == my_fanqi.shuoyou)
                {
                   if( write_plc_m((ushort)plc_m.yyz_dianji1, true))
                    {
                        if(write_plc_m((ushort)plc_m.yyz_dianji2, true))
                        {

                            return true;
                        }
                    }
                    
                }
                else if (my_pingtai == my_fanqi.pingban1)
                {
                   if( write_plc_m((ushort)plc_m.yyz_dianji1, true))
                    {
                        return true;
                    }
                }
                else if (my_pingtai == my_fanqi.pingban2)
                {
                    if(write_plc_m((ushort)plc_m.yyz_dianji2, true))
                    {
                        return true;
                    }
                }
                return false;

            }
            else
            {
                on_fanqi(false);
                if(write_plc_m((ushort)plc_m.yyz_dianji1, false))
                {

                    
                    return !write_plc_m((ushort)plc_m.yyz_dianji2, false);
                   
                   
                }
                else
                {
                    return true;
                }
               
            }
        }
        #endregion
        #region 写入到PLC
        /// <summary>
        /// 写入到PLC
        /// </summary>
        /// <param name="id">PLC地址</param>
        /// <param name="value">值</param>
        public bool write_plc_m(ushort id ,bool value)
        {
            ushort myid = (ushort)(id + 2048);
            modbus2.Write_M(4, myid, value,out cg);
            if (cg)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region 读取PLC
        public bool Read_plc_m(ushort id)
        {
           
          return  Read_plc_m( id,1)[0];
        }
        /// <summary>
        /// 读取PLC_M
        /// </summary>
        /// <param name="id">PLC地址</param>
        /// <param name="value">值</param>
        public bool[] Read_plc_m(ushort id,ushort nul)
        {
            ushort myid = (ushort)(id + 2048);
            // modbus.Write_M(4, myid, value, out cg);
            try
            {
                bool[] de = modbus2.red_M(4, myid, nul, out cg);
               
                if (cg)
                {
                    return de;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                return null;
                MessageBox.Show("读取PLC失败"+ex);
            }
        
           
        }
        /// <summary>
        /// 写入到M
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool Write_plc_m (ushort id,bool value)
        {
             id = (ushort)(id + 2048);
            try
            {
             modbus2.Write_M(4, id, value, out cg);
             //   modbus.Write_D(4, id, 0);
                if (cg)
                {
                    return true;
                }
                else { return false; }
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region 写入到PLCd
        /// <summary>
        /// 写入到PLC
        /// </summary>
        /// <param name="id">PLC地址</param>
        /// <param name="value">值</param>
        private bool write_plc_d(ushort id, ushort value)
        {
            ushort myid = (ushort)(id + 4096);
            // modbus.Write_M(4, myid, value, out cg);
            modbus2.Write_D(4, myid, value);
            if (cg)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #endregion
        public event ERROR error;//声明一个委托事件
        private bool _run;//程序运行
        public bool port_open
        {
            get { return modbus.portopen; }
            
        }
        public bool run
        {
            get { return _run; }
            set { _run = value; }
        }
      
        public PLC()
        {
            InitializeComponent();
            open();
          
        }
      
        config con = new config();
        
        public void open()
        {
            string com = con.ck_com;
            int btl = int.Parse(con.tx_Baud);
            int sjc = int.Parse(con.tx_sjc);
            System.IO.Ports.Parity pr = new System.IO.Ports.Parity();
            if (con.tx_jiaoyanwei == "无")
            {
                pr = System.IO.Ports.Parity.None;
            }
            else if (con.tx_jiaoyanwei == "奇")
            {
                pr = System.IO.Ports.Parity.Odd;
            }
            else if (con.tx_jiaoyanwei == "偶")
            {
                pr = System.IO.Ports.Parity.Even;
            }
            System.IO.Ports.StopBits jyw = new System.IO.Ports.StopBits();
            if (con.tx_stop == "1")
            {
                jyw = System.IO.Ports.StopBits.One;
            }
            else if (con.tx_stop == "2")
            {
                jyw = System.IO.Ports.StopBits.Two;

            }
            else
            {
                jyw = System.IO.Ports.StopBits.None;
            }
            int outitme = int.Parse(con.tx_outtime);
        
            modbus.openport_RUN(com, btl, sjc, pr, jyw, outitme, out cg);

            modbus2.openport_Ascii("COM3", 19200, 7, System.IO.Ports.Parity.Even, System.IO.Ports.StopBits.One, out cg);
         
        }
        Type t;
       public void red_jiaodu()
        {
            try
            {
                ushort[] jd = modbus.Red_D(2, 5, 2);
                ushort[] jd2 = modbus.Red_D(3, 5, 2);
                if (jd != null&&jd2!=null)
                {
                    _jiaodu1[0] = jd[0] * 0.0009155553;
                    _jiaodu1[1] = jd[1] * 0.0009155553;
                    _jiaodu2[0] = jd2[0] * 0.0009155553;
                    _jiaodu2[1] = jd2[1] * 0.0009155553;
                }
                st_jiaodu();
                
            }
            catch(Exception ex)
            {
                error_wt(true, "角度传感器错误", 10);
                LogHelper.log.error(t, "角度传感器错误"+ex);
            }

        }
    
      
        /// <summary>
        /// 读取电子尺
        /// </summary>
        public  void red_dzc_open()
        {
            
            
          
            ChildThreadException = null;
          
         //   ChildThreadException += PLC_ChildThreadException;
            if (port_open)
            {
                run = true;
                t_red_io = new Thread(red_io);
                t_red_io.Start();
            }else
            {
                run = false;
                if (error != null)
                {

                    error_wt(true, "RS-485通讯错误，请检查链接，故障排除后点击消除3", 9);
                }
                
            }
           
               //red_io();
        }
        private void error_wt(bool er,string str,int jb)
        {
         
            if (error != null)
            {
                error(true, str, 9);
              //  on_fanqi(false);
            }
            else
            {
                MessageBox.Show(str);
            }
        }
        private void PLC_ChildThreadException(string message)
        {
            MessageBox.Show(message,"系统提示");
          
        }

        public void w_yougang(ushort id, ushort value)
        {
            if (modbus.portopen)
            {
                bool cg= modbus.Write_D(9, id, value);

            }
           
        }
        public ushort[] red_yougang()
        {
            if (modbus.portopen)
            {
              return   modbus.Red_D(9, 0, 4);
            }
            return null;
        }
        #region 循环读取485数据
         public  ushort[] ugsd1, ugsd2, ugsd3, ugsd4;
      public  Thread t_tongbu;
        
        private void red_io()
        {
          
            while (run)
            {
                if (modbus.portopen)
                {
                    try
                    {
                        red_jiaodu();//读取角度
                        mk_03 = modbus.Red_D(2, 0, 2);
 
                        mk_02 = modbus.Red_D(3, 0, 2);
                        
                      
                        if (mk_03 != null && mk_02 != null)
                        {
                           // mk_03[0] = (ushort)(mk_03[0]+30);
                            if (!_sb_tiaoshi)
                            {
                               tongbu();
                            }
                          //  tongbu();

                        }
                        else
                        {
                           
                            error_wt(true, "RS-485通讯错误，请检查链接，故障排除后点击消除1", 9);
                            run = false;
                        }
                        Thread.Sleep(1);

                    }
                    catch(Exception ex)
                    {
                        error_wt(true, "RS-485通讯错误，请检查链接，故障排除后点击消除4", 9);
                        run = false;
                        OnChildThreadException(ex.Message);
                    }
                  
                }
                else
                {
                    Thread.Sleep(100);
                    OnChildThreadException("RS-485通讯错误，请检查链接，故障排除后点击消除2");

                   
                    error_wt(true, "RS-485通讯错误，请检查链接，故障排除后点击消除",9);
                    run = false;
                }
                //OnChildThreadException("错误哦" );
                Thread.Sleep(1);
            }

        }
        //油缸同步处理
        int error_num;
        private void tongbu()
        {

            if (mk_03 != null && mk_02 != null)
            {


                float yg2 = mk_03[1] * 0.0321f;
                float yg1 = mk_03[0] * 0.0321f;
                float yg3 = mk_02[0] * 0.0321f;
                float yg4 = mk_02[1] * 0.0321f;

                if (my_pingtai == my_fanqi.shuoyou )
                {
                    try
                    {
                        ugsd1 = modbus2.Red_D(4, 4208, 1);///油缸速度

                        if (ugsd1 != null)
                        {
                            
                            mytongbu(yg1, yg2, (int)my_yougang.yougang2, ugsd1[0], 0);
                            
                            mytongbu(yg1, yg3, (int)my_yougang.yougang3, ugsd1[0], 0);
                           
                            mytongbu(yg1, yg4, (int)my_yougang.yougang4, ugsd1[0], 0);
                            error_num = 0;
                        }
                        else
                        {
                            if (error_num > 3)
                            {
                               on_fanqi(false);
                                error_wt(true, "plc通讯异常", 3);
                            }
                            error_num++;

                        }

                     

                    }
                    catch
                    {
                        if (error_num > 3)
                        {
                            on_fanqi(false);
                            MessageBox.Show("PLC通讯过慢");
                        }
                        error_num++;

                    }


                    if (Math.Abs(yg1 - yg2) > 10)
                    {
                      on_fanqi(false);
                    }
                    if (Math.Abs(yg1 - yg3) > 10)
                    {
                       on_fanqi(false);
                    }
                    if (Math.Abs(yg1 - yg4) > 10)
                    {
                       on_fanqi(false);
                    }
                }
                else if (my_pingtai == my_fanqi.pingban1 )
                {//平台1或者油缸1#2#动作
                    ugsd1 = modbus2.Red_D(4, 4208, 1);///油缸速度

                    if (Math.Abs(yg1 - yg2) > 20)
                    {
                        on_fanqi(false);
                    }
                    mytongbu(yg1, yg2, (int)my_yougang.yougang2, ugsd1[0], 0);

                }
                else if (my_pingtai == my_fanqi.pingban2 )
                {
                    ugsd3 = modbus2.Red_D(4, 4228, 1);///油缸速度

                    if (Math.Abs(yg3 - yg4) > 20)
                    {
                       on_fanqi(false);
                    }
                    //实验

                    mytongbu(yg3, yg4, (int)my_yougang.yougang4, ugsd3[0], 0);
                }




            }


        }

        /// <summary>
        /// 油缸同步
        /// </summary>
        /// <param name="yg1">比较油缸主位置</param>
        /// <param name="yg2">比较油缸位置</param>
        /// <param name="yougang">调速油缸号</param>
        /// <param name="sudu">比较速度</param>
        private void mytongbu(float yg1, float yg2, ushort yougang, int sudu,float c) { 


         ushort bijsd = (ushort)sudu;
    
           // float b = (float)(bijsd + ((yg1 - yg2) /3) * 100);
            float b = (float)(bijsd + ((yg1 - yg2) /2) * 600 );

                if (b < 0)
                {
                    b = 0;
                }
                if (b > 64000)
                {
                    b = 64000;
                }
                ushort a = (ushort)b;
                if (bijsd>32000)
                {
                    if (a < 32000)
                    {
                        a = 32001;
                    }
                }
                if (bijsd<32000)
                {
                    if (a > 32000)
                    {
                        a = 31999;
                    }
                }
            try
            {
                write_plc_d(yougang, a);
            }
            catch
            {
                MessageBox.Show("xieru");
            }
               

         
        }
       
        
        #endregion
    }
}
