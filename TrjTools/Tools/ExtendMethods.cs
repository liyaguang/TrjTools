//******************************
// Written by Yaguang Li (liyaguang0123@gmail.com)
// Copyright (c) 2013, ISCAS
//
// Use and restribution of this code is subject to the GPL v3.
//******************************
using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using TrjTools.RoadNetwork;

namespace TrjTools.Tools
{
    public static class ExtendMethods
    {
        #region Extend Methods
        public static Point ToPoint(this GeoPoint p)
        {
            return new Point(p.Lng, p.Lat);
        }
        public static Coordinate ToCoordinate(this GeoPoint p)
        {
            return new Coordinate(p.Lng, p.Lat);
        }
        public static LineString ToLineString(this Trajectory trj)
        {
            if (trj.Count < 2) return null;
            List<Coordinate> coords = new List<Coordinate>();
            foreach (MotionVector p in trj)
            {
                coords.Add(p.point.ToCoordinate());
            }
            LineString route = new LineString(coords.ToArray());
            return route;
        }
        public static LineString ToLineString(this Edge e)
        {
            LineString route = new LineString(e.Geo.ToCoordinateArray());
            return route;
        }
        /// <summary>
        /// Get the minimum bounding rectangle
        /// </summary>
        /// <returns></returns>
        //public static Rectangle GetMBR(this MVTuple tuple)
        //{
        //    MBR mbr = MBR.EMPTY;
        //    mbr.Include(tuple.Start.point);
        //    mbr.Include(tuple.End.point);
        //    if (tuple.Path.Count > 1)
        //    {
        //        //have additonal edge
        //        for (int i = 1; i < tuple.Path.Count; i++)
        //        {
        //            mbr.UnionWith(tuple.Path[i].MBR);
        //        }
        //    }
        //    double[] min = new double[] { mbr.min[0], mbr.min[1], tuple.Start.t };
        //    double[] max = new double[] { mbr.max[0], mbr.max[1], tuple.End.t };
        //    Rectangle rect = new Rectangle(min, max);
        //    return rect;
        //}
        public static Envelope GetBox(this Edge e)
        {
            double minX, minY, maxX, maxY;
            Vertex From = e.Start;
            Vertex To = e.End;
            minX = Math.Min(From.Lng, To.Lng);
            maxX = Math.Max(From.Lng, To.Lng);
            minY = Math.Min(From.Lat, To.Lat);
            maxY = Math.Max(From.Lat, To.Lat);
            Envelope box = new Envelope(minX, maxX, minY, maxY);
            return box;
        }

        /// <summary>
        /// 合并两box
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static Envelope Merge(this Envelope b1, Envelope b2)
        {
            Envelope result;
            double minX, minY, maxX, maxY;
            minX = Math.Min(b1.MinX, b2.MinX);
            minY = Math.Min(b1.MinY, b2.MinY);
            maxX = Math.Max(b1.MaxX, b2.MaxX);
            maxY = Math.Max(b1.MaxY, b2.MaxY);
            //result = new Envelope(minX, minY, maxX, maxY);
            result = new Envelope(minX, maxX, minY, maxY);
            return result;
        }

        /// <summary>
        /// 扩充box使其包容点
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public static Envelope Merge(this Envelope b, Point p)
        {
            Envelope result;
            double minX, minY, maxX, maxY;
            minX = Math.Min(b.MinX, p.X);
            minY = Math.Min(b.MinY, p.Y);
            maxX = Math.Max(b.MaxX, p.X);
            maxY = Math.Max(b.MaxY, p.Y);
            result = new Envelope(minX, maxX, minY, maxY);
            return result;
        }
        public static Envelope Magnify(this Envelope b, double rate)
        {
            Envelope result;
            double minX, minY, maxX, maxY;
            double width, height;
            double r = (rate - 1) / 2.0;
            width = b.MaxX - b.MinX;
            height = b.MaxY - b.MinY;

            minX = b.MinX - r * width;
            maxX = b.MaxX + r * width;
            minY = b.MinY - r * height;
            maxY = b.MaxY + r * height;

            result = new Envelope(minX, maxX, minY, maxY);
            //b = result;
            return result;
        }
        public static IGeometry TransformGeometry(this GeometryTransform gt, IGeometry g, IMathTransform transform)
        {
            return null;
        }

        public static List<Trajectory> Truncate(this Trajectory trj, Envelope box)
        {
            List<Trajectory> trjs = new List<Trajectory>();
            Trajectory subTrj = new Trajectory();
            int minCount = 128;
            for (int i = 0; i < trj.Count; ++i)
            {
                if (box.Contains(trj[i].point.ToCoordinate()))
                {
                    subTrj.Add(trj[i]);
                }
                else
                {
                    if (subTrj.Count > minCount)
                    {
                        trjs.Add(subTrj);
                    }
                    subTrj = new Trajectory();
                }
            }
            if (subTrj.Count > minCount)
            {
                trjs.Add(subTrj);
            }
            return trjs;
        }
        #endregion Extend Methods
    }
}
