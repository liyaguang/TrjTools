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
    public class RefPoint
    {
        public RefPoint(long t, Edge e, double distance)
        {
            this.t = t;
            this.e = e;
            this.distance = distance;
        }
        public RefPoint(long t, GeoPoint point)
        {
            this.t = t;
            this._point = point;
        }
        public long t { get; set; }
        public Edge e { get; set; }
        private GeoPoint _point;
        /// <summary>
        /// The distance between this reference point and the start of the edge
        /// </summary>
        public double distance { get; set; }
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
                if (e != null)
                {
                    return e.Predict(e.Start.Point, distance);
                }
                else
                {
                    return _point;
                }
            }
        }
        public override string ToString()
        {
            return string.Format("{0},{1},{2}", Utility.LongToDateTime(t), eid, (short)distance);
        }
    }
}
