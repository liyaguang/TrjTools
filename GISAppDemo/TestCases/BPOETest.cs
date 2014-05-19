using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using log4net;
using TrjTools.RoadNetwork;
using TrjTools.MapMatching;
using System.Diagnostics;
using System.IO;
using GeoAPI.Geometries;
using TrjTools.Compress;
using TrjTools.Tools;
using TrjTools.Index.RTree;
using TrjTools.Index.Grid;

namespace GISAppDemo.TestCases
{
    class BPOETest
    {
        public static ILog logger = LogManager.GetLogger("info");
        static String baseTrjDir = @"D:\download\data\beijing_gps_byid";
        static String baseQueryDir = Path.Combine(Constants.DATA_DIR, "query");
        static String baseQueryResultDir = Path.Combine(Constants.DATA_DIR, "query_result");

        public static void RunTest()
        {
            //trjTest5();
            //devTest();
            //sampleTest();
            //predictTest();
            //busQueryTest();
            monthTest();
        }

        

        private static void monthTest()
        {
            // Read file
            String[] dirs = new String[] { "20120930", "20121201", };
            String targetDir = Path.Combine(baseQueryResultDir, "month");
            String targetFile = Path.Combine(baseQueryResultDir, "stat.txt");
            String[] queryFiles = new String[] { 
                "20121202","20121203", "20121204",
                "20121205","20121206","20121207",
                "20121208", "20121209", 
                "20121210", "20121211", "20121212", 
                "20121213", "20121214", "20121215", 
                "20121216", "20121217", "20121218", 
                "20121219", "20121220", "20121221", 
                "20121222", "20121223", "20121224", 
                "20121225", 
                "20121226", "20121227", "20121228", 
                "20121229", "20121230", "20121231"};
            StringBuilder sb = new StringBuilder();
            double[] devs = new double[] { 100, 200 };
            for (int i = 0; i < queryFiles.Length; ++i)
            {
                double[,] tcs = new double[dirs.Length, devs.Length];
                for (int j = 0; j < dirs.Length; ++j)
                {
                    String resultFileName = Path.Combine(targetDir, String.Format("d{0}_q{1}_dev200.csv", dirs[j], queryFiles[i]));
                    String[] lines = File.ReadAllLines(resultFileName);
                    foreach (var line in lines)
                    {
                        String[] fields = line.Split(',');
                        double startDist = double.Parse(fields[7]);
                        double endDist = double.Parse(fields[8]);
                        for (int k = 0; k < devs.Length; ++k)
                        {
                            if (startDist <= devs[k] && endDist <= devs[k])
                            {
                                ++tcs[j, k];
                            }
                        }
                    }
                    for (int k = 0; k < devs.Length; ++k)
                    {
                        tcs[j, k] /= 1.0 * lines.Length;
                    }
                }
                sb.AppendLine(String.Format("{0},{1},{2},{3},{4}", queryFiles[i], tcs[0, 0], tcs[0, 1], tcs[1, 0], tcs[1, 1]));
            }
            File.WriteAllText(targetFile, sb.ToString());

        }
        private static void predictTest()
        {
            // Read file
            String baseTrjDir = @"D:\download\data\beijing_gps_byid";
            String[] dirs = new String[] { "20121201", };
            String targetDir = Path.Combine(baseQueryResultDir, "predict2");
            //String[] queryFiles = new String[] { "20121201" };
            String[] queryFiles = new String[] { "20121208", "20121209", 
                "20121210", "20121211", "20121212", 
                "20121215", "20121222"};
            double sampleRate = 0.1, maxDev = 200;
            int dirLen = dirs.Length;
            for (int i = 0; i < dirs.Length; ++i)
            {
                String srcDir = Path.Combine(baseTrjDir, dirs[i]);
                dataQueryTest(srcDir, targetDir, queryFiles, sampleRate, maxDev);
            }
        }
        private static void devTest()
        {
            double[] devs = new double[] { 50, 100, 200, 400, 800 };
            String srcDir = Path.Combine(baseTrjDir, "20121201");
            String targetDir = Path.Combine(baseQueryResultDir, "devs");
            String queryFile = "20121208";
            double sampleRate = 0.1;
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            // load files
            var trjDict = loadMultiTrj(srcDir, sampleRate);
            // build index
            MBR mbr = new MBR(115.6, 39.5, 117.3, 40.6);
            GridMultiTrj trjIndex = new GridMultiTrj(trjDict, 150, mbr);
            // handle queries
            for (int i = 0; i < devs.Length; ++i)
            {
                String queryFileName = Path.Combine(baseQueryDir, queryFile + ".qry");
                String targetFileName = Path.Combine(targetDir,
                    String.Format("d{0}_q{1}_dev{2:#.}.csv", Path.GetFileNameWithoutExtension(srcDir), queryFile, devs[i]));
                Console.WriteLine("Src:{0}, Query:{1}", Path.GetFileNameWithoutExtension(srcDir), queryFile);
                handleQuery(trjDict, trjIndex, queryFileName, targetFileName, devs[i]);
            }
        }
        private static void sampleTest()
        {
            double[] samples = new double[] { 0.1, 0.2, 0.4 };
            String srcDir = Path.Combine(baseTrjDir, "20121201");
            String targetDir = Path.Combine(baseQueryResultDir, "samples");
            String queryFile = "20121208";
            double dev = 400;
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            // load files
            for (int i = 0; i < samples.Length; ++i)
            {
                var trjDict = loadMultiTrj(srcDir, samples[i]);
                // build index
                MBR mbr = new MBR(115.6, 39.5, 117.3, 40.6);
                GridMultiTrj trjIndex = new GridMultiTrj(trjDict, 150, mbr);
                // handle queries
                String queryFileName = Path.Combine(baseQueryDir, queryFile + ".qry");
                String targetFileName = Path.Combine(targetDir,
                    String.Format("d{0}_q{1}_dev{2:#.}.csv", Path.GetFileNameWithoutExtension(srcDir), queryFile, dev));
                Console.WriteLine("Src:{0}, Query:{1}", Path.GetFileNameWithoutExtension(srcDir), queryFile);
                handleQuery(trjDict, trjIndex, queryFileName, targetFileName, dev);
            }

        }
        private static void trjTest5()
        {
            // Read file
            String baseTrjDir = @"D:\download\data\beijing_gps_byid";
            //String baseTrjDir = @"D:\download\data\beijing_gps_byid";
            //String baseTrjDir = Path.Combine(Constants.TRJ_DIR, "Beijing_CData_byid");
            String[] dirs = new String[] { "20120930", "20121201", };
            String targetDir = Path.Combine(baseQueryResultDir, "month");
            /*
             *  "20121202", "20121203", 
                "20121204", "20121205", "20121206", 
                "20121207"
             */
            String[] queryFiles = new String[] { 
                "20121202","20121203", "20121204",
                "20121205","20121206","20121207",
                "20121208", "20121209", 
                "20121210", "20121211", "20121212", 
                "20121213", "20121214", "20121215", 
                "20121216", "20121217", "20121218", 
                "20121219", "20121220", "20121221", 
                "20121222", "20121223", "20121224", 
                "20121225", 
                "20121226", "20121227", "20121228", 
                "20121229", "20121230", "20121231"};
            double sampleRate = 0.05, maxDev = 200;
            int dirLen = dirs.Length;
            for (int i = 0; i < dirs.Length; ++i)
            {
                String srcDir = Path.Combine(baseTrjDir, dirs[i]);
                dataQueryTest(srcDir, targetDir, queryFiles, sampleRate, maxDev);
            }
        }

