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
using GeoAPI.Geometries;

namespace TrjTools.RoadNetwork
{
    /// <summary>
    /// Vertex of graph or road network
    /// </summary>
    public class Vertex
    {
        #region fields

        long id;

        public long ID
        {
            get { return id; }
        }

        GeoPoint point;

        public double Lat
        {
            get { return point.Lat; }
        }

        public double Lng
        {
            get { return point.Lng; }
        }
        private readonly Object syncRoot = new Object();
        private List<Edge> adjacentEdges = new List<Edge>();

        private List<Edge> outEdges = null;
        private List<Edge> inEdges = null;

        public List<Edge> OutEdges
        {
            get
            {
                if (outEdges == null)
                {
                    calculateInOut();
                }
                Debug.Assert(outEdges != null);
                return outEdges;
            }
        }
        public List<Edge> InEdges
        {
            get
            {
                if (inEdges == null)
                {
                    calculateInOut();
                }
                Debug.Assert(inEdges != null);
                return inEdges;
            }
        }
        #endregion fields
        public Vertex(long id, double lat, double lng)
        {
            this.id = id;
            this.point = new GeoPoint(lat, lng);
        }
        private void calculateInOut()
        {
            lock (syncRoot)
            {
                if (outEdges == null)
                {
                    int edgeSize = adjacentEdges.Count;
                    outEdges = new List<Edge>();
                    inEdges = new List<Edge>();
                    for (int i = 0; i < edgeSize; i++)
                    {
                        if (adjacentEdges[i].Start == this)
                        {
                            outEdges.Add(adjacentEdges[i]);
                        }
                        else
                        {
                            inEdges.Add(adjacentEdges[i]);
                        }
                    }
                }
            }

        }
        public override bool Equals(object obj)
        {
            if (obj is Vertex)
            {
                return (obj as Vertex).ID == this.ID;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return (int)this.ID ^ (int)(this.ID >> 32);
        }
        public void RegisterEdge(Edge e)
        {
            Debug.Assert(e.Start == this || e.End == this);
            lock (syncRoot)
            {
                this.adjacentEdges.Add(e);
            }
        }
        public GeoPoint ToPoint()
        {
            return this.point;
        }
        public GeoPoint Point
        {
            get
            {
                return this.point;
            }
        }
        public Coordinate Corrdinate
        {
            get
            {
                return new Coordinate(this.point.Lng, this.point.Lat);
            }
        }
        public override string ToString()
        {
            return String.Format("Vertex:{0}:({1},{2})", this.ID, this.Lng, this.Lat);
        }
    }
}
