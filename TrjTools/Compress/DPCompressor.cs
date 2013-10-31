using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TrjTools.RoadNetwork;

namespace TrjTools.Compress
{
    public class DPCompressor
    {
        /// <summary>
        /// Max allowed deviation between the refined trajectory  and the original one
        /// </summary>
        private double maxDev = 25;
        private Trajectory trj = null;

        public DPCompressor(Trajectory trj, double maxDev)
        {
            this.trj = trj;
            this.maxDev = maxDev;
        }
        public Trajectory Compress()
        {
            Trajectory refinedTrj = new Trajectory();
            int trjSize = trj.Count;
            int i = 0;
            while (i < trjSize - 1)
            {
                refinedTrj.Add(trj[i]);
                int j = i + 1;
                for (; j < trjSize; ++j)
                {
                    if (!canApproximate(i, j))
                    {
                        break;
                    }
                }
                i = j - 1;
            }
            refinedTrj.Add(trj[trjSize - 1]);
            return refinedTrj;
        }
        /// <summary>
        /// Check if the line formed by startIdx.point and endIdx.point can represents internal motion vectors
        /// </summary>
        /// <param name="startIdx"></param>
        /// <param name="endIdx"></param>
        /// <returns></returns>
        private bool canApproximate(int startIdx, int endIdx)
        {
            bool result = true;
            if (endIdx - startIdx <= 1)
            {
                return result;
            }
            MotionVector start = trj[startIdx], end = trj[endIdx];
            GeoPoint pStart = start.point, pEnd = end.point;
            double baseLat, baseLng;
            baseLat = (pEnd.Lat - pStart.Lat) / (end.t - start.t);
            baseLng = (pEnd.Lng - pStart.Lng) / (end.t - start.t);
            for (int i = startIdx + 1; i < endIdx; i++)
            {
                double lat, lng;
                double dev2;
                MotionVector middle = trj[i];
                lat = baseLat * (middle.t - start.t) + pStart.Lat;
                lng = baseLng * (middle.t - start.t) + pStart.Lng;
                dev2 = GeoPoint.GetDistance2(new GeoPoint(lat, lng), middle.point);
                //Debug.Assert(middle.e != null);
                if (dev2 > maxDev * maxDev)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        private double getDev(MotionVector start, MotionVector end, MotionVector middle)
        {
            Debug.Assert(end.t > start.t && middle.t > start.t && middle.t < end.t);
            GeoPoint pStart = start.point, pEnd = end.point;
            double lat, lng;
            lat = (pEnd.Lat - pStart.Lat) / (end.t - start.t) * (middle.t - start.t);
            lng = (pEnd.Lng - pStart.Lng) / (end.t - start.t) * (middle.t - start.t);
            double dev = GeoPoint.GetDistance(new GeoPoint(lat, lng), middle.point);
            return dev;
        }
    }
}