        private static void busQueryTest()
        {
            String srcDir = Path.Combine(baseTrjDir, "20121201");
            String queryFileName = Path.Combine(baseQueryDir, "bus_query.csv");
            String targetDir = Path.Combine(baseQueryResultDir, "bus_query");
            double maxDist = 800;
            //double sampleRate = 0.05;
            foreach (var sampleRate in new double[] { 0.05, 0.1 })
            {
                var trjDict = loadMultiTrj(srcDir, sampleRate);
                // build index
                MBR mbr = new MBR(115.6, 39.5, 117.3, 40.6);
                GridMultiTrj trjIndex = new GridMultiTrj(trjDict, 150, mbr);
                // handle queries
                {
                    String targetFileName = Path.Combine(targetDir,
                        String.Format("d{0}_bus_dev{1:#.}_sample{2:0.##}.csv", Path.GetFileNameWithoutExtension(srcDir), maxDist, sampleRate));
                    handleQuery(trjDict, trjIndex, queryFileName, targetFileName, maxDist);
                }
            }
        }
        private static void dataQueryTest(String srcDir, String targetDir, String[] queryFiles, double sampleRate, double maxDist)
        {
            // load files
            var trjDict = loadMultiTrj(srcDir, sampleRate);
            // build index
            MBR mbr = new MBR(115.6, 39.5, 117.3, 40.6);
            GridMultiTrj trjIndex = new GridMultiTrj(trjDict, 150, mbr);
            // handle queries
            for (int i = 0; i < queryFiles.Length; ++i)
            {
                String queryFileName = Path.Combine(baseQueryDir, queryFiles[i] + ".qry");
                String targetFileName = Path.Combine(targetDir,
                    String.Format("d{0}_q{1}_dev{2:#.}.csv", Path.GetFileNameWithoutExtension(srcDir), queryFiles[i], maxDist));
                Console.WriteLine("Src:{0}, Query:{1}", Path.GetFileNameWithoutExtension(srcDir), queryFiles[i]);
                handleQuery(trjDict, trjIndex, queryFileName, targetFileName, maxDist);
            }
        }

