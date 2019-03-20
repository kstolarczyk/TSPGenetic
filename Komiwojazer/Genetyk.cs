using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            this.GenerujPopulacje();
        }

        private void ShuffleArray(ref int[] array)
        {
            int len = array.Length;
            for(int i = len-1; i > 0; i--)
            {
                int j = rnd.Next(0, i+1);
                this.Swap<int>(ref array[i], ref array[j]);
            }
        }

        private void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        private void GenerujPopulacje()
        {
            int size = this.g.size;
            int ilosc = this.config.rozmiarPopulacji;
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

        public void Ocen1(int start, int count)
        {
            double avg = 0;
            for (int j = start; j < start+count; j++)
            {
                int v = 0;
                double dlugoscTrasy = 0;
                for (int i = 0; i < this.g.size; i++)
                {
                    int r = this.populacja[j].dna[v];
                    dlugoscTrasy += this.g.odleglosci[v, r];
                    v = r;
                }
                this.populacja[j].ocena = dlugoscTrasy;
                avg += dlugoscTrasy;
            }
            Console.WriteLine("Średni dystans populacji: {0}", (avg/count));
        }



        public void Ocen2(int start, int count)
        {
            double min = double.PositiveInfinity;
            for (int j = start; j < start+count; j++)
            {
                int v = 0;
                double dlugoscTrasy = 0;
                for (int i = 0; i < this.g.size; i++)
                {
                    int r = this.populacja[j].dna[v];
                    dlugoscTrasy += this.g.odleglosci[v, r];
                    v = r;
                }
                this.populacja[j].ocena = dlugoscTrasy;
                min = Math.Min(min, dlugoscTrasy);
            }

            double p = 0;
            for(int j = start; j < start+count-1; j++)
            {
                p += ((min)/(this.populacja[j].ocena*count));
                this.populacja[j].p = p;
            }
            this.populacja[start  + count - 1].p = 1;
        }

        public Osobnik SelekcjaRuletka(int ilosc)
        {
            double r = rnd.NextDouble();
            int left = 0, k;
            int right = ilosc;
            while(true)
            {
                k = (left + right)/2;
                if(r <= this.populacja[k].p)
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
            return this.populacja[k];
        }

        public Osobnik SelekcjaBO3(int ilosc) // best of three
        {
            int r1 = rnd.Next(ilosc);
            int r2 = rnd.Next(ilosc);
            int r3 = rnd.Next(ilosc);
            if(this.populacja[r1].ocena < this.populacja[r2].ocena)
            {
                return (this.populacja[r1].ocena < this.populacja[r3].ocena) ? this.populacja[r1] : this.populacja[r2];
            }
            else
            {
                return (this.populacja[r2].ocena < this.populacja[r3].ocena) ? this.populacja[r2] : this.populacja[r3];
            }
        }

        public Osobnik SelekcjaBO2(int ilosc) // best of two
        {
            int r1 = rnd.Next(ilosc);
            int r2 = rnd.Next(ilosc);
            return (this.populacja[r1].ocena < this.populacja[r2].ocena) ? this.populacja[r1] : this.populacja[r2];
        }

        private void ResetVisited(ref bool[] visited)
        {
            for(int i = 0; i < visited.Length; i++)
            {
                visited[i] = false;
            }
        }

        public Osobnik Krzyzuj(Osobnik rodzic1, Osobnik rodzic2, int size)
        {
            Osobnik dziecko = new Osobnik(size);
            bool[] visited = new bool[size];
            ResetVisited(ref visited);
            HashSet<int> lista = new HashSet<int>();
            List<int> lista2 = new List<int>(size);
            for(int i = 1; i < size; i++)
            {
                lista.Add(i);
            }
            int v = 0;
            visited[v] = true;
            
            while(lista.Count > 0)
            {
                int r = this.rnd.Next();
                if(r % 2 == 0)
                {
                    int x = rodzic1.dna[v];
                    if(visited[x])
                    {
                        x = rodzic2.dna[v];
                    }
                    if(visited[x])
                    {
                        dziecko.dna[v] = -1;
                        v = lista.First();
                        lista2.Add(v);
                        lista.Remove(v);
                        visited[v] = true;
                        continue;
                    }
                    dziecko.dna[v] = x;
                    v = x;
                }
                else
                {
                    int x = rodzic2.dna[v];
                    if (visited[x])
                    {
                        x = rodzic1.dna[v];
                    }
                    if (visited[x])
                    {
                        dziecko.dna[v] = -1;
                        v = lista.First();
                        lista2.Add(v);
                        lista.Remove(v);                  
                        visited[v] = true;
                        continue;
                    }
                    dziecko.dna[v] = x;
                    v = x;
                }
                lista.Remove(v);
                visited[v] = true;
            }

            dziecko.dna[v] = -1;
            v = 0;

            for(int i = 0; i < lista2.Count; i++)
            {
                visited[lista2[i]] = false;
            }

            for(int i = 1; i < size; i++)
            {
                if(dziecko.dna[v] != -1)
                {
                    v = dziecko.dna[v];
                    continue;
                }

                int r = this.rnd.Next();
                if(r % 2 == 0)
                {
                    int x = rodzic1.dna[v];
                    if(visited[x])
                    {
                        x = rodzic2.dna[v];
                    }
                    if(visited[x])
                    {
                        r = this.rnd.Next(lista2.Count);
                        x = lista2[r];
                        lista2.RemoveAt(r);
                    }
                    else
                    {
                        lista2.Remove(x);
                    }
                    dziecko.dna[v] = x;
                    v = x;
                }
                else
                {
                    int x = rodzic2.dna[v];
                    if (visited[x])
                    {
                        x = rodzic1.dna[v];
                    }
                    if (visited[x])
                    {
                        r = this.rnd.Next(lista2.Count);
                        x = lista2[r];
                        lista2.RemoveAt(r);
                    }
                    else
                    {
                        lista2.Remove(x);
                    }
                    dziecko.dna[v] = x;
                    v = x;
                }
                visited[v] = true;
            }
            dziecko.dna[v] = 0;
            return dziecko;
        }

        public void Mutuj(ref Osobnik dziecko, int size, int count = 1)
        {
            for(int i = 0; i < count; i++)
            {
                int x = this.rnd.Next(size);
                int y = this.rnd.Next(size);
                while (y == x)
                    y = this.rnd.Next(size);
                int tmp = dziecko.dna[x];
                dziecko.dna[x] = dziecko.dna[y];
                dziecko.dna[y] = tmp;
                int tmp2 = dziecko.dna[dziecko.dna[x]];
                dziecko.dna[dziecko.dna[x]] = dziecko.dna[tmp];
                dziecko.dna[tmp] = tmp2;
            }
        }

        private void Sort(int start, int count)
        {
            for(int i = start; i < start+count; i++)
            {
                int j = i-1;
                int k = i;
                while(this.populacja[k].ocena < this.populacja[j].ocena)
                {
                    this.Swap<Osobnik>(ref this.populacja[k--], ref this.populacja[j--]);
                    if (j < 0) break;
                }
            }
        }

        public void Start()
        {
            this.GenerujPopulacje();
            MethodInfo methodOcena = this.GetType().GetMethod(this.config.funkcjaOceny);
            MethodInfo methodSelekcja = this.GetType().GetMethod(this.config.metodaSelekcji);
            int ilosc = this.config.rozmiarPopulacji;
            int dzieciNaPokolenie = (int)(ilosc * this.config.wspolczynnikPotomkow);
            methodOcena.Invoke(this, new object[] { 0, ilosc });
            int pokolenie = 0;
            object[] parameters = new object[] { ilosc };
            while(pokolenie++ < this.config.iloscPokolen)
            {
                for(int i = ilosc; i < ilosc + dzieciNaPokolenie; i++)
                {
                    Osobnik rodzic1 = (Osobnik)methodSelekcja.Invoke(this, parameters);
                    Osobnik rodzic2 = (Osobnik)methodSelekcja.Invoke(this, parameters);
                    this.populacja[i] = this.Krzyzuj(rodzic1, rodzic2, this.g.size);
                }
                methodOcena.Invoke(this, new object[] { 0, ilosc+dzieciNaPokolenie });
                this.Sort(1, ilosc+dzieciNaPokolenie-1);
                Console.WriteLine("Pokolenie {0}", pokolenie);
                Console.WriteLine(this.populacja[0]);
            }
        }
    }
}
