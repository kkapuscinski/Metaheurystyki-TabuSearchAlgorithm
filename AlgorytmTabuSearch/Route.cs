using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgorytmTabuSearch
{
    public class Route
    {
        public int Cost { get; set;}
        public int TabuAdjustedCost { get; set;}
        public int[] Points { get; set; }
        public int ChangedIndex1 { get; set; }
        public int ChangedIndex2 { get; set; }
    }
}
