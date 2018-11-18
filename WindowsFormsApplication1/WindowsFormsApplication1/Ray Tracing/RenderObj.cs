using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public class RenderObj : Hitable
    {
        public virtual bool Hit(Ray ray, float t_min, float t_max, ref HitRecord record)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 圆
    /// </summary>
    public class Sphere : RenderObj
    {
        public Vec3 center { get; private set; }
        public float radius { get; private set; }

        public Sphere(Vec3 _center, float _radius)
        {
            center = _center;
            radius = _radius;
        }

        public override bool Hit(Ray ray, float t_min, float t_max, ref HitRecord record)
        {
            //ray == o+t*d; o 是射线原点,t是参数,d是方向
            //(p - c)^2 = radius^2;p是球面上一点,c是球心,radius是球半径
            //交点就是把射线方程代入球面方程化为一元二次方程求参数t
            Vec3 p_sub_c = ray.Origin - center;
            float A = Vec3.dot(ray.Direction, ray.Direction);
            float B = Vec3.dot(p_sub_c, ray.Direction);//(b^2)/4
            float C = Vec3.dot(p_sub_c, p_sub_c) - radius * radius;

            float b_square_sub_four_a_c_div_four = B * B - A * C;//(b^2 - 4ac)/4

            //方程组有解
            if (b_square_sub_four_a_c_div_four > 0)
            {
                float temp = (-B - (float)Math.Sqrt(b_square_sub_four_a_c_div_four)) / A;// (-b-sqrt(4ac))/2a 分子分母同时除以2就可以得到这个式子,这里的 b=2B
                if (temp > t_min && temp < t_max)
                {
                    if (record == null)
                    {
                        record = new HitRecord();
                    }
                    record.t = temp;
                    record.hit_point = ray.GetPoint(temp);
                    record.normal = (new Ray(center, record.hit_point)).Direction.unitize();//球心与球面上一点为法向量方向
                    return true;
                }

                temp = (-B + (float)Math.Sqrt(b_square_sub_four_a_c_div_four)) / A;
                if (temp > t_min && temp < t_max)
                {
                    if (record == null)
                    {
                        record = new HitRecord();
                    }
                    record.t = temp;
                    record.hit_point = ray.GetPoint(temp);
                    record.normal = (new Ray(center, record.hit_point)).Direction.unitize();//球心与球面上一点为法向量方向
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 矩形
    /// </summary>
    public class Rectangle : RenderObj
    {
        public override bool Hit(Ray ray, float t_min, float t_max, ref HitRecord record)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 三角形
    /// </summary>
    public class Triangle : RenderObj
    {
        public override bool Hit(Ray ray, float t_min, float t_max, ref HitRecord record)
        {
            throw new NotImplementedException();
        }
        ///到时候再进一步三角扇,三角带等用来表示多边形.参考fundamentals of computer graphic
    }
}
