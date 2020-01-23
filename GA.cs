using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace _10
{
    class GA
    {

        const int iterations = 50, n = 15;
        private List<int> Price, Weight;
        private int maxWeight, populationCount, summaryPrice, bestPopulationIndex, crossover, mutation, selection, firstPopulation;
        private List<Gene> populations;
        private Gene bestInPopulation;

        public GA(int count, int maxWeight, int crossover, int mutation, int selection, int first)
        {
            firstPopulation = first;
            this.crossover = crossover;
            this.mutation = mutation;
            this.selection = selection;
            bestInPopulation = new Gene();
            summaryPrice = 0;
            bestPopulationIndex = 0;
            populationCount = count;
            this.maxWeight = maxWeight;
            populations = new List<Gene>();
            Price = new List<int>();
            Weight = new List<int>();

            string line;
            int j = 0;
            StreamReader stream = new StreamReader("C:\\Users\\Elen\\Desktop\\visual\\file10.txt");
            while ((line = stream.ReadLine()) != null)
            {
                int i = 0;
                string[] nums = line.Split(' ');
                foreach (string num in nums)
                {
                    if (i == 1) Price.Add(Int32.Parse(num));
                    if (i == 2) Weight.Add(Int32.Parse(num));
                    i++;
                }
                summaryPrice += Price[j];
                j++;
            }
            stream.Dispose();
        }
        public Gene Search()
        {
            int step = 0, noChange = 0, previous = 0;
            var rand = new Random();

            switch (firstPopulation)
            {
                case 1: //random population
                    {
                        for (int i = 0; i < populationCount; i++)
                        {
                            Gene pop = new Gene();
                            for (int j = 0; j < n; j++) pop.alleles[j] = rand.Next(0, 2);
                            populations.Add(pop);
                        }
                        break;
                    }
                case 2: //using algorithm
                    {
                        List<float> Prob = new List<float>();
                        int e = 0;
                        while (Prob.Count < Price.Count)
                        {
                            Prob.Add((float)Price[e] / n);
                            e++;
                        }
                        while (populations.Count < populationCount)
                        {
                            Gene pop = new Gene();
                            int sumW = 0;
                            for (int j = 0; j < n; j++)
                            {
                                float choice = rand.Next(0, (int)((summaryPrice / n) * 100));
                                choice /= 100; //making it less than 1
                                int chosen = rand.Next(0, n);
                                float sum1 = 0, sum2 = 0;

                                for (int k = 1; k < Prob.Count; k++)
                                {
                                    sum1 += Prob[k - 1];
                                    sum2 = sum1 + Prob[k];
                                    if (choice > sum1 && choice <= sum2) chosen = k;
                                }
                                if (sumW + Weight[chosen] <= maxWeight)
                                {
                                    sumW += Weight[chosen];
                                    pop.alleles[chosen] = 1;
                                }
                                else break;
                            }
                            populations.Add(pop);
                        }
                        break;
                    }
            }
            //PrintAll();
            while (step < iterations)
            {
                CreateNewPopulation();
                bestPopulationIndex = FindBest();
                PrintBest();
                Console.WriteLine("  price of the best - " + bestInPopulation.price);
                //PrintAll();
                step++;
                if (previous < bestInPopulation.price)
                {
                    previous = bestInPopulation.price;
                    noChange = 0;
                }
                else if (previous == bestInPopulation.price) noChange++;
                if (noChange >= 10) break;
            }

            Console.WriteLine("number of all steps - " + step);
            return bestInPopulation;
        }

        private void PrintAll()
        {
            Console.WriteLine("current population");
            for (int i = 0; i < populationCount; i++)
            {
                Console.WriteLine((i + 1) + "gene - ");
                for (int j = 0; j < n; j++)
                {
                    Console.Write(populations[i].alleles[j]);
                }
                Console.WriteLine();
            }
        }

        private void PrintReproductive(List<Gene> rep)
        {
            Console.WriteLine("current reprod population - ");
            for (int k = 0; k < rep.Count; k++)
            {
                Console.WriteLine((k + 1) + " gene ");
                for (int j = 0; j < n; j++)
                {
                    Console.Write(rep[k].alleles[j]);
                }
                Console.WriteLine();
            }
        }

        private void PrintBest()
        {
            Console.WriteLine("current best -  ");
            for (int i = 0; i < n; i++)
            {
                Console.Write(bestInPopulation.alleles[i]);
            }

        }

        private void CreateNewPopulation()
        {
            List<Gene> reproductivePopulation = new List<Gene>();
            var rand = new Random();

            for (int i = 0; i < populationCount + rand.Next(1, populationCount); i++) //breed
            {
                int parent1 = 0, parent2 = 0;
                while (parent1 == parent2)
                {
                    parent1 = rand.Next(0, populationCount);
                    parent2 = rand.Next(0, populationCount);
                }
                reproductivePopulation.Add(Breed(parent1, parent2));
            }
            //PrintReprod(reprodPop);

            for (int i = 0; i < reproductivePopulation.Count; i++) //mutation
            {
                if (rand.Next(0, 100) <= 10)
                {
                    reproductivePopulation[i].Copy(Mutation(reproductivePopulation[i]));
                }
            }
            //PrintReprod(reprodPop);

            for (int i = 0; i < reproductivePopulation.Count; i++)
            {
                int sumPr = 0, sumW = 0;
                for (int j = 0; j < n; j++)
                {
                    sumPr += Price[j] * reproductivePopulation[i].alleles[j];
                    sumW += Weight[j] * reproductivePopulation[i].alleles[j];
                    //reprodPop[i].price += Price[j] * reprodPop[i].alleles[j];
                    //reprodPop[i].weight += Weight[j] * reprodPop[i].alleles[j];
                }
                reproductivePopulation[i].price = sumPr;
                reproductivePopulation[i].weight = sumW;
                summaryPrice += reproductivePopulation[i].price;
            }
            for (int i = 0; i < reproductivePopulation.Count; i++) reproductivePopulation[i].fitness = (float)reproductivePopulation[i].price / (float)summaryPrice;

            Selection(reproductivePopulation);
            //PrintAll();
        }
        private Gene Mutation(Gene child)
        {
            var rand = new Random();
            switch (mutation)
            {
                case 1: //one gene
                    {
                        int allel = rand.Next(0, n); //random choice of allel to mutate
                        child.alleles[allel] = child.alleles[allel] ^ 1;
                        break;
                    }
                case 2: // macro
                    {
                        int allel1 = rand.Next(0, n), allel2 = rand.Next(0, n); //random choice of two allels to mutate
                        while (allel1 == allel2) allel2 = rand.Next(0, n);
                        child.alleles[allel1] = child.alleles[allel1] ^ 1;
                        child.alleles[allel2] = child.alleles[allel2] ^ 1;
                        break;
                    }
            }
            return child;
        }
        private void Selection(List<Gene> reprod)
        {
            var rand = new Random();
            switch (selection)
            {
                case 1: //roulette 
                    {
                        List<float> wheel = new List<float>();
                        float wheelSummary = 0;
                        for (int j = 0; j < reprod.Count; j++)
                        {
                            wheel.Add(reprod[j].fitness);
                            wheelSummary += wheel[j];
                        }

                        int i = 0;
                        while (i < populationCount)
                        {   //float rand from 0.0 to sumwheel
                            float choice = rand.Next(0, (int)(wheelSummary * 100)); // 0 - sumwheel * 100
                            choice /= 100; //making it less than 1
                            int chosen = 0;
                            float sum1 = 0, sum2 = 0;

                            for (int j = 1; j < reprod.Count; j++)
                            {
                                sum1 += wheel[j - 1];
                                sum2 = sum1 + wheel[j];
                                if (choice > sum1 && choice <= sum2) chosen = j;
                            }
                            CorrectWeight(reprod[chosen]);
                            populations[i].Copy(reprod[chosen]);
                            i++;
                        }
                        break;
                    }
                case 2: //tourney 
                    {
                        for (int i = 0; i < populationCount; i++)
                        {
                            List<Gene> tourney = new List<Gene>();
                            int max = 0, ind = 0;
                            while (tourney.Count < populationCount / 3)
                            {
                                int k = rand.Next(0, 100);
                                if (k < populationCount)
                                {
                                    ind++;
                                    tourney.Add(reprod[rand.Next(0, reprod.Count)]);
                                }
                            }
                            for (int j = 0; j < tourney.Count; j++)
                            {
                                if (max < tourney[j].price)
                                {
                                    max = tourney[j].price;
                                    ind = j;
                                }
                            }
                            CorrectWeight(tourney[ind]);
                            populations[i].Copy(tourney[ind]);
                        }
                        break;
                    }
            }
        }
        private Gene Breed(int p1, int p2)
        {
            Gene child = new Gene();
            child.Copy(populations[p1]);
            var rand = new Random();
            switch (crossover)
            {
                case 1:
                    {
                        int crossover = rand.Next(1, n);// Create the crossover point (not first)
                        int first = rand.Next(0, 100);// Which parent comes first?

                        int start = 0, final = n;
                        if (first < 60) start = crossover;  // If parent1 first- start from crossover.
                        else final = crossover + 1;// Else end at crossover.

                        for (int i = start; i < final; i++) child.alleles[i] = populations[p2].alleles[i];
                        break;
                    }
                case 2:
                    {
                        int crossover1 = rand.Next(1, n);// Create the crossover point (not first)
                        int crossover2 = rand.Next(1, n);// Create the crossover point (not first)
                        while (crossover1 == crossover2) crossover2 = rand.Next(1, n);

                        for (int i = crossover1; i < crossover2; i++) child.alleles[i] = populations[p2].alleles[i];
                        break;
                    }
            }
            return child;
        }
        private int FindBest()
        {
            int maxC = 0, ind = 0;
            for (int i = 0; i < populationCount; i++)
            {
                if (maxC < populations[i].price)
                {
                    maxC = populations[i].price;
                    ind = i;
                }
            }

            if (populations[ind].price > bestInPopulation.price)
            {
                bestInPopulation.Copy(populations[ind]);
                return ind;
            }
            else
            {
                populations[ind].Copy(bestInPopulation);
                return bestPopulationIndex;
            }
        }
        private void NullPopulations()
        {
            summaryPrice = 0;
            for (int i = 0; i < populationCount; i++)
            {
                if (i != bestPopulationIndex)
                {
                    populations[i].price = 0;
                    populations[i].weight = 0;
                    populations[i].fitness = 0;
                }
            }
        }

        private void CorrectWeight(Gene chosen)
        {
            while (chosen.weight > maxWeight)
            {
                int min = 50, ind = 0;
                for (int j = 0; j < n; j++)
                {
                    if (chosen.alleles[j] * Price[j] < min && chosen.alleles[j] != 0)
                    {
                        ind = j;
                        min = chosen.alleles[j] * Price[j];
                    }
                }
                chosen.alleles[ind] = 0;
                chosen.weight -= Weight[ind];
                chosen.price -= Price[ind];
            }
        }

    }
}
