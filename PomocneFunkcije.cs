using System;

namespace RacunanjeZatezneKamateConsole
{
    class PomocneFunkcije
    {
        static public string Datum(DateTime datum)
        {
            return $"{datum.Day}.{datum.Month}.{datum.Year}.";
        }
    }
}