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

        public static void GetSphereUV(Vec3 p, ref float u, ref float v)
        {
            //球面坐标投影到uv坐标
            p = p.normalize();
            //单位球面上坐标可以用θ(theta),φ(phi)两个角度变量表示.theta is the angle down from the pole, and phi is the angle around the axis through the poles,
            //比如
            //x = cos(phi) cos(theta)
            //y = sin(phi) cos(theta)
            //z = sin(theta)
            //然后uv坐标用这两个角度的话
            //u = phi / (2*Pi)  phi取值范围[0,2pi]
            //v = theta / Pi    theta取值范围[0,pi]
            double phi = Math.Atan2(p.y, p.x); //取值范围为-π≤θ≤π
            double theta = Math.Asin(p.z);//取值范围为-π/2 ≤θ≤π/2
            //范围映射下到需要的范围  phi取值范围[0,2pi]  theta取值范围[0,pi]
            phi += Math.PI;
            theta += Math.PI / 2;
            //求uv
            u = (float)(phi / (2 * Math.PI));
            v = (float)(theta / Math.PI);
        }
    }
}
