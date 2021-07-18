﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refterm
{
    public partial class Shaders
    {
        public static readonly byte[] ReftermVSShaderBytes =
        {
             68,  88,  66,  67, 173, 161,
            191, 148, 243,  58, 210,  73,
            205,  38,  94,  72,  97,  14,
             93,  15,   1,   0,   0,   0,
            196,   1,   0,   0,   3,   0,
              0,   0,  44,   0,   0,   0,
             96,   0,   0,   0, 148,   0,
              0,   0,  73,  83,  71,  78,
             44,   0,   0,   0,   1,   0,
              0,   0,   8,   0,   0,   0,
             32,   0,   0,   0,   0,   0,
              0,   0,   6,   0,   0,   0,
              1,   0,   0,   0,   0,   0,
              0,   0,   1,   1,   0,   0,
             83,  86,  95,  86,  69,  82,
             84,  69,  88,  73,  68,   0,
             79,  83,  71,  78,  44,   0,
              0,   0,   1,   0,   0,   0,
              8,   0,   0,   0,  32,   0,
              0,   0,   0,   0,   0,   0,
              1,   0,   0,   0,   3,   0,
              0,   0,   0,   0,   0,   0,
             15,   0,   0,   0,  83,  86,
             95,  80,  79,  83,  73,  84,
             73,  79,  78,   0,  83,  72,
             69,  88,  40,   1,   0,   0,
             80,   0,   1,   0,  74,   0,
              0,   0, 106,   8,   0,   1,
             96,   0,   0,   4,  18,  16,
             16,   0,   0,   0,   0,   0,
              6,   0,   0,   0, 103,   0,
              0,   4, 242,  32,  16,   0,
              0,   0,   0,   0,   1,   0,
              0,   0, 104,   0,   0,   2,
              1,   0,   0,   0,   1,   0,
              0,   7,  18,   0,  16,   0,
              0,   0,   0,   0,  10,  16,
             16,   0,   0,   0,   0,   0,
              1,  64,   0,   0,   1,   0,
              0,   0,  86,   0,   0,   5,
             18,   0,  16,   0,   0,   0,
              0,   0,  10,   0,  16,   0,
              0,   0,   0,   0,   0,   0,
              0,   7,  18,   0,  16,   0,
              0,   0,   0,   0,  10,   0,
             16,   0,   0,   0,   0,   0,
              1,  64,   0,   0,   0,   0,
              0, 191,   0,   0,   0,   7,
             18,  32,  16,   0,   0,   0,
              0,   0,  10,   0,  16,   0,
              0,   0,   0,   0,  10,   0,
             16,   0,   0,   0,   0,   0,
             85,   0,   0,   7,  18,   0,
             16,   0,   0,   0,   0,   0,
             10,  16,  16,   0,   0,   0,
              0,   0,   1,  64,   0,   0,
              1,   0,   0,   0,  86,   0,
              0,   5,  18,   0,  16,   0,
              0,   0,   0,   0,  10,   0,
             16,   0,   0,   0,   0,   0,
              0,   0,   0,   7,  18,   0,
             16,   0,   0,   0,   0,   0,
             10,   0,  16,   0,   0,   0,
              0,   0,   1,  64,   0,   0,
              0,   0,   0, 191,  56,   0,
              0,   7,  34,  32,  16,   0,
              0,   0,   0,   0,  10,   0,
             16,   0,   0,   0,   0,   0,
              1,  64,   0,   0,   0,   0,
              0, 192,  54,   0,   0,   8,
            194,  32,  16,   0,   0,   0,
              0,   0,   2,  64,   0,   0,
              0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,
              0,   0, 128,  63,  62,   0,
              0,   1
        };


        public static readonly byte[] ReftermPSShaderBytes =
        {
             68,  88,  66,  67, 128, 220,
             99, 152, 112,  30, 119, 113,
              9, 107, 241, 182, 138,  85,
             27,   5,   1,   0,   0,   0,
              0,   8,   0,   0,   3,   0,
              0,   0,  44,   0,   0,   0,
             96,   0,   0,   0, 148,   0,
              0,   0,  73,  83,  71,  78,
             44,   0,   0,   0,   1,   0,
              0,   0,   8,   0,   0,   0,
             32,   0,   0,   0,   0,   0,
              0,   0,   1,   0,   0,   0,
              3,   0,   0,   0,   0,   0,
              0,   0,  15,   3,   0,   0,
             83,  86,  95,  80,  79,  83,
             73,  84,  73,  79,  78,   0,
             79,  83,  71,  78,  44,   0,
              0,   0,   1,   0,   0,   0,
              8,   0,   0,   0,  32,   0,
              0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   3,   0,
              0,   0,   0,   0,   0,   0,
             15,   0,   0,   0,  83,  86,
             95,  84,  65,  82,  71,  69,
             84,   0, 171, 171,  83,  72,
             69,  88, 100,   7,   0,   0,
             80,   0,   0,   0, 217,   1,
              0,   0, 106,   8,   0,   1,
             89,   0,   0,   4,  70, 142,
             32,   0,   0,   0,   0,   0,
              3,   0,   0,   0, 162,   0,
              0,   4,   0, 112,  16,   0,
              0,   0,   0,   0,  12,   0,
              0,   0,  88,  24,   0,   4,
              0, 112,  16,   0,   1,   0,
              0,   0,  85,  85,   0,   0,
            100,  32,   0,   4,  50,  16,
             16,   0,   0,   0,   0,   0,
              1,   0,   0,   0, 101,   0,
              0,   3, 242,  32,  16,   0,
              0,   0,   0,   0, 104,   0,
              0,   2,   6,   0,   0,   0,
             28,   0,   0,   5,  50,   0,
             16,   0,   0,   0,   0,   0,
             70,  16,  16,   0,   0,   0,
              0,   0,  30,   0,   0,   9,
            194,   0,  16,   0,   0,   0,
              0,   0,   6,   4,  16,   0,
              0,   0,   0,   0,   6, 132,
             32, 128,  65,   0,   0,   0,
              0,   0,   0,   0,   1,   0,
              0,   0,  78,   0,   0,   9,
             50,   0,  16,   0,   1,   0,
              0,   0,   0, 208,   0,   0,
            230,  10,  16,   0,   0,   0,
              0,   0,  70, 128,  32,   0,
              0,   0,   0,   0,   0,   0,
              0,   0,  80,   0,   0,   8,
             50,   0,  16,   0,   0,   0,
              0,   0,  70,   0,  16,   0,
              0,   0,   0,   0,  70, 128,
             32,   0,   0,   0,   0,   0,
              1,   0,   0,   0,   1,   0,
              0,   7,  18,   0,  16,   0,
              0,   0,   0,   0,  26,   0,
             16,   0,   0,   0,   0,   0,
             10,   0,  16,   0,   0,   0,
              0,   0,  79,   0,   0,   8,
            194,   0,  16,   0,   1,   0,
              0,   0,   6,   4,  16,   0,
              1,   0,   0,   0, 166, 142,
             32,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   1,   0,
              0,   7,  18,   0,  16,   0,
              0,   0,   0,   0,  10,   0,
             16,   0,   0,   0,   0,   0,
             42,   0,  16,   0,   1,   0,
              0,   0,   1,   0,   0,   7,
             18,   0,  16,   0,   0,   0,
              0,   0,  58,   0,  16,   0,
              1,   0,   0,   0,  10,   0,
             16,   0,   0,   0,   0,   0,
             31,   0,   4,   3,  10,   0,
             16,   0,   0,   0,   0,   0,
             78,   0,   0,   9,   0, 208,
              0,   0,  50,   0,  16,   0,
              0,   0,   0,   0, 230,  10,
             16,   0,   0,   0,   0,   0,
             70, 128,  32,   0,   0,   0,
              0,   0,   0,   0,   0,   0,
             35,   0,   0,  10,  66,   0,
             16,   0,   0,   0,   0,   0,
             26,   0,  16,   0,   1,   0,
              0,   0,  42, 128,  32,   0,
              0,   0,   0,   0,   0,   0,
              0,   0,  10,   0,  16,   0,
              1,   0,   0,   0, 167,   0,
              0, 139,   2,  99,   0, 128,
            131, 153,  25,   0, 114,   0,
             16,   0,   1,   0,   0,   0,
             42,   0,  16,   0,   0,   0,
              0,   0,   1,  64,   0,   0,
              0,   0,   0,   0,  70, 114,
             16,   0,   0,   0,   0,   0,
              1,   0,   0,  10, 114,   0,
             16,   0,   2,   0,   0,   0,
            134,   1,  16,   0,   1,   0,
              0,   0,   2,  64,   0,   0,
            255, 255,   0,   0, 255,   0,
              0,   0, 255,   0,   0,   0,
              0,   0,   0,   0,  85,   0,
              0,  10, 194,   0,  16,   0,
              0,   0,   0,   0,   6,   4,
             16,   0,   1,   0,   0,   0,
              2,  64,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,
             16,   0,   0,   0,  31,   0,
              0,   0,  54,   0,   0,   5,
            130,   0,  16,   0,   2,   0,
              0,   0,  42,   0,  16,   0,
              0,   0,   0,   0,  35,   0,
              0,  10,  50,   0,  16,   0,
              3,   0,   0,   0, 198,   0,
             16,   0,   2,   0,   0,   0,
             70, 128,  32,   0,   0,   0,
              0,   0,   0,   0,   0,   0,
             70,   0,  16,   0,   0,   0,
              0,   0,  54,   0,   0,   8,
            194,   0,  16,   0,   3,   0,
              0,   0,   2,  64,   0,   0,
              0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,  45,   0,
              0, 137, 194,   0,   0, 128,
             67,  85,  21,   0, 242,   0,
             16,   0,   3,   0,   0,   0,
             70,  14,  16,   0,   3,   0,
              0,   0,  70, 126,  16,   0,
              1,   0,   0,   0, 138,   0,
              0,  15, 242,   0,  16,   0,
              4,   0,   0,   0,   2,  64,
              0,   0,   8,   0,   0,   0,
              8,   0,   0,   0,   8,   0,
              0,   0,   8,   0,   0,   0,
              2,  64,   0,   0,   8,   0,
              0,   0,  16,   0,   0,   0,
              8,   0,   0,   0,  16,   0,
              0,   0, 166,   5,  16,   0,
              1,   0,   0,   0,  43,   0,
              0,   5,  18,   0,  16,   0,
              5,   0,   0,   0,  26,   0,
             16,   0,   2,   0,   0,   0,
             43,   0,   0,   5,  98,   0,
             16,   0,   5,   0,   0,   0,
              6,   1,  16,   0,   4,   0,
              0,   0,  56,   0,   0,  10,
            210,   0,  16,   0,   1,   0,
              0,   0,   6,   9,  16,   0,
              5,   0,   0,   0,   2,  64,
              0,   0, 129, 128, 128,  59,
              0,   0,   0,   0, 129, 128,
            128,  59, 129, 128, 128,  59,
             43,   0,   0,   5,  18,   0,
             16,   0,   2,   0,   0,   0,
             42,   0,  16,   0,   2,   0,
              0,   0,  43,   0,   0,   5,
             98,   0,  16,   0,   2,   0,
              0,   0, 166,  11,  16,   0,
              4,   0,   0,   0,  56,   0,
              0,  10, 114,   0,  16,   0,
              2,   0,   0,   0,  70,   2,
             16,   0,   2,   0,   0,   0,
              2,  64,   0,   0, 129, 128,
            128,  59, 129, 128, 128,  59,
            129, 128, 128,  59,   0,   0,
              0,   0,   1,   0,   0,   8,
             18,   0,  16,   0,   0,   0,
              0,   0,  42, 128,  32,   0,
              0,   0,   0,   0,   1,   0,
              0,   0,   1,  64,   0,   0,
            255,   0,   0,   0, 138,   0,
              0,  16,  50,   0,  16,   0,
              4,   0,   0,   0,   2,  64,
              0,   0,   8,   0,   0,   0,
              8,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,
              2,  64,   0,   0,   8,   0,
              0,   0,  16,   0,   0,   0,
              0,   0,   0,   0,   0,   0,
              0,   0, 166, 138,  32,   0,
              0,   0,   0,   0,   1,   0,
              0,   0,  43,   0,   0,   5,
             18,   0,  16,   0,   5,   0,
              0,   0,  10,   0,  16,   0,
              0,   0,   0,   0,  43,   0,
              0,   5,  98,   0,  16,   0,
              5,   0,   0,   0,   6,   1,
             16,   0,   4,   0,   0,   0,
             56,   0,   0,   7, 114,   0,
             16,   0,   4,   0,   0,   0,
             70,   2,  16,   0,   2,   0,
              0,   0,  70,   2,  16,   0,
              5,   0,   0,   0, 138,   0,
              0,  15, 114,   0,  16,   0,
              5,   0,   0,   0,   2,  64,
              0,   0,   1,   0,   0,   0,
              1,   0,   0,   0,   1,   0,
              0,   0,   0,   0,   0,   0,
              2,  64,   0,   0,  28,   0,
              0,   0,  25,   0,   0,   0,
             27,   0,   0,   0,   0,   0,
              0,   0,  86,   5,  16,   0,
              1,   0,   0,   0,  56,   0,
              0,  10, 114,   0,  16,   0,
              4,   0,   0,   0,  70,   2,
             16,   0,   4,   0,   0,   0,
              2,  64,   0,   0, 129, 128,
            128,  59, 129, 128, 128,  59,
            129, 128, 128,  59,   0,   0,
              0,   0,  55,   0,   0,   9,
            114,   0,  16,   0,   2,   0,
              0,   0,   6,   0,  16,   0,
              5,   0,   0,   0,  70,   2,
             16,   0,   4,   0,   0,   0,
             70,   2,  16,   0,   2,   0,
              0,   0,  56,   0,   0,  10,
            114,   0,  16,   0,   4,   0,
              0,   0,  70,   2,  16,   0,
              2,   0,   0,   0,   2,  64,
              0,   0,   0,   0,   0,  63,
              0,   0,   0,  63,   0,   0,
              0,  63,   0,   0,   0,   0,
             55,   0,   0,   9, 114,   0,
             16,   0,   2,   0,   0,   0,
             86,   5,  16,   0,   5,   0,
              0,   0,  70,   2,  16,   0,
              4,   0,   0,   0,  70,   2,
             16,   0,   2,   0,   0,   0,
              0,   0,   0,   8,  18,   0,
             16,   0,   0,   0,   0,   0,
             58,   0,  16, 128,  65,   0,
              0,   0,   3,   0,   0,   0,
              1,  64,   0,   0,   0,   0,
            128,  63,  56,   0,   0,   7,
            114,   0,  16,   0,   3,   0,
              0,   0,  70,   2,  16,   0,
              2,   0,   0,   0,  70,   2,
             16,   0,   3,   0,   0,   0,
             50,   0,   0,   9, 114,   0,
             16,   0,   1,   0,   0,   0,
              6,   0,  16,   0,   0,   0,
              0,   0, 134,   3,  16,   0,
              1,   0,   0,   0,  70,   2,
             16,   0,   3,   0,   0,   0,
             39,   0,   0,   7,  18,   0,
             16,   0,   0,   0,   0,   0,
             42,   0,  16,   0,   5,   0,
              0,   0,   1,  64,   0,   0,
              0,   0,   0,   0,  80,   0,
              0,   8,  50,   0,  16,   0,
              3,   0,   0,   0,  86,   5,
             16,   0,   0,   0,   0,   0,
             38, 138,  32,   0,   0,   0,
              0,   0,   2,   0,   0,   0,
              1,   0,   0,   7,  18,   0,
             16,   0,   0,   0,   0,   0,
             10,   0,  16,   0,   0,   0,
              0,   0,  10,   0,  16,   0,
              3,   0,   0,   0,  79,   0,
              0,   8,  98,   0,  16,   0,
              0,   0,   0,   0,  86,   5,
             16,   0,   0,   0,   0,   0,
            246, 141,  32,   0,   0,   0,
              0,   0,   2,   0,   0,   0,
              1,   0,   0,   7,  18,   0,
             16,   0,   0,   0,   0,   0,
             26,   0,  16,   0,   0,   0,
              0,   0,  10,   0,  16,   0,
              0,   0,   0,   0,  39,   0,
              0,   7,  34,   0,  16,   0,
              0,   0,   0,   0,  58,   0,
             16,   0,   0,   0,   0,   0,
              1,  64,   0,   0,   0,   0,
              0,   0,   1,   0,   0,   7,
             34,   0,  16,   0,   0,   0,
              0,   0,  26,   0,  16,   0,
              3,   0,   0,   0,  26,   0,
             16,   0,   0,   0,   0,   0,
              1,   0,   0,   7,  34,   0,
             16,   0,   0,   0,   0,   0,
             42,   0,  16,   0,   0,   0,
              0,   0,  26,   0,  16,   0,
              0,   0,   0,   0,  60,   0,
              0,   7,  18,   0,  16,   0,
              0,   0,   0,   0,  26,   0,
             16,   0,   0,   0,   0,   0,
             10,   0,  16,   0,   0,   0,
              0,   0,  55,   0,   0,   9,
            114,   0,  16,   0,   0,   0,
              0,   0,   6,   0,  16,   0,
              0,   0,   0,   0,  70,   2,
             16,   0,   2,   0,   0,   0,
             70,   2,  16,   0,   1,   0,
              0,   0,  18,   0,   0,   1,
              1,   0,   0,   8, 130,   0,
             16,   0,   0,   0,   0,   0,
             58, 128,  32,   0,   0,   0,
              0,   0,   1,   0,   0,   0,
              1,  64,   0,   0, 255,   0,
              0,   0, 138,   0,   0,  16,
             50,   0,  16,   0,   1,   0,
              0,   0,   2,  64,   0,   0,
              8,   0,   0,   0,   8,   0,
              0,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   2,  64,
              0,   0,   8,   0,   0,   0,
             16,   0,   0,   0,   0,   0,
              0,   0,   0,   0,   0,   0,
            246, 143,  32,   0,   0,   0,
              0,   0,   1,   0,   0,   0,
             43,   0,   0,   5,  18,   0,
             16,   0,   2,   0,   0,   0,
             58,   0,  16,   0,   0,   0,
              0,   0,  43,   0,   0,   5,
             98,   0,  16,   0,   2,   0,
              0,   0,   6,   1,  16,   0,
              1,   0,   0,   0,  56,   0,
              0,  10, 114,   0,  16,   0,
              0,   0,   0,   0,  70,   2,
             16,   0,   2,   0,   0,   0,
              2,  64,   0,   0, 129, 128,
            128,  59, 129, 128, 128,  59,
            129, 128, 128,  59,   0,   0,
              0,   0,  21,   0,   0,   1,
             54,   0,   0,   5, 114,  32,
             16,   0,   0,   0,   0,   0,
             70,   2,  16,   0,   0,   0,
              0,   0,  54,   0,   0,   5,
            130,  32,  16,   0,   0,   0,
              0,   0,   1,  64,   0,   0,
              0,   0, 128,  63,  62,   0,
              0,   1
        };

        public static readonly byte[] ReftermCSShaderBytes =
        {
            68,  88,  66,  67, 190, 252,
            5, 194, 210,  57, 221, 140,
            14, 251, 167, 144, 241, 177,
        127,  71,   1,   0,   0,   0,
        152,   7,   0,   0,   3,   0,
            0,   0,  44,   0,   0,   0,
            60,   0,   0,   0,  76,   0,
            0,   0,  73,  83,  71,  78,
            8,   0,   0,   0,   0,   0,
            0,   0,   8,   0,   0,   0,
            79,  83,  71,  78,   8,   0,
            0,   0,   0,   0,   0,   0,
            8,   0,   0,   0,  83,  72,
            69,  88,  68,   7,   0,   0,
            80,   0,   5,   0, 209,   1,
            0,   0, 106,   8,   0,   1,
            89,   0,   0,   4,  70, 142,
            32,   0,   0,   0,   0,   0,
            3,   0,   0,   0, 162,   0,
            0,   4,   0, 112,  16,   0,
            0,   0,   0,   0,  12,   0,
            0,   0,  88,  24,   0,   4,
            0, 112,  16,   0,   1,   0,
            0,   0,  85,  85,   0,   0,
        156,  24,   0,   4,   0, 224,
            17,   0,   0,   0,   0,   0,
            85,  85,   0,   0,  95,   0,
            0,   2,  50,   0,   2,   0,
        104,   0,   0,   2,   6,   0,
            0,   0, 155,   0,   0,   4,
            8,   0,   0,   0,   8,   0,
            0,   0,   1,   0,   0,   0,
            30,   0,   0,   8,  50,   0,
            16,   0,   0,   0,   0,   0,
            70,   0,   2,   0,  70, 128,
            32, 128,  65,   0,   0,   0,
            0,   0,   0,   0,   1,   0,
            0,   0,  78,   0,   0,   9,
        194,   0,  16,   0,   0,   0,
            0,   0,   0, 208,   0,   0,
            6,   4,  16,   0,   0,   0,
            0,   0,   6, 132,  32,   0,
            0,   0,   0,   0,   0,   0,
            0,   0,  80,   0,   0,   7,
            50,   0,  16,   0,   1,   0,
            0,   0,  70,   0,   2,   0,
            70, 128,  32,   0,   0,   0,
            0,   0,   1,   0,   0,   0,
            1,   0,   0,   7,  18,   0,
            16,   0,   1,   0,   0,   0,
            26,   0,  16,   0,   1,   0,
            0,   0,  10,   0,  16,   0,
            1,   0,   0,   0,  79,   0,
            0,   8,  98,   0,  16,   0,
            1,   0,   0,   0, 166,  11,
            16,   0,   0,   0,   0,   0,
        166, 139,  32,   0,   0,   0,
            0,   0,   0,   0,   0,   0,
            1,   0,   0,   7,  18,   0,
            16,   0,   1,   0,   0,   0,
            26,   0,  16,   0,   1,   0,
            0,   0,  10,   0,  16,   0,
            1,   0,   0,   0,   1,   0,
            0,   7,  18,   0,  16,   0,
            1,   0,   0,   0,  42,   0,
            16,   0,   1,   0,   0,   0,
            10,   0,  16,   0,   1,   0,
            0,   0,  31,   0,   4,   3,
            10,   0,  16,   0,   1,   0,
            0,   0,  78,   0,   0,   9,
            0, 208,   0,   0,  50,   0,
            16,   0,   0,   0,   0,   0,
            70,   0,  16,   0,   0,   0,
            0,   0,  70, 128,  32,   0,
            0,   0,   0,   0,   0,   0,
            0,   0,  35,   0,   0,  10,
            66,   0,  16,   0,   0,   0,
            0,   0,  58,   0,  16,   0,
            0,   0,   0,   0,  42, 128,
            32,   0,   0,   0,   0,   0,
            0,   0,   0,   0,  42,   0,
            16,   0,   0,   0,   0,   0,
        167,   0,   0, 139,   2,  99,
            0, 128, 131, 153,  25,   0,
        114,   0,  16,   0,   1,   0,
            0,   0,  42,   0,  16,   0,
            0,   0,   0,   0,   1,  64,
            0,   0,   0,   0,   0,   0,
            70, 114,  16,   0,   0,   0,
            0,   0,   1,   0,   0,  10,
        114,   0,  16,   0,   2,   0,
            0,   0, 134,   1,  16,   0,
            1,   0,   0,   0,   2,  64,
            0,   0, 255, 255,   0,   0,
        255,   0,   0,   0, 255,   0,
            0,   0,   0,   0,   0,   0,
            85,   0,   0,  10, 194,   0,
            16,   0,   0,   0,   0,   0,
            6,   4,  16,   0,   1,   0,
            0,   0,   2,  64,   0,   0,
            0,   0,   0,   0,   0,   0,
            0,   0,  16,   0,   0,   0,
            31,   0,   0,   0,  54,   0,
            0,   5, 130,   0,  16,   0,
            2,   0,   0,   0,  42,   0,
            16,   0,   0,   0,   0,   0,
            35,   0,   0,  10,  50,   0,
            16,   0,   3,   0,   0,   0,
        198,   0,  16,   0,   2,   0,
            0,   0,  70, 128,  32,   0,
            0,   0,   0,   0,   0,   0,
            0,   0,  70,   0,  16,   0,
            0,   0,   0,   0,  54,   0,
            0,   8, 194,   0,  16,   0,
            3,   0,   0,   0,   2,  64,
            0,   0,   0,   0,   0,   0,
            0,   0,   0,   0,   0,   0,
            0,   0,   0,   0,   0,   0,
            45,   0,   0, 137, 194,   0,
            0, 128,  67,  85,  21,   0,
        242,   0,  16,   0,   3,   0,
            0,   0,  70,  14,  16,   0,
            3,   0,   0,   0,  70, 126,
            16,   0,   1,   0,   0,   0,
        138,   0,   0,  15, 242,   0,
            16,   0,   4,   0,   0,   0,
            2,  64,   0,   0,   8,   0,
            0,   0,   8,   0,   0,   0,
            8,   0,   0,   0,   8,   0,
            0,   0,   2,  64,   0,   0,
            8,   0,   0,   0,  16,   0,
            0,   0,   8,   0,   0,   0,
            16,   0,   0,   0, 166,   5,
            16,   0,   1,   0,   0,   0,
            43,   0,   0,   5,  18,   0,
            16,   0,   5,   0,   0,   0,
            26,   0,  16,   0,   2,   0,
            0,   0,  43,   0,   0,   5,
            98,   0,  16,   0,   5,   0,
            0,   0,   6,   1,  16,   0,
            4,   0,   0,   0,  56,   0,
            0,  10, 210,   0,  16,   0,
            1,   0,   0,   0,   6,   9,
            16,   0,   5,   0,   0,   0,
            2,  64,   0,   0, 129, 128,
        128,  59,   0,   0,   0,   0,
        129, 128, 128,  59, 129, 128,
        128,  59,  43,   0,   0,   5,
            18,   0,  16,   0,   2,   0,
            0,   0,  42,   0,  16,   0,
            2,   0,   0,   0,  43,   0,
            0,   5,  98,   0,  16,   0,
            2,   0,   0,   0, 166,  11,
            16,   0,   4,   0,   0,   0,
            56,   0,   0,  10, 114,   0,
            16,   0,   2,   0,   0,   0,
            70,   2,  16,   0,   2,   0,
            0,   0,   2,  64,   0,   0,
        129, 128, 128,  59, 129, 128,
        128,  59, 129, 128, 128,  59,
            0,   0,   0,   0,   1,   0,
            0,   8,  18,   0,  16,   0,
            0,   0,   0,   0,  42, 128,
            32,   0,   0,   0,   0,   0,
            1,   0,   0,   0,   1,  64,
            0,   0, 255,   0,   0,   0,
        138,   0,   0,  16,  50,   0,
            16,   0,   4,   0,   0,   0,
            2,  64,   0,   0,   8,   0,
            0,   0,   8,   0,   0,   0,
            0,   0,   0,   0,   0,   0,
            0,   0,   2,  64,   0,   0,
            8,   0,   0,   0,  16,   0,
            0,   0,   0,   0,   0,   0,
            0,   0,   0,   0, 166, 138,
            32,   0,   0,   0,   0,   0,
            1,   0,   0,   0,  43,   0,
            0,   5,  18,   0,  16,   0,
            5,   0,   0,   0,  10,   0,
            16,   0,   0,   0,   0,   0,
            43,   0,   0,   5,  98,   0,
            16,   0,   5,   0,   0,   0,
            6,   1,  16,   0,   4,   0,
            0,   0,  56,   0,   0,   7,
        114,   0,  16,   0,   4,   0,
            0,   0,  70,   2,  16,   0,
            2,   0,   0,   0,  70,   2,
            16,   0,   5,   0,   0,   0,
        138,   0,   0,  15, 114,   0,
            16,   0,   5,   0,   0,   0,
            2,  64,   0,   0,   1,   0,
            0,   0,   1,   0,   0,   0,
            1,   0,   0,   0,   0,   0,
            0,   0,   2,  64,   0,   0,
            28,   0,   0,   0,  25,   0,
            0,   0,  27,   0,   0,   0,
            0,   0,   0,   0,  86,   5,
            16,   0,   1,   0,   0,   0,
            56,   0,   0,  10, 114,   0,
            16,   0,   4,   0,   0,   0,
            70,   2,  16,   0,   4,   0,
            0,   0,   2,  64,   0,   0,
        129, 128, 128,  59, 129, 128,
        128,  59, 129, 128, 128,  59,
            0,   0,   0,   0,  55,   0,
            0,   9, 114,   0,  16,   0,
            2,   0,   0,   0,   6,   0,
            16,   0,   5,   0,   0,   0,
            70,   2,  16,   0,   4,   0,
            0,   0,  70,   2,  16,   0,
            2,   0,   0,   0,  56,   0,
            0,  10, 114,   0,  16,   0,
            4,   0,   0,   0,  70,   2,
            16,   0,   2,   0,   0,   0,
            2,  64,   0,   0,   0,   0,
            0,  63,   0,   0,   0,  63,
            0,   0,   0,  63,   0,   0,
            0,   0,  55,   0,   0,   9,
        114,   0,  16,   0,   2,   0,
            0,   0,  86,   5,  16,   0,
            5,   0,   0,   0,  70,   2,
            16,   0,   4,   0,   0,   0,
            70,   2,  16,   0,   2,   0,
            0,   0,   0,   0,   0,   8,
            18,   0,  16,   0,   0,   0,
            0,   0,  58,   0,  16, 128,
            65,   0,   0,   0,   3,   0,
            0,   0,   1,  64,   0,   0,
            0,   0, 128,  63,  56,   0,
            0,   7, 114,   0,  16,   0,
            3,   0,   0,   0,  70,   2,
            16,   0,   2,   0,   0,   0,
            70,   2,  16,   0,   3,   0,
            0,   0,  50,   0,   0,   9,
        114,   0,  16,   0,   1,   0,
            0,   0,   6,   0,  16,   0,
            0,   0,   0,   0, 134,   3,
            16,   0,   1,   0,   0,   0,
            70,   2,  16,   0,   3,   0,
            0,   0,  39,   0,   0,   7,
            18,   0,  16,   0,   0,   0,
            0,   0,  42,   0,  16,   0,
            5,   0,   0,   0,   1,  64,
            0,   0,   0,   0,   0,   0,
            80,   0,   0,   8,  50,   0,
            16,   0,   3,   0,   0,   0,
            86,   5,  16,   0,   0,   0,
            0,   0,  38, 138,  32,   0,
            0,   0,   0,   0,   2,   0,
            0,   0,   1,   0,   0,   7,
            18,   0,  16,   0,   0,   0,
            0,   0,  10,   0,  16,   0,
            0,   0,   0,   0,  10,   0,
            16,   0,   3,   0,   0,   0,
            79,   0,   0,   8,  98,   0,
            16,   0,   0,   0,   0,   0,
            86,   5,  16,   0,   0,   0,
            0,   0, 246, 141,  32,   0,
            0,   0,   0,   0,   2,   0,
            0,   0,   1,   0,   0,   7,
            18,   0,  16,   0,   0,   0,
            0,   0,  26,   0,  16,   0,
            0,   0,   0,   0,  10,   0,
            16,   0,   0,   0,   0,   0,
            39,   0,   0,   7,  34,   0,
            16,   0,   0,   0,   0,   0,
            58,   0,  16,   0,   0,   0,
            0,   0,   1,  64,   0,   0,
            0,   0,   0,   0,   1,   0,
            0,   7,  34,   0,  16,   0,
            0,   0,   0,   0,  26,   0,
            16,   0,   3,   0,   0,   0,
            26,   0,  16,   0,   0,   0,
            0,   0,   1,   0,   0,   7,
            34,   0,  16,   0,   0,   0,
            0,   0,  42,   0,  16,   0,
            0,   0,   0,   0,  26,   0,
            16,   0,   0,   0,   0,   0,
            60,   0,   0,   7,  18,   0,
            16,   0,   0,   0,   0,   0,
            26,   0,  16,   0,   0,   0,
            0,   0,  10,   0,  16,   0,
            0,   0,   0,   0,  55,   0,
            0,   9, 114,   0,  16,   0,
            0,   0,   0,   0,   6,   0,
            16,   0,   0,   0,   0,   0,
            70,   2,  16,   0,   2,   0,
            0,   0,  70,   2,  16,   0,
            1,   0,   0,   0,  18,   0,
            0,   1,   1,   0,   0,   8,
            18,   0,  16,   0,   1,   0,
            0,   0,  58, 128,  32,   0,
            0,   0,   0,   0,   1,   0,
            0,   0,   1,  64,   0,   0,
        255,   0,   0,   0, 138,   0,
            0,  16,  98,   0,  16,   0,
            1,   0,   0,   0,   2,  64,
            0,   0,   0,   0,   0,   0,
            8,   0,   0,   0,   8,   0,
            0,   0,   0,   0,   0,   0,
            2,  64,   0,   0,   0,   0,
            0,   0,   8,   0,   0,   0,
            16,   0,   0,   0,   0,   0,
            0,   0, 246, 143,  32,   0,
            0,   0,   0,   0,   1,   0,
            0,   0,  43,   0,   0,   5,
        114,   0,  16,   0,   2,   0,
            0,   0,  70,   2,  16,   0,
            1,   0,   0,   0,  56,   0,
            0,  10, 114,   0,  16,   0,
            0,   0,   0,   0,  70,   2,
            16,   0,   2,   0,   0,   0,
            2,  64,   0,   0, 129, 128,
        128,  59, 129, 128, 128,  59,
        129, 128, 128,  59,   0,   0,
            0,   0,  21,   0,   0,   1,
            54,   0,   0,   5, 130,   0,
            16,   0,   0,   0,   0,   0,
            1,  64,   0,   0,   0,   0,
        128,  63, 164,   0,   0,   6,
        242, 224,  17,   0,   0,   0,
            0,   0,  70,   5,   2,   0,
            70,  14,  16,   0,   0,   0,
            0,   0,  62,   0,   0,   1
    };
    }
}