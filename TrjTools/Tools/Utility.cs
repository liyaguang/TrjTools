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

namespace TrjTools.Tools
{
    public static class Utility
    {
        public const int TICKS_PER_SECOND = 10000000;
        static long baseTicks = new DateTime(1970, 1, 1, 0, 0, 0).Ticks;
        public static double refineDoubleZero(double val)
        {
            if (Math.Abs(val) < double.Epsilon)
            {
                val = double.Epsilon;
            }
            return val;
        }
        public static DateTime LongToDateTime(long seconds)
        {
            return new DateTime(seconds * TICKS_PER_SECOND);
        }
        public static DateTime IntToDateTime(int seconds)
        {
            return new DateTime(seconds * TICKS_PER_SECOND + baseTicks);
        }
        public static int DateTimeToInt(DateTime dt)
        {
            return (int)((dt.Ticks - baseTicks) / TICKS_PER_SECOND);
        }
        public static long DateTimeToLong(DateTime dt)
        {
            return dt.Ticks / TICKS_PER_SECOND;
        }
    }
}
