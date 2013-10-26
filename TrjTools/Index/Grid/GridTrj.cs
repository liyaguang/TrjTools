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
    public class GridTrj
    {
        private ILog logger = LogManager.GetLogger(typeof(GridEdge).FullName);

        private Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();

        private readonly int nCol;
        private readonly int nRow;
        private readonly double cellSize;
        private readonly MBR mbr;
        private Trajectory trj = null;
        #region methods
        public GridTrj(Trajectory trj, double cSize)
        {
            this.trj = trj;
            this.mbr = getMBR(trj);
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
            int count = trj.Count;
            for (int i = 0; i < count - 1; i++)
            {
                MBR mbr2 = MBR.EMPTY;
                mbr2.Include(trj[i].point);
                mbr2.Include(trj[i + 1].point);
                List<int> cids = getCells(mbr2);
                foreach(int cid in cids)
                {
                    List<int> list = null;
                    bool got = dict.TryGetValue(cid, out list);
                    if (!got)
                    {
                        list = new List<int>();
                        dict.Add(cid, list);
                    }
                    list.Add(i);
                }
            }
        }


        /// <summary>
        /// Get the edge with a distance roughly lower than radius from point p 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<int> RangeQuery(GeoPoint p, double radius)
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
            HashSet<int> result = RangeQuery(rect);
            return result;
        }
        /// <summary>
        /// Get the edge with a distance lower than radius from point p 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<int> RangeQuery(MBR rect)
        {
            HashSet<int> result = new HashSet<int>();
            List<int> cids = getCells(rect);
            int cands_count = cids.Count;
            foreach (int cid in cids)
            {
                List<int> idxs = null;
                bool got = dict.TryGetValue(cid, out idxs);
                if (got)
                {
                    foreach (int idx in idxs)
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
        #endregion methods
    }
}
