using Microsoft.Data.SqlClient;
using System;
using System.Data;
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
                    dgvPracownicy.Columns["NazwaStanowiska"].HeaderText = "Stanowisko";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("B³¹d po³¹czenia: " + ex.Message);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            FormDodaj oknoDodawania = new FormDodaj();

      
            oknoDodawania.ShowDialog();

       
            ZaladujDane();

        }

        private void btnOdswiez_Click(object sender, EventArgs e)
        {
            ZaladujDane();
        }
    }
}
