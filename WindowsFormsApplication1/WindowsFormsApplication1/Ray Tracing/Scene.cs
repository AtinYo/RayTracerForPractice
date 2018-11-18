using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    public class Scene
    {
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

        public System.Drawing.Color GetColor(int x, int y, Camera cam)
        {
            Ray ray = cam.GetViewRay(x, y);
            if (x == 480 && y == 270)
            {
                int a = 1;
            }
            HitRecord temp = null;

            HitRecord record = null;
            float t_min = float.MaxValue;

            for(int i = 0; i < renderobj_list.Count; i++)
            {
                if (renderobj_list[i].Hit(ray, 0.0001f, float.MaxValue, ref temp))
                {
                    if(temp.t< t_min)//寻找最近的相交信息
                    {
                        t_min = temp.t;
                        record = temp;
                    }
                }
            }

            //相交了再计算颜色
            if (record != null)
            {
                //计算颜色
                return System.Drawing.Color.FromArgb(
                    (int)(255 * (record.normal.x + 1) / 2),
                    (int)(255 * (record.normal.y + 1) / 2),
                    (int)(255 * (record.normal.z + 1) / 2));
                //return System.Drawing.Color.White;
            }
            else
            {
                //没有相交的话,给黑色
                return System.Drawing.Color.FromArgb(0, 0, 0);
            }
        }
    }
}
