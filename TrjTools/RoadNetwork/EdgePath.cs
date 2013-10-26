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
    /// A path that consists of edges
    /// </summary>
    public class EdgePath : IEnumerable<Edge>
    {
        List<Edge> edges = new List<Edge>();
        HashSet<long> vertices = new HashSet<long>();
        Vertex dummyVertex = null;
        public List<Edge> Edges
        {
            get { return edges; }
        }
        public EdgePath(List<Edge> edges)
        {
            this.edges = new List<Edge>(edges);
            foreach (Edge e in edges)
            {
                this.vertices.Add(e.Start.ID);
                this.vertices.Add(e.End.ID);
            }
        }
        public EdgePath(EdgePath path)
        {
            this.edges = new List<Edge>(path.edges);
            this.vertices = new HashSet<long>(path.vertices);
        }
        public EdgePath(Vertex v)
        {
            this.dummyVertex = v;
        }
        public EdgePath()
        {

        }

        public int Count
        {
            get
            {
                return this.edges.Count;
            }
        }
        public Edge this[int idx]
        {
            get
            {
                Edge e = null;
                if (idx < this.Count && idx >= 0)
                {
                    e = this.edges[idx];
                }
                return e;
            }
        }
        /// <summary>
        /// The start vertex
        /// </summary>
        public Vertex Start
        {
            get
            {
                Vertex v = dummyVertex;
                if (this.Count > 0)
                {
                    v = this[0].Start;
                }
                return v;
            }
        }
        /// <summary>
        /// The end vertex
        /// </summary>
        public Vertex End
        {
            get
            {
                Vertex v = dummyVertex;
                if (this.Count > 0)
                {
                    v = this[this.Count - 1].End;
                }
                return v;
            }
        }
        public Edge FirstEdge
        {
            get
            {
                Edge e = null;
                if (this.Count > 0)
                {
                    e = this[0];
                }
                return e;
            }
        }
        public Edge LastEdge
        {
            get
            {
                Edge e = null;
                if (this.Count > 0)
                {
                    e = this[this.Count - 1];
                }
                return e;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Edge e in this.edges)
            {
                sb.Append(e.ID).Append("->");
            }
            return sb.ToString();
        }
        //public void Append(EdgePath path)
        //{
        //    if (path != null)
        //    {
        //        int i = 0;
        //        if (this.edges.First() == path.edges.First())
        //        {
        //            i++;
        //        }
        //        for (; i < path.Count; i++)
        //        {
        //            this.edges.Add(path[i]);
        //        }
        //    }
        //}
        public void Add(Edge e)
        {
            if (e != null)
            {
                this.edges.Add(e);
                this.vertices.Add(e.Start.ID);
                this.vertices.Add(e.End.ID);
            }
            //this.dummyVertex = null;
        }
        public bool Contains(Edge e)
        {
            return this.edges.Contains(e);
        }
        public bool Contains(Vertex v)
        {
            return this.vertices.Contains(v.ID);
        }
        /// <summary>
        /// Predict the position from start after distance on this route
        /// </summary>
        /// <param name="start"></param>
        /// <param name="distance"></param>
        /// <param name="startIdx"></param>
        /// <returns></returns>
        public GeoPoint Predict(GeoPoint start, double distance, int startIdx = 0)
        {
            GeoPoint target = this.Last().End.ToPoint();
            //GeoPoint target = GeoPoint.INVALID;
            double currentDistance = 0;
            while (startIdx < this.Count)
            {
                Edge e = this[startIdx];
                int type = 0;
                double length = e.EndDistFrom(start, out type);
                if (currentDistance + length >= distance)
                {
                    double leftDistance = distance - currentDistance;
                    target = e.Predict(start, leftDistance);
                    break;
                }
                else
                {
                    currentDistance += length;
                    startIdx++;
                    start = e.End.ToPoint();
                }
            }
            return target;
        }
        public double DistanceOnRoute(GeoPoint fromPoint, Edge from, GeoPoint toPoint, Edge to)
        {
            GeoPoint fromProject = from.projectFrom(fromPoint);
            GeoPoint toProject = to.projectFrom(toPoint);
            if (from == to)
            {
                List<GeoPoint> points = new List<GeoPoint>();
                points.Add(fromProject);
                points.Add(toProject);
            }
            else if (from.End == to.Start)
            {
                List<GeoPoint> points = new List<GeoPoint>();
                points.Add(fromProject);
                points.Add(from.End.ToPoint());
                points.Add(toProject);
            }
            else
            {
                ////Directed road only
                //Vertex src = from.End;
                //Vertex dest = to.Start;
                //AStar astar = new AStar(this);
                //EdgeList path = astar.FindPath(src, dest, maxDist);
                //if (path != null && path.Count > 0)
                //{
                //    //build route
                //    List<GeoPoint> points = new List<GeoPoint>();
                //    points.Add(fromProject);
                //    for (int i = 0; i < path.Count; i++)
                //    {
                //        Edge e = path[i];
                //        points.Add(e.Start.ToPoint());
                //    }
                //    points.Add(path.Last().End.ToPoint());
                //    points.Add(toProject);
                //    route = new Polyline(points);

                //}
            }
            return 0.0;
        }

        public IEnumerator<Edge> GetEnumerator()
        {
            return this.edges.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.edges.GetEnumerator();
        }
    }
}
