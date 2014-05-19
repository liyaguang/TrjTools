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
using log4net;
using TrjTools.RoadNetwork;
using System.Diagnostics;

namespace TrjTools.Index.Grid
{
    public class GridMultiTrj
    {
        private ILog logger = LogManager.GetLogger(typeof(GridMultiTrj).FullName);

        private Dictionary<int, List<long>> trjDict = new Dictionary<int, List<long>>();

        private readonly int nCol;
        private readonly int nRow;
        private readonly double cellSize;
        private readonly MBR mbr;
        private Dictionary<long, Trajectory> trjs = null;
        #region methods
        public GridMultiTrj(Dictionary<long, Trajectory> trjs, double cSize, MBR mbr)
        {
            this.trjs = trjs;
            this.mbr = mbr;
            this.cellSize = cSize * Constants.D_PER_M;
            nCol = (int)(Math.Ceiling(mbr.Width / cellSize));
            nRow = (int)(Math.Ceiling(mbr.Height / cellSize));
            Debug.Assert(nCol > 0 && nRow > 0);
            buildIndex();
        }

        private MBR getMBR(Trajectory trj)
        {
            MBR mbr = MBR.EMPTY;
            int count = trj.Count;
            foreach(MotionVector mv in trj)
            {
                mbr.Include(mv.point);
            }
            return mbr;
        }
        private int getCell(GeoPoint p)
        {
            int row = getRow(p.Lat);
            int col = getColumn(p.Lng);
            return row * nCol + col;
        }
        private List<int> getCells(MBR mbr)
        {
            List<int> rst = new List<int>();
            int c1 = getCell(mbr.TopLeft);
            int c2 = getCell(mbr.BottomRight);
            Debug.Assert(c1 <= c2);
            int c1col = c1 % nCol;
            int c2col = c2 % nCol;

            Debug.Assert(c1col <= c2col);

            int c1row = (c1 - c1col) / nCol;
            int c2row = (c2 - c2col) / nCol;
            Debug.Assert(c1row <= c2row);

            int ncol = c2col - c1col + 1;
            int nrow = c2row - c1row + 1;
            for (int i = 0; i < nrow; i++)
            {
                for (int j = 0; j < ncol; j++)
                {
                    rst.Add(c1col + j + (c1row + i) * nCol);
                }
            }
            return rst;
        }

        /// <summary>
        ///  Given a longitude, get its column index  
        /// </summary>
        /// <param name="lng"></param>
        /// <returns></returns>
        private int getColumn(double lng)
        {
            //Debug.Assert(lng >= mbr.MinLng && lng <= mbr.MaxLng);
            if (lng <= mbr.MinLng)
            {
                return 0;
            }
            if (lng >= mbr.MaxLng)
            {
                return nCol - 1;
            }
            return (int)((lng - mbr.MinLng) / cellSize);
        }
        /// <summary>
        /// Given a latitude, get its row index  
        /// </summary>
        /// <param name="lat"></param>
        /// <returns></returns>
        private int getRow(double lat)
        {
            //Debug.Assert(lat >= mbr.MinLat && lat <= mbr.MaxLat);
            if (lat <= mbr.MinLat)
            {
                return 0;
            }
            if (lat >= mbr.MaxLat)
            {
                return nRow - 1;
            }
            return (int)((lat - mbr.MinLat) / cellSize);
        }
        private void buildIndex()
        {
            //insert edges into the grid
            int threadCount = 4, partCount = (int) Math.Ceiling(trjs.Count * 1.0 / threadCount);
            Dictionary<int, List<long>>[] dicts = new Dictionary<int, List<long>>[threadCount];
            for (int i = 0; i < threadCount; ++i)
            {
                dicts[i] = new Dictionary<int, List<long>>();
            }
            ParallelOptions op = new ParallelOptions();
            op.MaxDegreeOfParallelism = threadCount;
            var tmpTrjs = trjs.ToList();
            int trjCount = trjs.Count;
            //foreach (var item in trjs)
            Parallel.For(0, threadCount, j =>
            {
                for (int k = j * partCount; k < Math.Min((j + 1) * partCount, trjCount); ++k)
                {
                    var item = tmpTrjs[k];
                    long moid = item.Key;
                    var trj = item.Value;
                    int count = trj.Count;
                    var curDict = dicts[j];
                    for (int i = 0; i < count - 1; i++)
                    {
                        MBR mbr2 = MBR.EMPTY;
                        mbr2.Include(trj[i].point);
                        mbr2.Include(trj[i + 1].point);
                        List<int> cids = getCells(mbr2);
                        foreach (int cid in cids)
                        {
                            List<long> list = null;
                            bool got = curDict.TryGetValue(cid, out list);
                            if (!got)
                            {
                                list = new List<long>();
                                curDict.Add(cid, list);
                            }
                            list.Add(moid << 16 | (uint)i);
                        }
                    }
                }
            });
            // merge
            trjDict = dicts[0];
            for (int i = 1; i < threadCount; ++i)
            {
                foreach (var item in dicts[i])
                {
                    List<long> list = null;
                    if (!trjDict.TryGetValue(item.Key, out list))
                    {
                        trjDict[item.Key] = new List<long>(item.Value);
                    }
                    else
                    {
                        list.AddRange(item.Value);
                    }
                }
            }
        }


        /// <summary>
        /// Get the edge with a distance roughly lower than radius from point p 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<long> RangeQuery(GeoPoint p, double radius)
        {
            //get mbr
            double d_radius = radius * Constants.D_PER_M;	//radius in degree
            double minLat, minLng, maxLat, maxLng;
            double radius2 = radius * radius;
            minLng = p.Lng - d_radius;
            maxLng = p.Lng + d_radius;
            minLat = p.Lat - d_radius;
            maxLat = p.Lat + d_radius;
            MBR rect = new MBR(minLng, minLat, maxLng, maxLat);
            HashSet<long> result = RangeQuery(rect);
            return result;
        }
        /// <summary>
        /// Get the edge with a distance lower than radius from point p 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<long> RangeQuery(MBR rect)
        {
            HashSet<long> result = new HashSet<long>();
            List<int> cids = getCells(rect);
            int cands_count = cids.Count;
            foreach (int cid in cids)
            {
                List<long> idxs = null;
                bool got = trjDict.TryGetValue(cid, out idxs);
                if (got)
                {
                    foreach (var idx in idxs)
                    {
                        result.Add(idx);
                    }
                }
            }
            return result;
        }
        public void simpleWrite()
        {
            logger.Info("nihao");
        }

        public void mergeWith(GridMultiTrj gridIndex)
        {
            foreach (var item in gridIndex.trjDict)
            {
                List<long> list = null;
                if (!this.trjDict.TryGetValue(item.Key, out list))
                {
                    this.trjDict[item.Key] = new List<long>(item.Value);
                }
                else
                {
                    list.AddRange(item.Value);
                }
            }
        }
        #endregion methods
    }
}
