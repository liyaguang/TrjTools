using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using TrjTools.RoadNetwork;

namespace TrjTools.Compress
{
    public class TrjCompressor
    {
        public TrjCompressor(Graph g, double maxDev)
        {
            _g = g;
            _maxDev = maxDev;
        }
        /// <summary>
        /// Original version of compress
        /// The use of shortest path results in some drawbacks...
        /// </summary>
        /// <param name="trj"></param>
        /// <returns></returns>
        public VCompressedTrj Compress_v1(Trajectory trj)
        {
            if (trj.Count == 0) return null;
            List<RefPoint> refPoints = getRefPoints(trj);
            VCompressedTrj cTrj = new VCompressedTrj();
            VCompressedTrj.Item item = new VCompressedTrj.Item();
            double dist = 0, appDist = 0;
            Edge lastEdge = refPoints[0].e;
            item.RefPoint = refPoints[0];
            for (int i = 1; i < refPoints.Count; ++i)
            {
                RefPoint cur = refPoints[i], prev = refPoints[i - 1];
                double distance = 0.0, v = 0;
                byte roundV = 0, si = (byte)(cur.t - prev.t);
                if (cur.e == prev.e)
                {
                    distance = cur.distance - prev.distance;
                }
                else if (cur.e.Start == prev.e.End)
                {
                    distance = prev.e.Length - prev.distance + cur.distance;
                }
                else
                {
                    distance = prev.e.Length - prev.distance;
                    var path = _g.FindPath(prev.e.End, cur.e.Start);
                    foreach (var e in path)
                    {
                        distance += e.Length;
                    }
                    distance += cur.distance;
                }
                v = distance / si;
                roundV = (byte)(Math.Round(v) / _binSize);
                dist += distance;
                appDist += roundV * si * _binSize;
                if (Math.Abs(dist - appDist) >= _maxDev)
                {
                    // insert the reference point
                    cTrj.Items.Add(item);
                    // clear
                    item = new VCompressedTrj.Item();
                    item.RefPoint = cur;
                    dist = 0;
                    appDist = 0;
                }
                else
                {
                    item.Points.Add(new VCompressedMV(si, cur.eid, roundV));
                }
            }
            cTrj.Items.Add(item);
            return cTrj;
        }
        /// <summary>
        /// Do not use shortest path when the current edge are not directly connected with the next edge
        /// </summary>
        /// <param name="trj"></param>
        /// <returns></returns>
        public VCompressedTrj VelocityBasedCompress(Trajectory trj)
        {
            if (trj.Count == 0) return null;
            List<RefPoint> refPoints = getRefPoints(trj);
            VCompressedTrj cTrj = new VCompressedTrj();
            VCompressedTrj.Item item = new VCompressedTrj.Item();
            double dist = 0, appDist = 0;
            Edge lastEdge = refPoints[0].e;
            bool canApproximate = true;
            item.RefPoint = refPoints[0];
            for (int i = 1; i < refPoints.Count; ++i)
            {
                RefPoint cur = refPoints[i], prev = refPoints[i - 1];
                double distance = 0.0, v = 0;
                byte roundV = 0, si = (byte)(cur.t - prev.t);
                if (cur.e == null || prev.e == null)
                {
                    canApproximate = false;
                }
                else
                {
                    if (cur.e == prev.e)
                    {
                        canApproximate = true;
                        distance = cur.distance - prev.distance;
                    }
                    else if (cur.e.Start == prev.e.End)
                    {
                        canApproximate = true;
                        distance = prev.e.Length - prev.distance + cur.distance;
                    }
                    else
                    {
                        //canApproximate = false;
                        canApproximate = true;
                        distance = prev.e.Length - prev.distance;
                        var path = _g.FindPath(prev.e.End, cur.e.Start);
                        foreach (var e in path)
                        {
                            distance += e.Length;
                        }
                        distance += cur.distance;
                    }
                }
                if (canApproximate)
                {
                    v = distance / si;
                    roundV = (byte)(Math.Round(v / _binSize));
                    dist += distance;
                    appDist += roundV * si * _binSize;
                    if (Math.Abs(dist - appDist) >= _maxDev)
                    {
                        canApproximate = false;
                    }
                }
                if (canApproximate)
                {
                    item.Points.Add(new VCompressedMV(si, cur.eid, roundV));
                }
                else
                {
                    // insert the reference point
                    cTrj.Items.Add(item);
                    // clear
                    item = new VCompressedTrj.Item();
                    item.RefPoint = cur;
                    dist = 0;
                    appDist = 0;
                }
            }
            cTrj.Items.Add(item);
            return cTrj;
        }

