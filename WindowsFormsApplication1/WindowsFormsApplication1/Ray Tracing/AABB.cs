using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    //axial align bound box. 参考ray tracing in next week实现
    public class AABB
    {
        public AABB(Vec3 _min, Vec3 _max)
        {
            min = _min;
            max = _max;
        }

        //右手系的xyz下的min, max
        public Vec3 min { get; private set; }
        public Vec3 max { get; private set; }

        //原理,slabs方法,线交box变成 三维度的 区间相交比较.
        //比如, 先从2d出发,如果一条射线要相较于一个矩形(Vec2_min, Vec2_max). 那么射线p(t)=o+t*dir方程与线x=x_min或x=x_max或y=y_min或y=y_max交点的解分别为t0,t1,t2,t3
        //然后自己画个图,发现只有(t0,t1)区间和(t2,t3)区间有交集,射线才能与矩形相交
        //比如x=x_min交点解是 t = (x_min - o.x)/dir.x
        //如果射线还要考虑方向问题,沿着x轴方向的话,求出来的这个轴的t_min和t_max满足t_min<t_max;如果是反向的话,需要人为互换一下,保证t_min<t_max
        //推广到3d空间,就是xyz分成六个面,每两个面(比如x=x_min,x=x_max)构成一个区间,如果射线与aabb相交,那么射线的t必须在这个区间有解;
        //说白了就是三个区间(t0,t1)(t2,t3)(t4,t5)两两有交集
        public bool Hit(Ray ray, float t_min, float t_max)
        {
            //inv表示倒置的意思
            Vec3 dir = ray.Direction;
            Vec3 o = ray.Origin;
            for (int i = 0; i < 3; i++)
            {
                float inv_dir = 1f / dir[i];
                float t0 = (min[i] - o[i]) * inv_dir;
                float t1 = (max[i] - o[i]) * inv_dir;
                if (inv_dir < 0f)
                {
                    //说明不是沿着轴方向,交换一下
                    utils.Swap(ref t0, ref t1);
                }
                //t_min、t_max在三次循环中不断变更,从一开始的函数传入的默认值开始不断变化为xyz的区间
                t_min = t0 > t_min ? t0 : t_min;//举个例子,x的区间为(t0,t1),y的区间为(t2,t3).两个区间相交,就是满足 max(t0,t2) < min(t1,t3)
                t_max = t1 < t_max ? t1 : t_max;
                if (t_min >= t_max)
                    return false;
            }
            return true;
        }

        public static AABB GetSurroundingBox(AABB box1, AABB box2)
        {
            Vec3 min = new Vec3(
                box1.min.x <= box2.min.x ? box1.min.x : box2.min.x,
                box1.min.y <= box2.min.y ? box1.min.y : box2.min.y,
                box1.min.z <= box2.min.z ? box1.min.z : box2.min.z);

            Vec3 max = new Vec3(
                box1.max.x > box2.max.x ? box1.max.x : box2.max.x,
                box1.max.y > box2.max.y ? box1.max.y : box2.max.y,
                box1.max.z > box2.max.z ? box1.max.z : box2.max.z);

            return new AABB(min, max);
        }
    }
}
