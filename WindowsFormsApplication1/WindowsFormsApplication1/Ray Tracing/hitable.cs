using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public class HitRecord
    {
        public float t;//临时参数?比如射线碰撞到球,有两个交点,t是方程的两个解中的一个
        public Vec3 hit_point;//碰撞点
        public Vec3 normal;//表面法向量
        public BaseMaterial material;//物体材质,用来确定光照模型

        public HitRecord() { }

        public HitRecord(float _t, Vec3 _hit_point, Vec3 _normal, BaseMaterial _material)
        {
            t = _t;
            hit_point = _hit_point;
            normal = _normal;
            material = _material;
        }

        public HitRecord Copy()
        {
            return new HitRecord(t, hit_point, normal, material);
        }
    }

    public interface Hitable
    {
        bool Hit(Ray ray, float t_min, float t_max, ref HitRecord record);
    }
}
