using System;
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
                Console.WriteLine(graf.ToString());
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}
