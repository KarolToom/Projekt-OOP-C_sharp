using System;
using System.Data;
using Microsoft.Data.SqlClient;
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
            using (SqlConnection polaczenie = new SqlConnection(connectionString))
            {
                try
                {
                    polaczenie.Open();
                    string zapytanie = "SELECT IdPracownika, Imie, Nazwisko, PESEL FROM Pracownicy";
                    SqlDataAdapter adapter = new SqlDataAdapter(zapytanie, polaczenie);
                    System.Data.DataTable tabela = new System.Data.DataTable();
                    adapter.Fill(tabela);

                    dgvPracownicy.DataSource = tabela; // To wrzuci dane prosto do tabelki w GUI!
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

        }

        private void btnOdswiez_Click(object sender, EventArgs e)
        {
            ZaladujDane();
        }
    }
}
