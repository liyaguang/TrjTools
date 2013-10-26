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
using TrjTools.Index.Grid;
using log4net;

namespace TrjTools.MapMatching
{
    public class TrjTreeNode:IComparable,IComparable<TrjTreeNode>
    {
        public TrjTreeNode parent = null;
        public Vertex v = null;
        public EdgePath path = null;
        public int idx = -1;
        public double score = 0;
        public TrjTreeNode()
        {

        }
        public TrjTreeNode(TrjTreeNode parent)
        {
            this.parent = parent;
        }

        public int CompareTo(object obj)
        {

            throw new NotImplementedException();
        }

        public int CompareTo(TrjTreeNode other)
        {
            throw new NotImplementedException();
        }
    }
    public class TrjTreeNodeComparer : IComparer<TrjTreeNode>
    {

        public int Compare(TrjTreeNode x, TrjTreeNode y)
        {
            int result = x.idx - y.idx;
            if (result == 0)
            {
                result = (int)(x.v.ID - y.v.ID);
            }
            return result;
        }
    }
    public class PassbyMM:BaseMM
    {
        private const double GPS_ERROR_DISTANCE = 20;
        private const double sigma = 6.4;
        private const double sSigma = 1 / (sigma * sigma);    // deviation
        private readonly double alpha = -Math.Log(Math.Sqrt(2 * Math.PI) * sigma);
        private const double beta = 5;
        private const double sBeta = -1 / beta;
        readonly double lnBeta = Math.Log(beta);
        private ILog logger = LogManager.GetLogger(typeof(PassbyMM).FullName);

        int maxInterval = 300;//5min
        const int MAX_RADIUS = 100;
        const int RADIUS = 40;
        public PassbyMM(Graph g)
            : base(g)
        {

        }
        public override RoadNetwork.Trajectory match(RoadNetwork.Trajectory trajectory)
        {
            this.trj = trajectory;
            double cellSize = 50;
            GridTrj trjIndex = new GridTrj(trj, cellSize);
            SortedSet<TrjTreeNode> openTable = new SortedSet<TrjTreeNode>(new TrjTreeNodeComparer());
            List<TrjTreeNode> closeTable = new List<TrjTreeNode>();
            int trjCount=trj.Count;
            int currentIdx = 0;
            while (currentIdx < trjCount - 1)
            {
                //1. get initial candidates
                if (openTable.Count == 0)
                {
                    currentIdx = getInitialCands(openTable, currentIdx);
                }

                //2. Create candidate paths
                TrjTreeNode currentNode = null;
                while (openTable.Count > 0)
                {
                    Debug.Assert(currentIdx < trjCount);

                    //2.1.get the first node
                    currentNode = openTable.Min;
                    openTable.Remove(currentNode);
                    closeTable.Add(currentNode);

                    //2.2. create candidate paths
                    List<TrjTreeNode> cands = createCands(trjIndex, currentNode);
                    foreach (var cand in cands)
                    {
                        //2.3. check parallel paths
                        if (openTable.Contains(cand))
                        {
                            var tempCand = openTable.Where((n) => n == cand);
                            Trace.Assert(tempCand.Count() == 1, "More than two parallel paths were found!");
                            TrjTreeNode bestCand = getBestPath(cand, tempCand.First());
                            openTable.Add(bestCand);
                        }
                        else
                        {
                            openTable.Add(cand);
                        }
                    }
                    //2.3 prune and confirm
                    if (openTable.Count > 3)
                    {
                        openTable = pruneConfirm(openTable);
                    }
                    currentIdx = currentNode.idx + 1;   //for breaking the loop
                }
            }
            //3. Get the matched result
            Trajectory newTrj = getMatchedResult(closeTable);
            return newTrj;
        }

        /// <summary>
        /// Eliminating infeasible paths while confirming the path if necessary
        /// </summary>
        /// <param name="openTable"></param>
        /// <returns></returns>
        private SortedSet<TrjTreeNode> pruneConfirm(SortedSet<TrjTreeNode> openTable)
        {

            return openTable;
        }

        /// <summary>
        /// Choose the best path from the two parallel paths by considering the cost similarity <br/>
        /// However, if the speed limit information is not available, only the distance and difference between the trajectory and the real path will be used.
        /// </summary>
        /// <param name="cand"></param>
        /// <param name="tempCand"></param>
        /// <returns></returns>
        private TrjTreeNode getBestPath(TrjTreeNode cand, TrjTreeNode tempCand)
        {
            //1. get lowest common ancestor


            //2. get the two paths


            //3. evaluate the two paths

            return cand;
        }
        /// <summary>
        /// Evaluate the candidate path in terms of distance, speed etc.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private double evaluatePath(EdgePath path)
        {

            return 0.0;
        }
        /// <summary>
        /// Create candidate paths
        /// </summary>
        /// <param name="trjIndex"></param>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private List<TrjTreeNode> createCands(GridTrj trjIndex, TrjTreeNode currentNode)
        {
            //1.get candidate paths for the current node
            List<EdgePath> paths = buildPath(currentNode);
            List<TrjTreeNode> cands = new List<TrjTreeNode>();
            List<EdgePath> newPaths = new List<EdgePath>();
            double baseDist = GPS_ERROR_DISTANCE;
            int currentIdx = currentNode.idx;
            //2.find the trajectory point near the end vertex of the path
            foreach (EdgePath path in paths)
            {
                GeoPoint endPoint = path.End.Point;
                HashSet<int> idxs = trjIndex.RangeQuery(endPoint, baseDist);
                if (idxs.Count > 0)
                {
                    double minDist = baseDist;
                    int bestIdx = -1;
                    foreach (int idx in idxs)
                    {
                        if (idx >= currentIdx)
                        {
                            //Filter by real distance
                            double distance = Polyline.DistFrom(trj[idx].point, trj[idx + 1].point, endPoint);
                            if (distance <= minDist)
                            {
                                minDist = distance;
                                bestIdx = idx;
                            }
                        }
                    }
                    if (bestIdx >= 0)
                    {
                        //Create new node
                        TrjTreeNode node = new TrjTreeNode(currentNode);
                        node.idx = bestIdx;
                        node.path = path;
                        node.v = path.End;
                        cands.Add(node);
                    }
                }
            }
            return cands;
        }

