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
using log4net;

namespace TrjTools.MapMatching
{
    /// <summary>
    /// Abstract mapmatching base class
    /// </summary>
    public abstract class BaseMM
    {
        protected ILog logger = LogManager.GetLogger(typeof(BaseMM).FullName);
        protected Graph graph = null;
        protected Trajectory trj = null;
        public BaseMM(Graph g)
        {
            this.graph = g;
        }

        public void setTrj(Trajectory trj)
        {
            this.trj = trj;
        }

        /// <summary>
        /// Peform mapmatching on a trajectory
        /// </summary>
        /// <param name="trj"></param>
        public abstract Trajectory match(Trajectory trj);


    }
}
