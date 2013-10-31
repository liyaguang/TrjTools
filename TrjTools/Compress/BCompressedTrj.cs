using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrjTools.RoadNetwork;
using System.IO;

namespace TrjTools.Compress
{
    /// <summary>
    /// Beacon based compressed trajectory
    /// </summary>
    public class BCompressedTrj
    {
        public BCompressedTrj(long moid, double segmentLength) 
        {
            this.moid = moid;
            this.Items = new List<BCompressedMV>();
            this.SegmentLength = segmentLength;
            this.StartTime = 0;
        }
        public BCompressedTrj(long moid, double segmentLength, List<RefPoint> refPoints)
        {
            this.moid = moid;
            this.SegmentLength = segmentLength;
            this.Items = new List<BCompressedMV>();
            this.StartTime = 0;
            if (refPoints.Count > 0)
            {
                this.StartTime = refPoints[0].t;
                foreach (var p in refPoints)
                {
                    if (p.e != null)
                    {
                        int segmentID = (int)Math.Round(p.distance / SegmentLength);
                        Items.Add(new BCompressedMV(p.t, p.e, segmentID));
                    }
                    else
                    {
                        Items.Add(new BCompressedMV(p.t, p.Point));
                    }
                }
            }
        }
        public BCompressedTrj(string fileName, Graph g)
        {
            Load(fileName, g);
        }
        public void Load(String fileName, Graph g)
        {
            string[] lines = File.ReadAllLines(fileName);
            this.Items = new List<BCompressedMV>();
            // first line
            if (lines.Length <= 0) return;
            string[] fields = lines[0].Split(',');
            this.moid = long.Parse(fields[0]);
            long t = long.Parse(fields[1]);
            long eid = 0;
            this.StartTime = t;
            this.SegmentLength = double.Parse(fields[2]);
            // items
            for (int i = 1; i < lines.Length; ++i)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                fields = lines[i].Split(',');
                t += long.Parse(fields[0]);
                long tempEid;
                if (long.TryParse(fields[1], out tempEid))
                {
                    Edge e = null;
                    eid += tempEid;
                    g.Edges.TryGetValue(eid, out e);
                    int segmentID = int.Parse(fields[2]);
                    this.Items.Add(new BCompressedMV(t, e, segmentID));
                }
                else
                {
                    eid = 0;
                    double lat = double.Parse(fields[1]);
                    double lng = double.Parse(fields[2]);
                    this.Items.Add(new BCompressedMV(t, new GeoPoint(lat, lng)));
                }
            }
        }
        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            long prevTime = StartTime;
            long preEID = 0;
            sb.AppendLine(string.Format("{0},{1},{2}", moid, prevTime, SegmentLength));
            foreach(var item in Items)
            {
                // long eid = item.RefPoint.eid;
                if (item.e != null)
                {
                    sb.AppendLine(string.Format("{0},{1},{2}", item.t - prevTime, item.eid - preEID, item.segmentID));
                    preEID = item.eid;
                }
                else
                {
                    sb.AppendLine(string.Format("{0},{1},{2}", item.t - prevTime, item.Point.Lat, item.Point.Lng));
                    preEID = 0;
                }
                prevTime = item.t;
            }
            return sb.ToString();
        }
        public override string ToString()
        {
            return this.Items.ToString();
        }

        public Trajectory GetTrajectory()
        {
            Trajectory trj = null;
            throw new NotImplementedException();
        }

        public void Save(string ctrjFileName)
        {
            File.WriteAllText(ctrjFileName, this.Serialize());
        }

        public long moid { get; set; }
        public double SegmentLength { get; set; }
        public List<BCompressedMV> Items { get; set; }
        public long StartTime { get; set; }
    }
}
