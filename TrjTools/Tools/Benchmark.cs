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
using System.IO;
using System.Diagnostics;

namespace TrjTools.Tools
{
    public class Benchmark
    {
        public struct Record
        {
            private int time;

            public int Time
            {
                get { return time; }
                set { time = value; }
            }
            private double lat, lng;

            public double Lng
            {
                get { return lng; }
                set { lng = value; }
            }

            public double Lat
            {
                get { return lat; }
                set { lat = value; }
            }
            private int edgeId, stdEdgeId;


            public int EdgeId
            {
                get { return edgeId; }
                set { edgeId = value; }
            }

            public int StdEdgeId
            {
                get { return stdEdgeId; }
                set { stdEdgeId = value; }
            }

            private double distance;

            public double Distance
            {
                get { return distance; }
                set { distance = value; }
            }

            private double confidence;

            public double Confidence
            {
                get { return confidence; }
                set { confidence = value; }
            }

            private Boolean correct;

            public Boolean Correct
            {
                get { return correct; }
                set { correct = value; }
            }

        }

        private Dictionary<string, List<Record>> result = new Dictionary<string, List<Record>>();
        //private double correctRate = 0;
        private String datasetPath = null;
        public Benchmark(String datasetPath)
        {
            this.datasetPath = datasetPath;
        }
        public List<Record> CmpFile(string inputfile, string file, string stdFile)
        {
            StreamReader inSr = new StreamReader(inputfile);
            StreamReader sr = new StreamReader(file);
            StreamReader stdSr = new StreamReader(stdFile);
            int lineNumber = 0;
            //int correctNumber = 0;
            String line, stdLine, inLine;
            List<Record> list = new List<Record>();
            while (!sr.EndOfStream && !stdSr.EndOfStream)
            {

                inLine = inSr.ReadLine();
                line = sr.ReadLine();
                stdLine = stdSr.ReadLine();
                lineNumber++;
                Record rec = new Record();
                String[] fields = inLine.Split(',');
                rec.Time = int.Parse(fields[0]);
                rec.Lat = double.Parse(fields[1]);
                rec.Lng = double.Parse(fields[2]);
                //output
                fields = line.Split(',');
                int time = int.Parse(fields[0]);
                Debug.Assert(time == rec.Time);  //时间须相同
                rec.EdgeId = int.Parse(fields[1]);
                rec.Confidence = double.Parse(fields[2]);
                //stdoutput
                fields = stdLine.Split(',');
                time = int.Parse(fields[0]);
                Debug.Assert(time == rec.Time);  //时间须相同
                rec.StdEdgeId = int.Parse(fields[1]);
                if (rec.EdgeId == rec.StdEdgeId)
                {
                    rec.Correct = true;
                }
                else
                {
                    rec.Correct = false;
                }
                if (lineNumber > 1)
                {
                    Record prev = list[lineNumber - 2];
                    rec.Distance = LayerTools.GetDistance(rec.Lng, rec.Lat, prev.Lng, prev.Lat);
                }
                else
                {
                    rec.Distance = 0;
                }
                list.Add(rec);
            }
            inSr.Close();
            sr.Close();
            stdSr.Close();

            //显示正确率
            return list;
        }

        public void CmpDateset()
        {
            String inputDir = Path.Combine(datasetPath, "input");
            String outputDir = Path.Combine(datasetPath, "output");
            String stdOutputDir = Path.Combine(datasetPath, "std_output");
            String[] inputFiles = Directory.GetFiles(inputDir, "*.txt");
            List<Record> list = new List<Record>();
            foreach (var inputFile in inputFiles)
            {
                String fileName = Path.GetFileName(inputFile);
                String outputFile = Path.Combine(outputDir, fileName.Replace("input", "output"));
                String stdOutputFile = Path.Combine(stdOutputDir, fileName.Replace("input", "output"));
                List<Record> tmpResult = CmpFile(inputFile, outputFile, stdOutputFile);
                result.Add(fileName, tmpResult);
            }
        }
        /// <summary>
        /// Calculate statistical information
        /// </summary>
        public double GetStat()
        {
            int totalRecord = 0;
            int rightRecord = 0;
            double correctRate = 0;
            if (result == null || result.Count == 0)
            {
                CmpDateset();
            }
            foreach (var pair in result)
            {
                int currentRecord = pair.Value.Count;
                int currentRightRecord = pair.Value.Where(r => r.Correct).Count();
                double currentCR = currentRightRecord * 1.0 / currentRecord;
                //Console.WriteLine("{0}\tTotal:{1}\tCorrect:{2}\tRate:{3:00.00}", pair.Key, currentRecord, currentRightRecord, currentCR * 100);
                totalRecord += currentRecord;
                rightRecord += currentRightRecord;
            }
            //Total
            correctRate = rightRecord * 1.0 / totalRecord;
            //Console.WriteLine("Total:{0}\tCorrect:{1}\tRate:{2:00.00}", totalRecord, rightRecord, correctRate * 100);
            return correctRate;
        }
    }
}
