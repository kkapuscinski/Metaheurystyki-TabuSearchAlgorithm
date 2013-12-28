using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgorytmTabuSearch
{
    public class Route
    {
        public float Cost { get; private set;}
        public int[] Points { get; private set; }

        public Route(int[] points, float cost)
        {
            Cost = cost;
            Points = points;
        }

    }
}
