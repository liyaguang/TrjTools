using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrjTools.RoadNetwork;
using System.IO;

namespace GISAppDemo
{
    public class MapLoader
    {
        private static Dictionary<String, Graph> _loadedGraph = new Dictionary<String, Graph>();
        public static Graph Load(String mapName)
        {
            Graph g = null;
            if (!_loadedGraph.TryGetValue(mapName, out g))
            {
                string dir = Path.Combine(Constants.MAP_DIR, mapName);
                string edgeFile = Path.Combine(dir, "edges.txt");
                string vertexFile = Path.Combine(dir, "vertices.txt");
                string geoFile = Path.Combine(dir, "geos.txt");
                //string shpFile = Path.Combine(DATASET_DIR, "beijing_2011.shp");
                g = new Graph(vertexFile, edgeFile, geoFile);
                _loadedGraph[mapName] = g;
            }
            return g;
        }
    }
}