        public BCompressedTrj BeaconBasedCompress(Trajectory trj)
        {
            if (trj.Count == 0) return null;
            List<RefPoint> refPoints = getRefPoints(trj);
            BCompressedTrj ctrj = new BCompressedTrj(trj.moid, _maxDev * 2, refPoints);
            return ctrj;
        }

        public Trajectory DPCompress(Trajectory trj)
        {
            DPCompressor compressor = new DPCompressor(trj, _maxDev);
            return compressor.Compress();
        }
        private List<RefPoint> getRefPoints(Trajectory trj)
        {
            if (trj.Count == 0) return null;
            List<RefPoint> refPoints = new List<RefPoint>();
            for (int i = 0; i < trj.Count; ++i)
            {
                MotionVector mv = trj[i];
                RefPoint refPoint = null;
                if (mv.e != null)
                {
                    double distance = mv.e.DistOnLine(mv.e.Start.Point, mv.point);
                    refPoint = new RefPoint(mv.t, mv.e, distance);
                }
                else
                {
                    refPoint = new RefPoint(mv.t, mv.point);
                }
                refPoints.Add(refPoint);
            }
            // calcualte the deviation between trj and the ref points
            //String tempFileName = Path.Combine(Constants.TRJ_DIR, "1000099465_short_ref");
            //StreamWriter sw = new StreamWriter(tempFileName);
            //for (int i = 0; i < trj.Count; ++i)
            //{
            //    MotionVector mv = trj[i];
            //    double dev = GeoPoint.GetDistance(mv.point, refPoints[i].Point);
            //    sw.WriteLine("{0},{1},{2},{3},{4}", mv.t, mv.point.Lat, mv.point.Lng, mv.EdgeId, dev);
            //}
            //sw.Close();
            return refPoints;
        }

        public Trajectory Decompress(VCompressedTrj ctrj)
        {
            int size = ctrj.Items.Count;
            if (size <= 0) return null;
            Trajectory trj = new Trajectory();
            // Add the first reference point
            for (int i = 0; i < size; ++i)
            {
                var item = ctrj.Items[i];
                double dist = item.RefPoint.distance;    // distance from the start point the edge to the current point
                long t = item.RefPoint.t;
                // add the reference point first
                trj.Add(new MotionVector(item.RefPoint.Point, item.RefPoint.t));
                Edge lastEdge = item.RefPoint.e;
                // Calculate the position of each point
                for (int j = 0; j < item.Points.Count; ++j)
                {
                    var cMV = item.Points[j];
                    double totalDistance = cMV.si * cMV.v * _binSize;  // the distance covered during the interval
                    GeoPoint p = GeoPoint.INVALID;
                    t += cMV.si;
                    if (cMV.rid == lastEdge.ID)
                    {
                        dist += totalDistance;
                        p = lastEdge.Predict(lastEdge.Start.Point, dist);
                    }
                    else
                    {
                        Edge curEdge = _g.Edges[cMV.rid];
                        dist = totalDistance - (lastEdge.Length - dist);  // minus the distance left in the previous road
                        if (lastEdge.End == curEdge.Start)
                        {
                            p = curEdge.Predict(curEdge.Start.Point, dist);
                        }
                        else
                        {
                            var path = _g.FindPath(lastEdge.End, curEdge.Start);
                            if (path != null)
                            {
                                foreach ( var e in path)
                                {
                                    dist -= e.Length;
                                }
                                p = curEdge.Predict(curEdge.Start.Point, dist);
                            }
                            else
                            {
                                Debug.Assert(false);
                            }
                        }
                        lastEdge = curEdge;
                    }
                    trj.Add(new MotionVector(p, t));
                }
            }
            return trj;
        }

        private Graph _g;
        private double _maxDev;
        private int _binSize = 1;
    }
}
