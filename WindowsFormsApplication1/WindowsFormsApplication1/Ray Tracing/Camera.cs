using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public class Camera
    {
        private Vec3 viewpoint;
        private int width, height;//像素屏幕的宽、高
        private float distance;//像素屏幕与视点的距离
        public float render_depth { get; private set; }//渲染深度
        private Vec3 pixel_origin;//pixel coordinate origin
        private Vec3 pixel_horizontal;//pixel coordinate horizontal
        private Vec3 pixel_vertical;//pixel coordinate vertical
        private float aperture;//用来做聚焦模糊的defocus blur
        Vec3 u, v, w;//camera coordinate，可能不需要保存

        public Camera(Vec3 lookfrom, Vec3 lookat, Vec3 viewup, int _width, int _height, float _distance, float _render_depth, float _aperture=0f)
        {
            viewpoint = lookfrom;

            Vec3 viewdir = lookat - lookfrom;

            width = _width;
            height = _height;
            distance = _distance;
            render_depth = _render_depth;
            aperture = _aperture;

            //建立右手系的相机坐标
            w = -viewdir.normalize();//相机的z轴,对于相机往里(往后
            u = Vec3.cross(viewup, w).normalize();//相机的x轴,对于相机水平向右
            v = Vec3.cross(w, u).normalize();//相机的y轴,对于相机水平向上

            //按照相机的uvw坐标轴,构建二维像素坐标.[像素屏幕宽width,高height)
            pixel_origin = lookfrom - width / 2 * u - height / 2 * v - distance * w;
            pixel_horizontal = width * u;
            pixel_vertical = height * v;
        }

        /// <summary>
        /// 根据像素坐标(x,y)生成ray
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Ray GetViewRay(float x, float y)
        {
            if (aperture > 0)
            {
                // 聚焦模糊
                float rand_num = utils.Instance.GenerateRandomNum(0, 1f);
                Vec3 offset = u * rand_num + v * (1 - rand_num);
                return new Ray(viewpoint + offset, pixel_origin + x / width * pixel_horizontal + y / height * pixel_vertical - viewpoint - offset);
            }
            return new Ray(viewpoint, pixel_origin + x / width * pixel_horizontal + y / height * pixel_vertical - viewpoint);
        }
    }
}
