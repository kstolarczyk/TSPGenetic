using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komiwojazer
{
    class Genetyk
    {
        public Graf g { get; set; }
        public Random rnd;
        public Osobnik[] populacja { get; set; }
        private Konfiguracja config { get; set; }
        public Genetyk(Graf graf, Konfiguracja konfiguracja)
        {
            this.g = graf;
            this.config = konfiguracja;
            this.rnd = new Random();
        }

        private void ShuffleArray(ref int[] array)
        {
            int len = array.Length;
            for(int i = len-1; i > 0; i--)
            {
                int j = rnd.Next(0, i+1);
                this.Swap(ref array[i], ref array[j]);
            }
        }

        private void Swap(ref int a, ref int b)
        {
            int tmp = a;
            a = b;
            b = tmp;
        }

        private void GenerujPopulacje()
        {
            int size = this.g.size;
            int ilosc = this.config.iloscPokolen;
            int nadmiar = (int) Math.Floor(this.config.wspolczynnikPotomkow * ilosc);
            this.populacja = new Osobnik[ilosc + nadmiar];

            int[] kod = new int[size];
            for(int i = 0; i < size; i++)
            {
                kod[i] = i;
            }

            for(int i = 0; i < ilosc; i++)
            {
                this.ShuffleArray(ref kod);
                populacja[i] = new Osobnik(kod);
            }
        }

        private void Ocen1(ref Osobnik[] osobnicy, int ilosc)
        {
            for (int j = 0; j < ilosc; j++)
            {
                int v = 0;
                double dlugoscTrasy = 0;
                for (int i = 0; i < this.g.size; i++)
                {
                    int r = osobnicy[j].dna[v];
                    dlugoscTrasy += this.g.odleglosci[v, r];
                    v = r;
                }
                osobnicy[j].ocena = dlugoscTrasy;
            }
        }



        private void Ocen2(ref Osobnik[] osobnicy, int ilosc)
        {
            double min = double.PositiveInfinity;
            for (int j = 0; j < ilosc; j++)
            {
                int v = 0;
                double dlugoscTrasy = 0;
                for (int i = 0; i < this.g.size; i++)
                {
                    int r = osobnicy[j].dna[v];
                    dlugoscTrasy += this.g.odleglosci[v, r];
                    v = r;
                }
                osobnicy[j].ocena = dlugoscTrasy;
                min = Math.Min(min, dlugoscTrasy);
            }

            double p = 0;
            for(int j = 0; j < ilosc-1; j++)
            {
                p += min / osobnicy[j].ocena / ilosc;
                osobnicy[j].ocena = p;
            }
            osobnicy[ilosc - 1].ocena = 1;
        }
        

    }
}
