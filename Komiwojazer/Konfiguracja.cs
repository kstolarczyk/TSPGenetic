using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komiwojazer
{
    class Konfiguracja
    {
        public int rozmiarPopulacji { get; set; } // liczba naturalna - stała liczba populacji po każdej
        public double wspolczynnikPotomkow { get; set; } // wartość z przedziału (0;1) - jaka część populacji będzie stanowić liczbę krzyżowań (ile powstanie nowych potomków)
        public int iloscPokolen { get; set; } // maksymalna liczba pokoleń do zatrzymania algorytmu
        public string metodaSelekcji { get; set; } // nazwa metody selekcji w klasie Genetyk
        public string funkcjaOceny { get; set; } // nazwa funkcji oceny w klasie Genetyk

        public Konfiguracja(int rozmiarPopulacji, double wspolczynnikPotomkow, int iloscPokolen, string metodaSelekcji, string funkcjaOceny)
        {
            this.rozmiarPopulacji = rozmiarPopulacji;
            this.wspolczynnikPotomkow = wspolczynnikPotomkow;
            this.iloscPokolen = iloscPokolen;
            this.metodaSelekcji = metodaSelekcji;
            this.funkcjaOceny = funkcjaOceny;
        }
    }
}