        private static void handleQuery(Dictionary<long, Trajectory> trjDict,
            GridMultiTrj index, string queryFile, string targetFileName, double maxDist)
        {
            // read queries
            String[] queryLines = File.ReadAllLines(queryFile);
            int queryCount = queryLines.Length; // sampling
            int threadCount = 24, partSize = (int)Math.Ceiling(queryCount * 1.0 / threadCount);
            StringBuilder[] sbs = new StringBuilder[threadCount];
            int processedCount = 0;
            for (int i = 0; i < threadCount; ++i)
            {
                sbs[i] = new StringBuilder();
            }
            ParallelOptions op = new ParallelOptions();
            op.MaxDegreeOfParallelism = threadCount;
            int sampleRate = 20;
            Parallel.For(0, threadCount, op, i =>
            {
                for (int j = i * partSize; j < Math.Min((i + 1) * partSize, queryCount); ++j)
                {
                    Interlocked.Increment(ref processedCount);

                    if (processedCount % 1000 == 0)
                    {
                        Console.WriteLine("querying ... {0}%", processedCount * 100.0 / queryCount);
                    }
                    if (j % sampleRate != 0)
                    {
                        continue;
                    }
                    try
                    {
                        sbs[i].AppendLine(doSingleQuery(trjDict, index, queryLines[j], maxDist));
                    }
                    catch (Exception e)
                    {
                        logger.Info(e.Message);
                        logger.Info(String.Format("{0},{1}", trjDict, queryLines[j]));
                    }
                }
            });
            StreamWriter sw = new StreamWriter(targetFileName);
            for (int i = 0; i < threadCount; ++i)
            {
                sw.Write(sbs[i].ToString());
            }
            sw.Close();
        }

