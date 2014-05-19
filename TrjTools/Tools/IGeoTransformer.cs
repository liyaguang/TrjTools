using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrjTools.RoadNetwork;

namespace TrjTools.Tools
{
    public interface IGeoTransformer
    {
        GeoPoint Transform(GeoPoint point);
    }
}
