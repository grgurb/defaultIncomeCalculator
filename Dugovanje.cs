using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

namespace RacunanjeZatezneKamateConsole
{
    class Dugovanje
    {
        public static List<string[]> detalji = new List<string[]>();
        private readonly string sifraDugovanja;
        private readonly decimal glavnica;
        private readonly DateTime rok;
        private readonly DateTime danUplate;
        public Dugovanje(string sifraDugovanja, string glavnica, string rok, string danUplate)
        {
            try
            {
                this.sifraDugovanja = sifraDugovanja;
                if (!Decimal.TryParse(glavnica, out this.glavnica)) throw new ArgumentException("Neispravno uneta glavnica dugovanja.");
                if (!DateTime.TryParseExact(rok, "d.M.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out this.rok)) throw new ArgumentException("Neispravno unet datum roka uplate dugovanja.");
                if (!DateTime.TryParseExact(danUplate, "d.M.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out this.danUplate)) throw new ArgumentException("Neispravno unet datum dana uplate dugovanja.");
            }
            catch (Exception greska)
            {
                Console.WriteLine(greska);
            }
        }
        public string SifraDugovanja => sifraDugovanja;
        public decimal Glavnica => glavnica;
        public DateTime Rok => rok;
        public DateTime DanUplate => danUplate;
        public decimal DugovanjeNaDanUplate(KamatneStopeValutePeriodi kamatneStope, bool ispis)
        {
            decimal celokupnoDugovanje = 0;
            celokupnoDugovanje += glavnica;
            LinkedListNode<KamatnaStopa> trenutniNode = kamatneStope.Stope.First;
            foreach (KamatnaStopa stopa in kamatneStope.Stope)
            {
                if (DateTime.Compare(rok, stopa.DatumStupanjaNaSnagu) == 0)
                {
                    trenutniNode = kamatneStope.Stope.Find(stopa);
                    break;
                }
                else if (DateTime.Compare(rok, stopa.DatumStupanjaNaSnagu) < 0)
                {
                    KamatnaStopa tmp = kamatneStope.Stope.Find(stopa).Previous.Value;
                    celokupnoDugovanje += formulaDugovanja(glavnica, tmp.Stopa, (stopa.DatumStupanjaNaSnagu - rok).Days, tmp.DatumStupanjaNaSnagu, ispis);
                    trenutniNode = kamatneStope.Stope.Find(stopa);
                    break;
                }
            }
            while (DateTime.Compare(trenutniNode.Next.Value.DatumStupanjaNaSnagu, DanUplate) <= 0)
            {
                if (trenutniNode == kamatneStope.Stope.Last.Previous)
                {
                    break;
                }
                celokupnoDugovanje += formulaDugovanja(glavnica, trenutniNode.Value.Stopa, (trenutniNode.Next.Value.DatumStupanjaNaSnagu - trenutniNode.Value.DatumStupanjaNaSnagu).Days, trenutniNode.Value.DatumStupanjaNaSnagu, ispis);
                trenutniNode = trenutniNode.Next;
            }
            if (DateTime.Compare(kamatneStope.Stope.Last.Value.DatumStupanjaNaSnagu, danUplate) < 0)
            {
                celokupnoDugovanje += formulaDugovanja(glavnica, trenutniNode.Value.Stopa, (kamatneStope.Stope.Last.Value.DatumStupanjaNaSnagu - trenutniNode.Value.DatumStupanjaNaSnagu).Days, trenutniNode.Value.DatumStupanjaNaSnagu, ispis);
                celokupnoDugovanje += formulaDugovanja(glavnica, kamatneStope.Stope.Last.Value.Stopa, (danUplate - kamatneStope.Stope.Last.Value.DatumStupanjaNaSnagu).Days + 1, kamatneStope.Stope.Last.Value.DatumStupanjaNaSnagu, ispis);
            }
            else
            {
                celokupnoDugovanje += formulaDugovanja(glavnica, trenutniNode.Value.Stopa, (danUplate - trenutniNode.Value.DatumStupanjaNaSnagu).Days + 1, trenutniNode.Value.DatumStupanjaNaSnagu, ispis);
            }
            return celokupnoDugovanje;
        }
        public static decimal formulaDugovanja(decimal glavnica, decimal stopa, decimal brojDanaDocnje, DateTime prviDanObracunskiPeriod, bool ispis)
        {
            decimal danaUGodini = DateTime.IsLeapYear(prviDanObracunskiPeriod.Year) ? 366 : 365;
            decimal kamata = decimal.Round((glavnica * stopa * brojDanaDocnje) / (100 * danaUGodini), 2);
            Console.WriteLine($"Datum: {prviDanObracunskiPeriod}, broj dana: {brojDanaDocnje}, stopa: {stopa}, glavnica:{glavnica}, kamata:{kamata}");
            if (ispis)
            {
                detalji.Add(new string[] { prviDanObracunskiPeriod.ToString(), brojDanaDocnje.ToString(), stopa.ToString(), glavnica.ToString(), kamata.ToString() });
            }
            return decimal.Round(kamata, 2);
        }
        public static List<Dugovanje> ucitavanjeDugovanja(string lokacijaFajl)
        {
            List<Dugovanje> listaDugovanja = new List<Dugovanje>();
            try
            {
                using (StreamReader fajlStream = File.OpenText(lokacijaFajl))
                {
                    string tmp;
                    try
                    {
                        while ((tmp = fajlStream.ReadLine()) != null)
                        {
                            string[] tmpArray = tmp.Split(",");
                            listaDugovanja.Add(new Dugovanje(tmpArray[0], tmpArray[1], tmpArray[2], tmpArray[3]));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                return listaDugovanja;
            }
            catch (Exception greska)
            {
                Console.WriteLine(greska);
                return null;
            }
        }
    }
}