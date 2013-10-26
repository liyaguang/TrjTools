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
using TrjTools.RoadNetwork;
using System.Diagnostics;

namespace TrjTools.Algorithm
{
    public class AStar
    {
        private class Node : IComparable<Node>
        {
            /// <summary>
            /// Past  
            /// </summary>
            public double g;
            /// <summary>
            /// Estimated
            /// </summary>
            public double h;
            /// <summary>
            /// Edge used to generate path
            /// </summary>
            public Edge e;
            /// <summary>
            /// Total weight
            /// </summary>
            public double Weight
            {
                get
                {
                    return g + h;
                }
            }
            public Node parent;
            public Vertex v;

            public Node(Vertex v, double g, double h, Edge e, Node parent)
            {
                this.v = v;
                this.g = g;
                this.h = h;
                this.e = e;
                this.parent = parent;
                Debug.Assert(e == null || e.End == v);
            }

            public int CompareTo(Node other)
            {
                double diff = (this.Weight - other.Weight);
                int result = Math.Sign(diff);
                if (result == 0)
                {
                    result = (int)(this.v.ID - other.v.ID);
                }
                return result;
            }
            public override bool Equals(object obj)
            {
                return (obj is Node) && ((obj as Node).v.ID == this.v.ID);
            }
            public override int GetHashCode()
            {
                return this.v.GetHashCode();
            }
            public override string ToString()
            {
                return String.Format("Vertex:{0},Weight:{1:0.##},Edge:{2}", v.ID, Weight, e);
            }

        }

        private class NodeComparer : IComparer<Node>
        {
            public int Compare(Node x, Node y)
            {
                return (int)(x.Weight - y.Weight);
            }
        }
        private class NodeEquityComparer : IEqualityComparer<Node>
        {

            public bool Equals(Node x, Node y)
            {
                return x.v.ID == y.v.ID;
            }

            public int GetHashCode(Node obj)
            {
                return obj.v.GetHashCode();
            }
        }
        private Graph graph = null;
        public AStar(Graph g)
        {
            this.graph = g;
        }
        public EdgePath FindPath(Vertex src, Vertex dest, double maxDist = double.MaxValue)
        {
            SortedDictionary<Node, Node> openTable = new SortedDictionary<Node, Node>();
            Dictionary<long, Node> openVertexId = new Dictionary<long, Node>(); //Used to get a node by node id
            Dictionary<long, Node> closedSet = new Dictionary<long, Node>();
            GeoPoint srcPoint = src.ToPoint();
            GeoPoint destPoint = dest.ToPoint();

            //push initial node
            double estimatedCost = GeoPoint.GetDistance(destPoint, srcPoint);
            Node n = new Node(src, 0, estimatedCost, null, null);
            openTable.Add(n, n);
            openVertexId.Add(src.ID, n);

            //Begin search
            Node solution = null;
            bool removed = false;
            while (openTable.Count > 0)
            {
                //Pop the node with minimum weight
                Node parent = openTable.Keys.First();
                removed = openTable.Remove(parent);
                Debug.Assert(removed);
                removed = openVertexId.Remove(parent.v.ID);
                Debug.Assert(removed);
                closedSet.Add(parent.v.ID, parent);
                if (parent.Weight > maxDist)
                {
                    //Stop searching
                    break;
                }
                if (parent.v == dest)
                {
                    solution = parent;
                    break;
                }

                //Get children
                List<Edge> edges = parent.v.OutEdges;
                for (int i = 0; i < edges.Count; i++)
                {
                    Edge currentEdge = edges[i];
                    double g = parent.g + currentEdge.Length;
                    Vertex v = currentEdge.End;
                    double h = GeoPoint.GetDistance(v.ToPoint(), destPoint);
                    Node tmpNode = null;
                    if (closedSet.TryGetValue(v.ID, out tmpNode))
                    {
                        if (g + h >= tmpNode.Weight)
                        {
                            //this is a visited node 
                            continue;
                        }
                        else
                        {
                            removed = closedSet.Remove(v.ID);
                            Debug.Assert(removed);
                        }
                    }
                    if (openVertexId.TryGetValue(v.ID, out tmpNode))
                    {
                        //Check if it has a lower cost
                        if (tmpNode.Weight > h + g)
                        {
                            Debug.Assert(openTable.ContainsKey(tmpNode));
                            removed = openTable.Remove(tmpNode);
                            Debug.Assert(removed);
                            tmpNode.g = g;
                            tmpNode.h = h;
                            tmpNode.e = currentEdge;
                            tmpNode.parent = parent;
                            openTable.Add(tmpNode, tmpNode);
                            openVertexId[v.ID] = tmpNode;
                        }
                    }
                    else
                    {
                        Node newNode = new Node(v, g, h, currentEdge, parent);
                        openTable.Add(newNode, newNode);
                        openVertexId.Add(v.ID, newNode);
                    }
                }

            }

            //Find path
            List<Edge> path = null;
            EdgePath list = null;
            if (solution != null)
            {
                path = new List<Edge>();
                while (solution.parent != null)
                {
                    path.Add(solution.e);
                    solution = solution.parent;
                }
                path.Reverse();
                list = new EdgePath(path);
            }
            return list;
        }
        //public EdgeList FindPath(Vertex src, Vertex dest, double maxDist = double.MaxValue)
        //{
        //    BigList<Node> openTable = new BigList<Node>();
        //    HashSet<long> openVertexId = new HashSet<long>();
        //    HashSet<Node> closedSet = new HashSet<Node>();
        //    GeoPoint srcPoint = src.ToPoint();
        //    GeoPoint destPoint = dest.ToPoint();

