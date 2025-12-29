using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Ewidencja_Pracownikow
{
    public partial class Form1 : Form
    {
        string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EwidencjaPracownikow;Trusted_Connection=True;";
        public Form1()
        {
            InitializeComponent();

        }


        private void ZaladujDane()
        {
            string zapytanie = @"
        SELECT 
            P.IdPracownika, 
            P.Imie, 
            P.Nazwisko, 
            P.PESEL, 
            S.NazwaStanowiska AS Stanowisko,
            D.NazwaDzialu AS Dzial
        FROM Pracownicy P
        INNER JOIN Stanowiska S ON P.IdStanowiska = S.IdStanowiska
        INNER JOIN Dzialy D ON P.IdDzialu = D.IdDzialu";

            using (SqlConnection polaczenie = new SqlConnection(connectionString))
            {
                try
                {
                    polaczenie.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(zapytanie, polaczenie);
                    DataTable tabela = new DataTable();
                    adapter.Fill(tabela);

                    dgvPracownicy.DataSource = tabela;
                    dgvPracownicy.Columns["IdPracownika"].HeaderText = "ID";
                    dgvPracownicy.Columns["IdPracownika"].Width = 40;
                    dgvPracownicy.Columns["Imie"].HeaderText = "Imiê";
                    dgvPracownicy.Columns["Nazwisko"].HeaderText = "Nazwisko";
                    dgvPracownicy.Columns["PESEL"].HeaderText = "PESEL";
                    dgvPracownicy.Columns["Stanowisko"].Width = 120;

                    dgvPracownicy.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("B³¹d po³¹czenia: " + ex.Message);
                }
            }
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

                int id = (int)dgvPracownicy.CurrentRow.Cells["IdPracownika"].Value;
                string nazwisko = dgvPracownicy.CurrentRow.Cells["Nazwisko"].Value.ToString();

                var dialog = MessageBox.Show($"Czy na pewno usun¹æ pracownika {nazwisko}?", "PotwierdŸ", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            string sql = "DELETE FROM Pracownicy WHERE IdPracownika = @id";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                        ZaladujDane();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void btnZapiszZmiany_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPracownicy.CurrentRow != null)
                {
                    int id = (int)dgvPracownicy.CurrentRow.Cells["IdPracownika"].Value;
                    string imie = dgvPracownicy.CurrentRow.Cells["Imie"].Value.ToString();
                    string nazwisko = dgvPracownicy.CurrentRow.Cells["Nazwisko"].Value.ToString();

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string sql = "UPDATE Pracownicy SET Imie=@imie, Nazwisko=@nazwisko WHERE IdPracownika=@id";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@imie", imie);
                        cmd.Parameters.AddWithValue("@nazwisko", nazwisko);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Dane zaktualizowane!");
                    ZaladujDane();
                }
            }
            catch (Exception ex) { MessageBox.Show("B³¹d edycji: " + ex.Message); }
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
                    string[] linie = System.IO.File.ReadAllLines(ofd.FileName);

                    for (int i = 1; i < linie.Length; i++)
                    {
                        try
                        {
                            string[] dane = linie[i].Split(';'); // Rozdzielamy œrednikiem

                            if (dane.Length < 5) { bledy++; continue; }

                            string imie = dane[0];
                            string nazwisko = dane[1];
                            string pesel = dane[2];
                            decimal pensja = decimal.Parse(dane[3]);
                            string stanowisko = dane[4];

                            if (CzyPeselIstnieje(pesel))
                            {
                                pominieto++;
                                continue;
                            }

                            int idStanowiska = 1; // domyœlnie Kierowca
                            if (stanowisko == "Handlowiec") idStanowiska = 2;
                            else if (stanowisko == "Manager") idStanowiska = 3;

                        
                            ZapiszDoBazyZImportu(imie, nazwisko, pesel, pensja, idStanowiska);
                            dodano++;
                        }
                        catch (Exception)
                        {
                            bledy++; // Np. b³¹d parsowania liczby lub walidacja klasy Osoba
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

        private void ZapiszDoBazyZImportu(string i, string n, string p, decimal pensja, int idS)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Pracownicy (Imie, Nazwisko, PESEL, PensjaPodstawowa, IdDzialu, IdStanowiska) " +
                             "VALUES (@i, @n, @p, @pensja, 1, @idS)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@i", i);
                cmd.Parameters.AddWithValue("@n", n);
                cmd.Parameters.AddWithValue("@p", p);
                cmd.Parameters.AddWithValue("@pensja", pensja);
                cmd.Parameters.AddWithValue("@idS", idS);
                cmd.ExecuteNonQuery();
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
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
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

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
