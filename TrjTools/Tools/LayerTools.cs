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
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using BruTile;
using BruTile.Cache;
using BruTile.Web.Wms;
using SharpMap.Layers;
using TrjTools.RoadNetwork;

namespace TrjTools.Tools
{
    public static class LayerTools
    {
        private static readonly string MAP_CACHE_NAME = "..\\..\\MapCache.dat";
        private static ICoordinateTransformation wgs84toGoogle;
        private static ICoordinateTransformation googletowgs84;

        /// <summary>
        /// Wgs84 to Google Mercator Coordinate Transformation
        /// </summary>
        public static ICoordinateTransformation Wgs84toGoogleMercator
        {
            get
            {
                if (wgs84toGoogle == null)
                {
                    CoordinateSystemFactory csFac = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
                    CoordinateTransformationFactory ctFac = new CoordinateTransformationFactory();

                    IGeographicCoordinateSystem wgs84 = csFac.CreateGeographicCoordinateSystem(
                      "WGS 84", AngularUnit.Degrees, HorizontalDatum.WGS84, PrimeMeridian.Greenwich,
                      new AxisInfo("north", AxisOrientationEnum.North), new AxisInfo("east", AxisOrientationEnum.East));

                    List<ProjectionParameter> parameters = new List<ProjectionParameter>();
                    parameters.Add(new ProjectionParameter("semi_major", 6378137.0));
                    parameters.Add(new ProjectionParameter("semi_minor", 6378137.0));
                    parameters.Add(new ProjectionParameter("latitude_of_origin", 0.0));
                    parameters.Add(new ProjectionParameter("central_meridian", 0.0));
                    parameters.Add(new ProjectionParameter("scale_factor", 1.0));
                    parameters.Add(new ProjectionParameter("false_easting", 0.0));
                    parameters.Add(new ProjectionParameter("false_northing", 0.0));
                    IProjection projection = csFac.CreateProjection("Google Mercator", "mercator_1sp", parameters);

                    IProjectedCoordinateSystem epsg900913 = csFac.CreateProjectedCoordinateSystem(
                      "Google Mercator", wgs84, projection, LinearUnit.Metre, new AxisInfo("East", AxisOrientationEnum.East),
                      new AxisInfo("North", AxisOrientationEnum.North));

                    ((CoordinateSystem)epsg900913).DefaultEnvelope = new[] { -20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789 };
                    wgs84toGoogle = ctFac.CreateFromCoordinateSystems(wgs84, epsg900913);
                }
                return wgs84toGoogle;
            }
        }


        public static ICoordinateTransformation GoogleMercatorToWgs84
        {
            get
            {
                if (googletowgs84 == null)
                {
                    CoordinateSystemFactory csFac = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
                    CoordinateTransformationFactory ctFac = new CoordinateTransformationFactory();
                    IGeographicCoordinateSystem wgs84 = csFac.CreateGeographicCoordinateSystem(
                      "WGS 84", AngularUnit.Degrees, HorizontalDatum.WGS84, PrimeMeridian.Greenwich,
                      new AxisInfo("north", AxisOrientationEnum.North), new AxisInfo("east", AxisOrientationEnum.East));

                    List<ProjectionParameter> parameters = new List<ProjectionParameter>();
                    parameters.Add(new ProjectionParameter("semi_major", 6378137.0));
                    parameters.Add(new ProjectionParameter("semi_minor", 6378137.0));
                    parameters.Add(new ProjectionParameter("latitude_of_origin", 0.0));
                    parameters.Add(new ProjectionParameter("central_meridian", 0.0));
                    parameters.Add(new ProjectionParameter("scale_factor", 1.0));
                    parameters.Add(new ProjectionParameter("false_easting", 0.0));
                    parameters.Add(new ProjectionParameter("false_northing", 0.0));
                    IProjection projection = csFac.CreateProjection("Google Mercator", "mercator_1sp", parameters);

                    IProjectedCoordinateSystem epsg900913 = csFac.CreateProjectedCoordinateSystem(
                      "Google Mercator", wgs84, projection, LinearUnit.Metre, new AxisInfo("East", AxisOrientationEnum.East),
                      new AxisInfo("North", AxisOrientationEnum.North));

                    googletowgs84 = ctFac.CreateFromCoordinateSystems(epsg900913, wgs84);
                }

                return googletowgs84;

            }
        }



        public static Point WgsToGoogle(Point p)
        {
            GeometryFactory factory = new NetTopologySuite.Geometries.GeometryFactory();
            Point result = GeometryTransform.TransformGeometry(p, LayerTools.wgs84toGoogle.MathTransform, factory) as Point;
            return result;
        }

        public static Point GoogleToWgs(Point p)
        {
            GeometryFactory factory = new NetTopologySuite.Geometries.GeometryFactory();
            Point result = GeometryTransform.TransformGeometry(p, LayerTools.GoogleMercatorToWgs84.MathTransform, factory) as Point;
            return result;
        }

        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        /// <summary>
        /// Get the distance between two points marked by (lat,lon)
        /// </summary>
        /// <param name="latX"></param>
        /// <param name="lonX"></param>
        /// <param name="latY"></param>
        /// <param name="lonY"></param>
        /// <returns></returns>
        public static double GetDistance(double lonA, double latA, double lonB, double latB)
        {
            double radLatA = Rad(latA);
            double radLatB = Rad(latB);
            double a = radLatA - radLatB;
            double b = Rad(lonA) - Rad(lonB);
            double s = 2 * Math.Asin(Math.Sqrt(
                Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLatA) * Math.Cos(radLatB) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * 6378137.0;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        public static double GetDistance(GeoPoint p1, GeoPoint p2)
        {
            return GetDistance(p1.Lng, p1.Lat, p2.Lng, p2.Lat);
        }
        public static double GetDistance(Point p1, Point p2)
        {
            return GetDistance(p1.X, p1.Y, p2.X, p2.Y);
        }
        public static double GetLength(LineString line)
        {
            double length = 0;
            for (int i = 1; i < line.Count; i++)
            {
                length += GetDistance(line.Coordinates[i - 1].X, line.Coordinates[i - 1].Y, line.Coordinates[i].X, line.Coordinates[i].Y);
            }
            return length;
        }

        public static Point BeijingToWgs(Point p)
        {
            return BeijingToWgs(p.X, p.Y);
        }
        public static Point BeijingToWgs(double xx, double yy)
        {
            int base_x = 1161250000;
            int base_y = 397500000;
            int base_m = 10000000;
            double x, y;
            x = (xx + base_x) / base_m;
            y = (yy + base_y) / base_m;
            return new Point(x, y);
        }
        

    }
}
