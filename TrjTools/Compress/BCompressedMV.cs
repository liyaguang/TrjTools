using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrjTools.RoadNetwork;
using TrjTools.Tools;

namespace TrjTools.Compress
{
    /// <summary>
    /// Reference point used in the compressed trajectory
    /// </summary>
    public class BCompressedMV
    {
        public BCompressedMV(long t, Edge e, int segmentID)
        {
            this.t = t;
            this.e = e;
            this.segmentID = segmentID;
        }
        public BCompressedMV(long t, GeoPoint point)
        {
            this.t = t;
            this.e = null;
            this.segmentID = -1;
            this._point = point;
        }
        public long t { get; set; }
        public Edge e { get; set; }
        private GeoPoint _point;
        /// <summary>
        /// The distance between this reference point and the start of the edge
        /// </summary>
        public int segmentID { get; set; }
        public long eid
        {
            get
            {
                long id = -1;
                if (e != null)
                {
                    id = e.ID;
                }
                return id;
            }
        }
        /// <summary>
        /// Get the position of this point
        /// </summary>
        public GeoPoint Point
        {
            get
            {
                return this._point;
            }
        }
        public override string ToString()
        {
            return string.Format("{0},{1},{2}", Utility.LongToDateTime(t), eid, (short)segmentID);
        }
    }
}
