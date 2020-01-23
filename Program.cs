using System;
using System.IO;


namespace _10
{
    class Program
    {
        const int n = 15;
        static void Main(string[] args)
        {
            int count = 0, maxWeight = 95, mutation = 1, crossover = 1, selection = 2, firstPopulation = 2;
            Gene solution;

            Console.Write("enter count ");
            count = Int32.Parse(Console.ReadLine());
            Console.Write("choose crossover, 1 - one point, 2 - two point ");
            crossover = Int32.Parse(Console.ReadLine());
            Console.Write("choose mutation, 1 - one gene, 2 - two gene ");
            mutation = Int32.Parse(Console.ReadLine());
            Console.Write("choose selection, 1 - roulette, 2 - tourney ");
            selection = Int32.Parse(Console.ReadLine());
            Console.Write("choose first algorithm, 1 - random, 2 - greed ");
            firstPopulation = Int32.Parse(Console.ReadLine());

            GA ga = new GA(count, maxWeight, crossover, mutation, selection, firstPopulation);
            solution = ga.Search();

            for(int i = 0; i < n;i++)
            {
                Console.WriteLine(solution.alleles[i]);
            }
            Console.WriteLine(solution.price);
            Console.WriteLine(solution.weight);
        }
    }
}
