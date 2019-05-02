using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Komiwojazer
{
    class Genetyk
    {
        public Graf g { get; set; }
        public Random rnd;
        public Osobnik[] populacja { get; set; }
        private Konfiguracja config { get; set; }
        private Stopwatch watch { get; set; }
        private static object mtx = new object();
        public Genetyk(Graf graf, Konfiguracja konfiguracja)
        {
            this.g = graf;
            this.config = konfiguracja;
            this.rnd = new Random();
            this.GenerujPopulacje();
            this.watch = new Stopwatch();
        }

        private void ShuffleArray(ref int[] array)
        {
            int len = array.Length;
            for(int i = len-1; i > 0; i--)
            {
                int j = RandomGen.Next(0, i+1);
                this.Swap<int>(ref array[i], ref array[j]);
            }
        }

        private void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        private Osobnik Zachlanny(int start)
        {
            int size = this.g.size;
            Osobnik o = new Osobnik(size);
            List<int> lista = new List<int>(size);
            for(int i = 0; i < size; i++)
            {
                lista.Add(i);
            }
            lista.Remove(start);
            int v = start;
            while(lista.Count > 0)
            {
                o.dna[v] = this.Najblizszy(v, ref lista);
                v = o.dna[v];
            }
            o.dna[v] = start;
            return o;
        }

        private void GenerujPopulacje(bool alreadyGenerated = false)
        {
            int size = this.g.size;
            int ilosc = this.config.rozmiarPopulacji;
            int ileZachlannych = this.config.ileZachlannych;
            int start = 0;

            int nadmiar = (int)Math.Floor(this.config.wspolczynnikPotomkow * ilosc);
            this.populacja = new Osobnik[ilosc + nadmiar];
          
            for (int i = start; i < ileZachlannych; i++)
            {
                this.populacja[i] = this.Zachlanny(this.rnd.Next(size));
            }
            int[] kod = new int[size];
            for(int i = 0; i < size; i++)
            {
                kod[i] = i;
            }
        
            for(int i = ileZachlannych; i < ilosc; i++)
            {
                this.ShuffleArray(ref kod);
                this.populacja[i] = new Osobnik(kod);
            }
        }

        public double Ocen1(int start, int count)
        {
            double total = 0;
            Parallel.For<double>(start, start + count, () => 0, (j, loop, local) =>
            {
                int v = 0;
                double dlugoscTrasy = 0;
                for (int i = 0; i < this.g.size; i++)
                {
                    int r = this.populacja[j].dna[v];
                    dlugoscTrasy += this.g.odleglosci[v, r];
                    v = r;
                }
                this.populacja[j].ocena = (int)dlugoscTrasy;
                local += dlugoscTrasy;
                return local;
            },
            (x) => { lock (mtx) { total += x; }  });

            double avg = total / count;
            Console.Clear();
            Console.WriteLine("Średni dystans populacji: {0}", avg);
            Array.Sort(this.populacja, (a,b) => a == null ? 1 : b == null ? -1 : a.CompareTo(b));
            return avg;
        }



        public double Ocen2(int start, int count)
        {
            double avg = this.Ocen1(start, count);
            double totalp = 0;
            int ilosc = this.config.rozmiarPopulacji;
            Parallel.For<double>(0, ilosc, () => 0, (j, loop, local) =>
            {
                double p = 1.0 / this.populacja[j].ocena;
                this.populacja[j].p = p;
                local += p;
                return local;
            },
            (x) => { lock (mtx) { totalp += x; } });

            double currentp = 0;

            for(int j = 0; j < ilosc-1; j++)
            {
                currentp += this.populacja[j].p / totalp;
                this.populacja[j].p = currentp;
            }
            this.populacja[ilosc - 1].p = 1;
            return avg;
        }

        public void SelekcjaRuletka(out Osobnik rodzic1, out Osobnik rodzic2)
        {
            int ilosc = this.config.rozmiarPopulacji;
            double r1 = RandomGen.NextDouble();
            double r2 = RandomGen.NextDouble();
            rodzic1 = this.binarySearch(0, ilosc, r1);
            rodzic2 = this.binarySearch(0, ilosc, r2);
        }

        public Osobnik binarySearch(int left, int right, double r)
        {
            int k;
            while (true)
            {
                k = (left + right) / 2;
                if (r <= this.populacja[k].p)
                {
                    if (k == left) break;
                    right = k;
                }
                else
                {
                    if (k == left) { k = right; break; }
                    left = k;
                }
            }
            return this.populacja[k];
        }

        public void SelekcjaBO3(out Osobnik rodzic1, out Osobnik rodzic2) // best of three
        {
            int ilosc = this.config.rozmiarPopulacji;
            int r1 = RandomGen.Next(ilosc);
            int r2 = RandomGen.Next(ilosc);
            int r3 = RandomGen.Next(ilosc);
            int r4 = RandomGen.Next(ilosc);
            int r5 = RandomGen.Next(ilosc);
            int r6 = RandomGen.Next(ilosc);
            if (this.populacja[r1].ocena < this.populacja[r2].ocena)
            {
                rodzic1 = (this.populacja[r1].ocena < this.populacja[r3].ocena) ? this.populacja[r1] : this.populacja[r3];
            }
            else
            {
                rodzic1 = (this.populacja[r2].ocena < this.populacja[r3].ocena) ? this.populacja[r2] : this.populacja[r3];
            }
            if (this.populacja[r4].ocena < this.populacja[r5].ocena)
            {
                rodzic2 = (this.populacja[r4].ocena < this.populacja[r6].ocena) ? this.populacja[r4] : this.populacja[r6];
            }
            else
            {
                rodzic2 = (this.populacja[r5].ocena < this.populacja[r6].ocena) ? this.populacja[r5] : this.populacja[r6];
            }
        }

        public void SelekcjaBO2(out Osobnik rodzic1, out Osobnik rodzic2) // best of two
        {
            int ilosc = this.config.rozmiarPopulacji;
            int r1 = RandomGen.Next(ilosc);
            int r2 = RandomGen.Next(ilosc);
            int r3 = RandomGen.Next(ilosc);
            int r4 = RandomGen.Next(ilosc);
            if (this.populacja[r1].ocena < this.populacja[r2].ocena)
            {
                rodzic1 = this.populacja[r1];
            }
            else
            {
                rodzic1 = this.populacja[r2];
            }
            if (this.populacja[r3].ocena < this.populacja[r4].ocena)
            {
                rodzic2 = this.populacja[r3];
            }
            else
            {
                rodzic2 = this.populacja[r4];
            }
        }

        private void ResetVisited(ref bool[] visited)
        {
            for(int i = 0; i < visited.Length; i++)
            {
                visited[i] = false;
            }
        }

        private int Najblizszy(int current, ref List<int> lista2)
        {
            int index = lista2[0];
            double min = this.g.odleglosci[current,index];
            foreach(int a in lista2)
            {
                double odl = this.g.odleglosci[current, a];
                if(odl < min)
                {
                    min = odl;
                    index = a;
                }
            }
            lista2.Remove(index);
            return index;
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
                int r = RandomGen.Next(2);
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

                int r = RandomGen.Next(2);
                if(r % 2 == 0)
                {
                    int x = rodzic1.dna[v];
                    if(visited[x])
                    {
                        x = rodzic2.dna[v];
                    }
                    if(visited[x])
                    {
                        //r = RandomGen.Next(lista2.Count);
                        //x = lista2[r];
                        //lista2.RemoveAt(r);
                        x = this.Najblizszy(v, ref lista2);

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
                        //r = RandomGen.Next(lista2.Count);
                        //x = lista2[r];
                        //lista2.RemoveAt(r);
                        x = this.Najblizszy(v, ref lista2);
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
                int x = RandomGen.Next(size);
                int y = RandomGen.Next(size);
                while (y == x)
                    y = RandomGen.Next(size);
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
                while(populacja[k].ocena < populacja[j].ocena)
                {
                    this.Swap<Osobnik>(ref populacja[k--], ref populacja[j--]);
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
            int licznik = 100;
            int dzieciNaPokolenie = (int)(ilosc * this.config.wspolczynnikPotomkow);
            methodOcena.Invoke(this, new object[] { 0, ilosc });
            int pokolenie = 0;
            watch.Start();
            while(true && pokolenie++ < this.config.iloscPokolen)
            {
                Parallel.For(ilosc, ilosc + dzieciNaPokolenie, (i) =>
                {
                    object[] parameters = new object[] { null, null };
                    methodSelekcja.Invoke(this, parameters);
                    Osobnik rodzic1 = parameters[0] as Osobnik, rodzic2 = parameters[1] as Osobnik;
                    this.populacja[i] = this.Krzyzuj(rodzic1, rodzic2, this.g.size);
                    if (RandomGen.NextDouble() <= this.config.prawdMutacji)
                    {
                        this.Mutuj(ref this.populacja[i], this.g.size, 1);
                    }

                });
              

                double avg = (double)methodOcena.Invoke(this, new object[] { ilosc, dzieciNaPokolenie });
            
                Console.WriteLine("Najlepszy wynik: {0};\tLicz.pokoleń/sek: {1}", this.populacja[0].ocena, (pokolenie*1000.0/watch.ElapsedMilliseconds));
                if(avg <= this.populacja[0].ocena*1.003 && licznik-- < 0)
                {
                    break;
                }
                else
                {
                    licznik++;
                }
            }
            watch.Stop();
            Console.WriteLine("Pokolenie {0}", pokolenie-1);
            Console.WriteLine(this.populacja[0]);
        }
    }
}
