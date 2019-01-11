using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Ray_Tracing
{
    /// <summary>
    /// 搬运自raytracingnextweek
    /// </summary>
    public class Perlin
    {
        private static Perlin _instance;
        public static Perlin Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Perlin();
                return _instance;
            }
        }

        private Vec3[] rand_vec;
        private int[] perm_x;
        private int[] perm_y;
        private int[] perm_z;

        public Perlin()
        {
            rand_vec = GenerateRandVec();
            perm_x = GeneratePerm();
            perm_y = GeneratePerm();
            perm_z = GeneratePerm();
        }

        private float PerlinInterp(Vec3[][][] c, float u, float v, float w)
        {
            float uu = u * u * (3 - 2 * u);
            float vv = v * v * (3 - 2 * v);
            float ww = w * w * (3 - 2 * w);
            float accum = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vec3 weight_v = new Vec3(u - i, v - j, w - k);
                        accum += 
                            (i * uu + (1 - i) * (1 - uu)) * 
                            (j * vv + (1 - j) * (1 - vv)) * 
                            (k * ww + (1 - k) * (1 - ww)) * 
                            Vec3.dot(c[i][j][k], weight_v);
                    }
                }
            }
            return accum;
        }

        private void Permute(ref int[] perm)
        {
            int target;
            int temp;
            for (int i = perm.Length - 1; i > 0; i--)
            {
                target=(int)utils.GenerateRandomNum() * (i + 1);
                temp = perm[i];
                perm[i] = perm[target];
                perm[target] = temp;
            }
        }

        private int[] GeneratePerm()
        {
            int[] perm = new int[256];
            for (int i = 0; i < perm.Length; i++)
            {
                perm[i] = i;
            }
            Permute(ref perm);
            return perm;
        }

        private Vec3[] GenerateRandVec()
        {
            Vec3[] p = new Vec3[256];
            for (int i = 0; i < 256; i++)
            {
                p[i] = new Vec3(-1 + 2 * utils.GenerateRandomNum(), -1 + 2 * utils.GenerateRandomNum(), -1 + 2 * utils.GenerateRandomNum()).normalize();
            }
            return p;
        }

        public float Noise(Vec3 p)
        {
            double u = p.x - Math.Floor(p.x);
            double v = p.y - Math.Floor(p.y);
            double w = p.z - Math.Floor(p.z);
            int i = (int)Math.Floor(p.x);
            int j = (int)Math.Floor(p.y);
            int k = (int)Math.Floor(p.z);
            Vec3[][][] c = new Vec3[2][][];
            for (int m = 0; m < 2; m++)
            {
                c[m] = new Vec3[2][];
            }
            for (int m = 0; m < 2; m++)
            {
                for (int n = 0; n < 2; n++)
                {
                    c[m][n] = new Vec3[2];
                }
            }
            for (int di = 0; di < 2; di++)
            {
                for (int dj = 0; dj < 2; dj++)
                {
                    for (int dk = 0; dk < 2; dk++)
                    {
                        c[di][dj][dk] = rand_vec[perm_x[(i + di) & 255] ^ perm_y[(j + dj) & 255] ^ perm_z[(k + dk) & 255]];
                    }
                }
            }

            return PerlinInterp(c, (float)u, (float)v, (float)w);
        }


    }
}
