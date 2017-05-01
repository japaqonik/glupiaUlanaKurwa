using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FifteenPuzzle
{
    class Program
    {
        private static StreamReader skaner;
        private static Logic pietnastka;
        private static int wiersze;
        private static int kolumny;
        private static int[] tablica;
        static void Main(string[] args)
        {
            Console.WriteLine("Podaj nazwę pliku: ");
            String fileName = Console.ReadLine();
            string path = @"state\" + fileName + ".txt";
            if (File.Exists(path))
            {
                pietnastka = wczytajZPliku(path);
                pietnastka.wczytajParametry(args);
            }
            else
            {
                Console.WriteLine("Plik nie istnieje!!!\n" +
                                  "Wzkazówka:\n" +
                                  "\t Plik musi się znajdować w folderze data w głównym katalogu aplikacji.");
            }

            Console.ReadLine();
        }
        private static Logic wczytajZPliku(String path)
        {

            string[] text = System.IO.File.ReadAllLines(path);
            List<int> tempTab=new List<int>();

            foreach (var line in text)
            {
                if (line==text[0])
                {
                    string[] currentLine = line.Split(' ');
                    wiersze = Convert.ToInt32(currentLine[0]);
                    kolumny = Convert.ToInt32(currentLine[1]);
                }
                else
                {
                    string[] currentLine = line.Split(' ');
                    foreach (var intValue in currentLine)
                    {
                            tempTab.Add(Convert.ToInt32(intValue));
                    }
                }
            }

            tablica = tempTab.ToArray();
            return new Logic(tablica, wiersze, kolumny);
        }
    }
}
