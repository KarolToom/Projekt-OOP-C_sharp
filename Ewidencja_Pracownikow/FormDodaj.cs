using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ewidencja_Pracownikow
{
    public partial class FormDodaj : Form
    {
        public FormDodaj()
        {
            InitializeComponent();
        }

        private void btnZapisz_Click(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(txtImie.Text)) throw new Exception("Pole Imię jest puste!");


                string typ = cbTypPracownika.SelectedItem.ToString();
                string imie = txtImie.Text;
                string nazwisko = txtNazwisko.Text;
                string pesel = txtPESEL.Text;
                decimal pensja = decimal.Parse(txtPensja.Text);

                Pracownik nowy = null;

                switch (typ)
                {
                    case "Kierowca":
                        nowy = new Kierowca(imie, nazwisko, pesel, pensja, 500, 0.5m);
                        break;
                    case "Handlowiec":
                        nowy = new Handlowiec(imie, nazwisko, pesel, pensja, 10000, 5);
                        break;
                    case "Manager":
                        nowy = new Manager(imie, nazwisko, pesel, pensja, 2000, 3000);
                        break;
                    case "Pracownik Biurowy":
                        nowy = new PracownikBiurowy(imie, nazwisko, pesel, pensja, 1500);
                        break;
                }
                if (nowy != null)
                {
                    decimal suma = nowy.ObliczWynagrodzenie();
                    ZapiszDoBazy(nowy, typ);
                    MessageBox.Show($"Dodano pracownika: {nowy.Imie} {nowy.Nazwisko}, Wynagrodzenie: {suma:C}");
                    this.Close();
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Nieprawidłowy format danych. Pensja musi być liczbą.");
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Błąd bazy danych: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
            }
        }

        private void ZapiszDoBazy(Pracownik pracownik, string typ)
        {
            string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EwidencjaPracownikow;Trusted_Connection=True;Encrypt=False";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                int idStanowiskaZazyte = 1; // domyślnie 1

                if (typ == "Kierowca") idStanowiskaZazyte = 1;
                else if (typ == "Handlowiec") idStanowiskaZazyte = 2;
                else if (typ == "Manager") idStanowiskaZazyte = 3;
                else if (typ == "Pracownik Biurowy") idStanowiskaZazyte = 4;

                string zapytanie = "INSERT INTO Pracownicy (Imie, Nazwisko, PESEL, IdDzialu, IdStanowiska) VALUES (@imie, @nazwisko, @pesel, 1, @stanowisko)";

                using (SqlCommand cmd = new SqlCommand(zapytanie, conn))
                {
                    cmd.Parameters.AddWithValue("@Imie", pracownik.Imie);
                    cmd.Parameters.AddWithValue("@Nazwisko", pracownik.Nazwisko);
                    cmd.Parameters.AddWithValue("@Pesel", pracownik.Pesel);

                    cmd.Parameters.AddWithValue("@Stanowisko", idStanowiskaZazyte);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void txtPESEL_TextChanged(object sender, EventArgs e)
        {

        }
    }
}