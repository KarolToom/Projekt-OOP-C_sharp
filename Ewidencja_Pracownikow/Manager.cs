using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ewidencja_Pracownikow
{
    public class Manager : PracownikBiurowy // Klasa dziedzicząca po klasie PracownikBiurowy
    {
        public decimal PremiaMenadzerska { get; set; }
        public Manager(string imie, string nazwisko, string pesel, decimal pensja, decimal dodatekStazowy, decimal premiaMenadzerska)
            : base(imie, nazwisko, pesel, pensja, dodatekStazowy)
        {
            PremiaMenadzerska = premiaMenadzerska;
        }
        public override decimal ObliczPremie()
        {
            return DodatekStazowy + PremiaMenadzerska;
        }
    }
}
