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
using TrjTools.RoadNetwork;
using log4net;

namespace TrjTools.MapMatching
{
    /// <summary>
    /// Mapmatching with hidden markov method
    /// </summary>
    public class MM : BaseMM
    {
        private class Node
        {
            public Node parent = null;
            public double prob;
            public long edgeId;
            public int idx;
            public Node(double prob, long edgeId, int idx, Node parent)
            {
                this.parent = parent;
                this.prob = prob;
                this.idx = idx;
                this.edgeId = edgeId;
            }
            public override string ToString()
            {
                return String.Format("Edge:{0},Prob:{1:.##},Idx:{2}", edgeId, prob, idx);
            }
        }
        #region fields
        //Set match result
        private MotionVector[] mvs = null;
        //Parameter of emission prob
        const double sigma = 10;
        const double sSigma = 1 / (sigma * sigma);    // deviation
        readonly double alpha = -Math.Log(Math.Sqrt(2 * Math.PI) * sigma);
        const double beta = 5;
        const double sBeta = -1 / beta;
        readonly double lnBeta = Math.Log(beta);
        private ILog logger = LogManager.GetLogger(typeof(MM).FullName);

        int maxInterval = 300;//5min
        const int MAX_RADIUS = 100;
        const int RADIUS = 50;
        #endregion fields

        public MM(Graph g)
            : base(g)
        {

        }
        public override Trajectory match(Trajectory trj)
        {
            mvs = trj.ToArray();
            Dictionary<long, Node> T = new Dictionary<long, Node>();
            int trjSize = trj.Count;
            GeoPoint startPoint = trj[0].point;
            double radius = RADIUS;
            double maxRadius = MAX_RADIUS;
            //1. initialize T
            //1.1. get nearest edges
            HashSet<Edge> currentStates = null;
            //currentStates = graph.rangeQuery(startPoint, radius, maxRadius, 4);
            currentStates = getCandidateEdges(startPoint, radius);
            foreach (Edge e in currentStates)
            {
                double prob = getEmissionProbility(e, startPoint);
                T[e.ID] = new Node(prob, e.ID, 0, null);
            }

            //2. Forward viterbri
            for (int output = 0; output < trjSize - 1; output++)
            {
                Dictionary<long, Node> U = new Dictionary<long, Node>();// new dictionary of state prob
                double highest = double.NegativeInfinity;
                HashSet<Edge> nextStates = null;
                double currentRadius = radius;
                while (currentRadius <= maxRadius && double.IsNegativeInfinity(highest))
                {
                    nextStates = getCandidateEdges(trj[output + 1].point, currentRadius);
                    foreach (Edge nextState in nextStates)
                    {
                        //long argMax = 0;
                        double valMax = double.NegativeInfinity;
                        Node argMax = null;
                        foreach (Edge state in currentStates)
                        {
                            Node n = T[state.ID];
                            double vProb = n.prob;
                            if (double.IsNegativeInfinity(vProb))
                            {
                                continue;
                            }
                            double ep = getEmissionProbility(state, trj[output].point);
                            if (!double.IsNegativeInfinity(ep))
                            {
                                double tp = getTransitionProbility(state, trj[output].point, nextState, trj[output + 1].point);
                                vProb += ep + tp;
                                if (vProb > valMax)
                                {
                                    valMax = vProb;
                                    argMax = n;
                                }
                                if (vProb > highest)
                                {
                                    highest = vProb;
                                }
                            }

                        }
                        U[nextState.ID] = new Node(valMax, nextState.ID, output + 1, argMax);
                    }
                    currentRadius *= 2;
                }
                if (double.IsNegativeInfinity(highest))
                {
                    startPoint = trj[output + 1].point;

                    //Console.WriteLine("Makov is interrupted at idx:{0},({1:.######},{2:.######})", output + 1, startPoint.Lng, startPoint.Lat);
                    //1. set match result
                    setMatchResult(T);

                    //2. Set probability
                    foreach (Edge e in nextStates)
                    {
                        double prob = getEmissionProbility(e, startPoint);
                        U[e.ID] = new Node(prob, e.ID, output + 1, null);
                    }
                }
                T = U;
                currentStates = nextStates;
            }
            setMatchResult(T);
            return new Trajectory(mvs);
        }
        /// <summary>
        /// Get the simplified ln version of transportation prob
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="p1"></param>
        /// <param name="e2"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private double getTransitionProbility(Edge e1, GeoPoint p1, Edge e2, GeoPoint p2)
        {
            double prob = double.NegativeInfinity;
            double diff = 0;

            //1.get difference
            double dist = GeoPoint.GetDistance(p1, p2);
            //double maxDist = Math.Min(dist + 200, dist * 2 + 25);
            double maxDist = Math.Max(dist + 300, dist * 1.5);
            Polyline route = graph.FindPath(e1, p1, e2, p2, maxDist);
            if (route != null)
            {
                double routeLength = route.Length;
                if (routeLength < maxDist)
                {
                    diff = Math.Abs(dist - routeLength);
                    //get prob with diff
                    //prob = 1 / beta * Math.Exp(-diff / beta);
                    prob = diff * sBeta;
                }
            }
            return prob;
        }
        /// <summary>
        /// Get the simplified ln version of emission prob
        /// </summary>
        /// <param name="e"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private double getEmissionProbility(Edge e, GeoPoint point)
        {
            double prob = double.NegativeInfinity;
            int type = 0;

            double distance2 = e.Dist2From(point, out type);

            if (Math.Abs(type) < 1)
            {
                //penalty
                if (type != 0)
                {
                    distance2 *= 1.44;
                }
                prob = -0.5 * distance2 * sSigma;
            }
            else
            {
                prob = double.NegativeInfinity;
            }
            return prob;
        }

        private HashSet<Edge> getCandidateEdges(GeoPoint p, double radius)
        {
            double maxRadius = MAX_RADIUS;
            HashSet<Edge> cands = graph.RangeQuery(p, radius, maxRadius);
            foreach (Edge e in cands.ToList())
            {
                int type = 0;
                GeoPoint result;
                type = e.projectFrom(p, out result);
                if (type != 0)
                {
                    cands.Remove(e);
                }
            }
            return cands;
        }
        private void setMatchResult(Dictionary<long, Node> T)
        {
            //Debug.Assert(n != null);
            //1. Find the path with maximum prob
            Node maxNode = null;
            double maxVal = double.NegativeInfinity;
            foreach (var pair in T)
            {
                if (pair.Value.prob > maxVal)
                {
                    maxVal = pair.Value.prob;
                    maxNode = pair.Value;
                }
            }
            //Debug.Assert(maxNode != null);
            //2. set match result
            while (maxNode != null)
            {
                Debug.Assert(maxNode.edgeId > 0);
                mvs[maxNode.idx].e = graph.Edges[maxNode.edgeId];
                mvs[maxNode.idx].type = MotionVector.MatchType.SingleMatched;
                maxNode = maxNode.parent;
            }
        }
    }
}
