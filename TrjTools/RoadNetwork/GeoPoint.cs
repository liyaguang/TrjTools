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

namespace TrjTools.RoadNetwork
{
    /// <summary>
    /// A point with latitude and longtitude
    /// </summary>
    public struct GeoPoint
    {
        private const double DIVISOR = 10000000;
        public static GeoPoint INVALID = new GeoPoint(-1, -1);

        private int _lat;
        public double Lat
        {
            get
            {
                return _lat / DIVISOR;
            }
            set
            {
                _lat = (int)(value * DIVISOR);
            }
        }
        private int _lng;
        public double Lng
        {
            get
            {
                return _lng / DIVISOR;
            }
            set
            {
                _lng = (int)(value * DIVISOR);
            }
        }

        public GeoPoint(double lat, double lng)
        {
            this._lat = (int)(lat * DIVISOR);
            this._lng = (int)(lng * DIVISOR);
        }
        public bool IsValid
        {
            get
            {
                bool result = true;
                if (this.Lat == -1 && this.Lng == -1)
                {
                    result = false;
                }
                return result;
            }
        }
        public override string ToString()
        {
            return String.Format("({0},{1})", Lng, Lat);
        }
        #region static fields
        public const int M_PER_LAT = 110000;
        public const int M_PER_LNG = 70000;
        #endregion static fields

        #region static methods
        /// <summary>
        /// Get the approximated distance 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetDistance(GeoPoint p1, GeoPoint p2)
        {
            return Math.Sqrt(GetDistance2(p1, p2));
        }
        /// <summary>
        /// Get the square of actual distance, expected to be a little faster...
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetDistance2(GeoPoint p1, GeoPoint p2)
        {
            double height = Math.Abs(p2.Lat - p1.Lat) * M_PER_LAT; //110km per latitude
            double width = Math.Abs(p2.Lng - p1.Lng) * M_PER_LNG;	//70km per longitude
            return height * height + width * width;
        }
        public static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        /// <summary>
        /// Get the precise distance between to geo points
        /// </summary>
        /// <param name="pA"></param>
        /// <param name="pB"></param>
        /// <returns></returns>
        public static double GetPreciseDistance(GeoPoint pA, GeoPoint pB)
        {
            double latA = pA.Lat, lngA = pA.Lng;
            double latB = pB.Lat, lngB = pB.Lng;
            double radLatA = rad(latA);
            double radLatB = rad(latB);
            double a = radLatA - radLatB;
            double b = rad(lngA) - rad(lngB);
            double distance = 2 * Math.Asin(Math.Sqrt(
                Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLatA) * Math.Cos(radLatB) * Math.Pow(Math.Sin(b / 2), 2)));
            distance = distance * 6378137.0;
            distance = (int)(distance * 10000) / 10000;
            return distance;
        }
        #endregion static methods
    }
}
