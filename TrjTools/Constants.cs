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

namespace TrjTools
{
    /// <summary>
    /// Constants
    /// </summary>
    public static class Constants
    {
        public const double D_PER_M = 10e-6;	//about 10e-6 degree per meter
        public const double M_PER_LAT = 1.1e5;	//about 1.1e5 meters per latitude
#warning need to confirm
        public const double M_PER_LNG = .7e5;	//about .7e5 meters per latitude in beijing 

        public const double MAX_SPEED = 30; //max speed 30m/s
        public static readonly String BASE_DIR = getBaseDir();
        public static readonly String DATA_DIR = BASE_DIR + "data/";
        public static readonly String TRJ_DIR = DATA_DIR + "trj/";
        public static readonly String MAP_DIR = DATA_DIR + "map/";
        public static readonly String MM_RESULT_DIR = TRJ_DIR + "mm_result/";
        public static readonly String RAW_TRJ_DIR = TRJ_DIR + "raw/";
        public static readonly String TEST_DIR = DATA_DIR + "test/";
        public static readonly String SAMPLE_TEST_DIR = TEST_DIR + "samples/";
        public static readonly String POS_TEST_DIR = TEST_DIR + "pos/";
        public static readonly String FLUX_TEST_DIR = TEST_DIR + "flux/";
        public static readonly String SPEED_TEST_DIR = TEST_DIR + "speed/";

        private static String getBaseDir()
        {
            //String currentDir =Path.GetDirectoryName(new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName());
            String currentDir = Directory.GetCurrentDirectory();
            int pos = currentDir.LastIndexOf("bin", StringComparison.CurrentCultureIgnoreCase);
            if (pos > 0)
            {
                currentDir = currentDir.Substring(0, pos);
            }
            if (!currentDir.EndsWith("/") && !currentDir.EndsWith("\\"))
            {
                currentDir += "/";
            }
            return currentDir;
        }
    }
}
