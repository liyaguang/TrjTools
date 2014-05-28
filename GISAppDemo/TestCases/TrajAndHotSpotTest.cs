using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TrjTools.RoadNetwork;
using TrjTools.MapMatching;
using TrjTools.Index.Grid;
using EGIS.ShapeFileLib;
using TrjTools.Tools;

namespace GISAppDemo.TestCases
{
    public static class TrajAndHotSpotTest
    {
        public static void DoTest()
        {
            //string dir = Path.Combine(Constants.DATA_DIR, "beijingTrj");
            //string targetDir = Path.Combine(Constants.DATA_DIR, "beijingTrjPart");
            ////getBeijingTrjDir(dir, targetDir);
            //mergeBeijingTrjDir(dir, targetDir);
            //generateEdgeShape(targetDir);
            //GetParkingPoints(targetDir);
            GenerateThreeShapes();
        }
        class FirstCommaFieldComparer : IEqualityComparer<string>
        {

            public bool Equals(string x, string y)
            {
                string keyX = x.Split(new char[] { ',' }, 2)[0];
                string keyY = y.Split(new char[] { ',' }, 2)[0];
                return string.Equals(keyX, keyY);
            }

            public int GetHashCode(string obj)
            {
                string key = obj.Split(new char[] { ',' }, 2)[0];
                return key.GetHashCode();
            }
        }
        public static Dictionary<string, Trajectory> mergeBeijingTrjDir(string dir, string targetDir)
        {
            string[] files = Directory.GetFiles(dir, "109_*");
            var dict = new Dictionary<string, List<string>>();
            var trjs = new Dictionary<string, Trajectory>();
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            foreach (var fileName in files)
            {
                Console.WriteLine("Processing {0}", fileName);
                string outFileName = Path.Combine(targetDir, Path.GetFileName(fileName));
                mergeBeijingTrj(fileName, dict);
            }
            string trjDir = Path.Combine(Constants.DATA_DIR, "beijingTrjPart", "trj");
            // do mapmatching
            var graph = MapLoader.Load("Beijing_trust_oneside_no_dev");
            var mm = new MM(graph);
            int count = 0;
            foreach (var p in dict)
            {
                //Console.WriteLine("Device:{0}, Count:{1}", p.Key, p.Value.Count);
                //File.WriteAllLines(Path.Combine(trjDir, p.Key + ".trj"), p.Value.Distinct().OrderBy(a => a).ToArray());
                var trj = listToTrajectory(p.Value.Distinct(new FirstCommaFieldComparer()).OrderBy(a => a).ToArray());
                trjs[p.Key] = mm.match(trj);
                ++count;
                //if (count % 10 == 0)
                {
                    Console.WriteLine("File:{0}, Percentage:{1}%", p.Key, count * 100.0 / dict.Count);
                }
            }
            return trjs;
        }
        private static Trajectory listToTrajectory(IEnumerable<string> list)
        {
            List<MotionVector> mvs = new List<MotionVector>();
            long baseTime = new DateTime(1970, 1, 1).Ticks / MotionVector.TICKS_PER_SECOND;
            double latLngBase = 10000000.0;
            foreach (var s in list)
            {
                string[] fields = s.Split(new Char[] { ',' });
                long dt = baseTime + int.Parse(fields[0]);
                double lng = int.Parse(fields[1]) / latLngBase, lat = int.Parse(fields[2]) / latLngBase;
                mvs.Add(new MotionVector(new GeoPoint(lat, lng), dt));
            }
            return new Trajectory(mvs);
        }
        private static void mergeBeijingTrj(string fileName, Dictionary<string, List<string>> dict)
        {
            StreamReader sr = new StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] fields = line.Split(new char[] { ',' }, 21);
                if (fields.Length < 21) continue;
                string key = fields[4];
                string value = string.Join(",", fields[19], fields[15], fields[16]);
                List<string> values = null;
                if (!dict.TryGetValue(key, out values))
                {
                    values = new List<string>();
                    dict[key] = values;
                }
                values.Add(value);
            }
            sr.Close();
        }
        private static void getBeijingTrjDir(string dir, string targetDir)
        {
            string[] files = Directory.GetFiles(dir);
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            foreach (var fileName in files)
            {
                Console.WriteLine("Processing {0}", fileName);
                string outFileName = Path.Combine(targetDir, Path.GetFileName(fileName));
                getBeijingTrj(fileName, outFileName);
            }
        }
        private static void getBeijingTrj(string fileName, string outFileName)
        {
            StreamReader sr = new StreamReader(fileName);
            StreamWriter sw = new StreamWriter(outFileName);
            Console.WriteLine("Read Complete!");
            StringBuilder sb = new StringBuilder();
            long outCount = 0, readCount = 0, lineCount = 0;
            long fileSize = new FileInfo(fileName).Length;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                readCount += line.Length + 1;
                ++lineCount;
                string[] fields = line.Split(new char[] { ',' }, 20);
                if (fields.Length < 20) continue;
                int lat = int.Parse(fields[16]);
                int lng = int.Parse(fields[15]);
                if (lat > 392600000 && lat < 410300000 && lng > 1152500000 && lng < 1173000000)
                {
                    sb.AppendLine(line);
                    ++outCount;
                    if (outCount % 100000 == 0)
                    {
                        sw.Write(sb.ToString());
                        sb.Clear();
                        Console.WriteLine(outCount);
                    }
                }
                if (lineCount % 100000 == 0)
                {
                    Console.WriteLine("{0:.###}...", readCount * 1.0 / fileSize);
                }
            }
            if (sb.Length > 0)
            {
                sw.Write(sb.ToString());
            }
            sr.Close();
            sw.Close();

        }

        private static Dictionary<string, Trajectory> readTrjs(string path, Graph graph)
        {
            var trjs = new Dictionary<string, Trajectory>();
            var files = Directory.GetFiles(path, "*.trj");
            foreach(var file in files)
            {
                var key = Path.GetFileNameWithoutExtension(file);
                var trj = new Trajectory(file, graph);
                trjs[key] = trj;
            }
            return trjs;
        }
        private static IList<Edge> getCompletePath(IList<Edge> path, Graph graph)
        {
            if (path == null || path.Count <= 1)
            {
                return path;
            }
            List<Edge> fullPath = new List<Edge>();
            Edge lastEdge = path[0];
            fullPath.Add(lastEdge);
            for (int i = 1; i < path.Count; ++i)
            {
                Edge edge = path[i];
                if (edge == null) continue;
                if (lastEdge != null && edge.Start != lastEdge.End)
                {
                    double maxDist = 2000;
                    if (GeoPoint.GetDistance(lastEdge.End.Point, edge.Start.Point) < maxDist) //fill with shortest path if less than five 
                    {
                        var edges = graph.FindPath(lastEdge.End, edge.Start, maxDist * 1.5);
                        if (edges != null)
                        {
                            fullPath.AddRange(edges);
                        }
                    }
                }
                fullPath.Add(edge);
                lastEdge = edge;
            }
            return fullPath;
        }
        private static void GenerateEdgeShape(string path, string shapeFileName, bool addDev = false)
        {
            var graph = MapLoader.Load("Beijing_trust_oneside_no_dev");
            // read trjs
            var trjs = readTrjs(path, graph);
            // Get dictionary of edge -> Set(deviceId)
            var edgeDevicesDict = new Dictionary<long, HashSet<string>>();
            foreach(var p in trjs)
            {
                // get complete path
                var completePath = getCompletePath(p.Value.Path.ToList(), graph);
                foreach (var e in completePath)
                {
                    long edgeId = e.ID;
                    if (edgeId == 0) continue;
                    HashSet<string> devices = null;
                    if (!edgeDevicesDict.TryGetValue(edgeId, out devices))
                    {
                        devices = new HashSet<string>();
                        edgeDevicesDict[edgeId] = devices;
                    }
                    devices.Add(p.Key);
                }
            }
            // Generate shapefile
            setAsShapeFile(edgeDevicesDict, graph, shapeFileName, addDev);
            StringBuilder sb = new StringBuilder();
            foreach (var p in edgeDevicesDict.ToList().OrderByDescending(a => a.Value.Count))
            {
                sb.AppendLine(string.Format("{0},{1}", p.Key, p.Value.Count));
            }
            String outputFileName = Path.Combine(Constants.DATA_DIR, "beijingTrjPart", "stat", "edgeDeviceCount.txt");
            File.WriteAllText(outputFileName, sb.ToString());
        }

        private static void setAsShapeFile(Dictionary<long, HashSet<string>> edgeDevicesDict, Graph graph, string fileName, bool addDev = false)
        {
            //写入文件
            String rootDir = Path.GetDirectoryName(fileName);
            String shapeFileName = Path.GetFileNameWithoutExtension(fileName);
            ShapeType shapeType = ShapeType.PolyLine;
            var transform = new Wgs2MgsTransform();
            int threshold = 1;
            DbfFieldDesc[] fields = new DbfFieldDesc[] 
            { 
                new DbfFieldDesc { FieldName = "ID", FieldType = DbfFieldType.Character, FieldLength = 14, RecordOffset = 0 },
                //new DbfFieldDesc { FieldName = "Name", FieldType = DbfFieldType.Character, FieldLength = 18, RecordOffset = 14 },
            };
            ShapeFileWriter sfw = ShapeFileWriter.CreateWriter(rootDir, shapeFileName, shapeType, fields);
            foreach (var p in edgeDevicesDict)
            //foreach (Edge e in Edges.Values)
            {
                if (edgeDevicesDict.Count < threshold) continue;
                Edge e = graph.Edges[p.Key];
                String id = e.ID.ToString();
                if (e.ID % 2 == 0)
                {
                    continue;
                }
                String[] fieldData = new string[] { "" };
                List<PointF> vertices = new List<PointF>();
                for (int i = 0; i < e.Geo.Points.Count; i++)
                {
                    GeoPoint point = e.Geo.Points[i];
                    if (addDev)
                    {
                        point = transform.Transform(point);
                    }
                    float lng = (float)(point.Lng);
                    float lat = (float)(point.Lat);
                    vertices.Add(new PointF(lng, lat));
                }
                sfw.AddRecord(vertices.ToArray(), vertices.Count, fieldData);
            }
            sfw.Close();
        }
        public static void GenerateParkingPointsShape(string path, string shapeFileName, bool addDev = false)
        {
            var graph = MapLoader.Load("Beijing_trust_oneside_no_dev");
            // read trjs
            var trjs = readTrjs(path, graph);
            var parkPointsDict = new Dictionary<string, List<GeoPoint>>();
            var points = new List<GeoPoint>();
            foreach (var p in trjs.OrderByDescending(a => a.Value.Count))
            {
                var ps = p.Value.GetParkingPoints();
                // remove the point not in beijing
                ps = filterParkingPoints(ps);
                points.AddRange(ps);
                parkPointsDict[p.Key] = ps;
            }
            setParkingPointsAsShapeFile(points, shapeFileName, addDev);
        }
        public static Dictionary<string, List<GeoPoint>> GetParkingPoints(string path)
        {
            var graph = MapLoader.Load("Beijing_trust_oneside_no_dev");
            // read trjs
            var trjs = readTrjs(path, graph);
            var parkPointsDict = new Dictionary<string, List<GeoPoint>>();
            var points = new List<GeoPoint>();
            foreach(var p in trjs.OrderByDescending(a => a.Value.Count))
            {
                var ps = p.Value.GetParkingPoints();
                // remove the point not in beijing
                ps = filterParkingPoints(ps);
                points.AddRange(ps);
                parkPointsDict[p.Key] = ps;
            }
            //string fileName = Path.Combine(Constants.DATA_DIR, "beijingTrjPart", "stat", "parkingPoints.txt");
            //saveParkingPoints(parkPointsDict, fileName);
            string shapeFile = Path.Combine(Constants.DATA_DIR, "beijingTrjPart", "shp", "parkingPoints_dev.shp");
            setParkingPointsAsShapeFile(points, shapeFile, true);
            return parkPointsDict;
        }

        private static void saveParkingPoints(Dictionary<string, List<GeoPoint>> dict, string fileName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in dict.OrderBy(a => a.Key))
            {
                foreach(var point in p.Value)
                {
                    sb.AppendLine(String.Format("{0},{1}", point.Lat, point.Lng));
                }
            }
            File.WriteAllText(fileName, sb.ToString());
        }

        private static List<GeoPoint> filterParkingPoints(List<GeoPoint> points)
        {
            var result = new List<GeoPoint>();
            var graph = MapLoader.Load("Beijing_trust_oneside_no_dev");
            foreach(var point in points)
            {
                var vertices = graph.VertexRangeQuery(point, 50);
                if (vertices.Count > 0)
                {
                    result.Add(point);
                }
            }
            return result;
        }
        private static void setParkingPointsAsShapeFile(List<GeoPoint> points, string fileName, bool addDev = false)
        {
            //写入文件
            String rootDir = Path.GetDirectoryName(fileName);
            String shapeFileName = Path.GetFileNameWithoutExtension(fileName);
            ShapeType shapeType = ShapeType.Point;
            var transform = new Wgs2MgsTransform();
            DbfFieldDesc[] fields = new DbfFieldDesc[] 
            { 
                new DbfFieldDesc {FieldName = "ID", FieldType = DbfFieldType.Character, FieldLength = 14, RecordOffset = 0 },
                //new DbfFieldDesc { FieldName = "Name", FieldType = DbfFieldType.Character, FieldLength = 18, RecordOffset = 14 },
            };
            ShapeFileWriter sfw = ShapeFileWriter.CreateWriter(rootDir, shapeFileName, shapeType, fields);
            
            foreach (var p in points)
            {
                String[] fieldData = new string[] { " " };
                List<PointF> vertices = new List<PointF>();
                GeoPoint point = p;
                if (addDev)
                {
                    point = transform.Transform(point);
                }
                float lng = (float)(point.Lng);
                float lat = (float)(point.Lat);
                vertices.Add(new PointF(lng, lat));
                sfw.AddRecord(vertices.ToArray(), vertices.Count, fieldData);
            }
            sfw.Close();
        }
        public static void GenerateParkingRegionShapeFromFile(string fileName, bool addDev = false)
        {
            string txtFileName = Path.Combine(Constants.DATA_DIR, "beijingTrjPart", "stat", 
                "parkingRegion.txt");
            //string fileName = Path.Combine(Constants.DATA_DIR, "beijingTrjPart", "shp", "parkingRegion.shp");
            String rootDir = Path.GetDirectoryName(fileName);
            String shapeFileName = Path.GetFileNameWithoutExtension(fileName);
            var transform = new Wgs2MgsTransform();
            // Read txt
            string[] lines = File.ReadAllLines(txtFileName);

            ShapeType shapeType = ShapeType.PolyLine;
            DbfFieldDesc[] shpFields = new DbfFieldDesc[] 
            { 
                new DbfFieldDesc {FieldName = "ID", FieldType = DbfFieldType.Character, FieldLength = 14, RecordOffset = 0 },
                //new DbfFieldDesc { FieldName = "Name", FieldType = DbfFieldType.Character, FieldLength = 18, RecordOffset = 14 },
            };
            ShapeFileWriter sfw = ShapeFileWriter.CreateWriter(rootDir, shapeFileName, shapeType, shpFields);
            foreach (var line in lines)
            {
                var fields = line.Split(new char[] { ',' });
                if (fields.Length % 2 != 0) continue;
                String[] fieldData = new string[] { " " };
                List<PointF> vertices = new List<PointF>();
                for (int i = 0; i < fields.Length; i += 2)
                {
                    float lat = float.Parse(fields[i]);
                    float lng = float.Parse(fields[i + 1]);
                    GeoPoint point = new GeoPoint(lat, lng);
                    if (addDev)
                    {
                        point = transform.Transform(point);
                    }
                    vertices.Add(new PointF((float)point.Lng, (float)point.Lat));
                }
                sfw.AddRecord(vertices.ToArray(), vertices.Count, fieldData);
            }
            sfw.Close();
        }

        public static void GenerateParkingRegionShape(string targetDir, string fileName, bool addDev = false)
        {
            var dict = TrajAndHotSpotTest.GetParkingPoints(targetDir);
            List<GeoPoint> points = new List<GeoPoint>();
            foreach (var pair in dict)
            {
                points.AddRange(pair.Value);
            }
            var mbrs = TrajAndHotSpotTest.ExtractRegions(points);
            GenerateParkingRegionShape(mbrs, fileName, addDev);
        }
        public static List<MBR> ExtractRegions(List<GeoPoint> points)
        {
            List<MBR> regions = new List<MBR>();
            // if (lat > 392600000 && lat < 410300000 && lng > 1152500000 && lng < 1173000000)
            MBR mbr = new MBR(115.25, 39.26, 117.3, 41.03);
            double cellSize = 500 * TrjTools.Constants.D_PER_M;
            int minCount = 20;
            GridPoint index = new GridPoint(points, mbr, cellSize);
            regions = index.GetHotRegions(minCount);
            return regions;
        }
        public static void GenerateParkingRegionShape(List<MBR> mbrs, string fileName, bool addDev = false)
        {
            
            String rootDir = Path.GetDirectoryName(fileName);
            String shapeFileName = Path.GetFileNameWithoutExtension(fileName);
            var transform = new Wgs2MgsTransform();
            // Read txt
            ShapeType shapeType = ShapeType.PolyLine;
            DbfFieldDesc[] shpFields = new DbfFieldDesc[] 
            { 
                new DbfFieldDesc {FieldName = "ID", FieldType = DbfFieldType.Character, FieldLength = 14, RecordOffset = 0 },
                //new DbfFieldDesc { FieldName = "Name", FieldType = DbfFieldType.Character, FieldLength = 18, RecordOffset = 14 },
            };
            ShapeFileWriter sfw = ShapeFileWriter.CreateWriter(rootDir, shapeFileName, shapeType, shpFields);
            foreach (var mbr in mbrs)
            {
                String[] fieldData = new string[] { " " };
                List<PointF> vertices = new List<PointF>();
                GeoPoint[] points = new GeoPoint[] { mbr.TopLeft, mbr.TopRight, mbr.BottomRight, mbr.BottomLeft, mbr.TopLeft };
                for (int i = 0; i < points.Length; ++i)
                {
                    var point = points[i];
                    if (addDev)
                    {
                        point = transform.Transform(point);
                    }
                    vertices.Add(point.ToPointF());
                }
                sfw.AddRecord(vertices.ToArray(), vertices.Count, fieldData);
            }
            sfw.Close();
        }

        public static void GenerateThreeShapes()
        {
            // Generate edges
            string targetDir = Path.Combine(Constants.DATA_DIR, "beijingTrjPart");
            //string edgeShapeFileName = Path.Combine(Constants.DATA_DIR, "beijingTrjPart", "shp", "edge_dev.shp");
            //GenerateEdgeShape(targetDir, edgeShapeFileName, true);

            // Generate parking points
            string parkingPointsShapeFileName = Path.Combine(Constants.DATA_DIR, "beijingTrjPart", "shp", "parkingPoints_dev.shp");
            GenerateParkingPointsShape(targetDir, parkingPointsShapeFileName, true);

            // Generate parking regions
            //string parkingRegionShapeFileName = Path.Combine(Constants.DATA_DIR, "beijingTrjPart", "shp", "parkingRegions_dev.shp");
            //GenerateParkingRegionShape(targetDir, parkingRegionShapeFileName, true);
        }
    }
}
