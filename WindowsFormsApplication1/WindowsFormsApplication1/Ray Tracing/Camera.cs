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
        public Camera(Vec3 lookfrom, Vec3 lookat, Vec3 viewup, int _width, int _height, float _distance, float _render_depth)
        {
            viewpoint = lookfrom;

            Vec3 viewdir = lookat - lookfrom;

            width = _width;
            height = _height;
            distance = _distance;
            render_depth = _render_depth;

            //建立右手系的相机坐标
            Vec3 u, v, w;//camera coordinate，可能不需要保存
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
            return new Ray(viewpoint, pixel_origin + x / width * pixel_horizontal + y / height * pixel_vertical - viewpoint);
        }
    }
}
