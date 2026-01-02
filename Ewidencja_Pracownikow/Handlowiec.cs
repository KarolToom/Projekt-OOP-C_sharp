using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ewidencja_Pracownikow
{
    public class Handlowiec : Pracownik // Klasa dziedzicząca po klasie Pracownik
    {
        public decimal WartoscSprzedazy { get; set; }
        public decimal ProcentProwizji { get; set; }
        public Handlowiec(string imie, string nazwisko, string pesel, decimal pensja, decimal wartoscSprzedazy, decimal procentOdSprzedazy)
            : base(imie, nazwisko, pesel, pensja)
        {
            WartoscSprzedazy = wartoscSprzedazy;
            ProcentProwizji = procentOdSprzedazy;
        }
        public override decimal ObliczPremie()
        {
            return WartoscSprzedazy * (decimal)(ProcentProwizji / 100);
        }
    }
}
