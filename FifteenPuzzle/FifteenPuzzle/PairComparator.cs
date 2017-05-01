using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FifteenPuzzle
{
    class PairComparator : IComparer<Pair>
    {
        int[,] ulozonaTablica = new int[,] { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 }, {13, 14, 15, 0} };
        public int Compare(Pair tablica1, Pair tablica2)
        {
            int sumaTablica1 = 0;
            int sumaTablica2 = 0;

            int[,] tablica11 = zamienNaDwaWymiary(tablica1.plansza);
            int[,] tablica22 = zamienNaDwaWymiary(tablica2.plansza);

            for (int k = 0; k < 16; k++)
            {
                int[] znajdzWartoscUlozonaTablica = znajdzWartosc(k, ulozonaTablica);
                int[] znajdzWartoscTablicaPrzeszukiwania = znajdzWartosc(k, tablica11);
                sumaTablica1 += Math.Abs(znajdzWartoscUlozonaTablica[0] - znajdzWartoscTablicaPrzeszukiwania[0])
                                + Math.Abs(znajdzWartoscUlozonaTablica[1] - znajdzWartoscTablicaPrzeszukiwania[1]);
            }

            for (int k = 0; k < 16; k++)
            {
                int[] znajdzWartoscUlozonaTablica = znajdzWartosc(k, ulozonaTablica);
                int[] znajdzWartoscTablicaPrzeszukiwania = znajdzWartosc(k, tablica22);
                sumaTablica2 += Math.Abs(znajdzWartoscUlozonaTablica[0] - znajdzWartoscTablicaPrzeszukiwania[0])
                                + Math.Abs(znajdzWartoscUlozonaTablica[1] - znajdzWartoscTablicaPrzeszukiwania[1]);
            }

            if (sumaTablica1 < sumaTablica2)
            {
                return 1;
            }
            else if (sumaTablica1 > sumaTablica2)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private int[] znajdzWartosc(int wartosc, int[,] tablica)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (tablica[i, j] == wartosc)
                    {
                        int[] wynik = new int[] { i, j };
                        return wynik;
                    }
                }
            }
            return null;
        }

        int[,] zamienNaDwaWymiary(int[] tablica)
        {
            int kolumny = 4;
            int wiersze = 4;
            int[,] wynik = new int[kolumny, wiersze];

            int k = 0;
            for(int i = 0; i<kolumny;i++)
            for(int j = 0; j<wiersze;j++, k++)
                wynik[i, j] = tablica[k];
            return wynik;
        }
}
}