        private static String doSingleQuery(Dictionary<long, Trajectory> trjDict, GridMultiTrj index, String queryLine, double maxDist = 50)
        {
            String[] fields = queryLine.Split(',');
            // build query
            GeoPoint start = new GeoPoint(double.Parse(fields[3]), double.Parse(fields[4]));
            GeoPoint end = new GeoPoint(double.Parse(fields[5]), double.Parse(fields[6]));
            //start = CNTransform.WGS2MGS(start);
            //end = CNTransform.WGS2MGS(end);
            var startRect = start.GetMBR(maxDist).ToRectangle();
            var endRect = end.GetMBR(maxDist).ToRectangle();
            var startCands = index.RangeQuery(start, maxDist);
            var endCands = index.RangeQuery(end, maxDist);
            Dictionary<long, List<long>> startDict = new Dictionary<long, List<long>>(),
                endDict = new Dictionary<long, List<long>>();
            foreach (var cand in startCands)
            {
                List<long> list = null;
                if (!startDict.TryGetValue(cand >> 16, out list))
                {
                    list = new List<long>();
                    startDict[cand >> 16] = list;
                }
                list.Add(cand);
            }
            foreach (var cand in endCands)
            {
                List<long> list = null;
                if (!endDict.TryGetValue(cand >> 16, out list))
                {
                    list = new List<long>();
                    endDict[cand >> 16] = list;
                }
                list.Add(cand);
            }
            // get distance
            Dictionary<long, Tuple<double, double>> distances = new Dictionary<long, Tuple<double, double>>();
            HashSet<long> cands = new HashSet<long>(startDict.Keys);
            cands.IntersectWith(endDict.Keys);
            double minStartDist = 9999, minEndDist = 9999;
            double minDist = double.MaxValue;
            int validCandCount = 0;
            foreach (var cand in cands)
            {
                double startDist = 9999, endDist = 9999;
                var startList = startDict[cand];
                var endList = endDict[cand];
                var trj = trjDict[cand];
                foreach (var val in startList)
                {
                    int idx = (int)(val & 0xFFFF);
                    MotionVector cur = trj[idx], next = trj[idx + 1];
                    double tmpDist = Polyline.DistFrom(cur.point, next.point, start);
                    startDist = Math.Min(tmpDist, startDist);
                }
                foreach (var val in endList)
                {
                    int idx = (int)(val & 0xFFFF);
                    MotionVector cur = trj[idx], next = trj[idx + 1];
                    double tmpDist = Polyline.DistFrom(cur.point, next.point, end);
                    endDist = Math.Min(tmpDist, endDist);
                }
                if (startDist <= maxDist && endDist <= maxDist)
                {
                    ++validCandCount;
                }
                double tmpAvgDist = Math.Sqrt(startDist * startDist + endDist * endDist);
                if (tmpAvgDist < minDist)
                {
                    minDist = tmpAvgDist;
                    minStartDist = startDist;
                    minEndDist = endDist;
                }
            }
            return String.Format("{0},{1:0.0},{2:0.0},{3}", queryLine, minStartDist, minEndDist, validCandCount);
        }

