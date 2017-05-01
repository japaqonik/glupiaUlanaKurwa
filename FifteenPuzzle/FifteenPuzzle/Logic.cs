using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FifteenPuzzle
{
    class Logic
    {
        private Dictionary<int,int> rodzice = new Dictionary<int, int>();
        private List<int[]> odwiedzone = new List<int[]>();
        private List<Pair> doOdwiedzenia = new List<Pair>();
        private PairComparator aStarKomparator;

        String wybor;
        Direction[] kierunek;

        private int wiersze;
        private int kolumny;
        private int rozmiarTablicy;
        private int[] tablica;
        private int[] ulozonaTablica;
        private bool koniec;

        private long tKoniec;

        public Logic(int[] tablica, int wiersze, int kolumny)
        {
            this.tablica = tablica;
            this.wiersze = wiersze;
            rozmiarTablicy = wiersze * kolumny;
            this.kolumny = kolumny;
            wypelnijUlozonaTablice();
            doOdwiedzenia.Add(new Pair(tablica, 0));
            koniec = false;
        }
 
        public Logic(int[] table, int wiersze, int kolumny, String[] ruchy)
        {

            this.wiersze = wiersze;
            this.kolumny = kolumny;
            rozmiarTablicy = wiersze * kolumny;

            List<Direction> kierunek = new List<Direction>();

            foreach (String str in ruchy)
            {
                switch (str)
                {
                    case "G":
                        kierunek.Add(Direction.GORA);
                        break;
                    case "D":
                        kierunek.Add(Direction.DOL);
                        break;
                    case "L":
                        kierunek.Add(Direction.LEWO);
                        break;
                    case "P":
                        kierunek.Add(Direction.PRAWO);
                        break;
                }
            }
            System.Console.WriteLine("Tablica wejściowa");
            pokazTablice(table);
            doWizualizacji(table, kierunek);
        }

        public void wczytajParametry(String[] args)
        {
            switch (args.Length)
            {
                case 0:
                    System.Console.WriteLine("Brak parametru: wpisz -h lub --help aby uzyskać pomoc");
                    break;
                case 1:
                    switch (args[0])
                    {
                        case "--help":
                        case "-h":
                            System.Console.WriteLine("\n\nFifteenPuzzle.exe [SEARCH ALGORITHM] [ORDER]\n\n" +
                                                     "[SEARCH ALGORITHM]:\n" +
                                                     "\t-b, --bfs\n" +
                                                     "\t\tBFS algorithm\n" +
                                                     "\t-d, --dfs\n" +
                                                     "\t\tDFS algorithm\n" +
                                                     "\t-i --idfs\n" +
                                                     "\t\tiDFS algorithm\n" +
                                                     "\t-n, --n\n" +
                                                     "\t\tA Star algorithm\n\n" +
                                                     "[ORDER]\n" +
                                                     "\tR\n" +
                                                     "\t\tRandom\n" +
                                                     "\t[PERMUTATION]\n" +
                                                     "\t\tCombinantion of: {L, P, G, D}");
                            break;
                        default:
                            System.Console.WriteLine("Nie rozpoznano parametru: wpisz -h lub --help aby uzyskać pomoc");
                            break;
                    }
                    break;
                case 2:
                    this.wybor = args[1];
                    switch (args[0])
                    {
                        case "--bfs":
                        case "-b":
                        {
                            System.Console.WriteLine("strategia: BFS");
                            Stopwatch timer = new Stopwatch();
                            timer.Start();
                            BFS();
                            timer.Stop();
                            tKoniec = timer.ElapsedMilliseconds;
                            break;
                        }
                        case "--dfs":
                        case "-d":
                        {
                            System.Console.WriteLine("strategia: DFS");
                            Stopwatch timer = new Stopwatch();
                            timer.Start();
                            DFS(20);
                            timer.Stop();
                            tKoniec = timer.ElapsedMilliseconds;
                            break;
                        }
                        case "--idfs":
                        case "-i":
                        {
                            System.Console.WriteLine("strategia: DFS z pogłębianiem");
                            Stopwatch timer = new Stopwatch();
                            timer.Start();
                            IDFS();
                            timer.Stop();
                            tKoniec = timer.ElapsedMilliseconds;
                            break;
                        }
                        case "--nn":
                        case "-n":
                        {
                            System.Console.WriteLine("strategia: A*");
                            aStarKomparator = new PairComparator();
                            Stopwatch timer = new Stopwatch();
                            timer.Start();
                            aStar();
                            timer.Stop();
                            tKoniec = timer.ElapsedMilliseconds;
                            break;
                        }                    
                        default:
                            System.Console.WriteLine("Błędny parametr");
                            break;
                    }
                    System.Console.WriteLine("czas: " + tKoniec + " ms");
                    break;
                default:
                    System.Console.WriteLine("Zbyt dużo parametrów: wpisz -h lub --help aby uzyskać pomoc");
                    break;
            }
        }

        private void BFS()
        {
            while (doOdwiedzenia.Count != 0 && !koniec)
            {
                Pair pair = doOdwiedzenia.First();
                doOdwiedzenia.RemoveAt(0);
                odzwiedzIWyszukaj(pair);
            }
        }

        private void DFS(int glebokosc)
        {
            while (doOdwiedzenia.Count != 0 && !koniec)
            {
                Pair para = doOdwiedzenia.Last();
                doOdwiedzenia.RemoveAt(doOdwiedzenia.Count-1);
                int rodzicNr = para.rodzic;
                if ((glebokoscElementu(rodzicNr) > glebokosc) && glebokosc != 0)
                {
                    continue;
                }
                odzwiedzIWyszukaj(para);
            }
        }

        private void IDFS()
        {
            for (int i = 1; !koniec || i < 30; i++)
            {
                odwiedzone.Clear();
                rodzice.Clear();
                DFS(i);
                doOdwiedzenia.Add(new Pair(tablica, 0));
            }
        }

        private void aStar()
        {
            while (doOdwiedzenia.Count != 0 && !koniec)
            {
                doOdwiedzenia.Sort(aStarKomparator);
                doOdwiedzenia.Reverse();
                Pair p = doOdwiedzenia.First();
                odzwiedzIWyszukaj(p);
            }
        }

        private void wypelnijUlozonaTablice()
        {
            ulozonaTablica = new int[rozmiarTablicy];
            for (int i = 0; i < rozmiarTablicy - 1; i++)
            {
                ulozonaTablica[i] = i + 1;
            }
        }

        private void odzwiedzIWyszukaj(Pair para)
        {
            int[] aktPlansza = para.plansza;
            int rodzicNr = para.rodzic;
            if (!odwiedz(aktPlansza, rodzicNr))
            {
                return;
            }
            int rodzicNowyNr = odwiedzone.Count() - 1;
            foreach (Direction dir in wyborKierunku())
            {
                przesun(dir, aktPlansza, rodzicNowyNr);
            }
        }

        private bool odwiedz(int[] aktPlansza, int rodzic)
        {
            foreach (int[] plansza in odwiedzone)
            {
                if (IntArrayEquals(plansza, aktPlansza))
                {
                    return false;
                }
            }
            odwiedzone.Add(aktPlansza);
            rodzice.Add(odwiedzone.Count() - 1, rodzic);
            if (IntArrayEquals(aktPlansza, ulozonaTablica))
            {
                koniec = true;
                StringBuilder sb = new StringBuilder();
                znajdzRuchy(odwiedzone.Count() - 1, sb);
                wyswietlOutput(sb);
                return false;
            }
            return true;
        }

        private void wyswietlOutput(StringBuilder sb)
        {
            System.Console.WriteLine("ilość ruchów: " + sb.Length);
            Array.Reverse(sb.ToString().ToCharArray());
            System.Console.WriteLine("ruchy: " + sb);
            System.Console.WriteLine("odwiedzone węzły: " + odwiedzone.Count);
            System.Console.WriteLine("rozwiązanie: Tak");
        }

        private Direction[] wyborKierunku()
        {
            if (wybor.Equals("R"))
            {
                kierunek = new Direction[4];
                List<Direction> l = new List<Direction>();
                Random rand = new Random();
                foreach (Direction d in Enum.GetValues(typeof(Direction)))
                {
                    l.Add(d);
                }
                for (int i = 0; i < 4; i++)
                {
                    int randInt = rand.Next(0, l.Count());
                    kierunek[i] = l[randInt];
                    l.RemoveAt(randInt);
                }
            }
            else
            {
                if (kierunek == null)
                {
                    kierunek = new Direction[4];
                    int i = 0;
                    foreach (char znak in wybor.ToCharArray())
                    {
                        switch (znak)
                        {
                            case 'G':
                                kierunek[i] = Direction.GORA;
                                break;
                            case 'D':
                                kierunek[i] = Direction.DOL;
                                break;
                            case 'L':
                                kierunek[i] = Direction.LEWO;
                                break;
                            case 'P':
                                kierunek[i] = Direction.PRAWO;
                                break;
                        }
                        i++;
                    }
                }
            }
            return kierunek;
        }

        void pokazTablice(int[] tablica)
        {
            for (int i = 0; i < wiersze * kolumny; i++)
            {
                System.Console.Write(tablica[i]);
                System.Console.Write(" ");
                if ((i % kolumny) + 1 == kolumny)
                {
                    System.Console.WriteLine();
                }
            }
            System.Console.WriteLine();
        }

        private void przesun(Direction kierunek, int[] aktPlansza, int p)
        {
            int[] nowaPlansza = new int[aktPlansza.Length];
            Array.Copy(aktPlansza, nowaPlansza, aktPlansza.Length);

            int pozycja = znajdzZerowyElement(nowaPlansza);
            int temp;
            switch (kierunek)
            {
                case Direction.GORA:
                    if (pozycja < kolumny)
                    {
                        return;
                    }
                    temp = nowaPlansza[pozycja - kolumny];
                    nowaPlansza[pozycja - kolumny] = 0;
                    nowaPlansza[pozycja] = temp;
                    break;
                case Direction.DOL:
                    if (pozycja > (kolumny * wiersze - kolumny - 1))
                    {
                        return;
                    }
                    temp = nowaPlansza[pozycja + kolumny];
                    nowaPlansza[pozycja + kolumny] = 0;
                    nowaPlansza[pozycja] = temp;
                    break;
                case Direction.LEWO:
                    if (pozycja % kolumny == 0)
                    {
                        return;
                    }
                    temp = nowaPlansza[pozycja - 1];
                    nowaPlansza[pozycja - 1] = 0;
                    nowaPlansza[pozycja] = temp;
                    break;
                case Direction.PRAWO:
                    if ((pozycja + 1) % kolumny == 0)
                    {
                        return;
                    }
                    temp = nowaPlansza[pozycja + 1];
                    nowaPlansza[pozycja + 1] = 0;
                    nowaPlansza[pozycja] = temp;
                    break;
            }
            doOdwiedzenia.Add(new Pair(nowaPlansza, p));
        }

        private void doWizualizacji(int[] aktPlansza, List<Direction> ruchy)
        {

            int pozycja = znajdzZerowyElement(aktPlansza);
            int temp;
            Direction kierunek = ruchy.First();
            switch (kierunek)
            {
                case Direction.GORA:
                    if (pozycja < kolumny)
                    {
                        return;
                    }
                    temp = aktPlansza[pozycja - kolumny];
                    aktPlansza[pozycja - kolumny] = 0;
                    aktPlansza[pozycja] = temp;
                    break;
                case Direction.DOL:
                    if (pozycja > (kolumny * wiersze - kolumny - 1))
                    {
                        return;
                    }
                    temp = aktPlansza[pozycja + kolumny];
                    aktPlansza[pozycja + kolumny] = 0;
                    aktPlansza[pozycja] = temp;
                    break;
                case Direction.LEWO:
                    if (pozycja % kolumny == 0)
                    {
                        return;
                    }
                    temp = aktPlansza[pozycja - 1];
                    aktPlansza[pozycja - 1] = 0;
                    aktPlansza[pozycja] = temp;
                    break;
                case Direction.PRAWO:
                    if ((pozycja + 1) % kolumny == 0)
                    {
                        return;
                    }
                    temp = aktPlansza[pozycja + 1];
                    aktPlansza[pozycja + 1] = 0;
                    aktPlansza[pozycja] = temp;
                    break;
            }
            pokazTablice(aktPlansza);
            Thread.Sleep(1000);

            if (ruchy.Count() > 0)
            {
                doWizualizacji(aktPlansza, ruchy);
            }
        }

        private int znajdzZerowyElement(int[] board)
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private void znajdzRuchy(int wezelNr, StringBuilder sb)
        {
            int nastepnyWezel = rodzice[wezelNr];
            while (nastepnyWezel != 0)
            {
                sb.Append(rozpoznajRuch(nastepnyWezel, wezelNr));
                wezelNr = nastepnyWezel;
                nastepnyWezel = rodzice[wezelNr];
            }
            sb.Append(rozpoznajRuch(nastepnyWezel, wezelNr));
        }

        private String rozpoznajRuch(int wezel, int wezelRodzica)
        {
            int[] plansza1 = odwiedzone[wezel];
            int[] plansza2 = odwiedzone[wezelRodzica];
            int pozycja1 = znajdzZerowyElement(plansza1);
            int pozycja2 = znajdzZerowyElement(plansza2);
            if ((pozycja1 - kolumny) == pozycja2)
            {
                return "G";
            }
            else if ((pozycja1 + kolumny) == pozycja2)
            {
                return "D";
            }
            else if ((pozycja1 + 1) == pozycja2)
            {
                return "P";
            }
            else if ((pozycja1 - 1) == pozycja2)
            {
                return "L";
            }

            return "";
        }

        private int glebokoscElementu(int rodzic)
        {
            if (rodzic == 0)
            {
                return 0;
            }
            int licznik = 1, wezelNr;
            int nastepnyWezel = rodzice[rodzic];
            while (nastepnyWezel != 0)
            {
                licznik++;
                wezelNr = nastepnyWezel;
                nastepnyWezel = rodzice[wezelNr];
            }
            return licznik;
        }

        private bool IntArrayEquals(int[] t1, int[] t2)
        {
            int Counter = 0;
            for (int i = 0; i < t1.Length; i++)
            {
                    if (t1[i] == t2[i])
                        Counter++;         
            }
            if (Counter == t1.Length)
                return true;
            return false;
        }
    }
}
