using System;
using System.Collections.Generic;
using System.Text;

namespace _10
{
    class Gene
    {
        const int n = 15;
        public int[] alleles; //List size n = 15
        public int price, weight;
        public float fitness;

        public Gene()
        {
            alleles = new int[n];
            fitness = 0;
            price = 0;
            weight = 0;
        }
        public void Copy(Gene cop)
        {
            for (int k = 0; k < n; k++) this.alleles[k] = cop.alleles[k];
            this.price = cop.price;
            this.weight = cop.weight;
            this.fitness = cop.fitness;
        }
    }
}
