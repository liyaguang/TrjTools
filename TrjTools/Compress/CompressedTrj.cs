using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrjTools.RoadNetwork;

namespace TrjTools.Compress
{
    public class CompressedTrj
    {
        public class Item
        {
            public RefPoint RefPoint { get; set; }
            public List<CompressedMV> Points { get; set; }
            public Item()
            {
                this.Points = new List<CompressedMV>();
                this.RefPoint = null;
            }
        }
        public CompressedTrj() 
        {
            this.Items = new List<Item>();
        }
        public CompressedTrj(long moid)
        {
            this.moid = moid;
            this.Items = new List<Item>();
        }
        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(moid);
            foreach(var item in Items)
            {
                // long eid = item.RefPoint.eid;
                if (item.RefPoint.e != null)
                {
                    sb.Append(string.Format(":{0},{1},{2}", item.RefPoint.t, item.RefPoint.eid, (short)item.RefPoint.distance));
                    foreach (var p in item.Points)
                    {
                        sb.Append(string.Format("|{0},{1},{2}", p.si, p.rid, p.v));
                    }
                }
                else
                {
                    sb.Append(string.Format(":{0},{1},{2}", item.RefPoint.t, item.RefPoint.Point.Lat, item.RefPoint.Point.Lng));
                }
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
            return trj;
        }
        public long moid { get; set; }
        public List<Item> Items { get; set; }
    }
}
