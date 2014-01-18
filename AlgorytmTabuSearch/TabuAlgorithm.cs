using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmTabuSearch
{
    /// <summary>
    /// klasa opisująca algorytm Tabu Search
    /// </summary>
    public class TabuAlgorithm
    {
        public Random random;
        //Parametry algorytmu
        public int NumberOfIterations { get; private set; }
        public int NumberOfGeneratedCandidates { get; private set; }
        public int TabuLifetime { get; private set; }
        public float FrequencyAdjustment { get; private set; }

        //Struktury danych
        public int[,] DistanceArray { get; private set; }
        public int[,] TabuArray { get; private set; }

        //Wyniki algorytmu
        public Route BestSolution { get; private set; }
        public int BestSolutionIteration { get; private set; }

        private Route ActualSolution { get; set; }
        private List<Route> SolutionCandidates { get; set; }

        /// <summary>
        /// konstruktor obiektu algortymu Tabu Search
        /// </summary>
        /// <param name="distanceArray">Tablica odległości miast</param>
        /// <param name="numberOfIterations">Ilość iteracji</param>
        /// <param name="numberOfGeneratedCandidates"> Ilość generowanych kandydatów</param>
        /// <param name="tabuLifeTime">Czas trwania Tabu</param>
        /// <param name="frequencyAdjustment">Współczynnik częstotliwości</param>
        public TabuAlgorithm(int[,] distanceArray, int numberOfIterations, int numberOfGeneratedCandidates, int tabuLifeTime, float frequencyAdjustment)
        {

            if (distanceArray == null) throw new ArgumentNullException("distanceArray");
            if (numberOfIterations < 0) throw new ArgumentOutOfRangeException("numberOfIterations");
            if (numberOfGeneratedCandidates < 0) throw new ArgumentOutOfRangeException("numberOfGeneratedCandidates");
            if (tabuLifeTime < 0) throw new ArgumentOutOfRangeException("tabuLifeTime");

            //Inicjalizacja struktur danych
            DistanceArray = distanceArray;
            var mapSize = distanceArray.GetLength(0);
            TabuArray = new int[mapSize,mapSize];
            TabuArray.Initialize();

            //Inicjalizacja parametrów
            NumberOfIterations = numberOfIterations;
            NumberOfGeneratedCandidates = numberOfGeneratedCandidates;
            TabuLifetime = tabuLifeTime;
            FrequencyAdjustment = frequencyAdjustment;



            //tworzenie nowego generatora pseudolosowego z nowym ziarnem(konstruktor domyślny bierze pod uwagę aktualną datę
            random = new Random();
        }

        /// <summary>
        /// metoda wykonująca algorytm
        /// </summary>
        public void Run()
        {
            ActualSolution = GenerateRandomRoute();
            CalculateCost(ActualSolution);
            BestSolution = ActualSolution;
            for (int i = 0; i < NumberOfIterations; i++)
            {
                SolutionCandidates = generateCandidates();
                ActualSolution = SelectBestCandidate();
                if (ActualSolution.Cost < BestSolution.Cost)
                {
                    BestSolution = ActualSolution;
                    BestSolutionIteration = i;
                }
                UpdateMemory();
            }
        }
        
        /// <summary>
        /// Metoda do wyboru najlepszego kandydata
        /// </summary>
        /// <returns>Obiekt trasy</returns>
        private Route SelectBestCandidate()
        {
            Route tmpSolution = null;
            tmpSolution = SolutionCandidates.Where(s => s.Cost < BestSolution.Cost).OrderBy(s => s.Cost).FirstOrDefault();
            // jeśli kandydat jest lepszy od najlepszego rozwiązania staje sięaktualnym rozwiązaniem
            if (tmpSolution != null)
            {
                return tmpSolution;
            }


            tmpSolution = SolutionCandidates.Where(s => TabuArray[s.ChangedIndex1, s.ChangedIndex2] == 0).OrderBy(s => s.TabuAdjustedCost).FirstOrDefault();
            //Jeśli jest kandydat nie będący tabu aktywny. to wybierz kandydata o najlepszej wartości kosztu dostosowanego o częstoliwość
            if (tmpSolution != null)
            {
                
                return tmpSolution;
            }
            else
            {
                //wpp. Kryterium aspiracji wybierz najlepszego zabronionego kandydata uwzględniając koszt dostosowanego o częstotliwość
               return SolutionCandidates.OrderBy(s => s.TabuAdjustedCost).First();
            }
        }

        /// <summary>
        /// Metoda aktualizująca pamięć algorytmu
        /// </summary>
        private void UpdateMemory()
        {
            var tabuArrayLength = TabuArray.GetLength(0);
            for (int i = 0; i < tabuArrayLength; i++)
            {
                for (int j = i+1; j < tabuArrayLength; j++)
                {
                    //Jeśli miasto jest Tabu aktywne, to zmniejsz jego wartość o 1
                    if (TabuArray[j, i] > 0)
                    {
                        TabuArray[j, i]--;
                    }

                    // Zwiększ częstotliwość przebywania o 1
                    TabuArray[i, j]++;

                }
            }

            // dla aktualnej pary zamienionych miast ustaw Tabu aktywność i zresetuj licznik częstotliwości
            TabuArray[ActualSolution.ChangedIndex1, ActualSolution.ChangedIndex2] = TabuLifetime;
            TabuArray[ActualSolution.ChangedIndex2, ActualSolution.ChangedIndex1] = 0;

        }

        /// <summary>
        /// Metoda generująca kandydatów tras
        /// </summary>
        /// <returns></returns>
        private List<Route> generateCandidates()
        {
            var tmpCandidates = new List<Route>();
            var actualRoutePoints = ActualSolution.Points;
            int r1,r2;
            while (tmpCandidates.Count < NumberOfGeneratedCandidates)
	        {
                //Losowanie pary miast
	            var tmpPointArray = new int[actualRoutePoints.Length];
                actualRoutePoints.CopyTo(tmpPointArray, 0);
                
                r1 = random.Next(0, actualRoutePoints.Length);
                do{
                    r2 = random.Next(0, actualRoutePoints.Length);
                } while (r1 == r2);
                
                //zamiana miast w tablicy trasy
                int temp = tmpPointArray[r1];
                tmpPointArray[r1] = tmpPointArray[r2];
                tmpPointArray[r2] = temp;

                Route tmpRoute = null;
                if (r1 > r2)
                {
                    tmpRoute = new Route { Points = tmpPointArray, ChangedIndex1 = r1, ChangedIndex2 = r2 };
                }
                else
                {
                    tmpRoute = new Route { Points = tmpPointArray, ChangedIndex1 = r2, ChangedIndex2 = r1 };
                }

                // jeśli w liście kandydatów nie ma takiej pary zamienionych miast to dodaj ją do listy i oblicz jej koszt
                if (!tmpCandidates.Any(i => i.ChangedIndex1 == tmpRoute.ChangedIndex1 && i.ChangedIndex2 == tmpRoute.ChangedIndex2))
                {
                    CalculateCost(tmpRoute);
                    tmpCandidates.Add(tmpRoute);
                }
                
	        }
            return tmpCandidates;
        }

        /// <summary>
        /// metoda generuje losową trasę jako początek algorytmu
        /// </summary>
        /// <returns>obiekt trasy</returns>
        private Route GenerateRandomRoute()
        {
            var tmpRoute = new Route();
            var k = DistanceArray.GetLength(0);
            tmpRoute.Points = new int[k];
            
            // wypełniam tablicę kolejnymi liczbami od 0
            for (int i = 0; i < tmpRoute.Points.Length; i++)
            {
                tmpRoute.Points[i] = i;
            }
            for (int i = 0; i < tmpRoute.Points.Length; i++)
            {
                int randomIndex = random.Next(0, k - 1);
                int temp = tmpRoute.Points[randomIndex];
                tmpRoute.Points[randomIndex] = tmpRoute.Points[k - 1];
                tmpRoute.Points[k - 1] = temp;
                k--;
            }
            return tmpRoute;

        }

        /// <summary>
        /// Metoda obliczająca koszt trasy
        /// </summary>
        /// <param name="route">obiekt trasy</param>
        private void CalculateCost(Route route)
        {
            for (int i = 0; i < route.Points.Length - 1; i++)
            {
                route.Cost += DistanceArray[route.Points[i], route.Points[i + 1]];
            }
            // powrót do punktu startowego
            route.Cost += DistanceArray[route.Points[route.Points.Length - 1], route.Points[0]];
            
            // wyliczanie kosztu z uwzględnieniem częstotliwości
            route.TabuAdjustedCost = route.Cost;
            route.TabuAdjustedCost = Convert.ToInt32(route.TabuAdjustedCost - (FrequencyAdjustment * TabuArray[route.ChangedIndex2, route.ChangedIndex1]));
        }


    }

}
