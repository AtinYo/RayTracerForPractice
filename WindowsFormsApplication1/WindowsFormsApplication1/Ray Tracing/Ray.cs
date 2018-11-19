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

        public Ray(Vec3 start, Vec3 end)
        {
            org = start;
            dir = end - start;
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
