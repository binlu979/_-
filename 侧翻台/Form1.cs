using DevComponents.DotNetBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LogHelper;
using spMODBUS;
namespace 侧翻台
{
    public delegate void delegate_jianxi(float [] jianxi);
    
    public partial class Form1 : DevComponents.DotNetBar.RibbonForm
    {

        private event delegate_jianxi my_delgate_jianxi;
       
        PLC plc = new PLC();

        log log = new log();
        public static bool ERROR;
        Thread t_xunshi;
        bool t_xunshi_true = true;
        bool 上升下降;
        ad ad = new ad();
        private bool _tiaoshi=false;//调试
        /// <summary>
        /// 液压站温度
        /// </summary>
        private float wendu = 0;
        private bool tiaoshi
        {
            get { return _tiaoshi; }
            set { _tiaoshi = value; }
        }
        private bool _admin_mod=false;//管理员模式
        private bool admin_mod
        {
            get { return _admin_mod; }
            set { _admin_mod = value; }
        }
        public static float fanqi_jiaodu;
        
        public Form1()
        {
            InitializeComponent();
            //   eStyle style = (eStyle)Enum.Parse(typeof(eStyle), con.style);
            //   styleManager1.ManagerStyle = style;

            SetStyle
                     (ControlStyles.AllPaintingInWmPaint  //全部在窗口绘制消息中绘图
                     | ControlStyles.OptimizedDoubleBuffer //使用双缓冲
                     , true);
            //info();
        
            plc.error += Plc_error;

            this.ribbonControl1.KeyUp += RibbonControl1_KeyUp;
            plc.red_dzc_open();
            t_xunshi = new Thread(info);
            t_xunshi.Start();
            dataGridView1_load();//设置网格
            label23.Text ="";
            label24.Text = "";//2-2
            my_delgate_jianxi += Form1_my_delgate_jianxi;//测量间隙事件
            plc.st_jiaodu += plc_st_jiaodu;
        }

      
     

        private void RibbonControl1_KeyUp(object sender, KeyEventArgs e)
        {
            MessageBox.Show("sdf");
        }

        #region 设备报警错误事件
        bool error_485=false, error_net=false;
        string err_txt;
        /// <summary>
        /// 错误事件
        /// </summary>
        /// <param name="err"></param>
        /// <param name="message"></param>
        public void Plc_error(bool err, string message, int id)
        {
            try
            {
                this.Invoke((EventHandler)delegate//线程锁放置线程之间干扰
                {
                    ERROR = true;
                    LED_ERROR.b_Value = true;
                   
                    if (err_txt != message)
                    {
                        lb_mes.Text += message + "\r\n";
                        err_txt = message;
                    }
                  
                });
                if (id == 9)
                {
                    error_485 = true;
                }
                
            }
            catch
            {

            }

        }
        private void LED_ERROR_Click(object sender, EventArgs e)
        {
            if (error_485)
            {
                if (!plc.port_open)
                {
                    plc.open();
                    ERROR = false;
                }
                plc.red_dzc_open();

            }

        }
        #endregion
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        float[][] mod_cz = new float[9][];

