using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Ewidencja_Pracownikow
{
    public partial class Form1 : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EwidencjaPracownikow;Trusted_Connection=True;";
        public Form1()
        {
            InitializeComponent();

        }


        private void ZaladujDane(string sortowanie = "Nazwisko")
        {
            List<Pracownik> listaObiektowa = new List<Pracownik>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT P.Imie, P.Nazwisko, P.PESEL, S.Nazwa as Stanowisko, 
                 W.SumaCalkowita as Pensja, W.ParametrLiczbowy1, W.ParametrLiczbowy2 
                 FROM Pracownicy P 
                 JOIN Stanowiska S ON P.IdStanowiska = S.IdStanowiska
                 LEFT JOIN Wynagrodzenia W ON P.IdPracownika = W.IdPracownika";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string imie = reader["Imie"].ToString();
                    string nazwisko = reader["Nazwisko"].ToString();
                    string pesel = reader["PESEL"].ToString();
                    string stanowisko = reader["Stanowisko"].ToString();
                    decimal pensja = reader["Pensja"] != DBNull.Value ? Convert.ToDecimal(reader["Pensja"]) : 0;
                    decimal p1 = reader["ParametrLiczbowy1"] != DBNull.Value ? Convert.ToDecimal(reader["ParametrLiczbowy1"]) : 0;
                    decimal p2 = reader["ParametrLiczbowy2"] != DBNull.Value ? Convert.ToDecimal(reader["ParametrLiczbowy2"]) : 0;

                    if (stanowisko == "Kierowca")
                        listaObiektowa.Add(new Kierowca(imie, nazwisko, pesel, pensja, (int)p1, p2));
                    else if (stanowisko == "Handlowiec")
                        listaObiektowa.Add(new Handlowiec(imie, nazwisko, pesel, pensja, p1, p2));
                    else if (stanowisko == "Manager")
                        listaObiektowa.Add(new Manager(imie, nazwisko, pesel, pensja, p1, p2));
                    else
                        listaObiektowa.Add(new PracownikBiurowy(imie, nazwisko, pesel, pensja, p1));
                }
            }
            // Podpinamy listê obiektów do Grida
            var querySort = listaObiektowa.Select(p => new
            {
                // U¿ywamy Imiê, Nazwisko, PESEL z klasy Osoba
                Imiê = p.Imie,
                Nazwisko = p.Nazwisko,
                PESEL = p.Pesel, // Upewnij siê, ¿e w klasie Osoba masz 'Pesel'
                Stanowisko = p.GetType().Name,
                Pensja_Zasadnicza = p.PensjaZasadnicza,

                // Mapowanie parametrów dla SelectionChanged
                Parametr1 = p is Kierowca ? (p as Kierowca).PrzejechaneKilometry :
                    p is Handlowiec ? (p as Handlowiec).WartoscSprzedazy :
                    p is PracownikBiurowy ? (p as PracownikBiurowy).DodatekStazowy : 0,

                Parametr2 = p is Kierowca ? (p as Kierowca).StawkaZaKilometr :
                    p is Handlowiec ? (p as Handlowiec).ProcentProwizji :
                    p is Manager ? (p as Manager).PremiaMenadzerska : 0,

                // Wykorzystanie polimorfizmu do obliczeñ
                Premia = p.ObliczPremie(),
                Suma = p.ObliczWynagrodzenie()
            });

            // --- LOGIKA SORTOWANIA (LINQ) ---
            if (sortowanie == "Nazwisko")
                querySort = querySort.OrderBy(x => x.Nazwisko);
            else if (sortowanie == "Pensja")
                querySort = querySort.OrderByDescending(x => x.Pensja_Zasadnicza);
            else if (sortowanie == "Premia")
                querySort = querySort.OrderByDescending(x => x.Premia);
            else if (sortowanie == "Suma")
                querySort = querySort.OrderByDescending(x => x.Suma);

            // Na samym koñcu podpinamy gotow¹, posortowan¹ listê do Grida
            dgvPracownicy.DataSource = querySort.ToList();

            dgvPracownicy.Columns["Pensja_Zasadnicza"].HeaderText = "Pensja zasadnicza";

            dgvPracownicy.ReadOnly = true;
        }



        private void dgvPracownicy_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            FormDodaj oknoDodawania = new FormDodaj();


            oknoDodawania.ShowDialog();


            ZaladujDane();

        }

        private void btnUsun_Click(object sender, EventArgs e)
        {
            if (dgvPracownicy.CurrentRow != null)
            {
                // Pobieramy dane do potwierdzenia i ID do usuniêcia
                // Upewnij siê, ¿e kolumna w Gridzie nazywa siê "IdPracownika" lub pobierz j¹ po PESEL
                string imie = dgvPracownicy.CurrentRow.Cells["Imiê"].Value.ToString();
                string nazwisko = dgvPracownicy.CurrentRow.Cells["Nazwisko"].Value.ToString();
                string pesel = dgvPracownicy.CurrentRow.Cells["PESEL"].Value.ToString();

                var potwierdzenie = MessageBox.Show($"Czy na pewno chcesz usun¹æ pracownika {imie} {nazwisko} (PESEL: {pesel}) oraz wszystkie jego dane finansowe?",
                                                    "Potwierdzenie usuniêcia",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning);

                if (potwierdzenie == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            // U¿ywamy transakcji, bo usuwamy z dwóch tabel
                            SqlTransaction trans = conn.BeginTransaction();

                            try
                            {
                                // 1. Najpierw usuwamy z tabeli Wynagrodzenia (Klucz obcy!)
                                string sqlW = @"DELETE FROM Wynagrodzenia 
                                        WHERE IdPracownika = (SELECT IdPracownika FROM Pracownicy WHERE PESEL = @pesel)";
                                SqlCommand cmdW = new SqlCommand(sqlW, conn, trans);
                                cmdW.Parameters.AddWithValue("@pesel", pesel);
                                cmdW.ExecuteNonQuery();

                                // 2. Potem usuwamy z tabeli Pracownicy
                                string sqlP = "DELETE FROM Pracownicy WHERE PESEL = @pesel";
                                SqlCommand cmdP = new SqlCommand(sqlP, conn, trans);
                                cmdP.Parameters.AddWithValue("@pesel", pesel);
                                cmdP.ExecuteNonQuery();

                                trans.Commit();
                                MessageBox.Show("Pracownik zosta³ pomyœlnie usuniêty z systemu.");
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                throw new Exception("B³¹d podczas usuwania z bazy: " + ex.Message);
                            }
                        }
                        // Odœwie¿amy widok
                        ZaladujDane();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Proszê najpierw zaznaczyæ pracownika w tabeli.");
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Pliki CSV (*.csv)|*.csv";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                int dodano = 0;
                int pominieto = 0;
                int bledy = 0;

                try
                {
                    string[] linie = File.ReadAllLines(ofd.FileName);

                    for (int i = 1; i < linie.Length; i++)
                    {
                        try
                        {
                            string[] dane = linie[i].Split(';');

                            if (dane.Length < 5) { bledy++; continue; }

                            string imie = dane[0];
                            string nazwisko = dane[1];
                            string pesel = dane[2];
                            decimal pensja = decimal.Parse(dane[3]);
                            string stanowiskoZPliku = dane[4].Trim();

                            if (CzyPeselIstnieje(pesel))
                            {
                                pominieto++;
                                continue;
                            }

                            int idStanowiska = 1;
                            if (stanowiskoZPliku == "Handlowiec") idStanowiska = 2;
                            else if (stanowiskoZPliku == "Manager") idStanowiska = 3;
                            else if (stanowiskoZPliku == "Pracownik Biurowy") idStanowiska = 4;

                            int idDzialu = 1; // Domyœlnie Transport

                            if (stanowiskoZPliku == "Handlowiec")
                            {
                                idDzialu = 2; // Sprzeda¿
                            }
                            else if (stanowiskoZPliku == "Pracownik Biurowy" || stanowiskoZPliku == "Manager")
                            {
                                idDzialu = 3; // Administracja
                            }


                            ZapiszDoBazyZImportu(imie, nazwisko, pesel, pensja, idDzialu, idStanowiska);
                            dodano++;
                        }
                        catch (Exception)
                        {
                            bledy++;
                        }
                    }
                    ZaladujDane();
                    MessageBox.Show($"Import zakoñczony!\nDodano: {dodano}\nPominiêto (duplikaty): {pominieto}\nB³êdy danych: {bledy}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("B³¹d odczytu pliku: " + ex.Message);
                }
            }
        }

        private bool CzyPeselIstnieje(string pesel)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Pracownicy WHERE PESEL = @p", conn);
                cmd.Parameters.AddWithValue("@p", pesel);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private void ZapiszDoBazyZImportu(string imie, string nazwisko, string pesel, decimal pensja, int idDzialu, int idStanowiska)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlTransaction transakcja = conn.BeginTransaction();

                try
                {

                    string sqlPracownik = @"
                INSERT INTO Pracownicy (Imie, Nazwisko, PESEL, IdDzialu, IdStanowiska) 
                VALUES (@imie, @nazwisko, @pesel, @idDzialu, @idStanowiska);
                SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdP = new SqlCommand(sqlPracownik, conn, transakcja);
                    cmdP.Parameters.AddWithValue("@imie", imie);
                    cmdP.Parameters.AddWithValue("@nazwisko", nazwisko);
                    cmdP.Parameters.AddWithValue("@pesel", pesel);
                    cmdP.Parameters.AddWithValue("@idDzialu", idDzialu);
                    cmdP.Parameters.AddWithValue("@idStanowiska", idStanowiska);


                    int noweIdPracownika = Convert.ToInt32(cmdP.ExecuteScalar());


                    string sqlWynagrodzenie = @"
                INSERT INTO Wynagrodzenia (IdPracownika, DataObliczenia, SumaCalkowita) 
                VALUES (@idPracownik, @data, @suma)";

                    SqlCommand cmdW = new SqlCommand(sqlWynagrodzenie, conn, transakcja);
                    cmdW.Parameters.AddWithValue("@idPracownik", noweIdPracownika);
                    cmdW.Parameters.AddWithValue("@data", DateTime.Now);
                    cmdW.Parameters.AddWithValue("@suma", pensja);

                    cmdW.ExecuteNonQuery();


                    transakcja.Commit();
                }
                catch (Exception ex)
                {

                    transakcja.Rollback();
                    throw ex;
                }
            }
        }

        private void btnEksport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Pliki CSV (*.csv)|*.csv";
            sfd.FileName = "Eksport_Pracownikow_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        string naglowki = "";
                        for (int i = 0; i < dgvPracownicy.Columns.Count; i++)
                        {
                            naglowki += dgvPracownicy.Columns[i].HeaderText + (i < dgvPracownicy.Columns.Count - 1 ? ";" : "");
                        }
                        sw.WriteLine(naglowki);


                        foreach (DataGridViewRow row in dgvPracownicy.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                string linia = "";
                                for (int i = 0; i < dgvPracownicy.Columns.Count; i++)
                                {

                                    string wartosc = row.Cells[i].Value?.ToString() ?? "";
                                    linia += wartosc + (i < dgvPracownicy.Columns.Count - 1 ? ";" : "");
                                }
                                sw.WriteLine(linia);
                            }
                        }
                    }
                    MessageBox.Show("Dane zosta³y pomyœlnie wyeksportowane do pliku!", "Sukces");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("B³¹d podczas eksportu: " + ex.Message);
                }
            }
        }

        private void btnOdswiez_Click(object sender, EventArgs e)
        {
            ZaladujDane();
        }

        private void dgvPracownicy_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPracownicy.CurrentRow != null)
            {
                // Pobieramy nazwê stanowiska z kolumny "Stanowisko" (upewnij siê, ¿e taka nazwa jest w Gridzie)
                string stanowisko = dgvPracownicy.CurrentRow.Cells["Stanowisko"].Value.ToString();

                // Odwo³ujemy siê do kolumn, które przechowuj¹ dane do premii
                // Zak³adamy, ¿e nazywaj¹ siê one w Gridzie "Parametr1" i "Parametr2"
                var col1 = dgvPracownicy.Columns["Parametr1"];
                var col2 = dgvPracownicy.Columns["Parametr2"];

                if (col1 == null || col2 == null) return;

                switch (stanowisko)
                {
                    case "Kierowca":
                        col1.HeaderText = "Kilometry";
                        col2.HeaderText = "Stawka za km";
                        col2.Visible = true;
                        break;
                    case "Handlowiec":
                        col1.HeaderText = "Wartoœæ sprzeda¿y";
                        col2.HeaderText = "Prowizja (%)";
                        col2.Visible = true;
                        break;
                    case "Manager":
                        col1.HeaderText = "Dodatek sta¿owy";
                        col2.HeaderText = "Premia Menad¿era";
                        col2.Visible = true;
                        break;
                    case "Pracownik Biurowy":
                        col1.HeaderText = "Dodatek sta¿owy";
                        col2.HeaderText = "-";
                        col2.Visible = false; // Pracownik biurowy ma tylko jeden parametr
                        break;
                }
            }
        }

        private void cmbSortowanie_SelectedIndexChanged(object sender, EventArgs e)
        {
            ZaladujDane(cmbSortowanie.SelectedItem.ToString());
        }

        private void btnEdytuj_Click(object sender, EventArgs e)
        {
            if (dgvPracownicy.CurrentRow != null)
            {
                // 1. Pobieramy ID zaznaczonego pracownika
                // Upewnij siê, ¿e kolumna z ID nie jest ukryta lub jest dostêpna w obiekcie
                // Najbezpieczniej pobraæ PESEL i znaleŸæ ID, albo dodaæ IdPracownika do obiektu anonimowego w ZaladujDane
                string pesel = dgvPracownicy.CurrentRow.Cells["PESEL"].Value.ToString();
                int idDoEdycji = PobierzIdPoPeselu(pesel); // Trzeba dopisaæ ma³¹ metodê pomocnicz¹

                // 2. Otwieramy FormDodaj w trybie EDYCJI (przekazujemy ID)
                FormDodaj oknoEdycji = new FormDodaj(idDoEdycji);
                if (oknoEdycji.ShowDialog() == DialogResult.OK)
                {
                    ZaladujDane(); // Odœwie¿amy listê po zamkniêciu okna
                }
            }
        }
        private int PobierzIdPoPeselu(string pesel)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT IdPracownika FROM Pracownicy WHERE PESEL = @p", conn);
                cmd.Parameters.AddWithValue("@p", pesel);
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
