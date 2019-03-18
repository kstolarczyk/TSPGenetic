using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komiwojazer
{
    class Osobnik
    {
        public int[] dna { get; set; }
        public double ocena { get; set; }
        public Osobnik(int[] kod) : this(kod.Length)
        {
            int len = kod.Length;
            for(int i = 0; i < len; i++)
            {
                int index = (i + 1 == len) ? 0 : i + 1;
                this.dna[kod[i]] = kod[index];
            }
        }

        public Osobnik(int size)
        {
            this.dna = new int[size];
        }
    }
}
