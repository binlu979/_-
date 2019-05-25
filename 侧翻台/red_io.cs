using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace 侧翻台
{
    class red_io
    {
        Thread t_red;
        public void red_id()
        {
            info();
        }
        public void info()
        {
            t_red = new Thread(red_plc);
        }
        public void red_plc()
        {

        }
    }
}
