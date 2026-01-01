using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Ewidencja_Pracownikow
{
    public partial class FormDodaj : Form
    {
        // Connection string
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EwidencjaPracownikow;Integrated Security=True";

        // Zmienna przechowująca ID (jeśli null = tryb dodawania, jeśli liczba = edycja)
        private int? idEdytowanegoPracownika = null;

        // Flaga, która blokuje nadpisywanie danych domyślnymi wartościami podczas ładowania z bazy
        private bool wTrakcieLadowania = false;

        // Konstruktor domyślny (dla dodawania)
        public FormDodaj()
        {
            InitializeComponent();
            ObslugaInterfejsu();
        }

        // Konstruktor dla edycji (przyjmuje ID)
        public FormDodaj(int idPracownika)
        {
            InitializeComponent();
            idEdytowanegoPracownika = idPracownika;
            ObslugaInterfejsu();
        }

        private void ObslugaInterfejsu()
        {
            if (idEdytowanegoPracownika != null)
            {
                // TRYB EDYCJI
                this.Text = "Edycja pracownika";
                btnZapisz.Text = "Zaktualizuj";

                // Najpierw ładujemy dane, co ustawi też widoczność pól
                ZaladujDaneDoEdycji(idEdytowanegoPracownika.Value);
            }
            else
            {
                // TRYB DODAWANIA
                this.Text = "Dodawanie pracownika";
                btnZapisz.Text = "Dodaj";

                // Ukrywamy parametry na starcie, bo jeszcze nie wybrano stanowiska
                UkryjParametry();
            }
        }

        private void UkryjParametry()
        {
            lblParametr1.Visible = false; txtParametr1.Visible = false;
            lblParametr2.Visible = false; txtParametr2.Visible = false;
            txtParametr1.Text = "0"; txtParametr2.Text = "0";
        }

        // Ta metoda odpowiada za pokazywanie pól i zmianę nazw etykiet
        private void cbTypPracownika_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTypPracownika.SelectedItem == null) return;

            string stanowisko = cbTypPracownika.SelectedItem.ToString();

            // Zawsze pokazujemy pierwszy parametr, drugi zależy od stanowiska
            lblParametr1.Visible = true;
            txtParametr1.Visible = true;

            // Konfiguracja nazw i widoczności drugiego parametru
            if (stanowisko == "Kierowca")
            {
                lblParametr1.Text = "Przejechane km:";
                lblParametr2.Text = "Stawka za km:";
                lblParametr2.Visible = true;
                txtParametr2.Visible = true;

                // Ustawiamy domyślne wartości TYLKO jeśli NIE ładujemy danych z bazy
                if (!wTrakcieLadowania)
                {
                    txtParametr1.Text = "500";
                    txtParametr2.Text = "0,5";
                }
            }
            else if (stanowisko == "Handlowiec")
            {
                lblParametr1.Text = "Wartość sprzedaży:";
                lblParametr2.Text = "Prowizja (%):";
                lblParametr2.Visible = true;
                txtParametr2.Visible = true;

                if (!wTrakcieLadowania)
                {
                    txtParametr1.Text = "10000";
                    txtParametr2.Text = "5";
                }
            }
            else if (stanowisko == "Manager")
            {
                lblParametr1.Text = "Dodatek stażowy:";
                lblParametr2.Text = "Premia menadżerska:";
                lblParametr2.Visible = true;
                txtParametr2.Visible = true;

                if (!wTrakcieLadowania)
                {
                    txtParametr1.Text = "2000";
                    txtParametr2.Text = "3000";
                }
            }
            else // Pracownik Biurowy
            {
                lblParametr1.Text = "Dodatek stażowy:";

                // Biurowy nie ma drugiego parametru
                lblParametr2.Visible = false;
                txtParametr2.Visible = false;
                txtParametr2.Text = "0"; // Zerujemy ukryte pole

                if (!wTrakcieLadowania)
                {
                    txtParametr1.Text = "1500";
                }
            }
        }

        private void ZaladujDaneDoEdycji(int id)
        {
            // Włączamy flagę - "TERAZ ŁADUJEMY Z SQL, PROSZĘ NIE USTAWIAĆ WARTOŚCI DOMYŚLNYCH"
            wTrakcieLadowania = true;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT P.Imie, P.Nazwisko, P.PESEL, S.Nazwa, W.SumaCalkowita, W.ParametrLiczbowy1, W.ParametrLiczbowy2 
                               FROM Pracownicy P
                               JOIN Stanowiska S ON P.IdStanowiska = S.IdStanowiska
                               LEFT JOIN Wynagrodzenia W ON P.IdPracownika = W.IdPracownika
                               WHERE P.IdPracownika = @id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtImie.Text = reader["Imie"].ToString();
                    txtNazwisko.Text = reader["Nazwisko"].ToString();
                    txtPESEL.Text = reader["PESEL"].ToString();
                    txtPensja.Text = reader["SumaCalkowita"].ToString();

                    // To wywoła zdarzenie SelectedIndexChanged, ale flaga 'wTrakcieLadowania' zablokuje nadpisanie liczb
                    cbTypPracownika.SelectedItem = reader["Nazwa"].ToString();

                    // Teraz ręcznie wpisujemy wartości z bazy danych do textboxów
                    txtParametr1.Text = reader["ParametrLiczbowy1"] != DBNull.Value ? reader["ParametrLiczbowy1"].ToString() : "0";
                    txtParametr2.Text = reader["ParametrLiczbowy2"] != DBNull.Value ? reader["ParametrLiczbowy2"].ToString() : "0";
                }
            }

            // Wyłączamy flagę - koniec ładowania
            wTrakcieLadowania = false;
        }

        private void btnZapisz_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtImie.Text)) throw new Exception("Uzupełnij imię!");
                if (cbTypPracownika.SelectedItem == null) throw new Exception("Wybierz stanowisko!");

                string imie = txtImie.Text;
                string nazwisko = txtNazwisko.Text;
                string pesel = txtPESEL.Text;
                decimal pensja = decimal.Parse(txtPensja.Text);
                string stanowisko = cbTypPracownika.SelectedItem.ToString();

                decimal p1 = string.IsNullOrEmpty(txtParametr1.Text) ? 0 : decimal.Parse(txtParametr1.Text);
                decimal p2 = string.IsNullOrEmpty(txtParametr2.Text) ? 0 : decimal.Parse(txtParametr2.Text);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    try
                    {
                        int idS = (stanowisko == "Kierowca") ? 1 : (stanowisko == "Handlowiec") ? 2 : (stanowisko == "Manager") ? 3 : 4;
                        int idD = (stanowisko == "Kierowca" || stanowisko == "Handlowiec") ? 1 : 2;

                        if (idEdytowanegoPracownika == null)
                        {
                            // INSERT
                            string sqlP = @"INSERT INTO Pracownicy (Imie, Nazwisko, PESEL, IdDzialu, IdStanowiska) 
                                            VALUES (@i, @n, @p, @idD, @idS); SELECT SCOPE_IDENTITY();";
                            SqlCommand cmdP = new SqlCommand(sqlP, conn, trans);
                            cmdP.Parameters.AddWithValue("@i", imie);
                            cmdP.Parameters.AddWithValue("@n", nazwisko);
                            cmdP.Parameters.AddWithValue("@p", pesel);
                            cmdP.Parameters.AddWithValue("@idD", idD);
                            cmdP.Parameters.AddWithValue("@idS", idS);

                            int noweId = Convert.ToInt32(cmdP.ExecuteScalar());

                            string sqlW = @"INSERT INTO Wynagrodzenia (IdPracownika, DataObliczenia, SumaCalkowita, ParametrLiczbowy1, ParametrLiczbowy2) 
                                            VALUES (@id, @data, @suma, @p1, @p2)";
                            SqlCommand cmdW = new SqlCommand(sqlW, conn, trans);
                            cmdW.Parameters.AddWithValue("@id", noweId);
                            cmdW.Parameters.AddWithValue("@data", DateTime.Now);
                            cmdW.Parameters.AddWithValue("@suma", pensja);
                            cmdW.Parameters.AddWithValue("@p1", p1);
                            cmdW.Parameters.AddWithValue("@p2", p2);
                            cmdW.ExecuteNonQuery();
                        }
                        else
                        {
                            // UPDATE
                            string sqlP = @"UPDATE Pracownicy SET Imie=@i, Nazwisko=@n, PESEL=@p, IdDzialu=@idD, IdStanowiska=@idS 
                                            WHERE IdPracownika=@id";
                            SqlCommand cmdP = new SqlCommand(sqlP, conn, trans);
                            cmdP.Parameters.AddWithValue("@i", imie);
                            cmdP.Parameters.AddWithValue("@n", nazwisko);
                            cmdP.Parameters.AddWithValue("@p", pesel);
                            cmdP.Parameters.AddWithValue("@idD", idD);
                            cmdP.Parameters.AddWithValue("@idS", idS);
                            cmdP.Parameters.AddWithValue("@id", idEdytowanegoPracownika);
                            cmdP.ExecuteNonQuery();

                            string sqlW = @"UPDATE Wynagrodzenia SET SumaCalkowita=@suma, ParametrLiczbowy1=@p1, ParametrLiczbowy2=@p2 
                                            WHERE IdPracownika=@id";
                            SqlCommand cmdW = new SqlCommand(sqlW, conn, trans);
                            cmdW.Parameters.AddWithValue("@suma", pensja);
                            cmdW.Parameters.AddWithValue("@p1", p1);
                            cmdW.Parameters.AddWithValue("@p2", p2);
                            cmdW.Parameters.AddWithValue("@id", idEdytowanegoPracownika);
                            cmdW.ExecuteNonQuery();
                        }

                        trans.Commit();
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch { trans.Rollback(); throw; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
            }
        }

        // --- PUSTE METODY DLA DESIGNERA ---
        private void txtPESEL_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }

        private void txtParametr2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}