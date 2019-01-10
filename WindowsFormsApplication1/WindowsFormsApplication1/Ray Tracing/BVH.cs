using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    #region 三个比较函数类
    public class CompareInAxisX : IComparer<Hitable>
    {
        public int Compare(Hitable x, Hitable y)
        {
            AABB a = null;
            AABB b = null;
            x.GetBoundingBox(0, 0, ref a);
            y.GetBoundingBox(0, 0, ref b);
            if (a.min.x < b.min.x)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

    }

    public class CompareInAxisY : IComparer<Hitable>
    {
        public int Compare(Hitable x, Hitable y)
        {
            AABB a = null;
            AABB b = null;
            x.GetBoundingBox(0, 0, ref a);
            y.GetBoundingBox(0, 0, ref b);
            if (a.min.y < b.min.y)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    public class CompareInAxisZ : IComparer<Hitable>
    {
        public int Compare(Hitable x, Hitable y)
        {
            AABB a = null;
            AABB b = null;
            x.GetBoundingBox(0, 0, ref a);
            y.GetBoundingBox(0, 0, ref b);
            if (a.min.z < b.min.z)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
    #endregion

    public class BVH_Node : Hitable
    {
        public Hitable left_node { get; private set; }
        public Hitable right_node { get; private set; }
        public AABB aabb { get; private set; }

        public BVH_Node(List<Hitable> nodes,int start_index, int end_index, float t_min, float t_max)
        {
            //这里就不做nodes为null或者长度为0的考虑了,放到外面去做
            //参考raytracingnextweek的方法.每次随机选轴然后排序,然后平分两部分.不断二分直到只剩下1-2个节点.
            int node_num = end_index - start_index + 1;
            if (node_num == 1)
            {
                left_node = nodes[start_index];
                right_node = null;//书的话是跟left一样.其实如果是这样的话,hit和构造包围盒都需要再判断.看个人喜欢吧
            }
            else if (node_num == 2)
            {
                left_node = nodes[start_index];
                right_node = nodes[end_index];
            }
            else
            {
                switch((int)utils.GenerateRandomNum(0,3))
                {
                    case 0:
                    case 1:
                        nodes.Sort(start_index, node_num, new CompareInAxisX());
                        break;
                    case 2:
                        nodes.Sort(start_index, node_num, new CompareInAxisY());
                        break;
                    case 3:
                        nodes.Sort(start_index, node_num, new CompareInAxisZ());
                        break;
                }
                int mid = (start_index+end_index)/2;//下面递归不会报错的,因为入口的时候就已经判断好了
                left_node = new BVH_Node(nodes, start_index, mid, t_min, t_max);
                right_node = new BVH_Node(nodes, mid + 1, end_index, t_min, t_max);
            }

            AABB left = null;
            AABB right = null;
            if (left_node != null)
                left_node.GetBoundingBox(t_min, t_max, ref left);
            if (right_node != null)
                right_node.GetBoundingBox(t_min, t_max, ref right);
            if (left != null && right != null)
            {
                aabb = AABB.GetSurroundingBox(left, right);
            }
            else
            {
                aabb = left != null ? left : right;
            }
        }

        public bool Hit(Ray ray, float t_min, float t_max, ref HitRecord record)
        {
            if (aabb != null && aabb.Hit(ray, t_min, t_max))
            {
                HitRecord temp1 = null;
                HitRecord temp2 = null;
                if (left_node != null && right_node != null)
                {
                    bool hit_left = left_node.Hit(ray, t_min, t_max, ref temp1);
                    bool hit_right = right_node.Hit(ray, t_min, t_max, ref temp2);
                    if (hit_left && hit_right)
                    {
                        record = temp1.t < temp2.t ? temp1 : temp2;
                        return true;
                    }
                    else if (hit_left)
                    {
                        record = temp1;
                        return true;
                    }
                    else if (hit_right)
                    {
                        record = temp2;
                        return true;
                    }
                }
                else if (left_node != null)
                {
                    if (left_node.Hit(ray, t_min, t_max, ref record))
                    {
                        return true;
                    }
                }
                else if (right_node != null)
                {
                    if (right_node.Hit(ray, t_min, t_max, ref record))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool GetBoundingBox(float t_min, float t_max, ref AABB _aabb)
        {
            _aabb = aabb;
            return true;
        }

    }

    /// <summary>
    /// 层次包围盒的思想是先检测最外层(root)是否有hit到,hit到的话,root的左右节点取hit得到的record.t最小的作为新的root,继续递归判断.
    /// 复杂点主要在于划分场景中的物品来构造一个比较合理的BVH.(让hit的效率高一点).划分实现放在了BVN_Node的construction里面
    /// 当然AABB作为最基础部分,hit函数也是要牢记原理
    /// 其实这里应该有场景物体创建、销毁、移动的时候,BVH树变化重新计算的各种考虑的,这些等以后更深入再搞吧
    /// </summary>
    public class BVH
    {
        public BVH_Node root { get; private set; }
        private List<Hitable> hitable_list;

        public BVH()
        {
            hitable_list = new List<Hitable>();
        }

        public bool AddHitable(Hitable a)
        {
            AABB temp = null;
            if (a.GetBoundingBox(0, 0, ref temp))
            {
                hitable_list.Add(a);
                return true;
            }
            return false;
        }

        public bool RemoveHitable(Hitable a)
        {
            return hitable_list.Remove(a);
        }

        public void Init(float t_min, float t_max)
        {
            if (hitable_list.Count > 0)
            {
                root = new BVH_Node(hitable_list, 0, hitable_list.Count - 1, t_min, t_max);
            }
        }

        public bool Hit(Ray ray, float t_min, float t_max, ref HitRecord record)
        {
            return root.Hit(ray, t_min, t_max, ref record);
        }
    }
}
