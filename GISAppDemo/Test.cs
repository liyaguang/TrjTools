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
using System.Threading;
using TrjTools.RoadNetwork;
using TrjTools.MapMatching;
using System.Diagnostics;
using System.IO;
using GeoAPI.Geometries;
using TrjTools.Compress;
using TrjTools.Tools;
using TrjTools.Index.RTree;
using TrjTools.Index.Grid;
using GISAppDemo.TestCases;
using log4net;

namespace GISAppDemo
{
    static class Test
    {
        public static ILog logger = LogManager.GetLogger("info");
        public static void Start()
        {
            //mmTest();
            //mmTest2();
            //shpTest();
            //compressTest();
            //compressTest2();
            ////compressTest3();
            //compressTest4();
            //compressTest5();
            //compressTest6();
            //efficienceTest();
            //samplingTest2();
            //sizeStat();
            //convertToUndirected();
            //shortenEdgeId();
            //reorganizationTest();
            //devTest();
            //speedTest();
            //inOutDegreeTest();
            //trjTest();
            //trjTest4();
            //trjTest2();
            //mmTest3();
            //queryTest();
            //BPOETest.RunTest();
            //mergeTrjTest();
            //devMapTest();
            //WAMapTest();
            //welcome();
            
            TrajAndHotSpotTest.DoTest(); 
        }
        static void welcome()
        {
            Console.WriteLine("程序入口为：Test.cs中的Start函数。");
            Console.WriteLine("TrjTools.Compress中为压缩相关类");
            Console.WriteLine("其中BCompressTrj为Beacon-based方法，VCompressedTrj为Velocity-based方法");
        }

        static void WAMapTest()
        {
            string mapDirName = "WA";
            string newMapDirName = "WA";
            string orginalMapDir = Path.Combine(Constants.MAP_DIR, mapDirName);
            string targetMapDir = Path.Combine(Constants.MAP_DIR, newMapDirName);

            string edgeFile = Path.Combine(orginalMapDir, "edges.txt");
            string vertexFile = Path.Combine(orginalMapDir, "vertices.txt");
            string geoFile = Path.Combine(orginalMapDir, "geos.txt");
            Graph g = new Graph(vertexFile, edgeFile, geoFile);
            string targetShpFileName = Path.Combine(targetMapDir, "WA");
            //double offsetLat = -0.001245, offsetLng = -0.006200;
            //double offsetLat = -0.001247, offsetLng = -0.005937;
            //g.SaveAsShpFile(targetShpFileName, offsetLat, offsetLng);
            g.SaveAsShpFile(targetShpFileName);
            //g.SaveMapData(targetMapDir, false, offsetLat, offsetLng);
        }
        static void devMapTest()
        {
            //string mapDirName = "Beijing_trust_oneside_no_dev";
            string mapDirName = "Beijing_hjf_no_dev";
            string newMapDirName = "Beijing_hjf_no_dev";
            string orginalMapDir = Path.Combine(Constants.MAP_DIR, mapDirName);
            string targetMapDir = Path.Combine(Constants.MAP_DIR, newMapDirName);

            string edgeFile = Path.Combine(orginalMapDir, "edges.txt");
            string vertexFile = Path.Combine(orginalMapDir, "vertices.txt");
            string geoFile = Path.Combine(orginalMapDir, "geos.txt");
            Graph g = new Graph(vertexFile, edgeFile, geoFile);
            string targetShpFileName = Path.Combine(targetMapDir, "Beijing");
            string mgs2WgsFileName = Path.Combine(Constants.DATA_DIR, "other", "Mars2Wgs.txt");
            var transform = new Mgs2WgsTransform(mgs2WgsFileName);
            //double offsetLat = -0.001245, offsetLng = -0.006200;
            //double offsetLat = -0.001247, offsetLng = -0.005937;
            //g.SaveAsShpFile(targetShpFileName, offsetLat, offsetLng);
            g.SaveAsShpFile(targetShpFileName);
            //g.SaveMapData(targetMapDir, transform);
        }
        static void inOutDegreeTest()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            long edgeID = 1;
            Edge e = g.Edges[edgeID];
            // Get in/out degree
            int outDegree = e.End.OutEdges.Count;
            int inDegree = e.Start.InEdges.Count;
        }
        static void reorganizationTest()
        {
            string[] dirs = new string[] { "matched", "bct", "vct", "dp" };
            string[] exts = new string[] { "trj", "bct", "ctrj", "trj" };
            string sourcePath = Path.Combine(Constants.TRJ_DIR, "exp2");
            string targetPath = Path.Combine(Constants.TRJ_DIR, "exp4/part2/");
            string bctPath = Path.Combine(Constants.TRJ_DIR, "exp4/bct/");
            int targetNum = 200;
            //string[] trjFileNames = Directory.GetFiles(Path.Combine(targetPath, dirs[0]));
            string[] trjFileNames = Directory.GetFiles(targetPath);
            // bounding box size : 116.286, 39.985 116.384, 39.938
            Envelope box = new Envelope(116.286, 116.384, 39.985, 39.938);
            List<Trajectory> trjs = new List<Trajectory>();
            List<BCompressedTrj> bcts = new List<BCompressedTrj>();
            Graph g = MapLoader.Load("beijing_2011");
            TrjCompressor compressor = new TrjCompressor(g, 50);
            int count = 0;
            //foreach (var trjFileName in trjFileNames)
            Parallel.ForEach(trjFileNames, (trjFileName) =>
            {
                Trajectory trj = new Trajectory(trjFileName, g);
                string fileName = Path.GetFileNameWithoutExtension(trjFileName);
                var newTrj = compressor.BeaconBasedCompress(trj);
                string targetFileName = Path.Combine(bctPath, string.Format("{0}.bct", fileName));
                newTrj.Save(targetFileName);
                //bcts.Add(newTrj);
                //trjs.AddRange(trj.Truncate(box));
            });
            for (int i = 0; i < bcts.Count; ++i)
            {
                string targetFileName = Path.Combine(bctPath, string.Format("{0}.trj", i + 1));
                bcts[i].Save(targetFileName);
            }
            //for (int i = 0; i < trjs.Count; ++i)
            //{
            //    string targetFileName = Path.Combine(targetPath, string.Format("{0}.trj", i + 1));
            //    trjs[i].Save(targetFileName);
            //}
        }
        static long GetDirectorySize(string p)
        {
            // 1.
            // Get array of all file names.
            string[] a = Directory.GetFiles(p, "*.*");

            // 2.
            // Calculate total bytes of all files in a loop.
            long b = 0;
            foreach (string name in a)
            {
                // 3.
                // Use FileInfo to get length of each file.
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            // 4.
            // Return total size
            return b;
        }
        public static void shpTest()
        {
            string mapDirName = "Beijing_2011_Unidirectional";
            string newMapDirName = "Beijing_2011_Unidirectional2";
            string orginalMapDir = Path.Combine(Constants.MAP_DIR, mapDirName);
            string targetMapDir = Path.Combine(Constants.MAP_DIR, newMapDirName);

            string edgeFile = Path.Combine(orginalMapDir, "edges.txt");
            string vertexFile = Path.Combine(orginalMapDir, "vertices.txt");
            string geoFile = Path.Combine(orginalMapDir, "geos.txt");
            Graph g = new Graph(vertexFile, edgeFile, geoFile);
            string targetShpFileName = Path.Combine(targetMapDir, "Beijing");
            double offsetLat = -0.001245, offsetLng = -0.006200;
            //double offsetLat = -0.001247, offsetLng = -0.005937;
            g.SaveAsShpFile(targetShpFileName, offsetLat, offsetLng);
            g.SaveMapData(targetMapDir, false, offsetLat, offsetLng);
        }
        public static void compressTest()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            String trjFileName = Path.Combine(Constants.TRJ_DIR, "1000099465.trj");
            String matchedTrjFileName = Path.Combine(Constants.TRJ_DIR, "1000099465_matched.trj");
            Trajectory trj = new Trajectory(trjFileName, 1);
            // map matching
            MM mm = new MM(g);
            trj = mm.match(trj);
            trj.Save(matchedTrjFileName, 2);
            // compress
            TrjCompressor compressor = new TrjCompressor(g, 25);
            var compressTrj = compressor.VelocityBasedCompress(trj);

            Console.WriteLine(compressTrj.Items.Count);
            String targetFile = Path.Combine(Constants.TRJ_DIR, "foo2.ctrj");
            String targetFile2 = Path.Combine(Constants.TRJ_DIR, "foo2.trj");
            // Save the compressed trj
            StreamWriter sw = new StreamWriter(targetFile);
            sw.Write(compressTrj.Serialize());
            sw.Close();
            // Decompress the trajectory
            Trajectory origTrj = compressor.Decompress(compressTrj);
            origTrj.Save(targetFile2, 0);
            // print deviation
            double totalDev = 0;
            for (int i = 0; i < origTrj.Count; ++i)
            {
                double dev = GeoPoint.GetPreciseDistance(origTrj[i].point, trj[i].point);
                totalDev += dev;
                Console.WriteLine("{0}:{1}", i, dev);
            }
            Console.WriteLine("Ave dev:{0}", totalDev / origTrj.Count);
        }

