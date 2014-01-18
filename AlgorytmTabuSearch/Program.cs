using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmTabuSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            //zrobić wczytywanie mapy i wyliczanie odległości

            // domyślne parametry
            //optimal berlin52 7544.365901904087
            //optimal kroA200 29368
            string mapPath = "kroA200.txt";
            int numberOfIterations = 5000;
            int numberOfGeneratedCandidates = 200;
            int tabuLifeTime = 50;
            float frequencyAdjustment = 0.01F;

            // czytanie parametrów podanych przy wywołaniu
            var argsCount = args.Count();
            if (argsCount >= 1)
            {
                if (args[0] == "-help")
                {
                    Console.WriteLine("Parametry do podania nalezy podawać po kolei oddzielone spacjami");
                    Console.WriteLine("1. Ścieżka do pliku z współrzędnymi ");
                    Console.WriteLine("2. Ilość iteracji algorytmu  integer");
                    Console.WriteLine("3. Ilość generowanych kandydatów(par miast do zamiany) integer");
                    Console.WriteLine("4. Czas trwania karencji Tabu  integer");
                    Console.WriteLine("5. Współczynnik dostosowania częstotliwości  float");
                    return;
                }
                else if (argsCount != 5)
                {
                    Console.WriteLine("nie prawidłowa ilość parametrów. skorzystaj z opcji -help");
                    return;
                }
                else
                {
                    mapPath = args[0];
                    numberOfIterations = Convert.ToInt32(args[1]);
                    numberOfGeneratedCandidates = Convert.ToInt32(args[2]);
                    tabuLifeTime = Convert.ToInt32(args[3]);
                    frequencyAdjustment = Convert.ToSingle(args[4]);
                }


            }
            // wczytywanie mapy i kalkulacja odległości

            var cities = ReadCitiesCoordinates(mapPath);
            var distanceMap = CalculateDistanceForCoordinates(cities);

            // zmienne tymczasowe
            var bestSolutions = new List<Route>();
            var bestSolutionsIteration = new List<int>();

            // wykonanie algorytmu dziesięciokrotnie
            for (int i = 0; i < 10; i++)
            {
                
                var algorithm = new TabuAlgorithm(distanceMap, numberOfIterations, numberOfGeneratedCandidates, tabuLifeTime, frequencyAdjustment);
                algorithm.Run();
                // zapis najlepszych osobników
                bestSolutions.Add(algorithm.BestSolution);
                bestSolutionsIteration.Add(algorithm.BestSolutionIteration);
            }
            
            // zapis do pliku
            FileStream fs = new FileStream(DateTime.Now.ToString("HH_mm_ss") + ".csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            Console.SetOut(sw);
            Console.WriteLine("Iteracja najlepszego rozwiązania; Koszt najlepszego rozwiązania");
            for (int i = 0; i < 10; i++)
            {

                Console.WriteLine("{0};{1}", bestSolutionsIteration[i], bestSolutions[i].Cost.ToString("0.000000000"));
            }

            Console.WriteLine("Średni koszt;Nalepsza wartość kosztu;Najgorsza wartość kosztu");
            Console.WriteLine("{0};{1};{2}", bestSolutions.Average(g => g.Cost), bestSolutions.Min(g => g.Cost), bestSolutions.Max(g => g.Cost));
            Console.WriteLine("Średnia ilość iteracji;Nalepsza ilość iteracji;Najgorsza ilość iteracji");
            Console.WriteLine("{0};{1};{2}", bestSolutionsIteration.Average(), bestSolutionsIteration.Min(), bestSolutionsIteration.Max());

            var z = bestSolutions.OrderBy(b => b.Cost).First();

            foreach (var item in z.Points)
            {
                Console.WriteLine(item+1);
            }

            sw.Flush();
            fs.Flush(true);
            fs.Close();
            
        }

        /// <summary>
        /// metoda wczytująca współrzędne miast z pliku do tablicy
        /// </summary>
        /// <param name="filePath">ścieżka do pliku (pierwsza linia ilość miast, pozostałe współrzędne oddzielone spacją)</param>
        /// <returns>tablica int współrzędnych miast(n,2)</returns>
        public static int[,] ReadCitiesCoordinates(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            var line = sr.ReadLine();
            var numberOfCities = Convert.ToInt32(line);
            var citiesCoordinates = new int[numberOfCities, 2];
            for (int i = 0; i < numberOfCities; i++)
            {
                line = sr.ReadLine();
                if (line == null) throw new Exception();
                var coordinates = line.Split(' ');
                for (int j = 0; j < coordinates.Length; j++)
                {
                    citiesCoordinates[i, j] = Convert.ToInt32(coordinates[j]);
                }
            }

            return citiesCoordinates;
        }

        /// <summary>
        /// metoda obliczająca dystans (Euklidesowy) między miastami 
        /// </summary>
        /// <param name="coordinates">tablica współrzędnych</param>
        /// <returns>tablica odległości miast (n,n)</returns>
        public static int[,] CalculateDistanceForCoordinates(int[,] coordinates)
        {
            var coordinatesLength = coordinates.GetLength(0);
            var distanceMap = new int[coordinatesLength, coordinatesLength];
            for (int i = 0; i < coordinatesLength; i++)
            {
                for (int j = i+1; j < coordinatesLength; j++)
                {
                    
                    var distance = Math.Sqrt(Math.Pow(coordinates[j, 0] - coordinates[i, 0], 2) + Math.Pow(coordinates[j, 1] - coordinates[i,1], 2));
                    distanceMap[i, j] = Convert.ToInt32(distance);
                    distanceMap[j, i] = Convert.ToInt32(distance);
                }
                
            }
            return distanceMap;
        }
    }
}
