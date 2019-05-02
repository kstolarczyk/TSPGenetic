using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komiwojazer
{
    class Osobnik : IComparable
    {
        public int[] dna { get; set; }
        public int ocena { get; set; }
        public double p { get; set; }
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

        public override string ToString()
        {
           
            return "ocena: " + this.ocena + "\n[" + new Func<string>(() => { string val = ""; for (int i = 0; i < this.dna.Length; i++) val += this.dna[i] + ","; return val; })() + "]\n";
        }

        public int CompareTo(object obj)
        {
            if (obj == null || this == null)
            {
                return 1;
            }
            Osobnik other = (Osobnik)obj;
            if(this.ocena < other.ocena)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

    }
}
