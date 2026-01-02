using System;
using System.Windows.Forms;

namespace Ewidencja_Pracownikow
{
    public partial class FormDodaj : Form // Okno dodawania/edycji pracownika
    {
        private IRepozytorium _baza;
        private int? _idEdytowanego = null;
        private bool _ladowanieDanych = false;

        public FormDodaj() // Konstruktor Dodawania
        {
            InitializeComponent();
            _baza = new RepozytoriumSQL();
            KonfigurujWidok();
        }

        public FormDodaj(int id) // Konstruktor Edycji
        {
            InitializeComponent();
            _baza = new RepozytoriumSQL();
            _idEdytowanego = id;
            KonfigurujWidok();
            WypelnijDaneDoEdycji();
        }

        private void KonfigurujWidok() // Konfiguracja widoku w zależności od trybu (dodawanie/edycja). Dwie funkcje w jednym oknie
        {
            if (_idEdytowanego.HasValue)
            {
                Text = "Edycja pracownika";
                btnZapisz.Text = "Zaktualizuj";
            }
            else
            {
                Text = "Dodawanie pracownika";
                btnZapisz.Text = "Dodaj";
                UkryjParametry();
            }
        }

        private void WypelnijDaneDoEdycji() // Wypełnianie danych do edycji
        {
            _ladowanieDanych = true; // Zablokowanie zdarzenia zmiany typu pracownika podczas wczytywania danych
            try
            {
                var dane = _baza.PobierzDoEdycji(_idEdytowanego.Value);
                if (dane != null)
                {
                    txtImie.Text = dane.Imie;
                    txtNazwisko.Text = dane.Nazwisko;
                    txtPESEL.Text = dane.Pesel;
                    txtPensja.Text = dane.Pensja.ToString();

                    cbTypPracownika.SelectedItem = dane.NazwaStanowiska;

                    txtParametr1.Text = dane.P1.ToString();
                    txtParametr2.Text = dane.P2.ToString();
                }
            }
            finally
            {
                _ladowanieDanych = false;
            }
        }

        private void UkryjParametry() // Ukrywanie parametrów specyficznych dla stanowiska. Dla okna dodawania
        {
            lblParametr1.Visible = false; txtParametr1.Visible = false;
            lblParametr2.Visible = false; txtParametr2.Visible = false;
        }

        private void cbTypPracownika_SelectedIndexChanged(object sender, EventArgs e) // Zmiana typu pracownika
        {
            if (cbTypPracownika.SelectedItem == null) return;
            string rola = cbTypPracownika.SelectedItem.ToString();

            lblParametr1.Visible = true;  
            txtParametr1.Visible = true; 

            switch (rola)
            {
                case "Kierowca":
                    lblParametr1.Text = "Km:"; lblParametr2.Text = "Stawka:";
                    lblParametr2.Visible = true; txtParametr2.Visible = true;
                    if (!_ladowanieDanych) { txtParametr1.Text = "500"; txtParametr2.Text = "0,5"; }
                    break;
                case "Handlowiec":
                    lblParametr1.Text = "Sprzedaż:"; lblParametr2.Text = "Prowizja %:";
                    lblParametr2.Visible = true; txtParametr2.Visible = true;
                    if (!_ladowanieDanych) { txtParametr1.Text = "10000"; txtParametr2.Text = "5"; }
                    break;
                case "Manager":
                    lblParametr1.Text = "Dodatek:"; lblParametr2.Text = "Premia Mgr:";
                    lblParametr2.Visible = true; txtParametr2.Visible = true;
                    if (!_ladowanieDanych) { txtParametr1.Text = "2000"; txtParametr2.Text = "3000"; }
                    break;
                default: // Biurowy
                    lblParametr1.Text = "Dodatek:";
                    lblParametr2.Visible = false; txtParametr2.Visible = false;
                    if (!_ladowanieDanych) { txtParametr1.Text = "1500"; txtParametr2.Text = "0"; }
                    break;
            }
        }

        private void btnZapisz_Click(object sender, EventArgs e) // Zapis danych (dodawanie/edycja)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(txtImie.Text)) throw new Exception("Podaj imię"); // Walidacja danych
                if (cbTypPracownika.SelectedItem == null) throw new Exception("Wybierz stanowisko"); // Walidacja danych

                string imie = txtImie.Text;
                string nazwisko = txtNazwisko.Text;
                string pesel = txtPESEL.Text;
                decimal pensja = decimal.Parse(txtPensja.Text);
                decimal p1 = decimal.Parse(txtParametr1.Text);
                decimal p2 = txtParametr2.Visible ? decimal.Parse(txtParametr2.Text) : 0;
                string stanowisko = cbTypPracownika.SelectedItem.ToString();

                Pracownik p = null;
                int idS = 1, idD = 1; // Identyfikatory działu i stanowiska

                if (stanowisko == "Kierowca") { p = new Kierowca(imie, nazwisko, pesel, pensja, (int)p1, p2); idS = 1; idD = 1; }
                else if (stanowisko == "Handlowiec") { p = new Handlowiec(imie, nazwisko, pesel, pensja, p1, p2); idS = 2; idD = 2; }
                else if (stanowisko == "Manager") { p = new Manager(imie, nazwisko, pesel, pensja, p1, p2); idS = 3; idD = 3; }
                else { p = new PracownikBiurowy(imie, nazwisko, pesel, pensja, p1); idS = 4; idD = 3; }

                // Zapis do bazy
                if (_idEdytowanego.HasValue)
                {
                    _baza.EdytujPracownika(_idEdytowanego.Value, p, idD, idS);
                }
                else
                {
                    _baza.DodajPracownika(p, idD, idS);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) // Obsługa błędów
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message);
            }
        }
        private void txtPESEL_TextChanged(object sender, EventArgs e) { } 
        private void label1_Click(object sender, EventArgs e) { }
        private void txtParametr2_TextChanged(object sender, EventArgs e) { }
    }
}