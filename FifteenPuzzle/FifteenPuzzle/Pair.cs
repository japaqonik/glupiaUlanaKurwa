using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FifteenPuzzle
{
    class Pair
    {
        public int[] plansza;
       
        public int rodzic;
        public Pair(int[] plansza, int rodzic)
        {
            this.plansza = plansza;
            this.rodzic = rodzic;
        }
        public String toString()
        {
            String wynik = "";
            foreach (int b in plansza)
            {
                wynik += b + " ";
            }

            return wynik + "\n";
        }
    }
}
