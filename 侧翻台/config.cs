using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using inio;
using System.Windows.Forms;
namespace 侧翻台
{
    class config
    {
        #region 用户设置
        private static user _admin = user.youke;
        public user admin
        {
            get { return _admin; }
            set { _admin = value; }
        }
        public enum user
        {
            admin = 0,
            user = 1,
            youke = 2,

        }
        ini ad = new ini();
        #endregion

        string file = Application.StartupPath + "\\system.sys";
        private static float _wendu = 0;
        private static bool _jiare = false;
        private static bool _fan;
        /// <summary>
        /// 当前油温
        /// </summary>
        public static float wendu
        {
            get { return _wendu;}
            set { _wendu = value; }
        }
        /// <summary>
        /// 加热
        /// </summary>
        public static bool jiare
        {
            set { _jiare = value; }
            get { return jiare; }
        }
        /// <summary>
        /// 风扇
        /// </summary>
        public static bool fan
        {
            set { _fan = value; }
            get { return _fan; }
        }

        #region 获取或设置通讯参数
        /// <summary>
        /// 获取或设置COM口
        /// </summary>
        public string ck_com
        {
            get { return read_tongxun("tx_COM"); }
            set { write_tongxun("tx_com", value.ToString()); }
        }

        /// <summary>
        /// 获取或设置波特率
        /// </summary>
        public string tx_Baud
        {
            get { return read_tongxun("tx_Baud"); }
            set { write_tongxun("tx_Baud", value.ToString()); }
        }

        /// <summary>
        ///获取或设置数据长
        /// </summary>

        public string tx_sjc
        {
            get { return read_tongxun("tx_sjc"); }
            set { write_tongxun("tx_sjc", value.ToString()); }
        }

        /// <summary>
        /// 设置停止位
        /// </summary>
        public string tx_stop
        {
            get { return read_tongxun("tx_stop"); }
            set { write_tongxun("tx_stop", value.ToString()); }
        }
        /// <summary>
        /// 获取或设置校验位
        /// </summary>
        public string tx_jiaoyanwei
        {
            get { return read_tongxun("tx_ins"); }
            set { write_tongxun("tx_ins", value.ToString()); }
        }
        /// <summary>
        /// 获取或设置停止时间
        /// </summary>
        public string tx_outtime
        {
            get { return read_tongxun("tx_outtime"); }
            set { write_tongxun("tx_outtime", value.ToString()); }
        }
        private string read_tongxun(string name)
        {
            try
            {
                return inio.ini.INIGetStringValue(file, "communication", name, null);
            }
            catch { return null; }

        }
        private void write_tongxun(string name, string value)
        {
            ini.INIWriteValue(file, "communication", name, value);

        }
        #endregion


        #region 获取或设置模板风格
        public  string style
        {
            get { return read("style", "tp"); }
            set { write("style", "tp", value); }
        }

        #endregion
        private void write(string fcalss, string name, string value)
        {
            ini.INIWriteValue(file, fcalss, name, value);
        }
        private string read(string fclass, string name)
        {
            string sjc = inio.ini.INIGetStringValue(file, fclass, name, null);

            return sjc;
        }

        public void write_jiaozheng(int i, int a)
        {
            ini.INIWriteValue(file, "jiaozheng", i.ToString(), a.ToString());
        }
        public int red_jiaozheng(int a)
        {
            string sjc = inio.ini.INIGetStringValue(file, "jiaozheng", a.ToString(), null);

            return int.Parse(sjc);


        }
    }
}
