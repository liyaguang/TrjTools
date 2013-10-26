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
    /// Motion vector (lat,lng,t)
    /// </summary>
    public struct MotionVector
    {
        public const int TICKS_PER_SECOND = 10000000;
        public enum MatchType
        {
            NoneMatched = 0,
            SingleMatched = 1,
            MultiMatched = 2
        }
        /// <summary>
        /// Time represented by seconds
        /// </summary>
        public long t;
        public GeoPoint point;
        public float v;
        //public int direction;
        public Edge e;
        public String orginalString;
        public long EdgeId
        {
            get
            {
                long edgeId = 0;
                if (e != null)
                {
                    edgeId = e.ID;
                }
                return edgeId;
            }
        }
        public MatchType type;
        //public HashSet<Edge> candidateEdges;
        public MotionVector(double lat, double lng, String t, double v, int direction)
        {
            this.t = DateTime.Parse(t).Ticks / TICKS_PER_SECOND;
            this.point = new GeoPoint(lat, lng);
            this.v = (float)v;
            this.e = null;
            //this.candidateEdges = new HashSet<Edge>();
            this.type = MatchType.NoneMatched;
            this.orginalString = "";
        }
        public MotionVector(GeoPoint p, long t)
        {
            this.point = p;
            this.t = t;
            this.v = 0;
            //this.candidateEdges = new HashSet<Edge>();
            this.type = MatchType.NoneMatched;
            this.e = null;
            this.orginalString = "";
        }
        public override string ToString()
        {
            return String.Format("{0}:{1},{2}", this.Time.ToString("HH:mm:ss"), this.point, this.e);
        }
        /// <summary>
        /// Readable version of the time
        /// </summary>
        public DateTime Time
        {
            get
            {
                return new DateTime(this.t * TICKS_PER_SECOND);
            }
        }
    }
}
