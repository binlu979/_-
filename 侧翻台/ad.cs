using Advantech.Adam;
using spMODBUS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace 侧翻台
{
    
    class ad
    {
    
        private AdamSocket adamModbus1, adamModbus2, adamModbus3, adamModbus4, adamModbus5, adamModbus6, adamModbus7, adamModbus8, adamModbus9;
        private Adam6000Type m_Adam6000Type;
        private string m_szIP1, m_szIP2, m_szIP3, m_szIP4, m_szIP5, m_szIP6, m_szIP7, m_szIP8, m_szIP9, m_szIP10;
        private int m_iPort;
        private int m_iCount;
        private int m_iAiTotal;
        private bool[] m_bChEnabled;
        private ushort[] m_byRange;
       public Thread red_net;

        bmodbus modbus = new bmodbus();
        private bool tongxun_error = false;
        public bool tx_error
        {
            get { return tongxun_error; }
           
        }
     
        public  ad()
        {
            info();
        }
        public void info()
        {
            
            #region 定义模块参数
            // m_bStart = false;			// the action stops at the beginning
            m_szIP1 = "10.0.0.2";   // modbus slave IP address
            m_szIP2 = "10.0.0.3";   // modbus slave IP address
            m_szIP3 = "10.0.0.4";   // modbus slave IP address
            m_szIP4 = "10.0.0.5";   // modbus slave IP address
            m_szIP5 = "10.0.0.6";   // modbus slave IP address
            m_szIP6 = "10.0.0.7";   // modbus slave IP address
            m_szIP7 = "10.0.0.8";   // modbus slave IP address
            m_szIP8 = "10.0.0.9";   // modbus slave IP address
            m_szIP9 = "10.0.0.10";   // modbus slave IP address
        
            m_iPort = 502;				// modbus TCP port is 502
            adamModbus1 = new AdamSocket();
            adamModbus1.SetTimeout(500, 500, 500); // 设置超时时间
            adamModbus1.AdamSeriesType = AdamType.Adam6200; // set AdamSeriesType for  ADAM-6217

            adamModbus2 = new AdamSocket();
            adamModbus2.SetTimeout(500, 500, 500); // 设置超时时间
            adamModbus2.AdamSeriesType = AdamType.Adam6200; // set AdamSeriesType for  ADAM-6217

            adamModbus3 = new AdamSocket();
            adamModbus3.SetTimeout(500, 500, 500); // 设置超时时间
            adamModbus3.AdamSeriesType = AdamType.Adam6200; // set AdamSeriesType for  ADAM-6217

            adamModbus4 = new AdamSocket();
            adamModbus4.SetTimeout(500, 500, 500); // 设置超时时间
            adamModbus4.AdamSeriesType = AdamType.Adam6200; // set AdamSeriesType for  ADAM-6217

            adamModbus5 = new AdamSocket();
            adamModbus5.SetTimeout(500, 500, 500); // 设置超时时间
            adamModbus5.AdamSeriesType = AdamType.Adam6200; // set AdamSeriesType for  ADAM-6217

            adamModbus6 = new AdamSocket();
            adamModbus6.SetTimeout(500, 500, 500); // 设置超时时间
            adamModbus6.AdamSeriesType = AdamType.Adam6200; // set AdamSeriesType for  ADAM-6217

            adamModbus7 = new AdamSocket();
            adamModbus7.SetTimeout(500, 500, 500); // 设置超时时间
            adamModbus7.AdamSeriesType = AdamType.Adam6200; // set AdamSeriesType for  ADAM-6217

            adamModbus8 = new AdamSocket();
            adamModbus8.SetTimeout(500, 500, 500); // 设置超时时间
            adamModbus8.AdamSeriesType = AdamType.Adam6200; // set AdamSeriesType for  ADAM-6217

            adamModbus9 = new AdamSocket();
            adamModbus9.SetTimeout(500, 500, 500); // 设置超时时间
            adamModbus9.AdamSeriesType = AdamType.Adam6200; // set AdamSeriesType for  ADAM-6217

            m_Adam6000Type = Adam6000Type.Adam6217; // the sample is for ADAM-6217
            m_iAiTotal = AnalogInput.GetChannelTotal(m_Adam6000Type);
            m_bChEnabled = new bool[m_iAiTotal];
            m_byRange = new ushort[m_iAiTotal];
            #endregion
        
            red_net = new Thread(red);
            red_net.Start();
        }
        public bool open;
        public void red()
        {
            tongxun_error = false;
             open = true;
            while (open)
            {
                adamModbus1.Connect(m_szIP1, ProtocolType.Tcp, m_iPort);
                if (adamModbus1.Connected)
                {
                    adamModbus2.Connect(m_szIP2, ProtocolType.Tcp, m_iPort);
                    if (adamModbus2.Connected)
                    {
                        adamModbus3.Connect(m_szIP3, ProtocolType.Tcp, m_iPort);
                        if (adamModbus3.Connected)
                        {
                            adamModbus4.Connect(m_szIP4, ProtocolType.Tcp, m_iPort);
                            if (adamModbus4.Connected)
                            {
                                adamModbus5.Connect(m_szIP5, ProtocolType.Tcp, m_iPort);
                                if (adamModbus5.Connected)
                                {
                                    adamModbus6.Connect(m_szIP6, ProtocolType.Tcp, m_iPort);
                                    if (adamModbus6.Connected)
                                    {
                                        adamModbus7.Connect(m_szIP7, ProtocolType.Tcp, m_iPort);
                                        if (adamModbus7.Connected)
                                        {
                                            adamModbus8.Connect(m_szIP8, ProtocolType.Tcp, m_iPort);
                                            if (adamModbus8.Connected)
                                            {
                                                adamModbus9.Connect(m_szIP9, ProtocolType.Tcp, m_iPort);
                                                if (adamModbus9.Connected)
                                                {
                                                    open = false;
                                                    caiji();
                                                  
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }


                Thread.Sleep(1000);
            
            
               
               
                
            }
          
      
        }
      public  bool _start=true;
        public bool t_start
        {
            set { _start = value; }

        }
        bool mod_cg=true;
        bool start = false;
        // List<float[]> modadata = new List<float[]>();
        float[][] modadata = new float[9][];
        float[][] modata_jiaozheng = new float[9][];

        public float[][] mod_value_jiaozheng
        {
           // get { return modadata; }
             get { return jiaozheng(mod_value); }
        }
        public float[][] mod_value
        {
            get { return modadata; }
        }
        config con = new config();

        private float[][] jiaozheng(float[][] data)
        {
    
           
            int y = 0;
            for (int i = 0; i < 8; i++)
            {
                float[] ce = new float[8];
                for (int a = 0; a < 8; a++)
                {
                    if (data[i] != null)
                    {
                        int add = con.red_jiaozheng(y);
                        float ddd = data[i][a];
                        ce[a] = ddd - add;
                        y++;
                    }
                   
                    //  dataGridView1.Rows[id].Cells[3].Value = ad.mod_value[i][a].ToString();
                  
                   
                }
                modata_jiaozheng[i] = ce;
                //
            }
            modata_jiaozheng[8] = data[8];
            return modata_jiaozheng;


        }
        private void caiji()
        {
            m_iCount = 0; // reset the reading counter
            int y = 0;
            int iStart = 1;
            int iIdx;
            int[] iData = null;
            float[] fValue1 = new float[m_iAiTotal], fValue2 = new float[m_iAiTotal], fValue3 =new float[m_iAiTotal], fValue4 = new float[m_iAiTotal], 
                fValue5 = new float[m_iAiTotal], fValue6 = new float[m_iAiTotal], fValue7 = new float[m_iAiTotal], fValue8 = new float[m_iAiTotal], fValue9 = new float[m_iAiTotal];
            float[] fove = new float[8];
            while (_start)
            {

                if (adamModbus1.Connected)
                {

                    if (adamModbus1.Modbus().ReadInputRegs(iStart, m_iAiTotal, out iData))//一次性读取所有数据
                    {
                        tongxun_error = true;
                        for (iIdx = 0; iIdx < 8; iIdx++)
                        {
                            // fValue1[iIdx] =(float)Math.Round( AnalogInput.GetScaledValue(m_Adam6000Type, 384, (ushort)iData[iIdx]),2);//modbus转换成mA
                            fValue1[iIdx] = (float)Math.Round((float)iData[iIdx] * 0.07629511f,0);
                          //  fove[iIdx] = (float)Math.Round((float)iData[iIdx] * 0.07629511f, 0)- con.red_jiaozheng(y);
                            y++;
                        }
                       
                        modadata[0]= fValue1;
                        //  modata_jiaozheng[0] = fove;
                    }
                    else { tongxun_error = false;  }
                     
                    if (adamModbus2.Modbus().ReadInputRegs(iStart, m_iAiTotal, out iData))//一次性读取所有数据
                    {
                        for (iIdx = 0; iIdx < 8; iIdx++)
                        {
                            fValue2[iIdx] = (float)Math.Round((float)iData[iIdx] * 0.04577707f, 0);
                           // fove[iIdx] = (float)Math.Round((float)iData[iIdx] * 0.04577707f, 0) - con.red_jiaozheng(y);
                            y++;
                        }
                        modadata[1] = fValue2;
                        // modata_jiaozheng[1] = fove;
                    }
                    else { tongxun_error = false;  }
                    if (adamModbus3.Modbus().ReadInputRegs(iStart, m_iAiTotal, out iData))//一次性读取所有数据
                    {
                        for (iIdx = 0; iIdx < 8; iIdx++)
                        {
                            fValue3[iIdx] = (float)Math.Round((float)iData[iIdx] * 0.07629511f, 0);
                        }
                        modadata[2] = fValue3;
                    }
                    else { tongxun_error = false;  }
                    if (adamModbus4.Modbus().ReadInputRegs(iStart, m_iAiTotal, out iData))//一次性读取所有数据
                    {
                        for (iIdx = 0; iIdx < 8; iIdx++)
                        {
                            fValue4[iIdx] = (float)Math.Round((float)iData[iIdx] * 0.04577707f, 0);
                        }
                        modadata[3] = fValue4;
                    }
                    else { tongxun_error = false;  }
                    if (adamModbus5.Modbus().ReadInputRegs(iStart, m_iAiTotal, out iData))//一次性读取所有数据
                    {
                        for (iIdx = 0; iIdx < 8; iIdx++)
                        {
                            fValue5[iIdx] = (float)Math.Round((float)iData[iIdx] * 0.04577707f, 0);
                        }
                        modadata[4] = fValue5;
                    }
                    if (adamModbus6.Modbus().ReadInputRegs(iStart, m_iAiTotal, out iData))//一次性读取所有数据
                    {
                        for (iIdx = 0; iIdx < 8; iIdx++)
                        {
                            fValue6[iIdx] = (float)Math.Round((float)iData[iIdx] * 0.07629511f, 0);
                        }
                        modadata[5] = fValue6;
                    }
                    else { tongxun_error = false; }
                    if (adamModbus7.Modbus().ReadInputRegs(iStart, m_iAiTotal, out iData))//一次性读取所有数据
                    {
                        for (iIdx = 0; iIdx < 8; iIdx++)
                        {
                            fValue7[iIdx] = (float)Math.Round((float)iData[iIdx] * 0.04577707f, 0);
                        }
                        modadata[6] = fValue7;
                    }
                    if (adamModbus8.Modbus().ReadInputRegs(iStart, m_iAiTotal, out iData))//一次性读取所有数据
                    {
                        for (iIdx = 0; iIdx < 8; iIdx++)
                        {
                            fValue8[iIdx] = (float)Math.Round((float)iData[iIdx] * 0.07629511f, 0);
                        }
                        modadata[7] = fValue8;
                    }
                    else { tongxun_error = false; }
                    if (adamModbus9.Modbus().ReadInputRegs(iStart, m_iAiTotal, out iData))//一次性读取所有数据
                    {
                        for (iIdx = 0; iIdx < 8; iIdx++)
                        {
                            fValue9[iIdx] = iData[iIdx] ;
                        }
                         modadata[8] = fValue9;
                    }
                    else { tongxun_error = false;}
                    start = true;
                    Thread.Sleep(1000);

                }

                else
                {
                 
                    red();
                }
            }
        }
    }
}
