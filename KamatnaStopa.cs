using System;
using System.Globalization;

namespace RacunanjeZatezneKamateConsole
{
    class KamatnaStopa
    {
        private readonly DateTime datumStupanjaNaSnagu;
        private readonly decimal stopa;
        private decimal tmpDugovanjeZaPeriod = 0;
        public KamatnaStopa(string datum, string stopa)
        {
            try {
                if (!DateTime.TryParseExact(datum, "d.M.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out datumStupanjaNaSnagu)) throw new ArgumentException("Neispravno unet datum stope zatezne kamate.");
                if (!Decimal.TryParse(stopa, out this.stopa)) throw new ArgumentException("Neispravno uneta stopa zatezne kamate.");
            }
            catch(Exception greska)
            {
                Console.WriteLine(greska);
            }
        }
        public KamatnaStopa(DateTime datum, decimal stopa)
        {
            datumStupanjaNaSnagu = datum;
            this.stopa = stopa;
        }
        public DateTime DatumStupanjaNaSnagu => datumStupanjaNaSnagu;
        public decimal Stopa => stopa;
        public decimal TmpDugovanjeZaPeriod
        {
            get => tmpDugovanjeZaPeriod;
            set => tmpDugovanjeZaPeriod = value;
        }
        public override string ToString()
        {
            return  $"{PomocneFunkcije.Datum(DatumStupanjaNaSnagu)} {stopa}%";
        }
    }
}