        //    //push initial node
        //    double estimatedCost = GeoPoint.GetDistance(destPoint, srcPoint);
        //    Node n = new Node(src, 0, estimatedCost, null, null);
        //    openTable.Add(n);
        //    openVertexId.Add(src.ID);

        //    //Begin search
        //    Node solution = null;
        //    while (openTable.Count > 0)
        //    {
        //        //Pop the 
        //        //Node parent = openTable[0];
        //        Node parent = openTable[0];
        //        openTable.RemoveAt(0);
        //        closedSet.Add(parent);
        //        openVertexId.Remove(parent.v.ID);
        //        if (parent.Weight > maxDist)
        //        {
        //            //Stop searching
        //            break;
        //        }
        //        if (parent.v == dest)
        //        {
        //            solution = parent;
        //            break;
        //        }

        //        //Get children
        //        List<Edge> edges = parent.v.OutEdges;
        //        for (int i = 0; i < edges.Count; i++)
        //        {
        //            Edge currentEdge = edges[i];
        //            double g = parent.g + currentEdge.Length;
        //            Vertex v = currentEdge.End;
        //            double h = GeoPoint.GetDistance(v.ToPoint(), destPoint);
        //            Node newNode = new Node(v, g, h, currentEdge, parent);
        //            if (closedSet.Contains(newNode))
        //            {
        //                Node tmpNode = closedSet.Where(node => node.v.ID == v.ID).FirstOrDefault();
        //                if (newNode.Weight >= tmpNode.Weight)
        //                {
        //                    //this is a visited node 
        //                    continue;
        //                }
        //                else
        //                {
        //                    closedSet.Remove(tmpNode);
        //                }
        //            }
        //            if (openVertexId.Contains(v.ID))
        //            {
        //                //Check if it has a lower cost
        //                Node oldNode = null;
        //                int idx = 0;
        //                for (; idx < openTable.Count; idx++)
        //                {
        //                    if (openTable[idx].v == v)
        //                    {
        //                        oldNode = openTable[idx];
        //                        break;
        //                    }
        //                }
        //                Debug.Assert(idx < openTable.Count);
        //                if (oldNode.Weight > h + g)
        //                {
        //                    openTable.RemoveAt(idx);
        //                    oldNode.g = g;
        //                    oldNode.h = h;
        //                    oldNode.e = currentEdge;
        //                    oldNode.parent = parent;
        //                    openTable.Add(oldNode);
        //                }
        //            }
        //            else
        //            {
        //                openTable.Add(newNode);
        //                openVertexId.Add(newNode.v.ID);
        //            }
        //        }

