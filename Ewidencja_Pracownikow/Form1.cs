using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Ewidencja_Pracownikow
{
    public partial class Form1 : Form // G³ówne okno aplikacji
    {
        private IRepozytorium _baza; 

        public Form1()
        {
            InitializeComponent();
            _baza = new RepozytoriumSQL(); // Inicjalizacja repozytorium do obs³ugi bazy danych 
            ZaladujDane();
        }
        private void ZaladujDane(string sortowanie = "Nazwisko")
        {
            var listaObiektowa = _baza.PobierzWszystkich(); //Pobieramy listê obiektów Pracownik


            var querySort = listaObiektowa.Select(p => new
            {
                Imiê = p.Imie,
                Nazwisko = p.Nazwisko,
                PESEL = p.Pesel,
                Stanowisko = p.GetType().Name,
                Pensja_Zasadnicza = p.PensjaZasadnicza,

                Parametr1 = p is Kierowca ? (p as Kierowca).PrzejechaneKilometry :
                            p is Handlowiec ? (p as Handlowiec).WartoscSprzedazy :
                            p is PracownikBiurowy ? (p as PracownikBiurowy).DodatekStazowy : 0,

                Parametr2 = p is Kierowca ? (p as Kierowca).StawkaZaKilometr :
                            p is Handlowiec ? (p as Handlowiec).ProcentProwizji :
                            p is Manager ? (p as Manager).PremiaMenadzerska : 0,

                Premia = p.ObliczPremie(),
                Suma = p.ObliczWynagrodzenie()
            }); // Dostosowujemy dane do wyœwietlenia w gridzie

            // Sortowanie, jedno z za³o¿eñ: domyœlnie po nazwisku rosn¹co   
            if (sortowanie == "Nazwisko") querySort = querySort.OrderBy(x => x.Nazwisko);
            else if (sortowanie == "Pensja") querySort = querySort.OrderByDescending(x => x.Pensja_Zasadnicza);
            else if (sortowanie == "Premia") querySort = querySort.OrderByDescending(x => x.Premia);
            else if (sortowanie == "Suma") querySort = querySort.OrderByDescending(x => x.Suma);

            dgvPracownicy.DataSource = querySort.ToList(); // Przypisujemy do DataGridView
            FormatujGrid();
        }

        private void FormatujGrid()
        {
          
            if (dgvPracownicy.Columns["Premia"] != null) dgvPracownicy.Columns["Premia"].DefaultCellStyle.Format = "N2";
            if (dgvPracownicy.Columns["Suma"] != null) dgvPracownicy.Columns["Suma"].DefaultCellStyle.Format = "N2";
            if (dgvPracownicy.Columns["Parametr1"] != null) dgvPracownicy.Columns["Parametr1"].DefaultCellStyle.Format = "N2";
            if (dgvPracownicy.Columns["Pensja_Zasadnicza"] != null)
            {
                dgvPracownicy.Columns["Pensja_Zasadnicza"].HeaderText = "Pensja zasadnicza";
                dgvPracownicy.Columns["Pensja_Zasadnicza"].DefaultCellStyle.Format = "N2";
       
            }
            dgvPracownicy.ReadOnly = true;
        }

        private void btnDodaj_Click(object sender, EventArgs e) // Dodawanie nowego pracownika
        {
            FormDodaj oknoDodawania = new FormDodaj();
            if (oknoDodawania.ShowDialog() == DialogResult.OK)
            {
                ZaladujDane();
            }
        }

        private void btnEdytuj_Click(object sender, EventArgs e) // Edycja zaznaczonego pracownika
        {
            if (dgvPracownicy.CurrentRow != null)
            {
                string pesel = dgvPracownicy.CurrentRow.Cells["PESEL"].Value.ToString();
                int idDoEdycji = _baza.PobierzIdPoPeselu(pesel);

                if (idDoEdycji > 0)
                {
                    FormDodaj oknoEdycji = new FormDodaj(idDoEdycji);
                    if (oknoEdycji.ShowDialog() == DialogResult.OK)
                    {
                        ZaladujDane();
                    }
                }
            }
        }

        private void btnUsun_Click(object sender, EventArgs e) // Usuwanie zaznaczonego pracownika
        {
            if (dgvPracownicy.CurrentRow != null)
            {
                string imie = dgvPracownicy.CurrentRow.Cells["Imiê"].Value.ToString();
                string nazwisko = dgvPracownicy.CurrentRow.Cells["Nazwisko"].Value.ToString();
                string pesel = dgvPracownicy.CurrentRow.Cells["PESEL"].Value.ToString();

                var odp = MessageBox.Show($"Usun¹æ pracownika {imie} {nazwisko}?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (odp == DialogResult.Yes)
                {
                    try
                    {
                        _baza.UsunPracownika(pesel);
                        ZaladujDane();
                        MessageBox.Show("Usuniêto pomyœlnie.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("B³¹d usuwania: " + ex.Message);
                    }
                }
            }
        }

        private void cmbSortowanie_SelectedIndexChanged(object sender, EventArgs e) //Zmiana sortowania: po sumie, premii, pensji, nazwisku
        {
            if (cmbSortowanie.SelectedItem != null)
                ZaladujDane(cmbSortowanie.SelectedItem.ToString());
        }
        private void btnOdswiez_Click(object sender, EventArgs e) // Za³adowywanie danych z bazy "Pobierz dane z bazy"

        {
            ZaladujDane();
        }

        private void dgvPracownicy_SelectionChanged(object sender, EventArgs e) // Zmiana zaznaczenia w gridzie - dostosowanie nag³ówków kolumn parametrów
        {
            if (dgvPracownicy.CurrentRow != null)
            {
                string stanowisko = dgvPracownicy.CurrentRow.Cells["Stanowisko"].Value.ToString();
                var col1 = dgvPracownicy.Columns["Parametr1"];
                var col2 = dgvPracownicy.Columns["Parametr2"];

                if (col1 == null || col2 == null) return;

                switch (stanowisko) // Dostosowanie nag³ówków kolumn w zale¿noœci od typu pracownika
                {
                    case "Kierowca": col1.HeaderText = "Kilometry"; col2.HeaderText = "Stawka/km"; col2.Visible = true; break;
                    case "Handlowiec": col1.HeaderText = "Sprzeda¿"; col2.HeaderText = "Prowizja %"; col2.Visible = true; break;
                    case "Manager": col1.HeaderText = "Dodatek"; col2.HeaderText = "Premia Mgr"; col2.Visible = true; break;
                    case "PracownikBiurowy": col1.HeaderText = "Dodatek"; col2.Visible = false; break;
                }
            }
        }

        private void btnEksport_Click(object sender, EventArgs e) // Eksport danych do pliku CSV
        {    
            SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV|*.csv", FileName = $"Export_{DateTime.Now:yyyyMMdd}.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        string naglowki = string.Join(";", dgvPracownicy.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText));
                        sw.WriteLine(naglowki);

                        foreach (DataGridViewRow row in dgvPracownicy.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                string linia = string.Join(";", row.Cells.Cast<DataGridViewCell>().Select(c => c.Value?.ToString() ?? ""));
                                sw.WriteLine(linia);
                            }
                        }
                    }
                    MessageBox.Show("Eksport zakoñczony.");
                }
                catch (Exception ex) { MessageBox.Show("B³¹d eksportu: " + ex.Message); }
            }
        }

        private void btnImport_Click(object sender, EventArgs e) // Import danych z pliku CSV
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "CSV|*.csv" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                int dodano = 0, bledy = 0, pominieto = 0;
                var linie = File.ReadAllLines(ofd.FileName);

                for (int i = 1; i < linie.Length; i++)
                {
                    try
                    {
                        var d = linie[i].Split(';');
                        if (d.Length < 5) { bledy++; continue; }

                        string imie = d[0], nazwisko = d[1], pesel = d[2];
                        decimal pensja = decimal.Parse(d[3]);
                        string stanowisko = d[4].Trim();

                        if (_baza.CzyPeselIstnieje(pesel)) { pominieto++; continue; }
              
                        Pracownik nowyPracownik = null;
                        int idS = 1, idD = 1; // domyœlne stanowisko i dzia³ dla nowego pracownika (Kierowca)

                        if (stanowisko == "Kierowca") { nowyPracownik = new Kierowca(imie, nazwisko, pesel, pensja, 500, 0.5m); idS = 1; idD = 1; }
                        else if (stanowisko == "Handlowiec") { nowyPracownik = new Handlowiec(imie, nazwisko, pesel, pensja, 10000, 5); idS = 2; idD = 2; }
                        else if (stanowisko == "Manager") { nowyPracownik = new Manager(imie, nazwisko, pesel, pensja, 2000, 3000); idS = 3; idD = 3; }
                        else { nowyPracownik = new PracownikBiurowy(imie, nazwisko, pesel, pensja, 1500); idS = 4; idD = 3; }

                        _baza.DodajPracownika(nowyPracownik, idD, idS);
                        dodano++;
                    }
                    catch { bledy++; }
                }
                ZaladujDane();
                MessageBox.Show($"Import:\nDodano: {dodano}\nPominiêto: {pominieto}\nB³êdy: {bledy}");
            }
        }

        private void dgvPracownicy_CellContentClick(object sender, DataGridViewCellEventArgs e) // Nieu¿ywane zdarzenie
        {
        }
    }
}