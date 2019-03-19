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

        private Osobnik SelekcjaRuletka(Osobnik[] osobnicy, int ilosc)
        {
            double r = rnd.NextDouble();
            int left = 0, k;
            int right = ilosc;
            while(true)
            {
                k = left + right / 2;
                if(r <= osobnicy[k].ocena)
                {
                    if (k == left) break;
                    right = k;
                }
                else
                {
                    if(k == left) { k = right; break; }
                    left = k;
                }
            }
            return osobnicy[k];
        }

        private Osobnik SelekcjaBO3(Osobnik[] osobnicy, int ilosc) // best of three
        {
            int r1 = rnd.Next(ilosc);
            int r2 = rnd.Next(ilosc);
            int r3 = rnd.Next(ilosc);
            if(osobnicy[r1].ocena < osobnicy[r2].ocena)
            {
                return (osobnicy[r1].ocena < osobnicy[r3].ocena) ? osobnicy[r1] : osobnicy[r2];
            }
            else
            {
                return (osobnicy[r2].ocena < osobnicy[r3].ocena) ? osobnicy[r2] : osobnicy[r3];
            }
        }

        private Osobnik SelekcjaBO2(Osobnik[] osobnicy, int ilosc) // best of two
        {
            int r1 = rnd.Next(ilosc);
            int r2 = rnd.Next(ilosc);
            return (osobnicy[r1].ocena < osobnicy[r2].ocena) ? osobnicy[r1] : osobnicy[r2];
        }

        private void ResetVisited(ref bool[] visited)
        {
            for(int i = 0; i < visited.Length; i++)
            {
                visited[i] = false;
            }
        }

        private Osobnik Krzyzuj(Osobnik rodzic1, Osobnik rodzic2, int size)
        {
            Osobnik dziecko = new Osobnik(size);
            bool[] visited = new bool[size];
            ResetVisited(ref visited);

        }
    }
}
