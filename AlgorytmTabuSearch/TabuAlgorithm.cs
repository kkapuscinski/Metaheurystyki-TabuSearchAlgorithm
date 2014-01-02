using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmTabuSearch
{
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

        public TabuAlgorithm(int[,] distanceArray, int numberOfIterations, int numberOfGeneratedCandidates, int tabuLifeTime, float frequencyAdjustment)
        {
            //TODO: wyjątki !
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
            if (tmpSolution != null)
            {
                return tmpSolution;
            }
            else
            {
               return SolutionCandidates.OrderBy(s => s.TabuAdjustedCost).First();
            }
        }

        private void UpdateMemory()
        {
            var tabuArrayLength = TabuArray.GetLength(0);
            for (int i = 0; i < tabuArrayLength; i++)
            {
                for (int j = i+1; j < tabuArrayLength; j++)
                {
                    if (TabuArray[j, i] > 0)
                    {
                        TabuArray[j, i]--;
                    }

                    TabuArray[i, j]++;

                }
            }


            TabuArray[ActualSolution.ChangedIndex1, ActualSolution.ChangedIndex2] = TabuLifetime;
            TabuArray[ActualSolution.ChangedIndex2, ActualSolution.ChangedIndex1] = 0;

        }

        private List<Route> generateCandidates()
        {
            var tmpCandidates = new List<Route>();
            var actualRoutePoints = ActualSolution.Points;
            int r1,r2;
            while (tmpCandidates.Count < NumberOfGeneratedCandidates)
	        {
	            var tmpPointArray = new int[actualRoutePoints.Length];
                actualRoutePoints.CopyTo(tmpPointArray, 0);
                
                r1 = random.Next(0, actualRoutePoints.Length);
                do{
                    r2 = random.Next(0, actualRoutePoints.Length);
                } while (r1 == r2);
                

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

                //if (!tmpCandidates.Any(i => i.ChangedIndex1 == tmpRoute.ChangedIndex1 && i.ChangedIndex2 == tmpRoute.ChangedIndex2))
                //{
                    CalculateCost(tmpRoute);
                    tmpCandidates.Add(tmpRoute);
                //}
                
	        }
            return tmpCandidates;
        }

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
                SwapElements(tmpRoute.Points, randomIndex, k - 1);
                k--;
            }
            return tmpRoute;

        }

        private void CalculateCost(Route route)
        {
            for (int i = 0; i < route.Points.Length - 1; i++)
            {
                route.Cost += DistanceArray[route.Points[i], route.Points[i + 1]];
            }
            // powrót do punktu startowego
            route.Cost += DistanceArray[route.Points[route.Points.Length - 1], route.Points[0]];
            
            //TODO: wyliczanie kosztu tabu
            route.TabuAdjustedCost = route.Cost;
            route.TabuAdjustedCost = Convert.ToInt32(route.TabuAdjustedCost - (FrequencyAdjustment * TabuArray[route.ChangedIndex2, route.ChangedIndex1]));
        }


        private void SwapElements(int[] array, int index1, int index2)
        {
            int temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

    }

}
