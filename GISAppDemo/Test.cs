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
using TrjTools.RoadNetwork;
using TrjTools.MapMatching;
using System.IO;
using TrjTools.Compress;

namespace GISAppDemo
{
    static class Test
    {
        public static void Start()
        {
            //mmTest();
            //mmTest2();
            //shpTest();
            //compressTest();
            //compressTest2();
            //convertToUndirected();
            //shortenEdgeId();
        }
        public static void shpTest()
        {
            string DATASET_DIR = Path.Combine(Constants.MAP_DIR, "Beijing_2011_new");
            string DATASET_DIR2 = Path.Combine(Constants.MAP_DIR, "Beijing_2011_new");
            string edgeFile = Path.Combine(DATASET_DIR, "edges.txt");
            string vertexFile = Path.Combine(DATASET_DIR, "vertices.txt");
            string geoFile = Path.Combine(DATASET_DIR, "geos.txt");
            Graph g = new Graph(vertexFile, edgeFile, geoFile);
            string targetShpFileName = Path.Combine(DATASET_DIR, "beijing");
            //double offsetLat = -0.001245, offsetLng = -0.006200;
            double offsetLat = -0.001245, offsetLng = -0.006200;
            g.SaveAsShpFile(targetShpFileName);
            //g.SaveMapData(DATASET_DIR, false, offsetLat, offsetLng);
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
            var compressTrj = compressor.Compress(trj);

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
                var compressTrj = compressor.Compress(trj);

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
    }
}
