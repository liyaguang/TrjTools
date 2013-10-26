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
using System.Diagnostics;
using log4net;

namespace TrjTools.Index.Grid
{
    public class GridEdge
    {
        private ILog logger = LogManager.GetLogger(typeof(GridEdge).FullName);

        private Dictionary<int, List<Edge>> dict = new Dictionary<int, List<Edge>>();

        private readonly int nCol;
        private readonly int nRow;
        private readonly double cellSize;
        private readonly MBR mbr;

        #region methods
        public GridEdge(IEnumerable<Edge> edges, MBR mbr, double cellSize)
        {
            nCol = (int)(Math.Ceiling(mbr.Width / cellSize));
            nRow = (int)(Math.Ceiling(mbr.Height / cellSize));
            Debug.Assert(nCol > 0 && nRow > 0);
            this.cellSize = cellSize;
            this.mbr = mbr;
            buildIndex(edges);
        }
        private int getCell(GeoPoint p)
        {
            int row = getRow(p.Lat);
            int col = getColumn(p.Lng);
            return row * nCol + col;
        }
        /// <summary>
        /// Get the cells that might contain Edge e
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private List<int> getCells(Edge e)
        {
            return getCells(e.MBR);
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
        private void buildIndex(IEnumerable<Edge> edges)
        {
            //insert edges into the grid
            foreach (Edge e in edges)
            {
                List<int> ids = getCells(e);
                for (int j = 0; j < ids.Count; j++)
                {
                    List<Edge> list = null;
                    bool got = dict.TryGetValue(ids[j], out list);
                    if (!got)
                    {
                        list = new List<Edge>();
                        dict.Add(ids[j], list);
                    }
                    list.Add(e);
                }
            }
        }


        /// <summary>
        /// Get the edge with a distance roughly lower than radius from point p 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<Edge> RangeQuery(GeoPoint p, double radius)
        {
            HashSet<Edge> result = new HashSet<Edge>();
            List<int> cands = null;
            //get mbr
            double d_radius = radius * Constants.D_PER_M;	//radius in degree
            double minLat, minLng, maxLat, maxLng;
            double radius2 = radius * radius;
            minLng = p.Lng - d_radius;
            maxLng = p.Lng + d_radius;
            minLat = p.Lat - d_radius;
            maxLat = p.Lat + d_radius;
            MBR rect = new MBR(minLng, minLat, maxLng, maxLat);
            cands = getCells(rect);
            int cands_count = cands.Count;
            for (int i = 0; i < cands_count; i++)
            {
                List<Edge> edges = null;
                bool got = dict.TryGetValue(cands[i], out edges);
                if (got)
                {
                    int count = edges.Count;
                    for (int j = 0; j < count; j++)
                    {
                        if (edges[j].Dist2From(p) <= radius2)
                        {
                            result.Add(edges[j]);
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Get the edge with a distance lower than radius from point p 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public HashSet<Edge> rangeQuery(MBR mbr)
        {
            return null;
        }
        public void simpleWrite()
        {
            logger.Info("nihao");
        }
        #endregion methods
    }
}
