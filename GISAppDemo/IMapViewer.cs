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

namespace GISAppDemo
{
    public interface IMapViewer
    {
        void drawRoute(List<CmpForm.Record> list);

        void showEdgeInfo(int eid);

        void drawStdLine(long stdEdgeId);

        void drawLine(long edgeId);

        void drawPoint(double lat, double lng);

        void ClearMap();
    }
}
