using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public static class utils
    {
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

        public static double GenerateRandomNum_D()
        {
            return rand.NextDouble();
        }

        /// <summary>
        /// 返回一个介于 0.0 和 1.0 之间的随机数。
        /// </summary>
        /// <returns></returns>
        public static float GenerateRandomNum()
        {
            return (float)rand.NextDouble();
        }

        //返回[min, max]区间的一个随机数
        public static float GenerateRandomNum(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }

        //返回长度约为len的vec3
        public static Vec3 GenerateRandomVector(float len)
        {
            float x = GenerateRandomNum(0, len);
            float y = GenerateRandomNum(0, len - x);
            return new Vec3(x, y, len - x - y);
        }

        public static void Swap(ref float a, ref float b)
        {
            float temp = b;
            b = a;
            a = temp;
        }
    }
}
