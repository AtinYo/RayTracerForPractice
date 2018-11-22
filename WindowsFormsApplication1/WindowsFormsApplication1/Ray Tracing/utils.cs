using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public class utils
    {
        private static utils _instance;
        public static utils Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new utils();
                return _instance;
            }
        }

        private static Random _rand;
        public static Random rand
        {
            get
            {
                if (_rand == null)
                    _rand = new Random();
                return _rand;
            }
        }

        public double GenerateRandomNum_D()
        {
            return rand.NextDouble();
        }

        /// <summary>
        /// 返回一个介于 0.0 和 1.0 之间的随机数。
        /// </summary>
        /// <returns></returns>
        public float GenerateRandomNum()
        {
            return (float)rand.NextDouble();
        }
    }
}
