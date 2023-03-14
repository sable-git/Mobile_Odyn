using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuleModule_Odyn
{
    public static class StaticData
    {
        public static int[,] VtMan = new int[,]{ {150,48,192,240,288,336,383 },
            {155,52,210,262,315,367,420 },
            {160,57,228,285,342,399,456 },
            { 165,62,246,308,369,431,492 },
            {170, 66,264,330,396,462,528 },
            { 175 ,71, 282 ,353 ,424, 494, 565 },
            { 180,75,300,376,451,526,601 },
            { 185,80,319,398,478,558,637 },
            { 190,84,337,421,505,589,673 },
            { 195,89,355,444,532,621,710 },
            { 200,93 ,373,466,559,653,746 }};

        public static int[,] VtWoman = new int[,]{
            { 150,43,174,217,261,304,347 },
            { 155,48 ,192,240,288,336,384 },
            { 160,52,210,262,315,367,420 },
            { 165,57,228,285,342,399,456 },
            { 170,62,246,308,369,431,492 },
            { 175,66,264,330,397,463,529 },
            { 180,71,282,353,424,494,565 },
            { 185,75,301,376,451,526,601 },
            { 190,80,319,398,478,558,637 },
            { 195,84,337,421,505,589,674 },
            { 200,89,355,444,532,621,710 } };
        public static Dictionary<string, string> translate = new Dictionary<string, string> { { "sex", "Płeć" }, { "height", "Wysokość" }, { "TiTe", "Ti:Te" } };
        public static int fontSize = 0;
        //public static Node currentNode = null;
    }
}
