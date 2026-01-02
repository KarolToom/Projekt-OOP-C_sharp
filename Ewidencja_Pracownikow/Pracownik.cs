using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ewidencja_Pracownikow
{
    public abstract class Pracownik: Osoba // Klasa bazowa dla różnych typów pracowników
    {
        public decimal PensjaZasadnicza { get; set; }

        protected Pracownik(string imie, string nazwisko, string pesel, decimal pensja)
            : base(imie, nazwisko, pesel)
        {
            PensjaZasadnicza = pensja;
        }

        public abstract decimal ObliczPremie();

        public virtual decimal ObliczWynagrodzenie() 
        {
            return PensjaZasadnicza + ObliczPremie();
        }
    }
}
