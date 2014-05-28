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
    /// Minmum bounding rectangle
    /// </summary>
    public struct MBR
    {
        #region variables
        /// <summary>
        /// Number of dimensions in a rectangle.
        /// </summary>
        internal const int DIMENSIONS = 2;

        private const int LAT_IDX = 1;
        private const int LNG_IDX = 0;

        /// <summary>
        /// array containing the minimum value for each dimension; ie { min(lng), min(lat) }
        /// </summary>
        internal double[] max;

        /// <summary>
        /// array containing the maximum value for each dimension; ie { max(lng), max(lat) }
        /// </summary>
        internal double[] min;

        #endregion variables

        #region methods

        #region static variables
        public static MBR EMPTY
        {
            get
            {
                return new MBR(double.PositiveInfinity, double.PositiveInfinity, double.NegativeInfinity, double.NegativeInfinity);
            }
        }
        public static MBR ALL
        {
            get
            {
                return new MBR(double.NegativeInfinity, double.NegativeInfinity, double.PositiveInfinity, double.PositiveInfinity);
            }
        }
        #endregion static variables
        public MBR(double minLng, double minLat, double maxLng, double maxLat)
        {
            this.max = new double[] { maxLng, maxLat };
            this.min = new double[] { minLng, minLat };
        }

        public void UnionWith(MBR mbr)
        {
            this.min[LAT_IDX] = Math.Min(mbr.min[LAT_IDX], this.min[LAT_IDX]);
            this.max[LAT_IDX] = Math.Max(mbr.max[LAT_IDX], this.max[LAT_IDX]);
            this.min[LNG_IDX] = Math.Min(mbr.min[LNG_IDX], this.min[LNG_IDX]);
            this.max[LNG_IDX] = Math.Max(mbr.max[LNG_IDX], this.max[LNG_IDX]);
        }
        public void Include(GeoPoint p)
        {
            this.min[LAT_IDX] = Math.Min(p.Lat, this.min[LAT_IDX]);
            this.max[LAT_IDX] = Math.Max(p.Lat, this.max[LAT_IDX]);
            this.min[LNG_IDX] = Math.Min(p.Lng, this.min[LNG_IDX]);
            this.max[LNG_IDX] = Math.Max(p.Lng, this.max[LNG_IDX]);
        }
        bool contain(MBR mbr)
        {
            //return ((m.minx >= minx) && (m.maxx <= maxx) && (m.miny >= miny) && (m.maxy <= maxy));
            bool result = (this.min[LAT_IDX] <= mbr.min[LAT_IDX]) &&
                (this.min[LNG_IDX] <= mbr.min[LNG_IDX]) &&
                (this.max[LAT_IDX] >= mbr.max[LAT_IDX]) &&
                (this.max[LNG_IDX] >= mbr.max[LAT_IDX]);
            return result;
        }
        public bool Cover(GeoPoint p)
        {
            bool inside = false;
            if (p.Lat >= this.min[LAT_IDX]
                && p.Lat <= this.max[LAT_IDX]
                && p.Lng >= this.min[LNG_IDX]
                && p.Lng <= this.max[LNG_IDX])
            {
                inside = true;
            }
            return inside;
        }
        public override string ToString()
        {
            return String.Format("min({0},{1}),max({2},{3})",
                this.min[LNG_IDX], this.min[LAT_IDX], this.max[LNG_IDX], this.max[LAT_IDX]);
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is MBR)
            {
                MBR tmp = (obj as MBR?).Value;
                result = (tmp.MaxLat == MaxLat && tmp.MaxLng == MaxLng
                    && tmp.MinLat == MinLat && tmp.MinLng == MinLng);
            }
            return result;
        }
        public override int GetHashCode()
        {
            return MinLat.GetHashCode() ^ MinLng.GetHashCode() ^ MaxLat.GetHashCode() ^ MaxLng.GetHashCode();
        }
        #endregion methods

        #region Properties
        /// <summary>
        /// Return the height(maxLat-minLat)
        /// </summary>
        public double Height
        {
            get
            {
                return this.max[LAT_IDX] - this.min[LAT_IDX];
            }
        }
        /// <summary>
        /// Return the height(maxLng-minLng)
        /// </summary>
        public double Width
        {
            get
            {
                return this.max[LNG_IDX] - this.min[LNG_IDX];
            }
        }

        public double MinLat
        {
            get
            {
                return min[LAT_IDX];
            }
        }
        public double MaxLat
        {
            get
            {
                return max[LAT_IDX];
            }
        }
        public double MinLng
        {
            get
            {
                return min[LNG_IDX];
            }
        }
        public double MaxLng
        {
            get
            {
                return max[LNG_IDX];
            }
        }
        public GeoPoint TopLeft
        {
            get
            {
                return new GeoPoint(min[LAT_IDX], min[LNG_IDX]);
            }
        }
        public GeoPoint TopRight
        {
            get
            {
                return new GeoPoint(min[LAT_IDX], max[LNG_IDX]);
            }
        }
        public GeoPoint BottomLeft
        {
            get
            {
                return new GeoPoint(max[LAT_IDX], min[LNG_IDX]);
            }
        }
        public GeoPoint BottomRight
        {
            get
            {
                return new GeoPoint(max[LAT_IDX], max[LNG_IDX]);
            }
        }
        #endregion Properties
    }
}
