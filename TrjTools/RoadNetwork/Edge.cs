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
using System.Diagnostics;

namespace TrjTools.RoadNetwork
{
    /// <summary>
    /// 普通道路
    /// </summary>
    public class Edge
    {
        #region fields
        long id;

        public long ID
        {
            get { return id; }
        }
        Vertex start;

        public Vertex Start
        {
            get { return start; }
        }
        Vertex end;

        public Vertex End
        {
            get { return end; }
        }

        double length = -1;

        public double Length
        {
            get
            {
                if (length < 0)
                {
                    lock (syncRoot)
                    {
                        if (length < 0)
                        {
                            length = getLength();
                        }
                    }
                }
                return length;
            }
        }

        public MBR MBR
        {
            get
            {
                return Geo.MBR;
            }
        }

        /// <summary>
        /// Polyline that represents the geometry of the edge
        /// </summary>
        private Polyline geo = null;

        private String geoString = null;

        public String GeoString
        {
            set { geoString = value; }
        }

        public Polyline Geo
        {
            get
            {
                if (geo == null)
                {
                    //for multiple 
                    lock (syncRoot)
                    {
                        if (geo == null)
                        {
                            List<GeoPoint> points = new List<GeoPoint>();
                            if (String.IsNullOrEmpty(geoString))
                            {
                                points.Add(this.start.ToPoint());
                                points.Add(this.end.ToPoint());
                            }
                            else
                            {
                                String[] fields = geoString.Split('\t');
                                Debug.Assert(fields.Length % 2 == 0);
                                for (int i = 0; i < fields.Length; i += 2)
                                {
                                    double lat = double.Parse(fields[i]);
                                    double lng = double.Parse(fields[i + 1]);
                                    points.Add(new GeoPoint(lat, lng));
                                }
                                this.geoString = null;
                            }
                            geo = new Polyline(points);
                        }
                    }
                }
                return geo;
            }
            set { geo = value; }
        }

        private readonly Object syncRoot = new Object();
        private List<Edge> outEdges = null;

        public List<Edge> OutEdges
        {
            get
            {
                if (outEdges == null)
                {
                    //thread safe
                    outEdges = this.End.OutEdges;
                }
                return outEdges;
            }
        }

        private List<Edge> inEdges = null;

        public List<Edge> InEdges
        {
            get
            {
                if (inEdges == null)
                {
                    //thread safe
                    inEdges = this.Start.InEdges;
                }
                return inEdges;
            }
        }


        #endregion fields

        #region methods
        private double getLength()
        {
            //double len = GeoPoint.GetDistance(start.ToPoint(), end.ToPoint());
            double len = Geo.Length;
            return len;
        }
        public Edge(long id, Vertex start, Vertex end)
        {
            this.id = id;
            this.start = start;
            this.end = end;
        }
        public Edge(long id, Vertex start, Vertex end, double length)
        {
            this.id = id;
            this.start = start;
            this.end = end;
            this.length = length;
        }
        public Edge(long id, Vertex start, Vertex end, double length, double speedLimit, int type)
        {
            this.id = id;
            this.start = start;
            this.end = end;
            this.length = length;
            //this.speedLimit = speedLimit;
            //this.type = type;
        }

        /// <summary>
        /// Get the projection from a certain point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public GeoPoint projectFrom(GeoPoint p)
        {
            GeoPoint result = Geo.ProjectFrom(p);
            return result;
        }
        public int getSegmentID(GeoPoint point, long edgeID, Graph graph)
        {
            Edge e = null;
            int segmentID = -1;
            if (graph.Edges.TryGetValue(edgeID, out e))
            {
                GeoPoint result;
                e.Geo.ProjectFrom(point, out result, out segmentID);
            }
            return segmentID;
        }
        public int projectFrom(GeoPoint p, out GeoPoint result)
        {
            int segIdx = 0;
            int type = Geo.ProjectFrom(p, out result, out segIdx);
            return type;
        }
        /// <summary>
        /// Get the distance from a point to the edge
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double Dist2From(GeoPoint p)
        {
            int type = 0;
            return Geo.Dist2From(p, out type);
        }

        public double Dist2From(GeoPoint p, out int type)
        {
            return Geo.Dist2From(p, out type);
        }
        /// <summary>
        /// Get the distance from a point to the edge
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double DistFrom(GeoPoint p)
        {
            return Math.Sqrt(Dist2From(p));
        }
        public double DistFrom(GeoPoint p, out int type)
        {
            return Math.Sqrt(Dist2From(p, out type));
        }

        public GeoPoint Predict(GeoPoint start, double distance)
        {
            return this.Geo.Predict(start, distance);
        }
        public double EndDistFrom(GeoPoint p, out int type)
        {
            return this.Geo.EndDistFrom(p, out type);
        }
        /// <summary>
        /// Predict the position from start after distance on this route
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public double DistOnLine(GeoPoint from, GeoPoint to)
        {
            return this.Geo.DistOnLine(from, to);
        }
        /// <summary>
        /// Calculate the cosine value with line p1,p2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double CosWith(GeoPoint p1, GeoPoint p2)
        {
            return Geo.CosWith(p1, p2);
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj != null && obj is Edge)
            {
                result = (obj as Edge).ID == this.ID;
            }
            return result;
        }
        public override int GetHashCode()
        {
            return (int)this.ID ^ (int)(this.ID >> 32);
        }
        public override string ToString()
        {
            return String.Format("ID:{0},{1}->{2}", this.ID, this.Start.ID, this.end.ID);
        }
        #endregion methods
    }
}