        private static Dictionary<long, Trajectory> loadMultiTrj(String trjDir, double sampleRate)
        {
            // get file list
            String[] files = Directory.GetFiles(trjDir, "*.trj");
            Dictionary<long, Trajectory> dict = new Dictionary<long, Trajectory>();
            int threadCount = 24, partCount = threadCount * 2, fileCount = (int)(files.Length * sampleRate);
            int partSize = (int)Math.Ceiling(fileCount * 1.0 / partCount);
            List<Tuple<long, Trajectory>>[] lists = new List<Tuple<long, Trajectory>>[partCount];
            int loadedCount = 0;
            ParallelOptions op = new ParallelOptions();
            op.MaxDegreeOfParallelism = threadCount;
            for (int i = 0; i < partCount; ++i)
            {
                lists[i] = new List<Tuple<long, Trajectory>>();
            }
            Parallel.For(0, partCount, op, i =>
            {
                for (int j = i * partSize; j < Math.Min((i + 1) * partSize, fileCount); ++j)
                {
                    Interlocked.Increment(ref loadedCount);
                    if (loadedCount % 100 == 0)
                    {
                        Console.WriteLine("Loading {0:0.00}% ...", loadedCount * 100.0 / fileCount);
                    }
                    long moid;
                    if (long.TryParse(Path.GetFileNameWithoutExtension(files[j]), out moid))
                    {
                        var trj = readTrj(files[j]);
                        if (trj != null)
                        {
                            lists[i].Add(new Tuple<long, Trajectory>(moid, trj));
                        }
                    }
                }
            });
            // merge
            for (int i = 0; i < threadCount; ++i)
            {
                foreach (var item in lists[i])
                {
                    dict[item.Item1] = item.Item2;
                }
            }
            return dict;
        }
        public static Trajectory readTrj(String fileName)
        {

            Trajectory trj = null;
            try
            {
                trj = new Trajectory(fileName, 1);
            }
            catch (Exception e)
            {
                logger.Info(e.Message);
            }
            return trj;
        }
        #region deprecated
        private static void handleQuery(Dictionary<long, Trajectory> trjDict,
            RTree<long> index, string queryFile, string targetFileName)
        {
            // read queries
            String[] queryLines = File.ReadAllLines(queryFile);
            int queryCount = queryLines.Length;
            int threadCount = 4, partSize = (int)Math.Ceiling(queryCount * 1.0 / threadCount);
            StringBuilder[] sbs = new StringBuilder[threadCount];
            for (int i = 0; i < threadCount; ++i)
            {
                sbs[i] = new StringBuilder();
            }
            for (int i = 0; i < threadCount; ++i)
            {
                for (int j = i * partSize; j < Math.Min((i + 1) * partSize, queryCount); ++j)
                {
                    sbs[i].AppendLine(doSingleQuery(trjDict, index, queryLines[j]));
                }
            }
            StreamWriter sw = new StreamWriter(targetFileName);
            for (int i = 0; i < threadCount; ++i)
            {
                sw.Write(sbs[i].ToString());
            }
            sw.Close();
        }

        private static String doSingleQuery(Dictionary<long, Trajectory> trjDict, RTree<long> index, String queryLine, double maxDist = 50)
        {
            String[] fields = queryLine.Split(',');
            // build query
            GeoPoint start = new GeoPoint(double.Parse(fields[3]), double.Parse(fields[4]));
            GeoPoint end = new GeoPoint(double.Parse(fields[5]), double.Parse(fields[6]));
            var startRect = start.GetMBR(maxDist).ToRectangle();
            var endRect = end.GetMBR(maxDist).ToRectangle();
            var startCands = index.Intersects(startRect);
            var endCands = index.Intersects(endRect);
            Dictionary<long, List<long>> startDict = new Dictionary<long, List<long>>(),
                endDict = new Dictionary<long, List<long>>();
            foreach (var cand in startCands)
            {
                List<long> list = null;
                if (!startDict.TryGetValue(cand >> 16, out list))
                {
                    list = new List<long>();
                    startDict[cand >> 16] = list;
                }
                list.Add(cand);
            }
            foreach (var cand in endCands)
            {
                List<long> list = null;
                if (!endDict.TryGetValue(cand >> 16, out list))
                {
                    list = new List<long>();
                    endDict[cand >> 16] = list;
                }
                list.Add(cand);
            }
            // get distance
            double startDist = 99999, endDist = 99999;
            foreach (var item in startDict)
            {
                if (endDict.ContainsKey(item.Key))
                {
                    var trj = trjDict[item.Key];
                    foreach (var val in item.Value)
                    {
                        int idx = (int)(val & 0xFFFF);
                        MotionVector cur = trj[idx], next = trj[idx + 1];
                        double tmpDist = Polyline.DistFrom(cur.point, next.point, start);
                        startDist = Math.Min(tmpDist, startDist);
                    }
                }
            }
            foreach (var item in endDict)
            {
                if (startDict.ContainsKey(item.Key))
                {
                    var trj = trjDict[item.Key];
                    foreach (var val in item.Value)
                    {
                        int idx = (int)(val & 0xFFFF);
                        MotionVector cur = trj[idx], next = trj[idx + 1];
                        double tmpDist = Polyline.DistFrom(cur.point, next.point, end);
                        endDist = Math.Min(tmpDist, endDist);
                    }
                }
            }
            return String.Format("{0},{1:0.0},{2:0.0}", queryLine, startDist, endDist);
        }

        #endregion deprecated
    }

}
