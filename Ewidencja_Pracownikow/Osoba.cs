using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ewidencja_Pracownikow
{
    public abstract class Osoba
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Pesel { get; set; }

        protected Osoba(string imie, string nazwisko, string pesel)
        {
            Imie = imie;
            Nazwisko = nazwisko;
            Pesel = pesel;
        }

        public virtual string WyswietlDane()
        {
            return $"Imię: {Imie}, Nazwisko: {Nazwisko}, PESEL: {Pesel}";
        }
    }
}
