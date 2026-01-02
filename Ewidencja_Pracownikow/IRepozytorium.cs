using System.Collections.Generic;

namespace Ewidencja_Pracownikow
{
    public interface IRepozytorium
    {
        
        List<Pracownik> PobierzWszystkich(); // Pobiera listę obiektów (Kierowca, Manager itd.)

        DanePracownikaDoEdycji PobierzDoEdycji(int id); // Pobiera dane pracownika do edycji na podstawie jego ID

        void DodajPracownika(Pracownik pracownik, int idDzialu, int idStanowiska);
        void EdytujPracownika(int idPracownika, Pracownik pracownik, int idDzialu, int idStanowiska);
        void UsunPracownika(string pesel);
        bool CzyPeselIstnieje(string pesel);
        int PobierzIdPoPeselu(string pesel);
    }

    public class DanePracownikaDoEdycji // Klasa pomocnicza do przechowywania danych pracownika do edycji
    {
        public int IdPracownika { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Pesel { get; set; }
        public decimal Pensja { get; set; }
        public decimal P1 { get; set; }
        public decimal P2 { get; set; }
        public string NazwaStanowiska { get; set; }
    }
}