        /// <summary>
        /// Build paths within the error ellipse
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private List<EdgePath> buildPath(TrjTreeNode currentNode)
        {
            Vertex startV = currentNode.v;
            int predIdx = currentNode.idx;
            int currentIdx = currentNode.idx + 1;

            GeoPoint currentPoint = trj[currentIdx].point;
            GeoPoint nextPoint = trj[currentIdx + 1].point;
            int interval = (int)(trj[currentIdx + 1].t - trj[currentIdx].t);
            double maxSpeed = Constants.MAX_SPEED;
            Queue<EdgePath> rawCandidatePaths = new Queue<EdgePath>();
            List<EdgePath> candidatePaths = new List<EdgePath>();
            //define the maximum allowed distance, i.e., the 2c of the error ellipse
            double maxDistance = Math.Min(interval * maxSpeed, Math.Max(GeoPoint.GetDistance(currentPoint, nextPoint) * 2, GeoPoint.GetDistance(currentPoint, nextPoint) + GPS_ERROR_DISTANCE));
            //get initial candidate
            if (currentNode.path != null && currentNode.path.Count > 0)
            {
                rawCandidatePaths.Enqueue(new EdgePath() { currentNode.path.LastEdge });
            }
            else
            {
                Trace.Assert(currentNode.v.InEdges.Count > 0);
                //Choose the first element of the InEdges 
                EdgePath path = new EdgePath(currentNode.v);
                rawCandidatePaths.Enqueue(path);
            }
            while (rawCandidatePaths.Count > 0)
            {
                EdgePath path = rawCandidatePaths.Dequeue();
                //extend it
                foreach (Edge e in path.End.OutEdges)
                {
                    if (path.Contains(e.End))   //avoid duplicate vertices
                    {
                        continue;
                    }
                    EdgePath newPath = new EdgePath(path);
                    newPath.Add(e);
                    GeoPoint p = e.End.Point;
                    GeoPoint result;
                    int projectType = e.projectFrom(nextPoint, out result);
                    double distance = GeoPoint.GetDistance(result, nextPoint);
                    if (projectType == 0 && distance < GPS_ERROR_DISTANCE)
                    {
                        candidatePaths.Add(newPath);
                    }
                    else if (GeoPoint.GetDistance(p, currentPoint) + GeoPoint.GetDistance(p, nextPoint) < maxDistance)
                    {
                        //need further extension
                        rawCandidatePaths.Enqueue(newPath);
                    }
                }
            }
            return candidatePaths;
        }

        /// <summary>
        /// Get initial candidates, i.e., adjacent vertices
        /// </summary>
        /// <param name="head"></param>
        /// <param name="openTable"></param>
        /// <param name="currentIdx"></param>
        /// <returns>the currentIdx, larger than or equal to the given value</returns>
        private int getInitialCands(SortedSet<TrjTreeNode> openTable, int currentIdx)
        {
            double baseDist = GPS_ERROR_DISTANCE;
            bool initialVertexFound = false;
            int trjCount = trj.Count;
            while (openTable.Count == 0 && currentIdx < trjCount - 1)
            {
                GeoPoint currentPoint = trj[currentIdx].point;
                GeoPoint anotherPoint = trj[currentIdx + 1].point;
                HashSet<Vertex> currentVertices = graph.VertexRangeQuery(currentPoint, baseDist);
                foreach (Vertex v in currentVertices)
                {
                    double tmpDist = Polyline.DistFrom(currentPoint, anotherPoint, v.Point);
                    if (tmpDist <= baseDist)
                    {
                        TrjTreeNode node = new TrjTreeNode()
                        {
                            v = v,
                            path = null,
                            parent = null,
                            idx = currentIdx,
                            score = 0
                        };
                        initialVertexFound = true;
                        openTable.Add(node);
                    }
                }
                if (!initialVertexFound)
                {
                    currentIdx++;
                }
            }
            return currentIdx;
        }

        private Trajectory getMatchedResult(List<TrjTreeNode> closeTable)
        {

            throw new NotImplementedException();
        }

    }
}
