using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ewidencja_Pracownikow
{
    public class PracownikBiurowy : Pracownik
    {
        public decimal DodatekStazowy { get; set; }

        public PracownikBiurowy(string imie, string nazwisko, string pesel, decimal pensja, decimal dodatekStazowy)
            : base(imie, nazwisko, pesel, pensja)
        {
            DodatekStazowy = dodatekStazowy;
        }

        public override decimal ObliczPremie()
        {
            return DodatekStazowy;
        }


    }
}
