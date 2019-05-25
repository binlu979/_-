using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.Instrumentation;

namespace 侧翻台
{
    public partial class xianshiLED : UserControl
    {
        float text1 = 0.00f, text2 = 0.00f, text3 = 0.00f, text4 = 0.00f, text5 = 0;
        public xianshiLED()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.BackColor = Color.Transparent;
        
        }
        public float b_text1
        {
            set { text1  = value; pu_value(); }
            get { return text1; }

        }
        public float b_text2
        {
            set { text2 = value; pu_value(); }
            get { return text2; }

        }
        public float b_text3
        {
            set { text3 = value; pu_value(); }
            get { return text3; }
        }
        public float b_text4
        {
            set { text4 = value; pu_value(); }
            get { return text4; }
        }
        public float b_text5
        {
            set { text4 = value; pu_value(); }
            get { return text5; }
        }
        private void gaugeControl4_Click(object sender, EventArgs e)
        {
      
        }

        private void xianshiLED_Resize(object sender, EventArgs e)
        {
            xianshiLED fr = new xianshiLED();
            gaugeControl1.Width = this.Width / 2 - 4;
            gaugeControl1.Height = this.Height / 3 - 5;
            gaugeControl2.Width = this.Width / 2 - 4;
            gaugeControl2.Height = this.Height / 3 - 5;
            int y2 = gaugeControl2.Location.Y;
            this.gaugeControl2.Location = new Point(gaugeControl1.Width + 6, y2);

            gaugeControl5.Width = this.Width - 4;
            gaugeControl5.Height = this.Height / 3;
            gaugeControl5.Location = new Point(gaugeControl5.Location.X, this.Height / 3);

            gaugeControl3.Width = this.Width / 2 - 4;
            gaugeControl3.Height = this.Height / 3 - 5;
            gaugeControl3.Location = new Point(gaugeControl3.Location.X, this.Height - gaugeControl3.Height - 3);

            gaugeControl4.Width = this.Width / 2 - 4;
            gaugeControl4.Height = this.Height / 3 - 5;
            gaugeControl4.Location = new Point(gaugeControl3.Width + 6, this.Height - gaugeControl4.Height - 3);
        }
        void GaugeDisplay(GaugeControl gc, double value)
        {
            ((NumericIndicator)gc.GaugeItems[0]).Text = value.ToString();

        }
      private void pu_value()
        {
            /*
            textBoxX1.Text = text1.ToString();
             textBoxX2.Text = text2.ToString();
             textBoxX3.Text = text3.ToString();
             textBoxX4.Text = text4.ToString();
             textBoxX5.Text =  (text1  +  text2 + text3 + text4).ToString();
      */
            // DevComponents.Instrumentation.NumericIndicator numericIndicator1 = new DevComponents.Instrumentation.NumericIndicator();
            // NumericIndicator1
            // ((NumericIndicator)gc.GaugeItems[0]).Value = value;
            // gaugeControl5.GaugeItems[0].

            GaugeDisplay(gaugeControl1, text1);
            GaugeDisplay(gaugeControl2, text2);
            GaugeDisplay(gaugeControl3, text3);
            GaugeDisplay(gaugeControl4, text4);
            text5 = text1 + text2 + text3 + text4;
            GaugeDisplay(gaugeControl5, text5);
        }

        public static explicit operator xianshiLED(string v)
        {
            throw new NotImplementedException();
        }
    }
}
