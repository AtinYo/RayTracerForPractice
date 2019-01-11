using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public class BaseMaterial
    {
        public Vec3 attenuation { get; protected set; }//衰减率
        public float refracted_index { get; protected set; }//折射率,typically air = 1, glass = 1.3-1.7, diamond = 2.4[通常空气折射率1,玻璃1.3-1.7,钻石2.4]
        public BaseTexture texture { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="light">光源</param>
        /// <param name="view_ray">视线</param>
        /// <param name="record">相交信息</param>
        /// <param name="depth">渲染深度</param>
        /// <returns></returns>
        public virtual Vec3 GetColor(BaseLight light, Ray view_ray, HitRecord record, float depth)
        {
            throw new NotImplementedException();
        }

        public virtual Ray GetScatteredRay(Ray ray_in, HitRecord record)
        {
            throw new NotImplementedException();
        }

        public virtual bool Refracted(Ray ray_in, HitRecord record, ref Ray refracted)
        {
            return false;
        }

        /// <summary>
        /// 暂时这个函数用于算阴影,因为阴影是用shadow_ray去判断是否hit,但是对于电介质这类可以通透的并不能单单相交,暂时简陋地给个函数做判断处理.电介质返回true
        /// </summary>
        /// <returns></returns>
        public virtual bool CanLightTransimit()
        {
            return false;
        }

        public static readonly BaseMaterial NomarlMaterial = new DefaultMaterial(new NormalTexture(new Vec3(1, 1, 1)));
    }



    public class DefaultMaterial : BaseMaterial
    {
        public DefaultMaterial(BaseTexture _texture)
        {
            texture = _texture;
        }

        public override Vec3 GetColor(BaseLight light, Ray view_ray, HitRecord record, float depth)
        {
            //把光在某一点的颜色，用距离映射到颜色区间
            //[0, depth] -- > [0, 255]不过,len = 0的时候是255, len = depth的时候是0
            float coeffience = 1-(record.t / depth);
            return texture.GetColorValue(0, 0, null) * coeffience;
        }
    }



    public class LambertianMaterial : BaseMaterial
    {
        private Vec3 diffuse;//散射系数或表面颜色,这里我作为表面颜色
        public LambertianMaterial(Vec3 _diffuse, Vec3 _attenuation, BaseTexture _texture)
        {
            diffuse = _diffuse;//(1,1,1)表示(255,255,255). [0, 255]映射到[0, 1]区间
            attenuation = _attenuation;//(0,0,0)到(1,1,1)区间的衰减系数
            texture = _texture;
        }

        public override Vec3 GetColor(BaseLight light, Ray view_ray, HitRecord record, float depth)
        {
            Vec3 l = -(light.GetLightRay(record.hit_point, view_ray.DeltaTime).Direction.normalize());

            float n_dot_l = Math.Max(0, Vec3.dot(record.normal, l));

            Vec3 I = Vec3.one;

            Vec3 color = texture == null ? diffuse : texture.GetColorValue(0, 0, record.hit_point);//这里还要改的，下面的material也是，感觉material用来定义反射之类的物理计算公式。颜色映射还是交给texture吧

            Vec3 dif = color * n_dot_l;

            Vec3 L = Vec3.product(I, dif);

            return L;
        }

        public override Ray GetScatteredRay(Ray ray_in, HitRecord record)
        {
            //漫反射
            //取单位法向量 + 单位半径球上任意一点(可以理解为法向量的endpoint出发，指向球面任意一点)
            return new Ray(record.hit_point, record.normal + utils.GenerateRandomVector(1f), ray_in.DeltaTime);
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
        public PhongMaterial(Vec3 _diffuse, Vec3 _specular, int _phong_exp, Vec3 _attenuation, BaseTexture _texture)
        {
            diffuse = _diffuse;//(1,1,1)表示(255,255,255). [0, 255]映射到[0, 1]区间
            specular = _specular;//同上
            phong_exp = _phong_exp;
            attenuation = _attenuation;
            texture = _texture;
        }

        //magic function.. = =,神奇的以0和0.12f这两个数为分界线做插值可以解决背光面高光问题...
        private float _smoothstep(float t1, float t2, float n_dot_l)
        {
            if (n_dot_l < t1)
                return t1;
            if (n_dot_l > t2)
                return 1;
            return n_dot_l;
        }

        public override Vec3 GetColor(BaseLight light, Ray view_ray, HitRecord record, float depth)
        {
            //Phong公式: L = Kd * I * max(0, n·l) + Ks * I * max(0, n·h)^p
            // Kd是散射系数, n是表面法向量,l是光源方向(从相交点指向光源), Ks是高光系数,h是v+l的单位向量,v是视线反方向(从相交点只想视点), I是光强(光在这一点的颜色)
            Vec3 l = -(light.GetLightRay(record.hit_point, view_ray.DeltaTime).Direction.normalize());
            float n_dot_l = Math.Max(0, Vec3.dot(record.normal, l));

            Vec3 h = (-view_ray.Direction + l).normalize();
            float n_dot_h = Math.Max(0, Vec3.dot(record.normal, h));

            Vec3 I = Vec3.one;

            Vec3 dif = diffuse * n_dot_l;

            Vec3 spe = specular * (float)Math.Pow(n_dot_h, phong_exp);

            spe *= _smoothstep(0, 0.12f, n_dot_l);//为了解决背光面高光的问题,如果n_dot_l<0直接spe=0会有颜色间断层,需要做插值处理

            Vec3 L = Vec3.product(I, dif + spe);

            //上面有两个bug/疑问
            //(1)背光面也能出现高光,因为公式里n·l<0并不影响 Ks * I * max(0, n·h)^p的值;
            //(2)求出来的L超过(1,1,1)范围了.---暂时强行映射到[0,1]
            //上面两个问题估计都是要改diffuse和specular来处理
            //目前先特殊处理下,以后看看大佬们怎么搞的
            return L;
        }

        public override Ray GetScatteredRay(Ray ray_in, HitRecord record)
        {
            //镜面反射...
            Vec3 scattered_dir = ray_in.Direction - 2 * Vec3.dot(ray_in.Direction, record.normal) * record.normal;
            return new Ray(record.hit_point, scattered_dir, ray_in.DeltaTime);
        }
    }


    //电介质材质.
    public class DielectricMaterial : BaseMaterial
    {
        private Vec3 diffuse;//散射系数或表面颜色

        public DielectricMaterial(Vec3 _diffuse, float _refracted_index, BaseTexture _texture)
        {
            diffuse = _diffuse;
            attenuation = Vec3.one;
            refracted_index = _refracted_index;
            texture = _texture;
        }

        public override Vec3 GetColor(BaseLight light, Ray view_ray, HitRecord record, float depth)
        {
            Vec3 l = -(light.GetLightRay(record.hit_point, view_ray.DeltaTime).Direction.normalize());

            float n_dot_l = Math.Max(0, Vec3.dot(record.normal, l));

            Vec3 I = Vec3.one;

            Vec3 dif = diffuse * n_dot_l;

            Vec3 L = Vec3.product(I, dif);

            return L;
        }

        public override Ray GetScatteredRay(Ray ray_in, HitRecord record)
        {
            //镜面反射...
            Vec3 scattered_dir = ray_in.Direction - 2 * Vec3.dot(ray_in.Direction, record.normal) * record.normal;
            return new Ray(record.hit_point, scattered_dir, ray_in.DeltaTime);
        }

        ///懒得注释了,折射公式推一推就有了
        public override bool Refracted(Ray ray_in, HitRecord record, ref Ray refracted)
        {
            Vec3 outward_normal;
            float ni_over_nt;
            # region 判断从空气到介质还是介质到空气.此外这里折射计算可能有误,参考的ray tracing in one weekend,等以后看看其他书籍比较一下
            float ray_dir_dot_n = Vec3.dot(ray_in.Direction, record.normal);
            if (ray_dir_dot_n > 0)
            {
                outward_normal = -record.normal;
                ni_over_nt = record.material.refracted_index / 1f;
            }
            else
            {
                outward_normal = record.normal;
                ni_over_nt = 1f / record.material.refracted_index;
            }
            #endregion

            Vec3 v = ray_in.Direction.normalize();
            float dt = Vec3.dot(v, outward_normal);
            float discriminant = 1.0f - ni_over_nt * ni_over_nt * (1 - dt * dt);
            if (discriminant > 0)//说明可以折射
            {
                Vec3 refracted_dir = ni_over_nt * (v - outward_normal * dt) - outward_normal * (float)Math.Sqrt(discriminant);
                refracted = new Ray(record.hit_point, refracted_dir, ray_in.DeltaTime);
                return true;
            }
            return false;
        }

        public override bool CanLightTransimit()
        {
            return true;
        }
    }
}
