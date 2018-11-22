using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public class Scene
    {
        # region config 配置相关
        public static readonly int MAX_DEPTH = 50;//反射递归最大次数
        public static readonly int SAMPLING_TIMES = 16;//多少次采样[抗锯齿]
        #endregion

        private List<RenderObj> renderobj_list = new List<RenderObj>();

        private BaseLight light = null;//以后学完相关再加多光源

        public void AddRenderObj(RenderObj obj)
        {
            renderobj_list.Add(obj);
        }

        public void RemoveRenderObj(RenderObj obj)
        {
            renderobj_list.Remove(obj);
        }

        public void SetLightSource(BaseLight _light)
        {
            light = _light;
        }

        public bool CheckHit(Ray ray, float t_min, float t_max, ref HitRecord record, bool need_check_transimit = false)
        {
            bool hit = false;

            HitRecord temp = null;

            float t_cur_min = float.MaxValue;

            for (int i = 0; i < renderobj_list.Count; i++)
            {
                if (renderobj_list[i].Hit(ray, 0.0001f, t_max, ref temp))
                {
                    if (need_check_transimit && !renderobj_list[i].material.CanLightTransimit())//给计算影子射线hit到电介质用的.
                    {
                        continue;
                    }
                    if (temp.t < t_cur_min)//寻找最近的相交信息
                    {
                        t_cur_min = temp.t;
                        record = temp.Copy();
                        hit = true;
                    }
                }
            }

            return hit;
        }

        public Vec3 GetColorFromMaterial(Ray ray, float render_depth, float scatter_depth)
        {
            HitRecord record = null;

            //相交了再计算颜色
            if (CheckHit(ray, 0.0001f, render_depth, ref record))
            {
                //计算阴影
                HitRecord shadow_record = null;//没什么用
                Ray shadow_ray = new Ray(record.hit_point, light.light_origin - record.hit_point);
                if (CheckHit(shadow_ray, 0.0001f, render_depth, ref shadow_record, true))
                {
                    if (!shadow_record.material.CanLightTransimit())
                        return Vec3.zero;
                }

                //计算颜色
                //关于反射,这里采用 "材质的反射率为attenuation，则它传回的颜色是(1-attenuation)本身颜色，加上attenuation反射传回来的颜色"

                Vec3 surface_color = record.material.GetColor(light, ray, record, render_depth);//表面本身颜色
                var c1 = surface_color.product(Vec3.one - record.material.attenuation);

                if (scatter_depth < MAX_DEPTH)
                {
                    Ray new_ray = null;
                    if (!record.material.Refracted(ray, record, ref new_ray))
                    {
                        new_ray = record.material.GetScatteredRay(ray, record);
                    }
                    var c2 = GetColorFromMaterial(new_ray, render_depth, scatter_depth + 1).product(record.material.attenuation);

                    return c1 + c2;
                }
                else
                {
                    return Vec3.zero;
                }

                //法向量颜色
                //return System.Drawing.Color.FromArgb(
                //    (int)(255 * (record.normal.x + 1) / 2),
                //    (int)(255 * (record.normal.y + 1) / 2),
                //    (int)(255 * (record.normal.z + 1) / 2));


                //普通材质颜色
                //return BaseMaterial.NomarlMaterial.GetColor(light, null, record, cam.render_depth);
            }

            //return Vec3.one;//没有相交的话,给白色 或 达到了递归最大次数
            return _BackGroundColor(ray);//没有相交的话,给背景色
        }

        /// <summary>
        /// n重采样[抗锯齿]
        /// </summary>
        /// <param name="n"></param>
        private Vec3 _MultiSampling(Camera camera, int i, int j, int n)
        {
            Ray ray = null;
            Vec3 color_vec = Vec3.zero;
            if (n <= 0)
            {
                ray = camera.GetViewRay(i + 1, 540 - j);
                color_vec = GetColorFromMaterial(ray, camera.render_depth, 0);
                return color_vec;
            }
            else
            {
                for (int _n = 0; _n < n; _n++)
                {
                    //utils.Instance.GenerateRandomNum()*2-1 -->  [-1, 1]的随机数
                    ray = camera.GetViewRay((i + 1) + utils.Instance.GenerateRandomNum(), (540 - j) + utils.Instance.GenerateRandomNum());
                    color_vec += GetColorFromMaterial(ray, camera.render_depth, 0);
                }
                return color_vec / (float)n;
            }
        }

        public void CalculateColor(Camera camera, int i, int j, ref System.Drawing.Bitmap bitmap)
        {
            Vec3 color_vec = _MultiSampling(camera, i, j, SAMPLING_TIMES);

            bitmap.SetPixel(i, j, color_vec.ToColor());
        }

        private Vec3 _BackGroundColor(Ray ray)
        {
            Vec3 unit_direction = ray.Direction.normalize();
            float t = 0.5f * (unit_direction.y + 1.0f);
            return (1.0f - t) * Vec3.one + t * new Vec3(0.5f, 0.7f, 1.0f);
        }
    }
}
