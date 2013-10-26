using GeoAPI.Geometries;
//******************************
// Written by Yaguang Li (liyaguang0123@gmail.com)
// Copyright (c) 2013, ISCAS
//
// Use and restribution of this code is subject to the GPL v3.
//******************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrjTools.Tools;

namespace TrjTools.RoadNetwork
{
    /// <summary>
    /// Driving Route
    /// </summary>
    public class Polyline
    {
        //private double length = -1;
        private List<GeoPoint> points;

        public List<GeoPoint> Points
        {
            get
            {
                return points;
            }
        }
        public double Length
        {
            get
            {
                return getLength();
            }
        }

        public double PreciseLength
        {
            get
            {
                return getLength(true);
            }
        }
        public int Count
        {
            get
            {
                return this.Points.Count;
            }
        }

        private MBR? mbr = null;

        public MBR MBR
        {
            get
            {
                if (mbr == null)
                {
                    mbr = getMBR();
                }
                return mbr.Value;
            }
        }
        private MBR getMBR()
        {
            double minLat = double.PositiveInfinity, minLng = double.PositiveInfinity;
            double maxLat = double.NegativeInfinity, maxLng = double.NegativeInfinity;
            int pointCount = points.Count;
            for (int i = 0; i < pointCount; i++)
            {
                minLat = Math.Min(minLat, points[i].Lat);
                minLng = Math.Min(minLng, points[i].Lng);
                maxLat = Math.Max(maxLat, points[i].Lat);
                maxLng = Math.Max(maxLng, points[i].Lng);
            }
            MBR mbr = new MBR(minLng, minLat, maxLng, maxLat);
            return mbr;
        }
        private double getLength(bool isPrecise = false)
        {
            double tmpLen = 0;
            for (int i = 0; i < this.points.Count - 1; i++)
            {
                if (isPrecise)
                {
                    tmpLen += GeoPoint.GetPreciseDistance(points[i], points[i + 1]);
                }
                else
                {
                    tmpLen += GeoPoint.GetDistance(points[i], points[i + 1]);
                }
            }
            return tmpLen;
        }

        public Polyline(List<GeoPoint> points)
        {
            this.points = points;
        }
        /// <summary>
        /// Get projection from certain point
        /// </summary>
        /// <param name="p"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public int ProjectFrom(GeoPoint p, out GeoPoint result, out int segIdx)
        {
            int type = -1;
            double minDist = double.PositiveInfinity;
            GeoPoint tmpResult = GeoPoint.INVALID;
            result = GeoPoint.INVALID;
            segIdx = 0;
            for (int i = 0; i < this.points.Count - 1; i++)
            {
                int tmpType = Polyline.ProjectFrom(points[i], points[i + 1], p, out tmpResult);
                double tmpDist = GeoPoint.GetDistance2(tmpResult, p);

                if (tmpDist <= minDist)
                {
                    if (tmpType == 0 || type != 0)
                    {
                        //good projection is true or tmpType==0
                        type = tmpType;
                        minDist = tmpDist;
                        result = tmpResult;
                        segIdx = i;
                    }
                }
                //break;
            }
            return type;
        }
        public GeoPoint ProjectFrom(GeoPoint p)
        {
            int segIdx;
            GeoPoint result;
            ProjectFrom(p, out result, out segIdx);
            return result;
        }
        public double DistFrom(GeoPoint p, out  int type)
        {
            return Math.Sqrt(Dist2From(p, out type));
        }
        public double Dist2From(GeoPoint p, out  int type)
        {
            GeoPoint projection;
            int segIdx;
            type = ProjectFrom(p, out projection, out segIdx);
            return GeoPoint.GetDistance2(projection, p);
        }
        /// <summary>
        /// Get the distance from a point to this polyline
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double DistFrom(GeoPoint p)
        {
            int type;
            return DistFrom(p, out type);
        }
        /// <summary>
        /// Distance from p to the end of the polyline(by this route)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public double EndDistFrom(GeoPoint p, out int type)
        {
            GeoPoint result;
            int segIdx;
            type = ProjectFrom(p, out result, out segIdx);
            double distance = GeoPoint.GetDistance(p, points[segIdx + 1]);
            for (int i = segIdx + 1; i < points.Count - 1; i++)
            {
                distance += GeoPoint.GetDistance(points[i], points[i + 1]);
            }
            return distance;
        }
        /// <summary>
        /// Get the distance between the projections on the polyline
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public double DistOnLine(GeoPoint from, GeoPoint to)
        {
            GeoPoint fromProject, toProject;
            int fromSegIdx, toSegIdx;
            int fromType = ProjectFrom(from, out fromProject, out fromSegIdx);
            int toType = ProjectFrom(to, out toProject, out toSegIdx);
            double distance = 0;
            //Debug.Assert(fromType == 0 && toType == 0);
            if (fromSegIdx == toSegIdx)
            {
                distance = GeoPoint.GetDistance(fromProject, toProject);
            }
            else
            {
                distance = GeoPoint.GetDistance(fromProject, points[fromSegIdx + 1]);
                for (int i = fromSegIdx + 1; i < toSegIdx; i++)
                {
                    distance += GeoPoint.GetDistance(points[i], points[i + 1]);
                }
                distance += GeoPoint.GetDistance(points[toSegIdx], toProject);
            }
            //distance+=GeoPoint.GetDistance(fromProject,)
            return distance;
        }
        public static int ProjectFrom(GeoPoint start, GeoPoint end, GeoPoint p, out GeoPoint result)
        {
            int type = 0;
            double vY = end.Lat - start.Lat;
            double vX = end.Lng - start.Lng;
            double wY = p.Lat - start.Lat;
            double wX = p.Lng - start.Lng;

            //扭转LAT、LNG比例误差
            double vY_m = vY * Constants.M_PER_LAT;	//
            double vX_m = vX * Constants.M_PER_LNG;	//
            double wY_m = wY * Constants.M_PER_LAT;
            double wX_m = wX * Constants.M_PER_LNG;

            double bY, bX;

            double c1 = wY_m * vY_m + wX_m * vX_m;
            double c2 = vY_m * vY_m + vX_m * vX_m;

            result = GeoPoint.INVALID;

            if (c1 <= 0)
            {
                //when the given point is left of the source point
                result = start;
            }
            else if (c2 <= c1)
            {
                // when the given point is right of the target point
                result = end;
            }
            else //between the source point and target point
            {
                double b = c1 / c2;
                bY = start.Lat + b * vY;
                bX = start.Lng + b * vX;
                result = new GeoPoint(bY, bX);
            }
            type = (short)(c1 / c2);
            return type;
        }

