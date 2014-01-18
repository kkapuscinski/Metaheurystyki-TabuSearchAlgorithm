using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgorytmTabuSearch
{
    /// <summary>
    /// klasa opisująca trasę 
    /// </summary>
    public class Route
    {
        /// <summary>
        /// koszt trasy
        /// </summary>
        public int Cost { get; set;}
        /// <summary>
        /// koszt z uwzględniem tablicy częstotliwości * współczynnik częstotliwości
        /// </summary>
        public int TabuAdjustedCost { get; set;}
        /// <summary>
        /// Tablica miast pokolei odwiedzanych
        /// </summary>
        public int[] Points { get; set; }
        /// <summary>
        /// Informacja podawana podczas generowania kandydata. przechowuje indeks pierwszego miasta zamienionego
        /// </summary>
        public int ChangedIndex1 { get; set; }
        /// <summary>
        /// Informacja podawana podczas generowania kandydata. przechowuje indeks drugiego miasta zamienionego
        /// </summary>
        public int ChangedIndex2 { get; set; }
    }
}
