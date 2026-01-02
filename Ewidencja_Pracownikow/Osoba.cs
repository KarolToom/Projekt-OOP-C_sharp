using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ewidencja_Pracownikow
{
    public abstract class Osoba // Klasa bazowa reprezentująca osobę
    {
        private string _imie;
        private string _nazwisko;
        private string _pesel;
        public int Id { get; set; }
        public string Imie { 
            get => _imie;
            set
            {
                if(value.Any(char.IsDigit))
                    throw new ArgumentException("Imię nie może zawierać cyfr.");
                _imie = value;
            }
        }
        public string Nazwisko 
        { 
            get => _nazwisko;
            set 
            {
                if(value.Any(char.IsDigit))
                    throw new ArgumentException("Nazwisko nie może zawierać cyfr.");
                _nazwisko = value;
            }
        }

        public string Pesel
        {
            get => _pesel;
            set
            {
                if(value.Length != 11 || !value.All(char.IsDigit))
                    throw new ArgumentException("PESEL musi składać się z 11 cyfr.");
                _pesel = value;
            }
        }

        protected Osoba(string imie, string nazwisko, string pesel) // Konstruktor klasy Osoba
        {
            Imie = imie;
            Nazwisko = nazwisko;
            Pesel = pesel;
        }

        public virtual string WyswietlDane() // Metoda do wyświetlania danych osoby
        {
            return $"Imię: {Imie}, Nazwisko: {Nazwisko}, PESEL: {Pesel}";
        }
    }
}