        private void info()
        {

            while (t_xunshi_true)
            { 

                #region 获取串口数据
                if (plc.port_open)
                {

                   
                    Double[] jiaodu1 = plc.jiaodu1;
                    Double[] jiaodu2 = plc.jiaodu2;

                    ushort[] ygwz1 = PLC.mk_02;
                    ushort[] ygwz2 = PLC.mk_03;
                    if (PLC.dz_run)//如果油缸动作执行
                    {
                        #region 按角度停止
                        /*
                        if (上升下降)
                        {//上升
                            if (jiaodu1[1]+0.1>= float.Parse(text_fanqijiaodu.Text) || jiaodu2[0]+0.1>= float.Parse(text_fanqijiaodu.Text))
                            {
                                plc.on_fanqi(false);
                                led_daowei.b_Value = true;
                              
                            }
                        }
                        else
                        {//下降
                            if (jiaodu1 != null && jiaodu2 != null)
                            {
                                
                                if (plc.my_pingtai == PLC.my_fanqi.shuoyou)
                                {
                                    if (jiaodu1[1] <= float.Parse(text_fanqijiaodu.Text) + 0.05 || jiaodu2[0] <= float.Parse(text_fanqijiaodu.Text) + 0.05)
                                    {
                                       // plc.on_fanqi(false);
                                        led_daowei.b_Value = true;
                                    }
                                }
                                else if (plc.my_pingtai == PLC.my_fanqi.pingban1)
                                {
                                    if (jiaodu1[1] < float.Parse(text_fanqijiaodu.Text) + 0.05)
                                    {
                                        plc.on_fanqi(false);
                                        led_daowei.b_Value = true;
                                    }
                                }
                                else if (plc.my_pingtai == PLC.my_fanqi.pingban2)
                                {
                                    if (jiaodu2[0] <= float.Parse(text_fanqijiaodu.Text) + 0.05)
                                    {
                                        plc.on_fanqi(false);
                                        led_daowei.b_Value = true;
                                    }
                                } 
                            }
                        }*/
                        #endregion
                    }


                    try
                    {

                        if (ygwz1 != null && ygwz2 != null)
                        {
                            // thces();


                            Invoke((EventHandler)delegate//线程锁放置线程之间干扰

                            {

                                led_sbjx.b_Value = true;
                                error_485 = false;
                                float yg2 = ((float)ygwz2[1]) * 0.0321f;
                                float yg1 = ((float)ygwz2[0]) * 0.0321f;
                                float yg3 = ((float)ygwz1[0]) * 0.0321f;
                                float yg4 = ((float)ygwz1[1]) * 0.0321f;
                                lab_yg1_wz.Text = yg1.ToString();
                                lab_yg2_wz.Text = yg2.ToString();
                                lab_yg3_wz.Text = yg3.ToString();
                                lab_yg4_wz.Text = yg4.ToString();

                             

                                if (jiaodu1 != null && jiaodu2 != null)
                                {
                                    int xiaoshu =1;
                                    if (!admin_mod)
                                    {
                                        textBoxX1.Text = Math.Round(jiaodu1[1], xiaoshu).ToString();
                                        textBoxX2.Text = Math.Round(jiaodu2[0], xiaoshu).ToString();//2-1
                                    }
                                    if (tiaoshi)
                                    {
                                        label23.Text = Math.Round(jiaodu1[0], xiaoshu).ToString();
                                        label24.Text = Math.Round(jiaodu2[1], xiaoshu).ToString();//2-2
                                    }
                                 
                                    
                                    
                                }

                            });
                        }
                        else
                        {
                            led_sbjx.b_Value = false;
                        }
                    }
                    catch (Exception ex)
                    { }
                    Thread.Sleep(1);
                }
                else
                {//.Suspend()
                    led_sbjx.b_Value = false;

                }
                #endregion

                #region 获取称重数据
                if (!tiaoshi)
                {
                    mod_cz = ad.mod_value_jiaozheng;

                }
                else
                {
                    mod_cz = ad.mod_value;
                }

                //  
                if (mod_cz[0] != null)
                {
                    xianshiLED2.b_text1 = (int)mod_cz[0][5];
                    xianshiLED2.b_text2 = (int)mod_cz[0][6];
                    xianshiLED2.b_text3 = (int)mod_cz[0][4];
                    xianshiLED2.b_text4 = (int)mod_cz[0][7];

                    xianshiLED4.b_text1 = (int)mod_cz[0][1];
                    xianshiLED4.b_text2 = (int)mod_cz[0][2];
                    xianshiLED4.b_text3 = (int)mod_cz[0][0];
                    xianshiLED4.b_text4 = (int)mod_cz[0][3];
                }
                if (mod_cz[1] != null)
                {
                    xianshiLED1.b_text1 = (int)mod_cz[1][5];
                    xianshiLED1.b_text2 = (int)mod_cz[1][6];
                    xianshiLED1.b_text3 = (int)mod_cz[1][4];
                    xianshiLED1.b_text4 = (int)mod_cz[1][7];

                    xianshiLED3.b_text1 = (int)mod_cz[1][1];
                    xianshiLED3.b_text2 = (int)mod_cz[1][2];
                    xianshiLED3.b_text3 = (int)mod_cz[1][0];
                    xianshiLED3.b_text4 = (int)mod_cz[1][3];



                }
                if (mod_cz[2] != null)
                {




                    xianshiLED6.b_text1 = (int)mod_cz[2][5];
                    xianshiLED6.b_text2 = (int)mod_cz[2][6];
                    xianshiLED6.b_text3 = (int)mod_cz[2][4];
                    xianshiLED6.b_text4 = (int)mod_cz[2][7];

                    xianshiLED8.b_text1 = (int)mod_cz[2][1];
                    xianshiLED8.b_text2 = (int)mod_cz[2][2];
                    xianshiLED8.b_text3 = (int)mod_cz[2][0];
                    xianshiLED8.b_text4 = (int)mod_cz[2][3];
                }
                if (mod_cz[3] != null)
                {
                    xianshiLED5.b_text1 = (int)mod_cz[3][5];
                    xianshiLED5.b_text2 = (int)mod_cz[3][6];
                    xianshiLED5.b_text3 = (int)mod_cz[3][4];
                    xianshiLED5.b_text4 = (int)mod_cz[3][7];

                    xianshiLED7.b_text1 = (int)mod_cz[3][1];
                    xianshiLED7.b_text2 = (int)mod_cz[3][2];
                    xianshiLED7.b_text3 = (int)mod_cz[3][0];
                    xianshiLED7.b_text4 = (int)mod_cz[3][3];



                }
                if (mod_cz[4] != null)
                {

                    xianshiLED9.b_text1 = (int)mod_cz[4][5];
                    xianshiLED9.b_text2 = (int)mod_cz[4][6];
                    xianshiLED9.b_text3 = (int)mod_cz[4][4];
                    xianshiLED9.b_text4 = (int)mod_cz[4][7];

                    xianshiLED11.b_text1 = (int)mod_cz[4][1];
                    xianshiLED11.b_text2 = (int)mod_cz[4][2];
                    xianshiLED11.b_text3 = (int)mod_cz[4][0];
                    xianshiLED11.b_text4 = (int)mod_cz[4][3];



                }
                if (mod_cz[5] != null)
                {

                    xianshiLED10.b_text1 = (int)mod_cz[5][5];
                    xianshiLED10.b_text2 = (int)mod_cz[5][6];
                    xianshiLED10.b_text3 = (int)mod_cz[5][4];
                    xianshiLED10.b_text4 = (int)mod_cz[5][7];


                    xianshiLED12.b_text1 = (int)mod_cz[5][1];
                    xianshiLED12.b_text2 = (int)mod_cz[5][2];
                    xianshiLED12.b_text3 = (int)mod_cz[5][0];
                    xianshiLED12.b_text4 = (int)mod_cz[5][3];
                }
                if (mod_cz[6] != null)
                {



                    xianshiLED13.b_text1 = (int)mod_cz[6][5];
                    xianshiLED13.b_text2 = (int)mod_cz[6][6];
                    xianshiLED13.b_text3 = (int)mod_cz[6][4];
                    xianshiLED13.b_text4 = (int)mod_cz[6][7];

                    xianshiLED15.b_text1 = (int)mod_cz[6][1];
                    xianshiLED15.b_text2 = (int)mod_cz[6][2];
                    xianshiLED15.b_text3 = (int)mod_cz[6][0];
                    xianshiLED15.b_text4 = (int)mod_cz[6][3];

                }
                if (mod_cz[7] != null)
                {

                    xianshiLED14.b_text1 = (int)mod_cz[7][5];
                    xianshiLED14.b_text2 = (int)mod_cz[7][6];
                    xianshiLED14.b_text3 = (int)mod_cz[7][4];
                    xianshiLED14.b_text4 = (int)mod_cz[7][7];


                    xianshiLED16.b_text1 = (int)mod_cz[7][1];
                    xianshiLED16.b_text2 = (int)mod_cz[7][2];
                    xianshiLED16.b_text3 = (int)mod_cz[7][0];
                    xianshiLED16.b_text4 = (int)mod_cz[7][3];


                }
               
                
                                  
                                          
                #endregion
                #region 温度控制
                if (mod_cz[8] != null)
                {
                    wendu = (mod_cz[8][1] * 0.002288853f) - 50;
                    config.wendu = wendu;
                    if (wendu > 50)
                    {
                        plc.on_fanqi(false);
                        plc.yyzqd(false,2);
                        Plc_error(true, "液压站温度过高", 3);
                        lab_wendu.BackColor = Color.Red;
                    }
                    if (wendu < 0&&!config.jiare)
                    {//温度小于0度打开加热
                      //  config.jiare = true;
                        plc.write_plc_m((ushort)PLC.plc_m.jiare, true);
                        plc.write_plc_m((ushort)PLC.plc_m.fan, false);
                    }
                    if (wendu > 10 && !config.fan && yyz_on.Value)
                    {//温度大于50开启风扇
                        plc.write_plc_m((ushort)PLC.plc_m.fan, true);
                        plc.write_plc_m((ushort)PLC.plc_m.jiare, false);
                    }
                    if (!yyz_on.Value) { plc.write_plc_m((ushort)PLC.plc_m.fan, false); }
                    lab_wendu.Text = Math.Round(wendu, 2).ToString();
                }
                #endregion
                #region 间隙
                if (间隙测量)
                {
                    my_delgate_jianxi(mod_cz[8]);
                }
                #endregion
                #region 读取PLC
                plc.xintiao = true;

                bool[] plc_m = plc.Read_plc_m(100, 15);
                plc_error(plc_m);

                if (ad.tx_error)
                {//称重通讯
                 //   plc.on_fanqi(false);
                    led_hyjc.b_Value = true;


                }
             //   else { led_hyjc.b_Value = false; Plc_error(true, "称重传感器通讯错误，请检查网络连接 \r\n", 8); }
                #endregion


                Thread.Sleep(500);
            }

        }
        #region 获取PLC数据
        private void plc_error(bool[] m)
        {

            led_plc.b_Value = true;
            if (m != null)
            {
                if (m[0] || m[1])
                {
                    yyz_on.Value = true;
                }
                else
                {
                    yyz_on.Value = false;
                }
                if (m[0])
                {
                    led_yyz2.b_Value = true;


                }
                else { led_yyz2.b_Value = false; }
                if (m[1])
                {
                    led_yyz1.b_Value = true;
                    

                }
                else { led_yyz1.b_Value = false; }
                if (m[2])
                {//加热
                    led_jiare.b_Value = true;
                    config.jiare = true;
                    
                }
                else { led_jiare.b_Value = false; config.jiare = false; }
                if (m[3])
                {//液压站风扇
                    led_fan.b_Value = true;
                    config.fan = true;

                }
                else { led_fan.b_Value = false; config.fan = false; }
                if (m[4])
                {
                    led_ylf2.b_Value = true;

                }
                else { led_ylf2.b_Value = false; }
                if (m[5])
                {
                    led_lyf1.b_Value = true;

                }
                else { led_lyf1.b_Value = false; }
                if (m[6])
                {
                    led_xttx.b_Value = true;

                }
                else { led_xttx.b_Value = false; }
                if (m[7])
                {
                    plc.on_fanqi(false);
                    led_jstop.b_Value = true;
                    led_kzsjt.b_Value = true;
                    MessageBox.Show("控制室急停,请确认急停是否按下，消除后点击“确定”！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //   MessageBox.Show("控制室急停,请确认急停是否按下，消除后点击确定", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                }
                else { led_kzsjt.b_Value = false; led_jstop.b_Value = false; }
                if (m[8])
                {
                    plc.on_fanqi(false);
                    led_jstop.b_Value = true;
                    led_hlzjt.b_Value = true;

                    MessageBox.Show("护栏左急停,请确认急停是否按下，消除后点击“确定”！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else { led_hlzjt.b_Value = false; led_jstop.b_Value = false; }
                if (m[9])
                {
                    plc.on_fanqi(false);
                    led_jstop.b_Value = true;
                    led_hlyjt.b_Value = true;

                    MessageBox.Show("护栏右急停,请确认急停是否按下，消除后点击“确定”！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else { led_hlyjt.b_Value = false; led_jstop.b_Value = false; }
                if (m[10])
                {


                    led_ylfld.b_Value = true;


                }
                else { led_ylfld.b_Value = false; }
                if (m[11])
                {
                    plc.on_fanqi(false);
                    led_gy1glq.b_Value = false;
                    MessageBox.Show("1#高压过滤器报警,请检查高压过滤器是否堵塞", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                }
                else { led_gy1glq.b_Value = false; }
                if (m[12])
                {
                    plc.on_fanqi(false);
                    led_gy2glq.b_Value = true;
                    MessageBox.Show("2#高压过滤器报警,请检查高压过滤器是否堵塞", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                }
                else { led_gy2glq.b_Value = false; }
               
                if (m[14])
                {
                    led_plc.b_Value = false;
                    led_xtsb.b_Value = true;
                    plc.on_fanqi(false);

                }
                else { led_xtsb.b_Value = false; }
            }
            else
            {
                led_plc.b_Value = false;
                led_xtsb.b_Value = true;
              //  MessageBox.Show("心跳失败，请检查与PLC通信是否正常", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #region 按角度停止
        void plc_st_jiaodu()
        {
            Double[] jiaodu1 = plc.jiaodu1;
            Double[] jiaodu2 = plc.jiaodu2;
            if (PLC.dz_run)//如果油缸动作执行
            {
                if (上升下降)
                {//上升
                    if (jiaodu1[1] + 0.1 >= float.Parse(text_fanqijiaodu.Text) || jiaodu2[0] + 0.1 >= float.Parse(text_fanqijiaodu.Text))
                    {
                        plc.on_fanqi(false);
                        led_daowei.b_Value = true;

                    }
                }
                else
                {//下降
                    if (jiaodu1 != null && jiaodu2 != null)
                    {

                        if (plc.my_pingtai == PLC.my_fanqi.shuoyou)
                        {
                            if (jiaodu1[1] <= float.Parse(text_fanqijiaodu.Text) + 0.05 || jiaodu2[0] <= float.Parse(text_fanqijiaodu.Text) + 0.05)
                            {
                               plc.on_fanqi(false);
                                led_daowei.b_Value = true;
                            }
                        }
                        else if (plc.my_pingtai == PLC.my_fanqi.pingban1)
                        {
                            if (jiaodu1[1] < float.Parse(text_fanqijiaodu.Text) + 0.05)
                            {
                                plc.on_fanqi(false);
                                led_daowei.b_Value = true;
                            }
                        }
                        else if (plc.my_pingtai == PLC.my_fanqi.pingban2)
                        {
                            if (jiaodu2[0] <= float.Parse(text_fanqijiaodu.Text) + 0.05)
                            {
                                plc.on_fanqi(false);
                                led_daowei.b_Value = true;
                            }
                        }
                    }
                }
            }
        }

        #endregion
        private void sb_fw_Click(object sender, EventArgs e)
        {

        }
        #region 切换模版风格
        private void AppCommandTheme_Executed(object sender, EventArgs e)
        {

            ICommandSource source = sender as ICommandSource;
            if (source.CommandParameter is string)
            {
                eStyle style = (eStyle)Enum.Parse(typeof(eStyle), source.CommandParameter.ToString());
                con.style = style.ToString();
                // Using StyleManager change the style and color tinting
                if (StyleManager.IsMetro(style))
                {
                    // More customization is needed for Metro
                    // Capitalize App Button and tab
                    //   buttonFile.Text = buttonFile.Text.ToUpper();
                    foreach (BaseItem item in RibbonControl.Items)
                    {
                        // Ribbon Control may contain items other than tabs so that needs to be taken in account
                        RibbonTabItem tab = item as RibbonTabItem;
                        if (tab != null)
                            tab.Text = tab.Text.ToUpper();
                    }

                    // buttonFile.BackstageTabEnabled = true; // Use Backstage for Metro

                    ribbonControl1.RibbonStripFont = new System.Drawing.Font("Segoe UI", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    if (style == eStyle.Metro)
                        StyleManager.MetroColorGeneratorParameters = DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters.DarkBlue;

                    // Adjust size of switch button to match Metro styling
                    //  switchButtonItem1.SwitchWidth = 16;
                    //  switchButtonItem1.ButtonWidth = 48;
                    //  switchButtonItem1.ButtonHeight = 19;

                    // Adjust tab strip style
                    //  tabStrip1.Style = eTabStripStyle.Metro;

                    StyleManager.Style = style; // BOOM
                }
                else
                {
                    // If previous style was Metro we need to update other properties as well
                    if (StyleManager.IsMetro(StyleManager.Style))
                    {
                        ribbonControl1.RibbonStripFont = null;
                        // Fix capitalization App Button and tab
                        //  buttonFile.Text = ToTitleCase(buttonFile.Text);
                        foreach (BaseItem item in RibbonControl.Items)
                        {
                            // Ribbon Control may contain items other than tabs so that needs to be taken in account
                            RibbonTabItem tab = item as RibbonTabItem;
                            //  if (tab != null)
                            //      tab.Text = ToTitleCase(tab.Text);
                        }
                        // Adjust size of switch button to match Office styling
                        //  switchButtonItem1.SwitchWidth = 28;
                        //   switchButtonItem1.ButtonWidth = 62;
                        //   switchButtonItem1.ButtonHeight = 20;
                    }
                    // Adjust tab strip style
                    //   tabStrip1.Style = eTabStripStyle.Office2007Document;
                    StyleManager.ChangeStyle(style, Color.Empty);
                    // if (style == eStyle.Office2007Black || style == eStyle.Office2007Blue || style == eStyle.Office2007Silver || style == eStyle.Office2007VistaGlass)
                    //     buttonFile.BackstageTabEnabled = false;
                    //  else
                    //buttonFile.BackstageTabEnabled = true;
                }
            }
            else if (source.CommandParameter is Color)
            {
                if (StyleManager.IsMetro(StyleManager.Style))
                    StyleManager.MetroColorGeneratorParameters = new DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(Color.White, (Color)source.CommandParameter);
                else
                    StyleManager.ColorTint = (Color)source.CommandParameter;
            }
        }

        #endregion

        private void checkBoxX1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonX5_Click(object sender, EventArgs e)
        {
            lb_mes.Text = "";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }
        #region 平台切换
        private void switchButton2_ValueChanged(object sender, EventArgs e)
        {//单平台切换
            DevComponents.DotNetBar.Controls.SwitchButton but = (DevComponents.DotNetBar.Controls.SwitchButton)sender;
            if (but.Value)
            {//单平台
               
                radioButton4.Enabled = true;
                radioButton3.Enabled = true;

                plc.my_pingtai = PLC.my_fanqi.pingban1;
            }
            else
            {//双平台
                plc.my_pingtai = PLC.my_fanqi.shuoyou;
            
                radioButton4.Enabled = false;
                radioButton3.Enabled = false;

            }
         
        }
        #endregion
        private void buttonX1_Click(object sender, EventArgs e)
        {//设备复位
            PLC.sb_tiaoshi = true;
            plc.on_fanqi(true, -25);
        }
        Type f = typeof(Form1);
        private void buttonX2_Click(object sender, EventArgs e)
        {
            log.log_info(f, "记录数据");
            log.error(f, new Exception("sdf"));
        }


        private void buttonItem12_Click(object sender, EventArgs e)
        {
            if ((int)con.admin < 2)
            {
                Set set = new Set();
                set.ShowDialog();
            }
            else
            {
                MessageBox.Show("请登录后操作");
            }

        }
    
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            t_xunshi_true = false;//关闭前台显示线程
            plc.run = false;//关闭PLC运行
            ad._start = false;
            ad.t_start = false;
            ad.open = false;
        
            try
            {
                string file = Application.StartupPath + "\\cmd.bat";
                //  Thread.Sleep(10);

                //Environment.Exit(0);
                Application.Exit();
                Application.ExitThread(); 
               System.Diagnostics.Process.Start(file);
             
                
              
            }
            catch
            {               Environment.Exit(0);
            }
            
  

        }
        #region 打开关闭液压站
        private void kg_yeyazhan_ValueChanged(object sender, EventArgs e)
        {
            if (yyz_on.Value)
            {
                plc.my_yeyazhan_open = true;
            }
            else
            {
                 plc.my_yeyazhan_open = false;
            }
        }
        #endregion


        private void radioButton1_Click(object sender, EventArgs e)
        {

            RadioButton rb = (RadioButton)sender;
            int tag = int.Parse(rb.Tag.ToString());
            if (tag != (int)plc.my_pingtai)
            {
                string ad = "";
                if (tag == 1)
                {
                    

                    plc.my_pingtai = PLC.my_fanqi.pingban1;
                }
                else if (tag == 2)
                {
                    

                    plc.my_pingtai = PLC.my_fanqi.pingban2;
                }

            }
        }

        private void but_yundong_Click(object sender, EventArgs e)
        {
            if (plc.my_pingtai == PLC.my_fanqi.shuoyou)
            {

                if ((float)text_fanqijiaodu.Value > plc.jiaodu1[1])
                {

                }
                else if (text_fanqijiaodu.Value < plc.jiaodu1[1])
                {

                }
            }

        }

        private void but_stop_Click(object sender, EventArgs e)
        {
            plc.on_fanqi(false);
        }
        #region 翻起开始按钮
        private void but_start_Click(object sender, EventArgs e)
        {//翻起运动
            fanqi_jiaodu =float.Parse( text_fanqijiaodu.Text);
            Double[] jiaodu1 = plc.jiaodu1;
            Double[] jiaodu2 = plc.jiaodu2;
            admin_mod = false;//关闭管理员模式

            //翻起角度  txt_fanqijiaodu;
            if (plc.my_pingtai == PLC.my_fanqi.shuoyou)
            {

                if (float.Parse(text_fanqijiaodu.Text) < (float)jiaodu1[0] && float.Parse(text_fanqijiaodu.Text) < (float)jiaodu2[0])
                {//向下翻转
                    上升下降 = false;
                    int c = (int)(float.Parse(text_xiajiangjiaodu.Text) * 2.3);
                    plc.on_fanqi(true, -c);
                }
                else if (float.Parse(text_fanqijiaodu.Text) > (float)jiaodu1[0])
                {//向上翻起
                    上升下降 = true;
                    int c = (int)(float.Parse(text_xiajiangjiaodu.Text) * 2.3);
                    plc.on_fanqi(true, c);
                }
                else
                {
                    return;
                }
            }
            else if (plc.my_pingtai == PLC.my_fanqi.pingban1)
            {

                if (float.Parse(text_fanqijiaodu.Text) < (float)jiaodu1[0])
                {//向下翻转
                    上升下降 = false;
                    int c = (int)(float.Parse(text_xiajiangjiaodu.Text) * 2.3);
                    plc.on_fanqi(true, -c);

                }
                else if (float.Parse(text_fanqijiaodu.Text) > (float)jiaodu1[0])
                {//向上翻起
                    上升下降 = true;
                    int c = (int)(float.Parse(text_xiajiangjiaodu.Text) * 2.3);
                    plc.on_fanqi(true, c);
                }
                else
                {
                    return;
                }


            }
            else if (plc.my_pingtai == PLC.my_fanqi.pingban2)
            {

                if (float.Parse(text_fanqijiaodu.Text) < (float)jiaodu2[0])
                {//向下翻转
                    上升下降 = false;
                    int c = (int)(float.Parse(text_xiajiangjiaodu.Text) * 2.3);
                    plc.on_fanqi(true, -c);

                }
                else if (float.Parse(text_fanqijiaodu.Text) > (float)jiaodu2[0])
                {//向上翻起
                    上升下降 = true;
                    int c = (int)(float.Parse(text_xiajiangjiaodu.Text) * 2.3);
                    plc.on_fanqi(true, c);
                }
                else
                {
                    return;
                }
            }
            // plc.yyz_youguang(true, yougang, -trackBar1.Value);
        }
        #endregion
        private void but_stop_Click_1(object sender, EventArgs e)
        {
            plc.on_fanqi(false);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        config con = new config();
        private void buttonItem5_Click(object sender, EventArgs e)
        {
            winfrom.USER user = new winfrom.USER();
            user.ShowDialog();
            if (con.admin != config.user.youke)
            {
                string user_admin = con.admin.ToString();
                this.buttonItem5.Text = user_admin;
            }

            user.Close();
        }

        private void buttonItem13_Click(object sender, EventArgs e)
        {
           this.Close();
          //  Application.Exit();
        }

        #region 称重实验
        #region 设置网格控件
        private void dataGridView1_load()
        {
            dataGridView1.Rows.Add(8);
        }
        #endregion

        /// <summary>
        /// 实验开车称重实验按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonX9_Click(object sender, EventArgs e)
        {
            //  checkBoxX3
            int chengzhong_num = 0;
            if (chengzhong_chetou == 0)
            {
                MessageBox.Show("请选择车头方向!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add(8);
            for (int i = 0; i < 8; i++)
            {
                
                    dataGridView1.Rows[i].Visible = true;
                
            }
            #region 循环判断方法
            foreach (Control c in panelEx8.Controls)
            {
               
              
                if (c is DevComponents.DotNetBar.Controls.CheckBoxX && ((DevComponents.DotNetBar.Controls.CheckBoxX)c).Checked == true)
                {
                    
                    if (int.Parse(c.Tag.ToString())%2!=0)
                    {//奇数
                     
                        if (chengzhong_chetou == 1)
                        {//车头超前
                      

                            string a = "xianshiLED"+c.Tag.ToString();
                            xianshiLED xs = (xianshiLED)panelEx8.Controls.Find(a, true)[0];
                            dataGridView1.Rows[((int.Parse(c.Tag.ToString())+1)/2)-1].Cells[2].Value = xs.b_text5.ToString(); 
                        }
                        else
                        {//车头朝后
                            string a = "xianshiLED" + c.Tag.ToString();
                            xianshiLED xs = (xianshiLED)panelEx8.Controls.Find(a, true)[0];
                            dataGridView1.Rows[((int.Parse(c.Tag.ToString()) + 1) / 2)-1].Cells[1].Value = xs.b_text5.ToString(); 

                        }
                    }
                    else
                    {//偶数
                    
               
                        if (chengzhong_chetou == 1)
                        {//车头超前


                            string a = "xianshiLED" + c.Tag.ToString();
                            xianshiLED xs = (xianshiLED)panelEx8.Controls.Find(a, true)[0];
                            dataGridView1.Rows[(int.Parse(c.Tag.ToString())/2)-1].Cells[1].Value = xs.b_text5.ToString();
                        }
                        else
                        {//车头朝后
                            string a = "xianshiLED" + c.Tag.ToString();
                            xianshiLED xs = (xianshiLED)panelEx8.Controls.Find(a, true)[0];
                            dataGridView1.Rows[(int.Parse(c.Tag.ToString())/2)-1].Cells[2].Value = xs.b_text5.ToString();

                        }

                    }
                    chengzhong_num++;
                    // dataGridView1.Rows[i].Cells[3].Value = ad.mod_value[i][a].ToString();
                }

            }
            #endregion

         
            if (chengzhong_num == 0)
            {
                MessageBox.Show("请最少选择1块平板！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);return;
            } 
            chengzhong_jishuan();
        }
        int chengzhong_chetou = 0;
        private void chengzhong_jishuan()
        {//进行赋值给表格计算
            int znum=0;
            int zhouhao = 1;
            for (int i = 0; i < 8; i++)
            {
                
                object a = dataGridView1.Rows[i].Cells[1].Value;
                object b = dataGridView1.Rows[i].Cells[2].Value;
                if (a == null && b == null)
                {
                    dataGridView1.Rows[i].Visible=false;

                }
                else { dataGridView1.Rows[i].Cells[0].Value = zhouhao; zhouhao++; }
                if (a == null)
                {
                    a = "0";
                }
                if (b == null)
                {
                    b = "0";
                }
                znum += int.Parse(a.ToString()) + int.Parse(b.ToString());
                dataGridView1.Rows[i].Cells[3].Value = int.Parse(a.ToString()) + int.Parse(b.ToString()); //赋值总重量
            }
            label38.Text = znum.ToString();
        }
        private void buttonX13_Click(object sender, EventArgs e)
        {
            buttonX13.Text = "头";
            buttonX12.Text = "尾";
            buttonX13.SymbolColor = Color.Maroon;
            buttonX12.SymbolColor = Color.Black;
            chengzhong_chetou = 1;
        }

        private void buttonX12_Click(object sender, EventArgs e)
        {
            buttonX13.Text = "尾";
            buttonX12.Text = "头";
            buttonX12.SymbolColor = Color.Maroon;
            buttonX13.SymbolColor = Color.Black;
            chengzhong_chetou = 2;
        }
        #endregion

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
           
        }
        #region 定义快捷键
        private void textBoxX1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F1:
                    break;
                case Keys.F2:
                    break;
                case Keys.F3:
                    break;
                case Keys.F4:
                    break;
                case Keys.F5:
                    break;
                case Keys.F6:
                    break;
                case Keys.F7:
                    
                    if ((int)con.admin < 2)
                    {
                        if (tiaoshi)
                        {
                            tiaoshi = false;//调试模式
                            label23.Text = "";
                            label24.Text = "";
                            MessageBox.Show("已退出调试模式");
                        }
                        else
                        {
                            tiaoshi = true;//调试模式
                            MessageBox.Show("当前处于调试模式");
                        }
                     
                    }
                    break;
                case Keys.F8:
                    if ((int)con.admin < 1)
                    {
                        if (admin_mod)
                        {
                            admin_mod = false;//管理员模式
                            MessageBox.Show("已退出管理员模式");
                        }else
                        {
                            admin_mod = true;//管理员模式
                            MessageBox.Show("当前处于管理员模式");
                        }
                        
                    }
                  
                    break;
                case Keys.F9:
                    break;
                case Keys.F10:
                    break;
                default:
                    break;


            }
        }
        #endregion
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {//单元格停止编辑状态
            chengzhong_jishuan();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        #region 间隙测量
        private bool 间隙测量 = false;
        private void switchButton1_ValueChanged_1(object sender, EventArgs e)
        {

            if (switchButton1.Value)
            {
                dataGridView2.Enabled = true;
                textBox1.Enabled = true;
                间隙测量 = true;
            }
            else
            {
                dataGridView2.Enabled = false;
                textBox1.Enabled = false;
                间隙测量 = false;
            }
        }
        private float shangchi_jinxi=0;
        private int jiaodu_int=0;
        private void Form1_my_delgate_jianxi(float[] jianxi)
        {
            Invoke((EventHandler)delegate//线程锁放置线程之间干扰
             {
                 label_jianxi1.Text = jianxi[2].ToString();
                 label_jianxi2.Text = jianxi[3].ToString();
                 label_jianxi3.Text = jianxi[5].ToString();
                 if (textBox1.Text != "0")
                 {
                     if (shangchi_jinxi + float.Parse(textBox1.Text) <= float.Parse(textBoxX1.Text))
                     {
                         if (dataGridView2.RowCount  <= jiaodu_int-1)
                         {
                             dataGridView2.Rows.Add(1);
                         }
                         shangchi_jinxi = float.Parse(textBoxX1.Text);
                         jiaodu_int++;
                         dataGridView2.Rows[jiaodu_int].Cells[0].Value = textBoxX1.Text;
                         dataGridView2.Rows[jiaodu_int].Cells[1].Value = jianxi[2].ToString();
                         dataGridView2.Rows[jiaodu_int].Cells[2].Value = jianxi[3].ToString();
                         dataGridView2.Rows[jiaodu_int].Cells[3].Value = jianxi[4].ToString();
                     }
                 }
                 else
                 {
                     dataGridView2.Rows[0].Cells[0].Value = 0;
                     dataGridView2.Rows[0].Cells[1].Value = jianxi[2].ToString();
                     dataGridView2.Rows[0].Cells[2].Value = jianxi[3].ToString();
                     dataGridView2.Rows[0].Cells[3].Value = jianxi[4].ToString();
                 }
             });
        }
        #endregion
        private void buttonX4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("13");
            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add(8);
        }

        private void xianshiLED15_Load(object sender, EventArgs e)
        {

        }
    }
}
