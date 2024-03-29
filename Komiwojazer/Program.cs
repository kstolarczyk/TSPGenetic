﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komiwojazer
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Wpisz nazwę pliku instancji");
            string fileName = Console.ReadLine();
            try
            {
                Graf graf = new Graf(fileName);
                //Console.WriteLine(graf.ToString());
                Konfiguracja conf = new Konfiguracja(2000, 1, 0.01, 5000, 0, "SelekcjaBO3", "Ocen1");
                Genetyk gen = new Genetyk(graf, conf);
                gen.Start();
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            
            Console.ReadKey();
        }
    }
}
