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
        public Vec3 light_origin { private set; get; }

        public BaseLight(Vec3 origin)
        {
            light_origin = origin;
        }

        public virtual Ray GetLightRay(Vec3 point, float time)
        {
            throw new NotImplementedException();
        }
    }

    public class PointLight : BaseLight
    {
        public PointLight(Vec3 origin) : base(origin)
        {

        }

        public override Ray GetLightRay(Vec3 point, float time)
        {
            return new Ray(light_origin, point - light_origin, time);
        }
    }
}
