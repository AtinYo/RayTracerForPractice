using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public class BaseTexture
    {
        public BaseTexture()
        {

        }

        public virtual Vec3 GetColorValue(float u, float v, Vec3 p)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 纯色
    /// </summary>
    public class NormalTexture : BaseTexture
    {
        private Vec3 default_color;

        public NormalTexture(Vec3 _default_color)
        {
            default_color = _default_color;
        }

        public override Vec3 GetColorValue(float u, float v, Vec3 p)
        {
            return default_color;
        }
    }


    /// <summary>
    /// 棋盘texture
    /// </summary>
    public class CheckerTexture : BaseTexture
    {
        private BaseTexture odd;
        private BaseTexture even;
        private int n;

        public CheckerTexture(BaseTexture _odd, BaseTexture _even, int _n = 50)
        {
            odd = _odd;
            even = _even;
            n = _n;
        }

        public override Vec3 GetColorValue(float u, float v, Vec3 p)
        {
            Vec3 p_N = p.normalize();
            double sines = Math.Sin(n * p_N.x) * Math.Sin(n * p_N.y) * Math.Sin(n * p_N.z);//n这个系数越大,格子间距越小.但是这个算法有问题,可能是我传入的p用的问题
            if (sines <= 0)
            {
                return odd.GetColorValue(u, v, p);
            }
            else
            {
                return even.GetColorValue(u, v, p);
            }
        }
    }

    public class NoiseTexture : BaseTexture
    {
        public NoiseTexture()
        {

        }

        public override Vec3 GetColorValue(float u, float v, Vec3 p)
        {
            return Vec3.one * Perlin.Instance.Noise(p);
        }
    }

    //目前先直接用球面映射吧
    public class ImageTexture : BaseTexture
    {
        private System.Drawing.Bitmap bitmap;
        private Vec3 center;
        //目前先直接用球面映射吧
        public ImageTexture(string imagepath, Vec3 _center)
        {
            bitmap = new System.Drawing.Bitmap(imagepath);
            center = _center;
        }

        public override Vec3 GetColorValue(float u, float v, Vec3 p)
        {
            utils.GetSphereUV(p - center, ref u, ref v);
            return Vec3.ToVec3(bitmap.GetPixel((int)(u * bitmap.Width), (int)(v * bitmap.Height)));
        }
    }
}
