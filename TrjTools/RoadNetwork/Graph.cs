//******************************
// Written by Yaguang Li (liyaguang0123@gmail.com)
// Copyright (c) 2013, ISCAS
//
// Use and restribution of this code is subject to the GPL v3.
//******************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EGIS.ShapeFileLib;
using log4net;
using TrjTools.Algorithm;
using TrjTools.Index.Grid;
using TrjTools.Tools;


namespace TrjTools.RoadNetwork
{
    public class Graph
    {
        private Dictionary<long, Edge> edges;

        public Dictionary<long, Edge> Edges
        {
            get { return edges; }
        }
        private Dictionary<long, Vertex> vertices;

        public Dictionary<long, Vertex> Vertices
        {
            get { return vertices; }
        }
        private ILog logger = LogManager.GetLogger(typeof(Graph).FullName);
        private GridEdge edgeIndex = null;

        public GridEdge EdgeIndex
        {
            get 
            {
                if (this.edgeIndex == null)
                {
                    lock (this)
                    {
                        if (this.edgeIndex == null)
                        {
                            this.edgeIndex = new GridEdge(edges.Values, mbr, edgeCellSize);
                        }
                    }
                }
                return edgeIndex;
            }
        }
        private GridVertex vertexIndex;
        private MBR mbr;
        //private double edgeCellSize = 500 * Constants.D_PER_M;
        private double edgeCellSize = 25 * Constants.D_PER_M;
        private double vertexCellSize = 50 * Constants.D_PER_M;


        private long base_id = 100000000000;
        public Graph(String vertexFile, String edgeFile, String geometryFile = null)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            loadVertices(vertexFile);
            Console.WriteLine("Vertex:{0}ms", sw.ElapsedMilliseconds);
            Console.WriteLine("Memory:{0}", GC.GetTotalMemory(false) / 1024 / 1024);
            loadEdges(edgeFile);
            Console.WriteLine("Edge:{0}ms", sw.ElapsedMilliseconds);
            Console.WriteLine("Memory:{0}", GC.GetTotalMemory(false) / 1024 / 1024);

            if (geometryFile != null)
            {
                loadGeometry(geometryFile);
            }
            Console.WriteLine("Gem:{0}ms", sw.ElapsedMilliseconds);
            //Console.ReadLine();
            buildRNIndex();
            Console.WriteLine("Memory:{0}", GC.GetTotalMemory(false) / 1024 / 1024);
            Console.WriteLine("Index:{0}ms", sw.ElapsedMilliseconds);

        }
        /// <summary>
        /// Get the edge with a distance roughly lower than radius from point p 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<Edge> RangeQuery(GeoPoint p, double radius)
        {
            return this.EdgeIndex.RangeQuery(p, radius);
        }
        /// <summary>
        /// Get the edge with a distance roughly lower than radius from point p 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<Edge> RangeQuery(GeoPoint p, double radius, double maxRadius, int minSize = 0)
        {
            HashSet<Edge> result = null;
            while (radius <= maxRadius && (result == null || result.Count <= minSize))
            {
                result = RangeQuery(p, radius);
                radius *= 2;
            }
            return result;
        }

