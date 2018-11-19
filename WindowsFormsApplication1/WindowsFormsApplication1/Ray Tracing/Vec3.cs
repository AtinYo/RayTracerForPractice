using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    /// <summary>
    /// can be vector3, color(rgb)
    /// </summary>
    public class Vec3
    {
        private float[] data;

        public Vec3(float x_r, float y_g, float z_b)
        {
            data = new float[3];
            data[0] = x_r;
            data[1] = y_g;
            data[2] = z_b;
        }

        public float x
        {
            get
            {
                return data[0];
            }
        }

        public float y
        {
            get
            {
                return data[1];
            }
        }

        public float z
        {
            get
            {
                return data[2];
            }
        }

        public float this[int i]
        {
            get
            {
                return data[i];
            }
        }

        public static Vec3 operator +(Vec3 l, Vec3 r)
        {
            return new Vec3(l.x + r.x, l.y + r.y, l.z + r.z);
        }

        public static Vec3 operator -(Vec3 l, Vec3 r)
        {
            return new Vec3(l.x - r.x, l.y - r.y, l.z - r.z);
        }

        public static Vec3 operator -(Vec3 v)
        {
            return new Vec3(-v.x, -v.y, -v.z);
        }

        public static Vec3 operator *(Vec3 v3, float num)
        {
            return new Vec3(v3.x * num, v3.y * num, v3.z * num);
        }

        public static Vec3 operator *(float num, Vec3 v3)
        {
            return new Vec3(num * v3.x, num * v3.y, num * v3.z);
        }

        public static Vec3 operator /(Vec3 v3, float num)
        {
            return new Vec3(v3.x / num, v3.y / num, v3.z / num);
        }

        public static float dot(Vec3 l, Vec3 r)
        {
            return l.x * r.x + l.y * r.y + l.z * r.z;
        }

        public static Vec3 cross(Vec3 l, Vec3 r)
        {
            return new Vec3(l.y * r.z - l.z * r.y, l.z * r.x - l.x * r.z, l.x * r.y - l.y * r.x);
        }

        /// <summary>
        /// 向量三个分量对应相乘
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Vec3 product(Vec3 l, Vec3 r)
        {
            return new Vec3(l.x * r.x, l.y * r.y, l.z * r.z);
        }

        public static readonly Vec3 unit = new Vec3(1, 1, 1);//单位向量

        public static readonly Vec3 zero = new Vec3(0, 0, 0);//零向量

        /// <summary>
        /// 返回单位化后的向量
        /// </summary>
        /// <returns></returns>
        public Vec3 normalize()
        {
            float l = len();
            if (l == 0)
                return new Vec3(0, 0, 0);
            return this / l;
        }

        public float len()
        {
            return (float)Math.Sqrt(data[0] * data[0] + data[1] * data[1] + data[2] * data[2]);
        }

        public System.Drawing.Color ToColor()
        {
            //保证Vec3的xyz在[0,1]区间
            for (int i = 0; i < 3; i++)
            {
                data[i] = Math.Max(0, data[i]);
                data[i] = Math.Min(1, data[i]);
            }

            return System.Drawing.Color.FromArgb((int)(x * 255), (int)(y * 255), (int)(z * 255));
        }
    }
}
