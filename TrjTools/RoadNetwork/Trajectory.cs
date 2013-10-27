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
using System.IO;
using System.Globalization;
using TrjTools.Tools;

namespace TrjTools.RoadNetwork
{
    /// <summary>
    /// Simple Trajectory: a list of movtion vectors 
    /// </summary>
    public class Trajectory : List<MotionVector>
    {
        public double Length
        {
            get
            {
                double length = 0;
                for (int i = 0; i < this.Count - 1; i++)
                {
                    double tmpLength = GeoPoint.GetDistance(this[i].point, this[i + 1].point);
                    double speed = tmpLength / (this[i + 1].Time - this[i].Time).TotalSeconds;
                    if (speed < 60)
                    {
                        length += tmpLength;
                    }
                    else
                    {
                        //Debug.Assert(false);
                        //Console.ReadLine();
                    }
                }
                return length;
            }
        }

        public long moid { get; set; }

        public Trajectory()
            : base()
        {
        }
        public Trajectory(List<MotionVector> mvs)
            : base(mvs)
        {
        }
        public Trajectory(MotionVector[] mvs)
            : base(mvs.ToList())
        {
        }
        public Trajectory(String fileName, int type = 0, Graph g = null)
            : base()
        {
            this.Load(fileName, type, g);
        }
        public Trajectory(String fileName, Graph graph)
            : base()
        {
            this.Load(fileName, graph);
        }
        public override string ToString()
        {
            return String.Format("Count={0}", this.Count);
        }
        public void Save(String fileName, int type = 0)
        {
            StringBuilder sb = new StringBuilder();
            switch (type)
            {
                case 0:
                    {
                        for (int i = 0; i < this.Count; i++)
                        {
                            long edgeId = 0;
                            if (this[i].e != null)
                            {
                                edgeId = this[i].e.ID;
                                //sw.WriteLine("{0}\t{1:.000000}\t{2:.000000}\t{3}", this[i].t, this[i].point.Lat, this[i].point.Lng, edgeId);
                                sb.AppendLine(String.Format("{0}\t{1:.000000}\t{2:.000000}\t{3}", this[i].t, this[i].point.Lat, this[i].point.Lng, edgeId));
                            }
                            else
                            {
                                //sw.WriteLine("{0}\t{1:.000000}\t{2:.000000}", this[i].t, this[i].point.Lat, this[i].point.Lng);
                                sb.AppendLine(String.Format("{0}\t{1:.000000}\t{2:.000000}", this[i].t, this[i].point.Lat, this[i].point.Lng));
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        for (int i = 0; i < this.Count; i++)
                        {
                            //long edgeId = 0;
                            //edgeId = this[i].e.ID;
                            //sw.WriteLine("{0}\t{1:.000000}\t{2:.000000}\t{3}", this[i].t, this[i].point.Lat, this[i].point.Lng, edgeId);
                            sb.AppendLine(String.Format("{0},{1}", this[i].orginalString, this[i].EdgeId));
                        }
                        break;
                    }
                case 2:
                    {
                        for (int i = 0; i < this.Count; i++)
                        {
                            long edgeId = 0;
                            string time = Utility.LongToDateTime(this[i].t).ToString("yyyyMMddHHmmss");
                            if (this[i].e != null)
                            {
                                edgeId = this[i].e.ID;
                                sb.AppendLine(String.Format("{0},{1:.000000},{2:.000000},{3}", time, this[i].point.Lat, this[i].point.Lng, edgeId));
                            }
                            else
                            {
                                sb.AppendLine(String.Format("{0},{1:.000000},{2:.000000}", time, this[i].point.Lat, this[i].point.Lng));
                            }
                        }
                        break;
                    }
            }

            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(sb.ToString());
            sw.Close();
        }
        public void SaveForCmp(String fileName)
        {
            StreamWriter sw = new StreamWriter(fileName);
            for (int i = 0; i < this.Count; i++)
            {
                long edgeId = 0;
                if (this[i].e != null)
                {
                    edgeId = this[i].e.ID;
                }
                sw.WriteLine("{0},{1},1.0", this[i].t, edgeId);
            }
            sw.Close();
        }
        private int detectTrjFileType(string line)
        {
            int type = 0;
            String[] fields = line.Split(new char[] { '\t', ',' });
            CultureInfo provider = CultureInfo.InvariantCulture;
            String format = "yyyyMMddHHmmss";
            DateTime t;
            if (DateTime.TryParseExact(fields[0], format, provider, DateTimeStyles.None, out t))
            {
                type = 1;
            }
            return type; 
        }
        /// <summary>
        /// Load trajectory from a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type">0:original, 1:20120930062459,lat,lng,v,dir,state, 2:t,lat,lng</param>
        public void Load(String fileName, Graph g = null)
        {
            String[] lines = File.ReadAllLines(fileName);
            if (lines.Length == 0) return;
            int type = detectTrjFileType(lines[0]);
            CultureInfo provider = CultureInfo.InvariantCulture;
            String format = "yyyyMMddHHmmss";
            foreach (String line in lines)
            {
                string[] fields = line.Split(new char[] { '\t', ',' });
                long time = 0;
                if (fields.Length >= 3)
                {
                    if (type == 0)
                    {
                        time = long.Parse(fields[0]);
                    }
                    else if (type == 1)
                    {
                        DateTime t = DateTime.ParseExact(fields[0], format, provider);
                        time = t.Ticks / MotionVector.TICKS_PER_SECOND;
                    }
                    double lat = double.Parse(fields[1]);
                    double lng = double.Parse(fields[2]);
                    MotionVector mv = new MotionVector(new GeoPoint(lat, lng), time);
                    if (g != null && fields.Length > 3)
                    {
                        long eid = long.Parse(fields.Last());
                        if (eid > 0)
                        {
                            mv.e = g.Edges[eid];
                            mv.type = MotionVector.MatchType.SingleMatched;
                        }
                    }
                    this.Add(mv);
                }
            }
        }
        public void Load(String fileName, int type, Graph g = null)
        {
            //StreamReader sr = new StreamReader(fileName);
            String[] lines = File.ReadAllLines(fileName);
            if (lines.Length == 0) return;
            string firstLine = lines[0];

            switch (type)
            {
                case 0:
                    {
                        foreach (String line in lines)
                        {
                            String[] fields = line.Split(new char[] { '\t', ',' });
                            if (fields.Length >= 3)
                            {
                                long time = long.Parse(fields[0]);
                                double lat = double.Parse(fields[1]);
                                double lng = double.Parse(fields[2]);
                                this.Add(new MotionVector(new GeoPoint(lat, lng), time));
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        String format = "yyyyMMddHHmmss";
                        foreach (String line in lines)
                        {
                            String[] fields = line.Split(',');
                            if (fields.Length >= 3)
                            {
                                DateTime t = DateTime.ParseExact(fields[0], format, provider);
                                long time = t.Ticks / MotionVector.TICKS_PER_SECOND;
                                double lat = double.Parse(fields[1]);
                                double lng = double.Parse(fields[2]);
                                MotionVector mv = new MotionVector(new GeoPoint(lat, lng), time);
                                mv.orginalString = line;
                                if (g != null)
                                {
                                    long eid = long.Parse(fields.Last());
                                    if (eid > 0)
                                    {
                                        mv.e = g.Edges[eid];
                                        mv.type = MotionVector.MatchType.SingleMatched;
                                    }
                                }
                                this.Add(mv);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        foreach (String line in lines)
                        {
                            String[] fields = line.Split(',');
                            if (fields.Length >= 3)
                            {
                                long time = long.Parse(fields[0]);
                                double lat = double.Parse(fields[1]);
                                double lng = double.Parse(fields[2]);
                                this.Add(new MotionVector(new GeoPoint(lat, lng), time));
                            }
                        }
                        break;
                    }
            }

        }
        /// <summary>
        /// Load trajectory from a file
        /// </summary>
        /// <summary>
        /// Remove outliers. <br/>
        /// We assume that the first mv is not outlier
        /// </summary>
        /// <returns></returns>
        public Trajectory RemoveOutlier()
        {
            int trjSize = this.Count;
            if (trjSize <= 1)
            {
                return this;
            }
            MotionVector[] mvs = this.ToArray();
            double maxSpeed = 60; //60m/s
            this.Clear();
            this.Add(mvs[0]);
            for (int i = 1; i < trjSize; i++)
            {
                double distance = GeoPoint.GetDistance(mvs[i - 1].point, mvs[i].point);
                double inteval = (mvs[i].Time - mvs[i - 1].Time).TotalSeconds;
                double speed = distance / inteval;
                if (speed <= maxSpeed)
                {
                    this.Add(mvs[i]);
                }
                else
                {
                    //Debug.Assert(false);
                }
            }
            return this;
        }

        /// <summary>
        /// Separate the trajectory into serveral smaller ones
        /// </summary>
        /// <param name="maxInteval"></param>
        /// <returns></returns>
        public List<Trajectory> Separate(int maxInteval)
        {
            List<Trajectory> list = new List<Trajectory>();
            int trjSize = this.Count;
            if (trjSize <= 1)
            {
                list.Add(this);
                return list;
            }
            MotionVector[] mvs = this.ToArray();
            Trajectory trj = new Trajectory();
            trj.Add(mvs[0]);
            for (int i = 1; i < trjSize; i++)
            {
                double inteval = (mvs[i].Time - mvs[i - 1].Time).TotalSeconds;
                if (inteval > maxInteval)
                {
                    list.Add(trj);
                    trj = new Trajectory();
                }
                trj.Add(mvs[i]);
            }
            return list;
        }
        public EdgePath Path
        {
            get
            {
                EdgePath path = new EdgePath();
                Edge lastEdge = null;
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].e != lastEdge)
                    {
                        path.Add(this[i].e);
                        lastEdge = this[i].e;
                    }
                }
                return path;
            }
        }

        public Trajectory Slice(int from, int length)
        {
            if (from < 0 || length <= 0 || from + length >= this.Count)
            {
                return null;
            }
            MotionVector[] mvs = new MotionVector[length];
            this.CopyTo(from, mvs, 0, length);
            return new Trajectory(mvs);
        }

        //public MotionVector At(DateTime time, Graph g = null)
        //{
        //    MotionVector result = new MotionVector();
        //    result.point = GeoPoint.INVALID;
        //    if (this.Count == 0
        //        || time < this[0].Time || time > this[this.Count - 1].Time)
        //    {
        //        return result;
        //    }
        //    for (int i = 1; i < this.Count; i++)
        //    {
        //        if (time >= this[i - 1].Time && time <= this[i].Time)
        //        {
        //            var mt = new MVTuple();
        //            mt.Start = this[i - 1];
        //            mt.End = this[i];
        //            //get shortest path
        //            if (g != null)
        //            {
        //                if (mt.Start.e != mt.End.e && mt.Start.e != null && mt.End.e != null)
        //                {
        //                    var list = g.FindPath(mt.Start.e.End, mt.End.e.Start);
        //                    if (list != null)
        //                    {
        //                        mt.Path = new EdgeList(list);
        //                    }
        //                }
        //            }
        //            result = mt.projectFrom(time);
        //            //speed
        //            double dist = GeoPoint.GetDistance(this[i - 1].point, this[i].point);
        //            result.v = (float)(dist / (this[i].t - this[i - 1].t));
        //            break;
        //        }
        //    }
        //    return result;
        //}

        public double SpeedAt(DateTime time)
        {
            double speed = -1;
            MotionVector result = new MotionVector();
            result.point = GeoPoint.INVALID;
            if (this.Count == 0
                || time < this[0].Time || time > this[this.Count - 1].Time)
            {
                return speed;
            }
            for (int i = 1; i < this.Count; i++)
            {
                if (time >= this[i - 1].Time && time <= this[i].Time)
                {

                }
            }
            return speed;
        }
    }
}