        public static double DistFrom(GeoPoint start, GeoPoint end, GeoPoint p)
        {
            GeoPoint result;
            ProjectFrom(start, end, p, out result);
            double distance = 0;
            distance = GeoPoint.GetDistance(p, result);
            return distance;
        }

        /// <summary>
        /// Calculate the cosine value with line p1,p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double CosWith(GeoPoint p1, GeoPoint p2)
        {
            double wY, wX;
            double vY, vX;
            GeoPoint start, end;
            int startIdx;
            ProjectFrom(p1, out start, out startIdx);
            start = this.Points[startIdx];
            end = this.Points[startIdx + 1];
            vY = Utility.refineDoubleZero(end.Lat - start.Lat);
            vX = Utility.refineDoubleZero(end.Lng - start.Lng);
            wY = Utility.refineDoubleZero(p2.Lat - p1.Lat);
            wX = Utility.refineDoubleZero(p2.Lng - p1.Lng);
            double sum = vY * wY + vX * wX;
            double result = sum / Math.Sqrt(1.0 * (vY * vY + vX * vX) * (wY * wY + wX * wX));
            return result;
        }

        public GeoPoint Predict(GeoPoint start, double distance)
        {
            GeoPoint projStart;
            GeoPoint target = this.Points.Last();
            int segIdx = 0;
            int startType = ProjectFrom(start, out projStart, out segIdx);
            double currentDistance = 0;
            while (segIdx < this.Points.Count - 1)
            {
                double length = GeoPoint.GetDistance(start, Points[segIdx + 1]);
                if (currentDistance + length >= distance)
                {
                    double leftLength = distance - currentDistance;
                    double ratio = leftLength / length;
                    double lat = start.Lat + ratio * (Points[segIdx + 1].Lat - start.Lat);
                    double lng = start.Lng + ratio * (Points[segIdx + 1].Lng - start.Lng);
                    target = new GeoPoint(lat, lng);
                    break;
                }
                else
                {
                    currentDistance += length;
                    start = Points[segIdx + 1];
                    segIdx++;
                }
            }
            return target;
        }

        public Coordinate[] ToCoordinateArray()
        {
            Coordinate[] coords = new Coordinate[Count];
            for(int i = 0; i < Count; ++i)
            {
                coords[i] = points[i].ToCoordinate();
            }
            return coords;
        }
    }
}
