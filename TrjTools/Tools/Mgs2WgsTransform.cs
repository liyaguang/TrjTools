using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrjTools.RoadNetwork;

namespace TrjTools.Tools
{
    public class Mgs2WgsTransform : IGeoTransformer
    {
        public Mgs2WgsTransform(String mapFileName)
        {
            this.mapFileName = mapFileName;
            Initialize();
        }
        private String mapFileName = null;
        double[] lngTable = new double[660 * 450];
        double[] latTable = new double[660 * 450];
        bool InitTable = false;

        public GeoPoint Transform(GeoPoint point)
        {
            double lat, lng;
            Parse(point.Lng, point.Lat, out lng, out lat);
            return new GeoPoint(lat, lng);
        }
        private int GetID(int I, int J)
        {
            return I + 660 * J;
        }

        private void Initialize()
        {
            using (StreamReader sr = new StreamReader(mapFileName))
            {
                string s = sr.ReadToEnd();
                Match MP = Regex.Match(s, "(\\d+)");
                int i = 0;
                while (MP.Success)
                {
                    if (i % 2 == 0)
                    {
                        lngTable[i / 2] = Convert.ToDouble(MP.Value) / 100000.0;
                    }
                    else
                    {
                        latTable[(i - 1) / 2] = Convert.ToDouble(MP.Value) / 100000.0;
                    }
                    i++;
                    MP = MP.NextMatch();
                }
                InitTable = true;
            }
        }

        /// <summary>
        /// x是117左右，y是31左右
        /// </summary>
        /// <param name="xMars"></param>
        /// <param name="yMars"></param>
        /// <param name="xWgs"></param>
        /// <param name="yWgs"></param>
        private void Parse(double xMars, double yMars, out double xWgs, out double yWgs)
        {
            int i, j, k;
            double x1, y1, x2, y2, x3, y3, x4, y4, xtry, ytry, dx, dy;
            double t, u;
            xWgs = xMars;
            yWgs = yMars;
            if (!InitTable) return;

            xtry = xMars;
            ytry = yMars;

            for (k = 0; k < 10; ++k)
            {
                // 只对中国国境内数据转换
                if (outOfChina(ytry, xtry)) return;

                i = (int)((xtry - 72.0) * 10.0);
                j = (int)((ytry - 10.0) * 10.0);

                x1 = lngTable[GetID(i, j)];
                y1 = latTable[GetID(i, j)];
                x2 = lngTable[GetID(i + 1, j)];
                y2 = latTable[GetID(i + 1, j)];
                x3 = lngTable[GetID(i + 1, j + 1)];
                y3 = latTable[GetID(i + 1, j + 1)];
                x4 = lngTable[GetID(i, j + 1)];
                y4 = latTable[GetID(i, j + 1)];

                t = (xtry - 72.0 - 0.1 * i) * 10.0;
                u = (ytry - 10.0 - 0.1 * j) * 10.0;

                dx = (1.0 - t) * (1.0 - u) * x1 + t * (1.0 - u) * x2 + t * u * x3 + (1.0 - t) * u * x4 - xtry;
                dy = (1.0 - t) * (1.0 - u) * y1 + t * (1.0 - u) * y2 + t * u * y3 + (1.0 - t) * u * y4 - ytry;

                xtry = (xtry + xMars - dx) / 2.0;
                ytry = (ytry + yMars - dy) / 2.0;
            }

            xWgs = xtry;
            yWgs = ytry;

        }
        static bool outOfChina(double lat, double lon)
        {
            if (lon < 72.004 || lon > 137.8347)
                return true;
            if (lat < 0.8293 || lat > 55.8271)
                return true;
            return false;
        }

    }
}