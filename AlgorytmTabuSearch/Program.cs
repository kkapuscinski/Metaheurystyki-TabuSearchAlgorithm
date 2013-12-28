using System;
using System.Collections.Generic;
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
            string mapPath = "bays29.txt";
            int numberOfIterations = 1000;
            int numberOfGeneratedCandidates = 20;
            int tabuLifeTime = 5;
            float tabuBreakRate = 0.8F;
            float tabuBreakRateAdjustment = 0.8F;

            // czytanie parametrów podanych przy wywołaniu
            var argsCount = args.Count();
            if (argsCount >= 1)
            {
                if (args[0] == "-help")
                {
                    Console.WriteLine("Parametry do podania nalezy podawać po kolei oddzielone spacjami");
                    Console.WriteLine("1. ścieżka do pliku z współrzędnymi miast");
                    Console.WriteLine("2. ilość iteracji integer");
                    Console.WriteLine("3. ilość generowanych kandydatów zmiany integer");
                    Console.WriteLine("4. czas trwania tabu integer");
                    Console.WriteLine("5. waga dla łamania ograniczenia tabu float");
                    Console.WriteLine("6. poziom zmniejszania wagi łamania ograniczenia tabu float");
                    return;
                }
                else if (argsCount != 6)
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
                    tabuBreakRate = Convert.ToSingle(args[4]);
                    tabuBreakRateAdjustment = Convert.ToSingle(args[5]);
                }


            }
            // wczytywanie mapy i kalkulacja odległości

            var x = ReadCitiesCoordinates(mapPath);
            var z = CalculateDistanceForCoordinates(x);

            // zmienne tymczasowe
            var bestSolutions = new List<Route>();
            var bestSolutionsIteration = new List<int>();
            x.Initialize();
            // wykonanie algorytmu dziesięciokrotnie
            for (int i = 0; i < 10; i++)
            {
                
                var algorithm = new TabuAlgorithm(z, numberOfIterations, numberOfGeneratedCandidates, tabuLifeTime, tabuBreakRate, tabuBreakRateAdjustment);
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
            sw.Flush();
            fs.Flush(true);
            fs.Close();
            
        }

        public static int[,] ReadCitiesCoordinates(string filePath)
        {
            throw new NotImplementedException();
        }

        public static float[,] CalculateDistanceForCoordinates(int[,] coordinates)
        {
            throw new NotImplementedException();
        }
    }
}