        //    }

        //    //Find path
        //    List<Edge> path = null;
        //    EdgeList list = null;
        //    if (solution != null)
        //    {
        //        path = new List<Edge>();
        //        while (solution.parent != null)
        //        {
        //            path.Add(solution.e);
        //            solution = solution.parent;
        //        }
        //        path.Reverse();
        //        list = new EdgeList(path);
        //    }
        //    return list;
        //}

        //private void testEqual(BigList<Node> a, HashSet<long> b)
        //{
        //    Debug.Assert(a.Count == b.Count);
        //    foreach (var n in a)
        //    {
        //        Debug.Assert(b.Contains(n.v.ID));
        //    }
        //}


        public HashSet<Edge> GetCandiateEdges(Vertex src, GeoPoint destPoint, double maxCost, double maxDist)
        {
            SortedDictionary<Node, Node> openTable = new SortedDictionary<Node, Node>();
            Dictionary<long, Node> openVertexId = new Dictionary<long, Node>(); //Used to get a node by node id
            Dictionary<long, Node> closedSet = new Dictionary<long, Node>();
            GeoPoint srcPoint = src.ToPoint();
            HashSet<Edge> cands = new HashSet<Edge>();
            //GeoPoint destPoint = dest;

            //push initial node
            double estimatedCost = GeoPoint.GetDistance(destPoint, srcPoint);
            Node n = new Node(src, 0, estimatedCost, null, null);
            openTable.Add(n, n);
            openVertexId.Add(src.ID, n);

            //Begin search
            while (openTable.Count > 0)
            {
                //Pop the node with minimum weight
                Node parent = openTable.Keys.First();
                openTable.Remove(parent);
                openVertexId.Remove(parent.v.ID);
                closedSet.Add(parent.v.ID, parent);
                if (parent.Weight > maxCost)
                {
                    //Stop searching
                    break;
                }
                if (parent.e != null && parent.e.DistFrom(destPoint) < maxDist)
                {
                    cands.Add(parent.e);
                }
                //Get children
                List<Edge> edges = parent.v.OutEdges;
                for (int i = 0; i < edges.Count; i++)
                {
                    Edge currentEdge = edges[i];
                    //double g = parent.g + currentEdge.Length;
                    double g = parent.g;
                    GeoPoint result;
                    if (parent.e != null)
                    {
                        g += parent.e.Length;
                    }
                    Vertex v = currentEdge.End;
                    int type = currentEdge.projectFrom(destPoint, out result);
                    double h = 0;
                    if (type == 0)
                    {
                        h = GeoPoint.GetDistance(currentEdge.Start.ToPoint(), result) + GeoPoint.GetDistance(result, destPoint);
                    }
                    else
                    {
                        h = currentEdge.Length + GeoPoint.GetDistance(currentEdge.End.ToPoint(), destPoint);
                    }
                    Node tmpNode = null;
                    if (closedSet.TryGetValue(v.ID, out tmpNode))
                    {
                        if (g + h >= tmpNode.Weight)
                        {
                            //this is a visited node 
                            continue;
                        }
                        else
                        {
                            closedSet.Remove(v.ID);
                        }
                    }
                    if (openVertexId.TryGetValue(v.ID, out tmpNode))
                    {
                        //Check if it has a lower cost
                        if (tmpNode.Weight > h + g)
                        {
                            Debug.Assert(openTable.ContainsKey(tmpNode));
                            openTable.Remove(tmpNode);
                            tmpNode.g = g;
                            tmpNode.h = h;
                            tmpNode.e = currentEdge;
                            tmpNode.parent = parent;
                            openTable.Add(tmpNode, tmpNode);
                            openVertexId[v.ID] = tmpNode;
                        }
                    }
                    else
                    {
                        Node newNode = new Node(v, g, h, currentEdge, parent);
                        openTable.Add(newNode, newNode);
                        openVertexId.Add(v.ID, newNode);
                    }
                }

            }
            return cands;
        }
    }
}
