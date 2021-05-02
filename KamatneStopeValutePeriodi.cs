using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace RacunanjeZatezneKamateConsole
{
    class KamatneStopeValutePeriodi
    {
        private readonly string valuta;
        private readonly LinkedList<KamatnaStopa> stope = new LinkedList<KamatnaStopa>();
        public KamatneStopeValutePeriodi(string fajl)
        {
            try
            {
                using (StreamReader fajlStream = File.OpenText(fajl))
                {
                    valuta = fajlStream.ReadLine();
                    string tmp;
                    try
                    {
                        while((tmp = fajlStream.ReadLine()) != null)
                        {
                            string[] tmpArray = tmp.Split(",");
                            stope.AddLast(new KamatnaStopa(tmpArray[0], tmpArray[1]));
                        }
                        DateTime prviUGodini = new DateTime(stope.First.Value.DatumStupanjaNaSnagu.Year + 1, 1, 1);
                        while (DateTime.Compare(prviUGodini, stope.Last.Value.DatumStupanjaNaSnagu) < 0)
                        {
                            foreach (KamatnaStopa trenutniStopa in stope)
                            {
                                if (DateTime.Compare(trenutniStopa.DatumStupanjaNaSnagu, prviUGodini) > 0)
                                {
                                    stope.AddBefore(stope.Find(trenutniStopa), new LinkedListNode<KamatnaStopa>(new KamatnaStopa(prviUGodini, stope.Find(trenutniStopa).Previous.Value.Stopa)));
                                    break;
                                }
                            }
                            prviUGodini = prviUGodini.AddYears(1);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    stope.AddLast(new LinkedListNode<KamatnaStopa>(new KamatnaStopa(new DateTime(2021, 1, 1), stope.Last.Value.Stopa)));
                }
            }
            catch (Exception greska)
            {
                Console.WriteLine(greska);
            }
        }
        public string Valuta => valuta;
        public LinkedList<KamatnaStopa> Stope => stope;
        public override string ToString()
        {
            StringBuilder tmp = new StringBuilder();
            tmp.Append($"Valuta: {valuta}\n");
            foreach (KamatnaStopa stopa in stope)
            {
                tmp.Append($"{stopa}\n");
            }
            return tmp.ToString();
        }
        static public ArrayList nizStopaZaSveValute()
        {
            ArrayList tmp = new ArrayList();
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            dir += @"\kamatneStope";
            string[] stopeFajlovi = Directory.GetFiles(dir);
            foreach(string lokacija in stopeFajlovi)
            {
                tmp.Add(new KamatneStopeValutePeriodi(lokacija));
            }
            return tmp;
        }
    }
}