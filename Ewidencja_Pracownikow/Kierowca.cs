using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ewidencja_Pracownikow
{
    public class Kierowca : Pracownik
    {
        public int PrzejechaneKilometry { get; set; }
        public decimal StawkaZaKilometr { get; set; }

        public Kierowca(string imie, string nazwisko, string pesel, decimal pensja, int km, decimal stawkakm)
            : base(imie, nazwisko, pesel, pensja)
        {
            PrzejechaneKilometry = km;
            StawkaZaKilometr = stawkakm;
        }
        public override decimal ObliczPremie()
        {
            return PrzejechaneKilometry * StawkaZaKilometr;
        }
    }
}
