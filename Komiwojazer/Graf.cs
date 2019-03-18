using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Komiwojazer
{
    class Graf
    {
        public float[,] odleglosci { get; private set; }
        private int[,] wspolrzedne { get; set; }
        public int size { get; set; }
        public Graf(string instanceFileName)
        {
            StreamReader stream = new StreamReader(instanceFileName);
            int v, x, y, iter = 0;
            string line;
            if(!Int32.TryParse(stream.ReadLine(), out v))
            {
                throw new Exception("Nieprawidłowy format pliku instancji");
            }
            this.wspolrzedne = new int[v,2];
            this.odleglosci = new float[v, v];
            this.size = v;

            while((line = stream.ReadLine()) != null)
            {
                string[] cols = line.Split(' ');
                if(!Int32.TryParse(cols[1], out x) || !Int32.TryParse(cols[2], out y))
                {
                    throw new Exception("Nieprawidłowy format pliku, linia: " + (iter + 2));
                }
                this.wspolrzedne[iter, 0] = x;
                this.wspolrzedne[iter++, 1] = y;
            }

            for(int i = 0; i < v; i++)
            {
                for(int j = i+1; j < v; j++)
                {
                    float odleglosc = (float)Math.Sqrt(Math.Pow(this.wspolrzedne[j, 0] - this.wspolrzedne[i, 0], 2) + Math.Pow(this.wspolrzedne[j, 1] - this.wspolrzedne[i, 1], 2));
                    this.odleglosci[i, j] = odleglosc;
                    this.odleglosci[j, i] = odleglosc;
                }
            }
        }

        override public string ToString()
        {
            string returned = "";
            for(int i = 0; i < this.size; i++)
            {
                for(int j = 0; j < this.size; j++)
                {
                    if(i == j) {
                        returned += "***\t";
                    }
                    else
                    {
                        returned += this.odleglosci[i, j] + "\t";
                    }
                }
                returned += "\n";
            }
            return returned;
        }
    }
}