        /// <summary>
        /// Get the vertex with a mbr
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<Vertex> VertexRangeQuery(MBR rect)
        {
            return this.vertexIndex.RangeQuery(rect);
        }
        /// <summary>
        /// Get the vertex with a mbr
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<Vertex> VertexRangeQuery(GeoPoint p, double radius)
        {
            double minLat, minLng, maxLat, maxLng;
            double d_radius = radius * Constants.D_PER_M;	//radius in degree
            minLng = p.Lng - d_radius;
            maxLng = p.Lng + d_radius;
            minLat = p.Lat - d_radius;
            maxLat = p.Lat + d_radius;
            MBR rect = new MBR(minLng, minLat, maxLng, maxLat);
            return this.vertexIndex.RangeQuery(rect);
        }
        private void loadVertices(String fileName)
        {
            this.mbr = MBR.EMPTY;
            //id,lng,lat
            vertices = new Dictionary<long, Vertex>();
            using (StreamReader sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    String[] fields = line.Split('\t');
                    Debug.Assert(fields.Length == 3);
                    long id = long.Parse(fields[0]);
                    double lat = double.Parse(fields[1]);
                    double lng = double.Parse(fields[2]);
                    Vertex v = new Vertex(id, lat, lng);
                    vertices.Add(id, v);
                    this.mbr.Include(new GeoPoint(lat, lng));
                }
            }
        }
        private void loadEdges(String fileName)
        {
            edges = new Dictionary<long, Edge>();
            using (StreamReader sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();
                    String[] fields = line.Split('\t');
                    //Debug.Assert(fields.Length == 6);
                    long id = long.Parse(fields[0]);
                    long startId = long.Parse(fields[1]);
                    long endId = long.Parse(fields[2]);
                    Vertex start = Vertices[startId];
                    Vertex end = Vertices[endId];
                    Edge e = null;
                    if (fields.Length >= 6)
                    {
                        double length = double.Parse(fields[3]);
                        double speedLimit = int.Parse(fields[4]);
                        int type = int.Parse(fields[5]);
                        e = new Edge(id, start, end, length, speedLimit, type);
                    }
                    else
                    {
                        e = new Edge(id, start, end);
                    }
                    edges.Add(id, e);
                    start.RegisterEdge(e);
                    end.RegisterEdge(e);
                }
            }
        }
        /// <summary>
        /// Load geometry information of the edge
        /// </summary>
        /// <param name="fileName"></param>
        private void loadGeometry(String fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            //File.ReadAllLines(fileName);
            String line = String.Empty;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                String[] fields = line.Split(new char[] { '\t' }, 2);
                Debug.Assert(fields.Length == 2);
                long edgeId = long.Parse(fields[0]);
                Edge e = null;
                if (this.edges.TryGetValue(edgeId, out e))
                {
                    e.GeoString = fields[1];
                }
            }
        }
        /// <summary>
        /// Build grid index for road network
        /// </summary>
        private void buildRNIndex()
        {
            //this.edgeIndex = new GridEdge(edges.Values, mbr, edgeCellSize);
            this.vertexIndex = new GridVertex(vertices.Values, mbr, vertexCellSize);
        }
        /// <summary>
        /// Check if edge is single direction
        /// </summary>
        public void TestConnection()
        {
            foreach (Edge e in edges.Values)
            {
                //Check if there exists an edge end->start
                List<Edge> outEdges = e.End.OutEdges;
                bool found = false;
                foreach (Edge e2 in outEdges)
                {
                    Debug.Assert(e.End == e2.Start);
                    if (e2.End == e.Start)
                    {
                        found = true;
                        logger.Info(String.Format("{0}, {1}", e, e2));
                        break;
                    }
                }
                if (!found)
                {
                    //logger.Info(String.Format("Reverse Edge of {0}  not found!", e.ID));
                }
            }
        }
        public void CleanAndSaveEdgeData(String edgeFileName)
        {
            List<Edge> newEdges = new List<Edge>();
            HashSet<long> removedEdge = new HashSet<long>();

            foreach (Edge e in edges.Values.ToList())
            {
                //Check if there exists an edge end->start
                List<Edge> outEdges = e.End.OutEdges;
                bool found = false;
                foreach (Edge e2 in outEdges)
                {
                    Debug.Assert(e.End == e2.Start);
                    if (e2.End == e.Start)
                    {
                        found = true;
                        if (!removedEdge.Contains(e2.ID))
                        {
                            edges.Remove(e2.ID);
                            removedEdge.Add(e.ID);
                            removedEdge.Add(e2.ID);
                            //logger.Info(String.Format("{0}, {1}", e, e2));
                            logger.Info("remove:" + e2);
                        }
                        break;
                    }
                }
            }
            //write new dictionary
            //int count = 1;
            //String edgeFileName = Constants.MAP_DIR + "beijing_e2.txt";
            //String edgeFileName = Constants.MAP_DIR + "beijing_e2.txt";
            StreamWriter sw = new StreamWriter(edgeFileName);
            foreach (Edge e in edges.Values.OrderBy(e => e.ID))
            {
                //sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", e.ID, e.Start.ID, e.End.ID, e.Length, e.SpeedLimit, e.Type);
                sw.WriteLine("{0}\t{1}\t{2}", e.ID, e.Start.ID, e.End.ID);
                //count++;
                //sw.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", e.ID + base_id, e.End.ID, e.Start.ID, e.Length, e.SpeedLimit, e.Type);
                sw.WriteLine("{0}\t{1}\t{2}", e.ID + base_id, e.End.ID, e.Start.ID);
                //count++;
            }
            sw.Close();
        }
        public void SaveAsShpFile(String fileName, double offsetLat = 0, double offsetLng = 0)
        {
            //写入文件
            String rootDir = Path.GetDirectoryName(fileName);
            String shapeFileName = Path.GetFileNameWithoutExtension(fileName);
            ShapeType shapeType = ShapeType.PolyLine;
            DbfFieldDesc[] fields = new DbfFieldDesc[] 
            { 
                new DbfFieldDesc { FieldName = "ID", FieldType = DbfFieldType.Character, FieldLength = 14, RecordOffset = 0 },
                //new DbfFieldDesc { FieldName = "Name", FieldType = DbfFieldType.Character, FieldLength = 18, RecordOffset = 14 },
            };
            ShapeFileWriter sfw = ShapeFileWriter.CreateWriter(rootDir, shapeFileName, shapeType, fields);
            foreach (Edge e in Edges.Values)
            {
                String id = e.ID.ToString();
                if (e.ID % 2 == 0)
                {
                    continue;
                }
                String[] fieldData = new string[] { id };
                List<PointF> vertices = new List<PointF>();
                for (int i = 0; i < e.Geo.Points.Count; i++)
                {
                    float lng = (float)(e.Geo.Points[i].Lng + offsetLng);
                    float lat = (float)(e.Geo.Points[i].Lat + offsetLat);
                    vertices.Add(new PointF(lng, lat));
                }
                sfw.AddRecord(vertices.ToArray(), vertices.Count, fieldData);
            }
            sfw.Close();
        }
        /// <summary>
        /// Save the map data into three files: vertices.txt, edges.txt and geos.txt
        /// </summary>
        /// <param name="dir"></param>
        public void SaveMapData(String dir, bool reorderId = false, double offsetLat = 0, double offsetLng = 0)
        {
            String vertexFileName = Path.Combine(dir, "vertices.txt");
            String edgeFileName = Path.Combine(dir, "edges.txt");
            String geoFileName = Path.Combine(dir, "geos.txt");
            StreamWriter sw = null;
            if (File.Exists(vertexFileName) || File.Exists(edgeFileName) || File.Exists(geoFileName))
            {
                Console.WriteLine("Already Exists, Quiting...");
                return;
            }
            // Save vertices.txt
            sw = new StreamWriter(vertexFileName);
            foreach (var v in Vertices.Values)
            {
                sw.WriteLine("{0}\t{1}\t{2}", v.ID,
                    v.Lat + offsetLat, v.Lng + offsetLng);
            }
            sw.Close();
            // Save edges.txt
            sw = new StreamWriter(edgeFileName);
            foreach (var e in Edges.Values)
            {
                sw.WriteLine("{0}\t{1}\t{2}", e.ID, e.Start.ID, e.End.ID);
            }
            sw.Close();
            // Save geos.txt
            sw = new StreamWriter(geoFileName);
            foreach (var e in Edges.Values)
            {
                List<double> geos = new List<double>();
                foreach(var p in e.Geo.Points)
                {
                    geos.Add(p.Lat + offsetLat);
                    geos.Add(p.Lng + offsetLng);
                }
                sw.WriteLine("{0}\t{1}", e.ID, String.Join("\t", geos));
            }
            sw.Close();
        }

        public void SaveMapData(String dir, IGeoTransformer converter)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            String vertexFileName = Path.Combine(dir, "vertices.txt");
            String edgeFileName = Path.Combine(dir, "edges.txt");
            String geoFileName = Path.Combine(dir, "geos.txt");
            StreamWriter sw = null;
            if (File.Exists(vertexFileName) || File.Exists(edgeFileName) || File.Exists(geoFileName))
            {
                Console.WriteLine("Already Exists, Quiting...");
                return;
            }
            // Save vertices.txt
            sw = new StreamWriter(vertexFileName);
            foreach (var v in Vertices.Values)
            {
                GeoPoint newV = converter.Transform(v.Point);
                sw.WriteLine("{0}\t{1}\t{2}", v.ID, newV.Lat, newV.Lng);
            }
            sw.Close();
            // Save edges.txt
            sw = new StreamWriter(edgeFileName);
            foreach (var e in Edges.Values)
            {
                sw.WriteLine("{0}\t{1}\t{2}", e.ID, e.Start.ID, e.End.ID);
            }
            sw.Close();
            // Save geos.txt
            sw = new StreamWriter(geoFileName);
            foreach (var e in Edges.Values)
            {
                List<double> geos = new List<double>();
                foreach (var p in e.Geo.Points)
                {
                    var newP = converter.Transform(p);
                    geos.Add(newP.Lat);
                    geos.Add(newP.Lng);
                }
                sw.WriteLine("{0}\t{1}", e.ID, String.Join("\t", geos));
            }
            sw.Close();
        }
        /// <summary>
        /// Find a path between two point using A* algorithm
        /// </summary>
        /// <returns></returns>
        public List<Edge> FindPath(Vertex from, Vertex to, double maxDist = double.MaxValue)
        {
            AStar astar = new AStar(this);
            List<Edge> list = new List<Edge>();
            if (from != to)
            {
                var path = astar.FindPath(from, to, maxDist);
                if (path != null)
                {
                    list = path.Edges;
                }
            }
            return list;
        }

        /// <summary>
        /// Find a path between two point using A* algorithm
        /// </summary>
        /// <returns></returns>
        public Polyline FindPath(Edge from, GeoPoint fromPoint, Edge to, GeoPoint toPoint, double maxDist = double.MaxValue)
        {
            Polyline route = null;
            GeoPoint fromProject = from.projectFrom(fromPoint);
            GeoPoint toProject = to.projectFrom(toPoint);
            if (from == to)
            {
                List<GeoPoint> points = new List<GeoPoint>();
                points.Add(fromProject);
                points.Add(toProject);
                route = new Polyline(points);
            }
            else if (from.End == to.Start)
            {
                List<GeoPoint> points = new List<GeoPoint>();
                points.Add(fromProject);
                points.Add(from.End.ToPoint());
                points.Add(toProject);
                route = new Polyline(points);
            }
            else
            {
                //Directed road only
                Vertex src = from.End;
                Vertex dest = to.Start;
                AStar astar = new AStar(this);
                EdgePath path = astar.FindPath(src, dest, maxDist);
                if (path != null && path.Count > 0)
                {
                    //build route
                    List<GeoPoint> points = new List<GeoPoint>();
                    points.Add(fromProject);
                    for (int i = 0; i < path.Count; i++)
                    {
                        Edge e = path[i];
                        points.Add(e.Start.ToPoint());
                    }
                    points.Add(path.Last().End.ToPoint());
                    points.Add(toProject);
                    route = new Polyline(points);

                }
            }
            return route;
        }

        public bool IsReachable(Vertex from, Vertex to, double maxDist)
        {
            bool result = false;
            if (from != null && to != null)
            {
                if (from == to)
                {
                    result = true;
                }
                else
                {
                    AStar astar = new AStar(this);
                    var path = astar.FindPath(from, to, maxDist);
                    result = (path != null);
                }
            }
            return result;
        }
        public bool IsReachable(Edge from, GeoPoint fromPoint, Edge to, GeoPoint toPoint, double maxDist = double.MaxValue)
        {
            Polyline route = FindPath(from, fromPoint, to, toPoint, maxDist);
            bool result = false;
            if (route != null && route.Length < maxDist)
            {
                result = true;
            }
            return result;
        }

        public HashSet<Edge> GetCandiateEdges(Vertex src, GeoPoint destPoint, double maxCost, double maxDist)
        {
            AStar astar = new AStar(this);
            return astar.GetCandiateEdges(src, destPoint, maxCost, maxDist);
        }
    }
}
