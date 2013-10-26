using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrjTools.Compress
{
    public class CompressedMV
    {
        /// <summary>
        /// Sampling interval
        /// </summary>
        public byte si { get; set; }

        /// <summary>
        /// Road id
        /// </summary>
        public long rid { get; set; }

        /// <summary>
        /// Velocity
        /// </summary>
        public byte v { get; set; }

        public CompressedMV(byte si, long rid, byte v)
        {
            this.si = si;
            this.rid = rid;
            this.v = v;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", si, rid, v);
        }
    }
}
