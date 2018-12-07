using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public class Ray
    {
        private Vec3 org;//origin point
        private Vec3 dir;//direction,没单位化的
        private float time;//做动态模糊的,传入来的值其实是(time-time0)/(time1-time0);其中time是[time0, time1]区间中的一个随机数

        public Ray(Vec3 _org, Vec3 _dir, float _time)
        {
            org = _org;
            dir = _dir;
            time = _time;
        }

        public Vec3 Origin
        {
            get
            {
                return org;
            }
        }

        public Vec3 Direction
        {
            get
            {
                return dir;
            }
        }

        public float DeltaTime
        {
            get
            {
                return time;
            }
        }
        /// <summary>
        /// point in Ray function is : point(t) = origin + t*direction
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vec3 GetPoint(float t)
        {
            return org + t * dir;
        }
    }
}
