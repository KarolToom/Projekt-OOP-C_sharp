using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Ewidencja_Pracownikow
{
    public class RepozytoriumSQL : IRepozytorium // Implementacja repozytorium korzystająca z bazy danych SQL Server. Miejsce przechowywania danych
    {
        private readonly string _connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EwidencjaPracownikow;Trusted_Connection=True;";

        public List<Pracownik> PobierzWszystkich()
        {
            var lista = new List<Pracownik>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT P.Imie, P.Nazwisko, P.PESEL, S.Nazwa as Stanowisko, 
                                 W.SumaCalkowita as Pensja, W.ParametrLiczbowy1, W.ParametrLiczbowy2 
                                 FROM Pracownicy P 
                                 JOIN Stanowiska S ON P.IdStanowiska = S.IdStanowiska
                                 LEFT JOIN Wynagrodzenia W ON P.IdPracownika = W.IdPracownika";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
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
                            lista.Add(new Kierowca(imie, nazwisko, pesel, pensja, (int)p1, p2));
                        else if (stanowisko == "Handlowiec")
                            lista.Add(new Handlowiec(imie, nazwisko, pesel, pensja, p1, p2));
                        else if (stanowisko == "Manager")
                            lista.Add(new Manager(imie, nazwisko, pesel, pensja, p1, p2));
                        else
                            lista.Add(new PracownikBiurowy(imie, nazwisko, pesel, pensja, p1));
                    }
                }
            }
            return lista;
        }

        public DanePracownikaDoEdycji PobierzDoEdycji(int id) // Metoda do pobierania danych pracownika do edycji
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT P.Imie, P.Nazwisko, P.PESEL, S.Nazwa, W.SumaCalkowita, W.ParametrLiczbowy1, W.ParametrLiczbowy2 
                               FROM Pracownicy P
                               JOIN Stanowiska S ON P.IdStanowiska = S.IdStanowiska
                               LEFT JOIN Wynagrodzenia W ON P.IdPracownika = W.IdPracownika
                               WHERE P.IdPracownika = @id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new DanePracownikaDoEdycji
                            {
                                IdPracownika = id,
                                Imie = reader["Imie"].ToString(),
                                Nazwisko = reader["Nazwisko"].ToString(),
                                Pesel = reader["PESEL"].ToString(),
                                NazwaStanowiska = reader["Nazwa"].ToString(),
                                Pensja = reader["SumaCalkowita"] != DBNull.Value ? Convert.ToDecimal(reader["SumaCalkowita"]) : 0,
                                P1 = reader["ParametrLiczbowy1"] != DBNull.Value ? Convert.ToDecimal(reader["ParametrLiczbowy1"]) : 0,
                                P2 = reader["ParametrLiczbowy2"] != DBNull.Value ? Convert.ToDecimal(reader["ParametrLiczbowy2"]) : 0
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void DodajPracownika(Pracownik p, int idDzialu, int idStanowiska) // Metoda do dodawania pracownika
        {
           
            decimal p1 = 0, p2 = 0;
            if (p is Kierowca k) { p1 = k.PrzejechaneKilometry; p2 = k.StawkaZaKilometr; }
            else if (p is Handlowiec h) { p1 = h.WartoscSprzedazy; p2 = h.ProcentProwizji; }
            else if (p is Manager m) { p1 = m.DodatekStazowy; p2 = m.PremiaMenadzerska; }
            else if (p is PracownikBiurowy pb) { p1 = pb.DodatekStazowy; }

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlP = @"INSERT INTO Pracownicy (Imie, Nazwisko, PESEL, IdDzialu, IdStanowiska) 
                                        VALUES (@i, @n, @p, @idD, @idS); SELECT SCOPE_IDENTITY();";

                        int noweId;
                        using (var cmdP = new SqlCommand(sqlP, conn, trans))
                        {
                            cmdP.Parameters.AddWithValue("@i", p.Imie);
                            cmdP.Parameters.AddWithValue("@n", p.Nazwisko);
                            cmdP.Parameters.AddWithValue("@p", p.Pesel);
                            cmdP.Parameters.AddWithValue("@idD", idDzialu);
                            cmdP.Parameters.AddWithValue("@idS", idStanowiska);
                            noweId = Convert.ToInt32(cmdP.ExecuteScalar());
                        }

                        string sqlW = @"INSERT INTO Wynagrodzenia (IdPracownika, DataObliczenia, SumaCalkowita, ParametrLiczbowy1, ParametrLiczbowy2) 
                                        VALUES (@id, @data, @suma, @p1, @p2)";

                        using (var cmdW = new SqlCommand(sqlW, conn, trans))
                        {
                            cmdW.Parameters.AddWithValue("@id", noweId);
                            cmdW.Parameters.AddWithValue("@data", DateTime.Now);
                            cmdW.Parameters.AddWithValue("@suma", p.PensjaZasadnicza);
                            cmdW.Parameters.AddWithValue("@p1", p1);
                            cmdW.Parameters.AddWithValue("@p2", p2);
                            cmdW.ExecuteNonQuery();
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void EdytujPracownika(int idPracownika, Pracownik p, int idDzialu, int idStanowiska)
        {
            decimal p1 = 0, p2 = 0;
            if (p is Kierowca k) { p1 = k.PrzejechaneKilometry; p2 = k.StawkaZaKilometr; }
            else if (p is Handlowiec h) { p1 = h.WartoscSprzedazy; p2 = h.ProcentProwizji; }
            else if (p is Manager m) { p1 = m.DodatekStazowy; p2 = m.PremiaMenadzerska; }
            else if (p is PracownikBiurowy pb) { p1 = pb.DodatekStazowy; }

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlP = @"UPDATE Pracownicy SET Imie=@i, Nazwisko=@n, PESEL=@ps, IdDzialu=@idD, IdStanowiska=@idS 
                                        WHERE IdPracownika=@id";
                        using (var cmdP = new SqlCommand(sqlP, conn, trans))
                        {
                            cmdP.Parameters.AddWithValue("@i", p.Imie);
                            cmdP.Parameters.AddWithValue("@n", p.Nazwisko);
                            cmdP.Parameters.AddWithValue("@ps", p.Pesel);
                            cmdP.Parameters.AddWithValue("@idD", idDzialu);
                            cmdP.Parameters.AddWithValue("@idS", idStanowiska);
                            cmdP.Parameters.AddWithValue("@id", idPracownika);
                            cmdP.ExecuteNonQuery();
                        }

                        string sqlW = @"UPDATE Wynagrodzenia SET SumaCalkowita=@suma, ParametrLiczbowy1=@p1, ParametrLiczbowy2=@p2 
                                        WHERE IdPracownika=@id";
                        using (var cmdW = new SqlCommand(sqlW, conn, trans))
                        {
                            cmdW.Parameters.AddWithValue("@suma", p.PensjaZasadnicza);
                            cmdW.Parameters.AddWithValue("@p1", p1);
                            cmdW.Parameters.AddWithValue("@p2", p2);
                            cmdW.Parameters.AddWithValue("@id", idPracownika);
                            cmdW.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void UsunPracownika(string pesel) // Metoda do usuwania pracownika na podstawie PESEL
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlW = @"DELETE FROM Wynagrodzenia 
                                        WHERE IdPracownika = (SELECT IdPracownika FROM Pracownicy WHERE PESEL = @pesel)";
                        using (var cmdW = new SqlCommand(sqlW, conn, trans))
                        {
                            cmdW.Parameters.AddWithValue("@pesel", pesel);
                            cmdW.ExecuteNonQuery();
                        }

                        string sqlP = "DELETE FROM Pracownicy WHERE PESEL = @pesel";
                        using (var cmdP = new SqlCommand(sqlP, conn, trans))
                        {
                            cmdP.Parameters.AddWithValue("@pesel", pesel);
                            cmdP.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw; 
                    }
                }
            }
        }

        public bool CzyPeselIstnieje(string pesel) // Metoda do sprawdzania istnienia PESEL w bazie
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Pracownicy WHERE PESEL = @p", conn))
                {
                    cmd.Parameters.AddWithValue("@p", pesel);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public int PobierzIdPoPeselu(string pesel) // Metoda do pobierania IdPracownika na podstawie PESEL
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT IdPracownika FROM Pracownicy WHERE PESEL = @p", conn))
                {
                    cmd.Parameters.AddWithValue("@p", pesel);
                    object result = cmd.ExecuteScalar();
                    return result != null ? (int)result : 0;
                }
            }
        }
    }
}