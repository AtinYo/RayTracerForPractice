using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public class BaseMaterial
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="light">光源</param>
        /// <param name="view_ray">视线</param>
        /// <param name="record">相交信息</param>
        /// <param name="depth">渲染深度</param>
        /// <returns></returns>
        public virtual System.Drawing.Color GetColor(BaseLight light, Ray view_ray, HitRecord record, float depth)
        {
            throw new NotImplementedException();
        }

        public static readonly BaseMaterial NomarlMaterial = new DefaultMaterial(new Vec3(1, 1, 1));
    }

    public class DefaultMaterial : BaseMaterial
    {
        private Vec3 default_color;

        public DefaultMaterial(Vec3 _default_color)
        {
            default_color = _default_color;
        }

        public override System.Drawing.Color GetColor(BaseLight light, Ray view_ray, HitRecord record, float depth)
        {
            //把光在某一点的颜色，用距离映射到颜色区间
            //[0, depth] -- > [0, 255]不过,len = 0的时候是255, len = depth的时候是0
            float coeffience = 1-(record.t / depth);
            return (default_color * coeffience).ToColor();
        }
    }

    public class LambertianMaterial : BaseMaterial
    {
        private Vec3 diffuse;//散射系数或表面颜色

        public LambertianMaterial(Vec3 _diffuse)
        {
            diffuse = _diffuse;//(1,1,1)表示(255,255,255). [0, 255]映射到[0, 1]区间
        }

        public override System.Drawing.Color GetColor(BaseLight light, Ray view_ray, HitRecord record, float depth)
        {
            Vec3 l = -(light.GetLightRay(record.hit_point).Direction.normalize());

            float n_dot_l = Math.Max(0, Vec3.dot(record.normal, l));

            Vec3 I = new Vec3(1, 1, 1);

            Vec3 dif = diffuse * n_dot_l;

            Vec3 L = Vec3.product(I, dif);

            return L.ToColor();
        }
    }

    public class PhongMaterial : BaseMaterial
    {
        private Vec3 diffuse;//散射系数或表面颜色
        private Vec3 specular;//高光系数或高光颜色
        private int phong_exp;//Phong exponent

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_diffuse">散射系数或表面颜色;(1,1,1)表示(255,255,255). [0, 255]映射到[0, 1]区间</param>
        /// <param name="_specular">高光系数或高光颜色;(1,1,1)表示(255,255,255). [0, 255]映射到[0, 1]区间</param>
        /// <param name="_phong_exp">Phong exponent</param>
        public PhongMaterial(Vec3 _diffuse, Vec3 _specular, int _phong_exp)
        {
            diffuse = _diffuse;//(1,1,1)表示(255,255,255). [0, 255]映射到[0, 1]区间
            specular = _specular;//同上
            phong_exp = _phong_exp;
        }

        //magic function.. = =
        private float _smoothstep(float t1, float t2, float n_dot_l)
        {
            if (n_dot_l < t1)
                return t1;
            if (n_dot_l > t2)
                return 1;
            return n_dot_l;
        }

        public override System.Drawing.Color GetColor(BaseLight light, Ray view_ray, HitRecord record, float depth)
        {
            //Phong公式: L = Kd * I * max(0, n·l) + Ks * I * max(0, n·h)^p
            // Kd是散射系数, n是表面法向量,l是光源方向(从相交点指向光源), Ks是高光系数,h是v+l的单位向量,v是视线反方向(从相交点只想视点), I是光强(光在这一点的颜色)
            Vec3 l = -(light.GetLightRay(record.hit_point).Direction.normalize());
            float n_dot_l = Math.Max(0, Vec3.dot(record.normal, l));

            Vec3 h = (-view_ray.Direction + l).normalize();
            float n_dot_h = Math.Max(0, Vec3.dot(record.normal, h));

            Vec3 I = new Vec3(1, 1, 1);

            Vec3 dif = diffuse * n_dot_l;

            Vec3 spe = specular * (float)Math.Pow(n_dot_h, phong_exp);

            spe *= _smoothstep(0, 0.12f, n_dot_l);//为了解决背光面高光的问题,如果n_dot_l<0直接spe=0会有颜色间断层,需要做插值处理

            Vec3 L = Vec3.product(I, dif + spe);

            //上面有两个bug/疑问
            //(1)背光面也能出现高光,因为公式里n·l<0并不影响 Ks * I * max(0, n·h)^p的值;
            //(2)求出来的L超过(1,1,1)范围了.---暂时强行映射到[0,1]
            //上面两个问题估计都是要改diffuse和specular来处理
            //目前先特殊处理下,以后看看大佬们怎么搞的
            return L.ToColor();
        }
    }

}
