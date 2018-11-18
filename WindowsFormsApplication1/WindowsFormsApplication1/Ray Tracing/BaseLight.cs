using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    /// <summary>
    /// 暂时瞎写光源,到时候看完相关再改
    /// </summary>
    public class BaseLight
    {
        protected Vec3 light_origin;

        public BaseLight(Vec3 origin)
        {
            light_origin = origin;
        }

        public virtual Ray GetLight(Vec3 point)
        {
            throw new NotImplementedException();
        }

        public virtual Vec3 GetLightIntensity(int x, int y)
        {
            throw new NotImplementedException();
        }
    }

    public class PointLight : BaseLight
    {
        public PointLight(Vec3 origin) : base(origin)
        {

        }

        public override Ray GetLight(Vec3 point)
        {
            return new Ray(light_origin, point);
        }

        public override Vec3 GetLightIntensity(int x, int y)
        {
            return base.GetLightIntensity(x, y);
        }
    }
}