        public static void devTest()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            int a = 3 > 4 ? 5 : 4;
            string trjPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/100/matched");
            string ctrjPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/100/dp");
            string[] trjFileNames = Directory.GetFiles(trjPath, "*.trj");
            //double[] devs = new double[] { 5, 10, 20, 30, 50, 100, 200 };
            double[] devs = new double[] { 70, 100 };
            double[] averageDevs = new double[2] { 0, 0 };
            foreach (var trjFileName in trjFileNames)
            {
                string moid = Path.GetFileNameWithoutExtension(trjFileName);
                Trajectory trj = new Trajectory(trjFileName, g);
                for (int i = 0; i < devs.Length; ++i)
                {
                    TrjCompressor compressor = new TrjCompressor(g, devs[i]);
                    // bct
                    var newTrj = compressor.VelocityBasedCompress(trj);
                    var newTrj2 = compressor.Decompress(newTrj);

                    //var newTrj = compressor.DPCompress(trj);
                    // calculate average dev
                    double dev = 0;
                    for (int j = 0; j < trj.Count; ++j)
                    {
                        GeoPoint newPoint = trj[j].point;
                        if (trj[j].e != null)
                        {
                            newPoint = trj[j].e.projectFrom(newPoint);
                        }
                        double distance = GeoPoint.GetDistance(newTrj2[j].point, newPoint);
                        dev += distance;
                    }
                    Console.WriteLine("max dev:{0}, ave dev:{1}", devs[i], dev / trj.Count);
                    averageDevs[i] += dev / trj.Count;
                }
            }
            Console.WriteLine(averageDevs[0]);
            Console.WriteLine(averageDevs[1]);
        }
        public static void devTest2()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            string trjPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/100/matched");
            string ctrjPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/100/dp");
            string[] trjFileNames = Directory.GetFiles(trjPath, "*.trj");
            //double[] devs = new double[] { 5, 10, 20, 30, 50, 100, 200 };
            double[] devs = new double[] { 70, 100 };
            double[] averageDevs = new double[2] { 0, 0 };
            foreach (var trjFileName in trjFileNames)
            {
                string moid = Path.GetFileNameWithoutExtension(trjFileName);
                Trajectory trj = new Trajectory(trjFileName, g);
                for (int i = 0; i < devs.Length; ++i)
                {
                    TrjCompressor compressor = new TrjCompressor(g, devs[i]);
                    // bct
                    //var newTrj = compressor.BeaconBasedCompress(trj);
                    var newTrj = compressor.DPCompress(trj);
                    // calculate average dev
                    double dev = 0;
                    for (int j = 0; j < trj.Count; ++j)
                    {
                        var dpMv = newTrj.At(trj[j].t);
                        double distance = GeoPoint.GetDistance(dpMv.point, trj[j].point);
                        dev += distance;
                    }
                    Console.WriteLine("max dev:{0}, ave dev:{1}", devs[i], dev / trj.Count);
                    averageDevs[i] += dev / trj.Count;
                }
            }
            Console.WriteLine(averageDevs[0]);
            Console.WriteLine(averageDevs[1]);
        }
        public static void speedTest()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            string trjPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/100/matched");
            string[] trjFileNames = Directory.GetFiles(trjPath, "*.trj");
            Stopwatch sw = new Stopwatch();
            int queryCount = 0;
            int resultCount = 0;
            double totalDist = 0;
            foreach (var trjFileName in trjFileNames)
            {
                string moid = Path.GetFileNameWithoutExtension(trjFileName);
                Trajectory trj = new Trajectory(trjFileName, g);
                sw.Start();
                for (int i = 0; i < trj.Count; ++i)
                {
                    var res = g.VertexRangeQuery(trj[i].point, 30);
                    double dist = double.MaxValue;
                    foreach (var v in res)
                    {
                        double tmpDist = GeoPoint.GetDistance(v.Point, trj[i].point);
                        if (tmpDist < dist)
                        {
                            dist = tmpDist;
                        }
                    }
                    if (!double.IsInfinity(dist))
                        totalDist += dist;
                    resultCount += res.Count;
                }
                sw.Stop();
                queryCount += trj.Count;
                //if (queryCount > 10000) break;
            }
            long elapsed = sw.ElapsedMilliseconds;
            Console.WriteLine("Average query time: {0}ms, {1}/s", elapsed / queryCount, queryCount * 1000.0 / elapsed);
            Console.WriteLine("Total Dist:{0}", totalDist);
            Console.WriteLine("Result count:{0}", resultCount);
        }
        public static void compressTest2()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            string trjPath = Path.Combine(Constants.TRJ_DIR, "exp1");
            string[] trjFileNames = Directory.GetFiles(trjPath, "*.trj");
            int count = 0;
            foreach (var trjFileName in trjFileNames)
            //Parallel.ForEach(trjFileNames, (trjFileName) =>
            {
                string moid = Path.GetFileNameWithoutExtension(trjFileName);
                string ctrjFileName = Path.Combine(trjPath, string.Format("{0}.ctrj", moid));
                Trajectory trj = new Trajectory(trjFileName, 1);
                // map matching
                MM mm = new MM(g);
                trj = mm.match(trj);
                // compress
                TrjCompressor compressor = new TrjCompressor(g, 30);
                var compressTrj = compressor.VelocityBasedCompress(trj);

                // Save the compressed trj
                StreamWriter sw = new StreamWriter(ctrjFileName);
                sw.Write(compressTrj.Serialize());
                sw.Close();
                ++count;
                Console.WriteLine("{0}:{1}", count, ctrjFileName);
                // Decompress the trajectory
                //Trajectory origTrj = compressor.Decompress(compressTrj);
                //// print deviation
                //double totalDev = 0;
                //for (int i = 0; i < origTrj.Count; ++i)
                //{
                //    double dev = GeoPoint.GetPreciseDistance(origTrj[i].point, trj[i].point);
                //    totalDev += dev;
                //    //Console.WriteLine("{0}:{1}", i, dev);
                //}
                //Console.WriteLine("Ave dev:{0}", totalDev / origTrj.Count);
            }
            //);
        }
        public static void samplingTest()
        {
            string trjPath = Path.Combine(Constants.TRJ_DIR, "exp2/matched");
            string ctrjPath = Path.Combine(Constants.TRJ_DIR, "exp2/dp");
            string[] trjFileNames = Directory.GetFiles(trjPath, "*.trj");

        }
        public static void compressTest3()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            string trjPath = Path.Combine(Constants.TRJ_DIR, "exp2/matched");
            string ctrjPath = Path.Combine(Constants.TRJ_DIR, "exp2/ctrj");
            string[] trjFileNames = Directory.GetFiles(trjPath, "*.trj");
            int count = 0;
            foreach (var trjFileName in trjFileNames)
            {
                string moid = Path.GetFileNameWithoutExtension(trjFileName);
                string ctrjFileName = Path.Combine(ctrjPath, string.Format("{0}.ctrj", moid));
                if (File.Exists(ctrjFileName)) continue;
                Trajectory trj = new Trajectory(trjFileName, g);
                // compress
                TrjCompressor compressor = new TrjCompressor(g, 30);
                var compressTrj = compressor.VelocityBasedCompress(trj);
                // Save the compressed trj
                StreamWriter sw = new StreamWriter(ctrjFileName);
                sw.Write(compressTrj.Serialize());
                sw.Close();
                ++count;
                Console.WriteLine("{0}:{1}", count, ctrjFileName);
                // Decompress the trajectory
                //Trajectory origTrj = compressor.Decompress(compressTrj);
                //// print deviation
                //double totalDev = 0;
                //for (int i = 0; i < origTrj.Count; ++i)
                //{
                //    double dev = GeoPoint.GetPreciseDistance(origTrj[i].point, trj[i].point);
                //    totalDev += dev;
                //    //Console.WriteLine("{0}:{1}", i, dev);
                //}
                //Console.WriteLine("Ave dev:{0}", totalDev / origTrj.Count);
            }
            //);
        }
        public static void samplingTest2()
        {
            string[] dirs = new string[] { "matched", "bct", "vct", "dp" };
            string[] exts = new string[] { "trj", "bct", "ctrj", "trj" };
            string sourcePath = Path.Combine(Constants.TRJ_DIR, "exp2");
            int targetNum = 200;
            string targetPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/" + targetNum);
            string[] trjFileNames = Directory.GetFiles(Path.Combine(sourcePath, dirs[0]));
            int stepSize = trjFileNames.Length / targetNum;
            Random rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < trjFileNames.Length; i += stepSize)
            {
                // copy
                int randNum = Math.Min(trjFileNames.Length - 1, rand.Next() % stepSize + i);
                string fileName = Path.GetFileNameWithoutExtension(trjFileNames[randNum]);
                for (int j = 0; j < dirs.Length; ++j)
                {
                    string ext = exts[j];
                    string dir = dirs[j];
                    string srcFileName = Path.Combine(sourcePath, dir, fileName + "." + ext);
                    string targetFileName = Path.Combine(targetPath, dir, fileName + "." + ext);
                    File.Copy(srcFileName, targetFileName);
                }
            }
        }
        public static void sizeStat()
        {
            //string[] dirs = new string[] { "matched", "bct", "vct", "dp" };
            //string[] exts = new string[] { "trj", "bct", "ctrj", "trj" };
            string targetPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/");
            string[] subPaths = Directory.GetDirectories(targetPath, "200");
            string[] dirs = new string[] { "matched", "vct", "bct", "dp" };
            foreach (var subPath in subPaths)
            {
                foreach (var file in dirs)
                {
                    Console.Write("{0}\t", GetDirectorySize(Path.Combine(subPath, file)));
                    FileInfo fi = new FileInfo(Path.Combine(subPath, file + ".7z"));
                    Console.Write("{0}\t", fi.Length);
                }
                Console.WriteLine();
            }
        }
        public static void compressTest5()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            string trjPath = Path.Combine(Constants.TRJ_DIR, "exp2/matched");
            string ctrjPath = Path.Combine(Constants.TRJ_DIR, "exp2/bct");
            string[] trjFileNames = Directory.GetFiles(trjPath, "*.trj");
            int count = 0;
            foreach (var trjFileName in trjFileNames)
            {
                string moid = Path.GetFileNameWithoutExtension(trjFileName);
                string ctrjFileName = Path.Combine(ctrjPath, string.Format("{0}.bct", moid));
                Trajectory trj = new Trajectory(trjFileName, g);
                // compress
                BCompressedTrj ctrj = new BCompressedTrj(ctrjFileName, g);
                Debug.Assert(trj.Count == ctrj.Items.Count);
                double totalDev = 0;
                for (int i = 0; i < trj.Count; ++i)
                {
                    GeoPoint p1 = trj[i].point, p2;
                    if (ctrj.Items[i].e == null)
                    {
                        p2 = ctrj.Items[i].Point;
                    }
                    else
                    {
                        p2 = ctrj.Items[i].e.Predict(ctrj.Items[i].e.Start.Point, ctrj.Items[i].segmentID * ctrj.SegmentLength);
                    }
                    double dist = GeoPoint.GetDistance(p1, p2);
                    totalDev += dist;
                }
                Console.WriteLine(totalDev / trj.Count);
            }
        }
        public static void compressTest6()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            string trjPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/100/matched");
            string ctrjPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/100/dp");
            string[] trjFileNames = Directory.GetFiles(trjPath, "*.trj");
            //double[] devs = new double[] { 5, 10, 20, 30, 50, 100, 200 };
            double[] devs = new double[] { 500 };
            foreach (var trjFileName in trjFileNames)
            {
                string moid = Path.GetFileNameWithoutExtension(trjFileName);
                Trajectory trj = new Trajectory(trjFileName, g);
                for (int i = 0; i < devs.Length; ++i)
                {
                    TrjCompressor compressor = new TrjCompressor(g, devs[i]);
                    string dir = ctrjPath + "_" + devs[i];
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    // bct
                    string ctrjFileName = Path.Combine(dir, string.Format("{0}.trj", moid));
                    //var newTrj = compressor.BeaconBasedCompress(trj);
                    var newTrj = compressor.DPCompress(trj);
                    newTrj.Save(ctrjFileName);
                }
            }
        }
        public static void efficienceTest()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            Stopwatch sw = new Stopwatch();
            string trjPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/100/matched");
            string ctrjPath = Path.Combine(Constants.TRJ_DIR, "exp2/dataset/100/vct");
            string[] trjFileNames = Directory.GetFiles(trjPath, "*.trj");
            foreach (var trjFileName in trjFileNames)
            {
                sw.Start();
                string moid = Path.GetFileNameWithoutExtension(trjFileName);
                Trajectory trj = new Trajectory(trjFileName, g);
                TrjCompressor compressor = new TrjCompressor(g, 50);
                string dir = ctrjPath;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                // bct
                string ctrjFileName = Path.Combine(dir, string.Format("{0}.trj", moid));
                var newTrj = compressor.VelocityBasedCompress(trj);
                //var newTrj = compressor.BeaconBasedCompress(trj);
                //var newTrj = compressor.DPCompress(trj);
                sw.Stop();
                //newTrj.Save(ctrjFileName);
            }
            Console.WriteLine("Elapsed Time:{0}", sw.ElapsedMilliseconds);
        }
        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
        public static void shortenEdgeId()
        {
            string DATASET_DIR = Path.Combine(Constants.MAP_DIR, "Beijing_2011");
            string edgeFile = Path.Combine(DATASET_DIR, "edges.txt");
            string edgeFile2 = Path.Combine(DATASET_DIR, "edges2.txt");
            string geoFile = Path.Combine(DATASET_DIR, "geos.txt");
            string geoFile2 = Path.Combine(DATASET_DIR, "geos2.txt");
            // edge files
            StreamReader sr = new StreamReader(edgeFile);
            StreamWriter sw = new StreamWriter(edgeFile2);
            int count = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] fields = line.Split('\t');
                ++count;
                fields[0] = count.ToString();
                sw.WriteLine(String.Join("\t", fields));
                //Debug.Assert(fields.Length == 6);
            }
            sw.Close();
            sr.Close();
            sr = new StreamReader(geoFile);
            sw = new StreamWriter(geoFile2);
            count = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] fields = line.Split('\t');
                ++count;
                // swap
                fields[0] = count.ToString();
                sw.WriteLine(String.Join("\t", fields));
            }
            sw.Close();
            sr.Close();
        }
        public static void convertToUndirected()
        {
            string DATASET_DIR = Path.Combine(Constants.MAP_DIR, "Beijing_2011_new");
            string edgeFile = Path.Combine(DATASET_DIR, "edges.txt");
            string edgeFile2 = Path.Combine(DATASET_DIR, "edges2.txt");
            string geoFile = Path.Combine(DATASET_DIR, "geos.txt");
            string geoFile2 = Path.Combine(DATASET_DIR, "geos2.txt");
            // edge files
            StreamReader sr = new StreamReader(edgeFile);
            StreamWriter sw = new StreamWriter(edgeFile2);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] fields = line.Split('\t');
                //Debug.Assert(fields.Length == 6);
                sw.WriteLine(line);
                sw.WriteLine("1{0}\t{1}\t{2}", fields[0], fields[2], fields[1]);
            }
            sw.Close();
            sr.Close();
            sr = new StreamReader(geoFile);
            sw = new StreamWriter(geoFile2);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] fields = line.Split('\t');
                // swap
                int i = 0, j = (fields.Length - 1) / 2 - 1;
                while (i < j)
                {
                    Swap(ref fields[2 * i + 1], ref fields[2 * j + 1]);
                    Swap(ref fields[2 * i + 2], ref fields[2 * j + 2]);
                    ++i;
                    --j;
                }
                fields[0] = "1" + fields[0];
                sw.WriteLine(line);
                sw.WriteLine(String.Join("\t", fields));
            }
            sw.Close();
            sr.Close();
        }
        public static void mmTest()
        {
            Graph g = MapLoader.Load("WA");
            MM mm = new MM(g);
            Trajectory trj = new Trajectory();
            trj.Load(Path.Combine(Constants.WA_TRJ_DIR, "input_01.txt"), 2, g);
            Trajectory newTrj = mm.match(trj);
            String trjFile = Path.Combine(Constants.WA_TRJ_DIR, "output_01.txt");
            newTrj.SaveForCmp(trjFile);
        }
        public static void mmTest2()
        {
            Graph g = MapLoader.Load("Beijing_2011");
            //string matchedDictFile = Path.Combine(Constants.TRJ_DIR, "exp/matched_trj.txt");
            string trjDir = Path.Combine(Constants.TRJ_DIR, "exp2/trj");
            string outputDir = Path.Combine(Constants.TRJ_DIR, "exp2/matched");
            string[] trjFileNames = Directory.GetFiles(trjDir, "*.trj");
            int count = 0;
            foreach (var trjFileName in trjFileNames)
            //Parallel.ForEach(trjFileNames, (trjFileName) =>
            {
                string moid = Path.GetFileNameWithoutExtension(trjFileName);
                string matchedTrjName = Path.Combine(outputDir, string.Format("{0}.trj", moid));
                if (!File.Exists(matchedTrjName))
                {
                    Trajectory trj = new Trajectory(trjFileName, 1);
                    // map matching
                    MM mm = new MM(g);
                    trj = mm.match(trj);
                    trj.Save(matchedTrjName);
                    // Save the compressed trj
                    ++count;
                    Console.WriteLine("{0}:{1}", count, matchedTrjName);
                }
            }
            //);
        }
        public static void mmTest3()
        {
            String[] dirs = new String[] { 
                "20121201", "20121202", "20121203", 
                "20121204", "20121205", "20121206", 
                "20121207", "20121208", "20121209", 
                "20121210", "20121211", "20121212", 
                "20121213", "20121214", "20121215", 
                "20121216", "20121217", "20121218", 
                "20121219", "20121220", "20121221", 
                "20121222", "20121223", "20121224",
                "20121225", "20121226", "20121227", 
                "20121228", "20121229", "20121230", 
                "20121231"
            };
            int start = 0, size = 10;
            Console.Write("Start:");
            start = int.Parse(Console.ReadLine());
            Console.Write(" size:");
            size = int.Parse(Console.ReadLine());
            Graph g = MapLoader.Load("Beijing_2011_Unidirectional2");
            //string matchedDictFile = Path.Combine(Constants.TRJ_DIR, "exp/matched_trj.txt");
            //string baseTrjDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData_byid");
            String baseTrjDir = @"D:\download\data\beijing_gps_byid";
            string baseOutputDir = Path.Combine(Constants.TRJ_DIR, "mm_result");
            //String baseTrjDir = @"D:\download\data\beijing_gps";
            //String baseTrjDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData");
            foreach (var dir in dirs.Skip(start).Take(size))
            {
                String trjDir = Path.Combine(baseTrjDir, dir);
                String outputDir = Path.Combine(baseOutputDir, dir);
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }
                string[] trjFileNames = Directory.GetFiles(trjDir, "*.trj");
                int count = 0;
                //foreach (var trjFileName in trjFileNames)
                Parallel.ForEach(trjFileNames, (trjFileName) =>
                {
                    string moid = Path.GetFileNameWithoutExtension(trjFileName);
                    string matchedTrjName = Path.Combine(outputDir, string.Format("{0}.trj", moid));
                    Interlocked.Increment(ref count);
                    if (!File.Exists(matchedTrjName))
                    {
                        try
                        {
                            Trajectory trj = new Trajectory(trjFileName, 1);
                            // map matching
                            MM mm = new MM(g);
                            trj = mm.match(trj);
                            trj.Save(matchedTrjName, 1);
                            // Save the compressed trj
                            Console.WriteLine("{0}:{1}", count, matchedTrjName);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                });
            }
        }
        struct TrjItem
        {
            public string id;
            public double lat;
            public double lng;
            public String t;
            public float v;
        }
        public static void trjTest()
        {
            String trjDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData/20121231");
            String targetDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData_moid");
            String[] trjFiles = Directory.GetFiles(trjDir, "*.txt");
            Dictionary<String, List<TrjItem>> dict = new Dictionary<string, List<TrjItem>>();
            List<TrjItem> items = new List<TrjItem>();
            int batchSize = 1450, totalSize = trjFiles.Length;
            //int batchSize = 470, totalSize = 100;
            Array.Sort(trjFiles);
            for (int i = 0; i < totalSize; i += batchSize)
            {
                for (int j = i; j < Math.Min(i + batchSize, totalSize); ++j)
                {
                    String trjFile = trjFiles[j];
                    processTrjFile(trjFile, dict);
                    if (j % 10 == 0)
                    {
                        Console.WriteLine("Progress:{0:##.#}%", (j + 1) * 100.0 / trjFiles.Length);
                    }
                }
                writeResult(targetDir, dict);
                dict.Clear();
            }
            Console.WriteLine("{0} cars", dict.Count);
        }
        public static void trjTest2()
        {
            String trjDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData/20121231");
            String targetDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData_moid");
            String[] trjFiles = Directory.GetFiles(trjDir, "*.txt");
            List<TrjItem> items = new List<TrjItem>();
            int batchSize = 470, totalSize = trjFiles.Length;
            Array.Sort(trjFiles);
            for (int i = 0; i < totalSize; i += batchSize)
            {
                for (int j = i; j < Math.Min(i + batchSize, totalSize); ++j)
                {
                    String trjFile = trjFiles[j];
                    processTrjFile(trjFile, items);
                    if (j % 10 == 0)
                    {
                        Console.WriteLine("Progress:{0:##.#}%", (j + 1) * 100.0 / trjFiles.Length);
                    }
                }
                writeResult(targetDir, items);
                items.Clear();
            }
        }
        public static void trjTest3()
        {
            String trjDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData/20121231");
            String targetDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData_moid");
            String[] trjFiles = Directory.GetFiles(trjDir, "*.txt");
            Dictionary<String, StringBuilder> dict = new Dictionary<String, StringBuilder>();
            List<TrjItem> items = new List<TrjItem>();
            int batchSize = 1450, totalSize = trjFiles.Length;
            //int batchSize = 470, totalSize = 100;
            Array.Sort(trjFiles);
            for (int i = 0; i < totalSize; i += batchSize)
            {
                for (int j = i; j < Math.Min(i + batchSize, totalSize); ++j)
                {
                    String trjFile = trjFiles[j];
                    processTrjFile(trjFile, dict);
                    if (j % 10 == 0)
                    {
                        Console.WriteLine("Progress:{0:##.#}%", (j + 1) * 100.0 / trjFiles.Length);
                    }
                }
                writeResult(targetDir, dict);
                dict.Clear();
            }
            Console.WriteLine("{0} cars", dict.Count);
        }

        public static void trjTest4()
        {
            //String baseTrjDir = @"D:\download\data\beijing_gps";
            String baseTrjDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData");
            //String baseTragetDir = @"D:\download\data\beijing_gps_byid";
            String baseTragetDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData_byid");
            String[] dirs = new String[] { "20121231", "20121202", "20121203", 
                "20121204", "20121205", "20121206", 
                "20121207", "20121208", "20121209", 
                "20121210", "20121211", "20121212", 
                "20121213", "20121214", "20121215", 
                "20121216", "20121217", "20121218", 
                "20121219", "20121220", "20121221", 
                "20121222", "20121223", "20121224",
                "20121225", "20121226", "20121227", 
                "20121228", "20121229", "20121230", 
                "20121231"};
            foreach (var dir in dirs)
            {
                String trjDir = Path.Combine(baseTrjDir, dir);
                String targetDir = Path.Combine(baseTragetDir, dir);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                Console.WriteLine("Processing {0} ...", trjDir);
                String[] trjFiles = Directory.GetFiles(trjDir, "*.txt");
                Dictionary<String, StringBuilder> dict = new Dictionary<String, StringBuilder>();
                List<TrjItem> items = new List<TrjItem>();
                int batchSize = 10000, totalSize = trjFiles.Length;
                //int batchSize = 470, totalSize = 100;
                Array.Sort(trjFiles);
                for (int i = 0; i < totalSize; i += batchSize)
                {
                    for (int j = i; j < Math.Min(i + batchSize, totalSize); ++j)
                    {
                        String trjFile = trjFiles[j];
                        processTrjFile(trjFile, dict);
                        if (j % 10 == 0)
                        {
                            Console.WriteLine("Progress:{0:##.#}%", (j + 1) * 100.0 / trjFiles.Length);
                        }
                    }
                    writeResult(targetDir, dict);
                    dict.Clear();
                }
                Console.WriteLine("{0} cars", dict.Count);
            }
        }
        public static void queryTest()
        {
            String baseTrjDir = @"D:\download\data\beijing_gps_byid";
            String[] dirs = new String[] { "20121201", "20121202", "20121203", 
                "20121204", "20121205", "20121206", 
                "20121207", "20121208", "20121209", 
                "20121210", "20121211", "20121212", 
                "20121213", "20121214", "20121215", 
                "20121216", "20121217", "20121218", 
                "20121219", "20121220", "20121221", 
                "20121222", "20121223", "20121224",
                "20121225", "20121226", "20121227", 
                "20121228", "20121229", "20121230", 
                "20121231"};
            int dirLen = dirs.Length;
            for (int i = 0; i < dirLen; ++i)
            {
                processQueryDir(Path.Combine(baseTrjDir, dirs[i]));
            }
        }
        private static void mergeTrjTest()
        {
            String srcTrjDir = Path.Combine(Constants.TRJ_DIR, "mm_result", "20121201");
            String targetFile = Path.Combine(Constants.TRJ_DIR, "20121201.txt");
            String[] fileNames = Directory.GetFiles(srcTrjDir, "*.trj");
            List<Tuple<String, String>> data = new List<Tuple<string, string>>();
            //for (int i = 0; i < fileNames.Length; ++i)
            for (int i = 0; i < 10000; ++i)
            {
                String moid = Path.GetFileNameWithoutExtension(fileNames[i]);
                String[] lines = File.ReadAllLines(fileNames[i]);
                for (int j = 0; j < lines.Length; ++j)
                {
                    // 10098234552,39.452834,116.87999,20121201235832,34.2,0,10894
                    // 20121201002602,39.9956780,116.3369290,0.00,309,0,4,10894
                    String[] fields = lines[j].Split(',');
                    data.Add(
                        new Tuple<String, String>(fields[0],
                        String.Format("{0},{1},{2},{3},{4},{5},{6}",
                        moid, fields[1], fields[2], fields[0], fields[3], fields[5], fields[7])));
                }
            }
            //data.Sort(new Comparison<Tuple<string, string>>((a, b) => a.Item1.CompareTo(b.Item1)));
            data.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Count; ++i)
            {
                sb.AppendLine(data[i].Item2);
            }
            File.WriteAllText(targetFile, sb.ToString());
        }
        private static void processQueryDir(string dir)
        {
            String[] fileNames = Directory.GetFiles(dir, "*.trj");
            String targetFileName = Path.Combine(Constants.DATA_DIR, "query", Path.GetFileNameWithoutExtension(dir) + ".qry");
            StreamWriter sw = new StreamWriter(targetFileName);
            int threadCount = 20, fileCount = fileNames.Length;
            int partSize = (int)Math.Ceiling((double)fileCount / threadCount);
            StringBuilder[] sbs = new StringBuilder[threadCount];
            for (int j = 0; j < threadCount; ++j)
            {
                sbs[j] = new StringBuilder();
            }
            ParallelOptions op = new ParallelOptions();
            int count = 0;
            op.MaxDegreeOfParallelism = threadCount;
            Parallel.For(0, threadCount, op, j =>
            {
                for (int i = j * partSize; i < Math.Min((j + 1) * partSize, fileCount); ++i)
                {
                    procesQueryFile(fileNames[i], sbs[j]);
                    Interlocked.Increment(ref count);
                    if (count % 100 == 0)
                    {
                        Console.WriteLine("{0}%:{1}", count * 100.0 / fileCount, fileNames[i]);
                    }
                }
            });
            for (int j = 0; j < threadCount; ++j)
            {
                sw.Write(sbs[j].ToString());
            }
            sw.Close();
        }
        private static void procesQueryFile(string fileName, StringBuilder sb)
        {
            String[] lines = File.ReadAllLines(fileName);
            if (lines.Length < 10) return;
            int lineCount = lines.Length;
            String startTime = String.Empty, endTime = String.Empty;
            String startLat = String.Empty, startLng = String.Empty, endLat = String.Empty, endLng = String.Empty;
            String moid = Path.GetFileNameWithoutExtension(fileName);
            int state = 0;
            int queryStartIndex = 0;
            for (int i = 0; i < lineCount; ++i)
            {
                String[] fields = lines[i].Split(',');
                if (fields.Length != 7)
                {
                    continue;
                }
                String stateStr = fields[5];
                if (state == 0 && stateStr == "1")
                {
                    state = 1;
                    queryStartIndex = i;
                    startTime = fields[0];
                    startLat = fields[1];
                    startLng = fields[2];
                }
                else if (state == 1 && stateStr == "0")
                {
                    // end point
                    state = 0;
                    endTime = fields[0];
                    endLat = fields[1];
                    endLng = fields[2];
                    //sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6}", moid, startTime, endTime, startLat + "," + ));
                    if (i - queryStartIndex > 1)
                    {
                        sb.AppendLine(String.Join(",", moid, startTime, endTime, startLat, startLng, endLat, endLng));
                    }
                }
            }
        }
        private static void writeResult(string targetDir, Dictionary<String, StringBuilder> dict)
        {
            foreach (var item in dict)
            {
                try
                {
                    string fileName = Path.Combine(targetDir, item.Key + ".trj");
                    StreamWriter sw = new StreamWriter(fileName, true);
                    sw.Write(item.Value.ToString());
                    item.Value.Clear();
                    sw.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        private static void writeResult(string targetDir, List<TrjItem> items)
        {
            var groups = items.GroupBy(i => i.id);
            foreach (var group in groups)
            {
                string fileName = Path.Combine(targetDir, group.Key + ".trj");
                StreamWriter sw = new StreamWriter(fileName, true);
                foreach (var item in group.OrderBy(i => i.t))
                {
                    sw.WriteLine("{0},{1:###.######},{2:###.######},{3:0.#}", item.t, item.lat, item.lng, item.v);
                }
                sw.Close();
            }
        }
        private static void writeResult(string targetDir, Dictionary<String, List<TrjItem>> dict)
        {
            //foreach (var moid in dict.Take(100))
            foreach (var moid in dict)
            {
                string fileName = Path.Combine(targetDir, moid.Key + ".trj");
                StreamWriter sw = new StreamWriter(fileName, true);
                foreach (var item in moid.Value.OrderBy(k => k.t))
                {
                    sw.WriteLine("{0},{1:###.######},{2:###.######},{3:0.#}", item.t, item.lat, item.lng, item.v);
                }
                sw.Close();
            }
        }
        private static void processTrjFile(string trjFile, List<TrjItem> items)
        {
            StreamReader sr = new StreamReader(trjFile);
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();
                String[] fields = line.Split(',');
                if (fields.Length < 13) continue;
                float v;
                double lat, lng;
                if (float.TryParse(fields[8], out v) &&
                double.TryParse(fields[4], out lng) &&
                double.TryParse(fields[5], out lat))
                {
                    TrjItem item = new TrjItem()
                    {
                        t = fields[2],
                        v = v,
                        lat = lat,
                        lng = lng,
                        id = fields[2]
                    };
                    items.Add(item);
                }
            }
        }
        private static void processTrjFile(string trjFile, Dictionary<string, List<TrjItem>> dict)
        {
            StreamReader sr = new StreamReader(trjFile);
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();
                String[] fields = line.Split(',');
                if (fields.Length < 13) continue;
                String moid = fields[2];
                String t = fields[3];
                float v;
                double lat, lng;
                if (float.TryParse(fields[8], out v) &&
                double.TryParse(fields[4], out lng) &&
                double.TryParse(fields[5], out lat))
                {
                    TrjItem item = new TrjItem()
                    {
                        id = moid,
                        t = t,
                        v = v,
                        lat = lat,
                        lng = lng
                    };
                    List<TrjItem> list = null;
                    if (!dict.TryGetValue(moid, out list))
                    {
                        list = new List<TrjItem>();
                        dict[moid] = list;
                    }
                    list.Add(item);
                }
            }

        }
        private static void processTrjFile(String trjFile, Dictionary<String, StringBuilder> dict)
        {
            StreamReader sr = new StreamReader(trjFile);
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();
                // 25922,$JYJ,13311022704,20121231000247,116.6467972,39.8902512,430028382,147055425,0.00,0,2,4,50#
                try
                {
                    String[] fields = line.Split(new char[] { ',' });
                    StringBuilder list = null;
                    String moid = fields[2];
                    if (!dict.TryGetValue(moid, out list))
                    {
                        list = new StringBuilder();
                        dict[moid] = list;
                    }
                    //list.AppendLine(fields[3]);
                    list.AppendFormat("{0},{1},{2},{3},{4},{5},{6}\n",
                        fields[3], fields[5], fields[4], fields[8], fields[9], fields[10], fields[11]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            sr.Close();
        }
        //private static void readBigFile(string fileName)
        //{
        //    FileInfo file = new FileInfo(fileName);
        //    string targetFileName = string.Format("{0}.out", fileName);
        //    long size = file.Length, readSize = 0;
        //    int blockSize = 200 * 1024 * 1024;
        //    int bufferSize = blockSize + 1024;
        //    StringBuilder sb = new StringBuilder();
        //    char[] buffer = new char[bufferSize];
        //    StreamReader sr = new StreamReader(fileName);
        //    while (readSize < size)
        //    {
        //        long blockSize = bufSize;
        //        sr.Read(buffer, 0, bufSize);
        //        processBlock(buffer, sb);
        //    }
        //}
        private static void processBlock(char[] buffer, long bufferSize, StringBuilder sb)
        {
            long start = 0, end = 0;
            long buffSize = buffer.Length;
            for (end = 0; end < buffSize; ++end)
            {

            }

        }

    }
}
