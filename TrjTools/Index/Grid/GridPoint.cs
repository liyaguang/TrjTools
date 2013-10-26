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
    public class GridPoint
    {
        private ILog logger = LogManager.GetLogger(typeof(GridEdge).FullName);

        private Dictionary<int, List<Vertex>> dict = new Dictionary<int, List<Vertex>>();

        private readonly int nCol;
        private readonly int nRow;
        private readonly double cellSize;
        private readonly MBR mbr;

        public GridPoint(IEnumerable<Vertex> vertices, MBR mbr, double cellSize)
        {
            nCol = (int)(Math.Ceiling(mbr.Width / cellSize));
            nRow = (int)(Math.Ceiling(mbr.Height / cellSize));
            Debug.Assert(nCol > 0 && nRow > 0);
            this.cellSize = cellSize;
            this.mbr = mbr;
            buildIndex(vertices);
        }

        private int getCell(GeoPoint p)
        {
            int row = getRow(p.Lat);
            int col = getColumn(p.Lng);
            return row * nCol + col;
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
        private void buildIndex(IEnumerable<Vertex> vertices)
        {
            //insert edges into the grid
            foreach (Vertex v in vertices)
            {
                //List<int> ids = getCells(v.ToPoint());
                int id = getCell(v.ToPoint());
                List<Vertex> list = null;
                bool got = dict.TryGetValue(id, out list);
                if (!got)
                {
                    list = new List<Vertex>();
                    dict.Add(id, list);
                }
                list.Add(v);
            }
        }
        public HashSet<Vertex> RangeQuery(MBR rect)
        {
            HashSet<Vertex> result = new HashSet<Vertex>();
            List<int> cands = getCells(rect);
            int cands_count = cands.Count;
            for (int i = 0; i < cands_count; i++)
            {
                List<Vertex> vertices = null;
                bool got = dict.TryGetValue(cands[i], out vertices);
                if (got)
                {
                    foreach (var v in vertices)
                    {
                        if (rect.Cover(v.ToPoint()))
                        {
                            result.Add(v);
                        }
                    }
                    //result.UnionWith(vertices);
                }
            }
            return result;
        }
    }
}
