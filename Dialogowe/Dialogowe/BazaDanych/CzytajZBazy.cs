using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dialogowe.Model;
using System.Diagnostics;

namespace Dialogowe.BazaDanych
{
    class CzytajZBazy
    {
        #region Pobieranie danych
        //Pobiera wszystkie procki
        public ObservableCollection<Sprzet> pobierzProcesory()
        {
            ObservableCollection<Sprzet> lista = new ObservableCollection<Sprzet>();

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT * " +
                                                  "FROM Procesor " +
                                                  "LEFT JOIN Sprzet ON Procesor.FK_IDSprzet = Sprzet.IDSprzet;", Connect);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();


            while (dr.Read())
            {
                Procesor s = new Procesor();
                s.iloscSztuk = Convert.ToInt32(dr["Magazyn"]);
                s.cena = Convert.ToInt32(dr["Cena"]);
                s.idSprzetu = Convert.ToInt32(dr["IDSprzet"]);
                s.idPodzespolu = Convert.ToInt32(dr["ID"]);

                s.producent = dr["Producent"].ToString();
                s.liczbaRdzeni = Convert.ToInt32(dr["LiczbaRdzeni"]);

                lista.Add(s);
            }
            
            Connect.Close();

            return lista;
        }

        public ObservableCollection<Sprzet> pobierzPamieciRam()
        {
            ObservableCollection<Sprzet> lista = new ObservableCollection<Sprzet>();

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT * " +
                                                  "FROM PamiecRam " +
                                                  "LEFT JOIN Sprzet ON PamiecRam.FK_IDSprzet = Sprzet.IDSprzet;", Connect);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();


            while (dr.Read())
            {
                PamiecRam s = new PamiecRam();
                s.iloscSztuk = Convert.ToInt32(dr["Magazyn"]);
                s.cena = Convert.ToInt32(dr["Cena"]);
                s.idSprzetu = Convert.ToInt32(dr["IDSprzet"]);
                s.idPodzespolu = Convert.ToInt32(dr["ID"]);

                s.pojemnosc = Convert.ToInt32(dr["Pojemnosc"]);
                s.taktowanie = Convert.ToInt32(dr["Taktowanie"]);

                lista.Add(s);
            }

            Connect.Close();

            return lista;
        }

        public ObservableCollection<Sprzet> pobierzDyskiTwarde()
        {
            ObservableCollection<Sprzet> lista = new ObservableCollection<Sprzet>();

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT * " +
                                                  "FROM DyskTwardy " +
                                                  "LEFT JOIN Sprzet ON DyskTwardy.FK_IDSprzet = Sprzet.IDSprzet;", Connect);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();


            while (dr.Read())
            {
                DyskTwardy s = new DyskTwardy();
                s.iloscSztuk = Convert.ToInt32(dr["Magazyn"]);
                s.cena = Convert.ToInt32(dr["Cena"]);
                s.idSprzetu = Convert.ToInt32(dr["IDSprzet"]);
                s.idPodzespolu = Convert.ToInt32(dr["ID"]);

                s.pojemnosc = Convert.ToInt32(dr["Pojemnosc"]);

                lista.Add(s);
            }

            Connect.Close();

            return lista;
        }

        public ObservableCollection<Uzytkownik> pobierzUzytkownikow()
        {
            ObservableCollection<Uzytkownik> lista = new ObservableCollection<Uzytkownik>();

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT * " +
                                                  "FROM Klient", Connect);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();


            while (dr.Read())
            {
                Uzytkownik s = new Uzytkownik();
                s.haslo = dr["Haslo"].ToString();
                s.imie = dr["Imie"].ToString();
                s.id = Convert.ToInt32(dr["ID"]);

                lista.Add(s);
            }

            Connect.Close();

            return lista;
        }

        public int pobierzIDKlienta(string Login)
        {
            ObservableCollection<Uzytkownik> lista = new ObservableCollection<Uzytkownik>();

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT ID " +
                                                  "FROM Klient WHERE Imie = @Login ", Connect);

            czytajnik.Parameters.AddWithValue("@Login", Login);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            dr.Read();
            int no = Convert.ToInt32(dr[0]);

            Connect.Close();

            return no;
        }

        public ObservableCollection<Zamowienie> pobierzZamowienia(string Login)
        {
            ObservableCollection<Zamowienie> lista = new ObservableCollection<Zamowienie>();

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT * " +
                                                  "FROM Zamowienie " +
                                                  "WHERE FK_KlientID = @ID ", Connect);

            czytajnik.Parameters.AddWithValue("@ID", pobierzIDKlienta(Login));

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            while (dr.Read())
            {
                
                lista.Add(new Zamowienie
                {
                    id = Convert.ToInt32(dr["ID"]),
                    koszt = Convert.ToInt32(dr["Koszt"])
                });
            }

            Connect.Close();

            foreach(Zamowienie i in lista)
            {
                i.lista = pobierzPozycjeZamowienia(i.id);
            }

            return lista;
        }

        public ObservableCollection<PozycjaZamowienia> pobierzPozycjeZamowienia(int idZamowienia)
        {
            ObservableCollection<PozycjaZamowienia> lista = new ObservableCollection<PozycjaZamowienia>();

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT * " +
                                                  "FROM PozycjaZamowienia " +
                                                  "WHERE FK_IDZamowienie = @idZamowienia", Connect);

            czytajnik.Parameters.AddWithValue("@idZamowienia", idZamowienia);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new PozycjaZamowienia
                {
                    liczba = Convert.ToInt32(dr["Liczba"]),
                    id = Convert.ToInt32(dr["ID"]),
                    sprzet = new Sprzet
                    {
                        idSprzetu = Convert.ToInt32(dr["IDSprzetu"])
                    }
            });

                if (czyDysk(Convert.ToInt32(dr["IDSprzetu"])))
                {
                    lista[lista.Count - 1].typ = PozycjaZamowienia.typSprzetu.dyskTwardy;
                    lista[lista.Count - 1].sprzet = pobierzDyskTwardy(Convert.ToInt32(dr["IDSprzetu"]));
                }
                else if (czyProcesor(Convert.ToInt32(dr["IDSprzetu"])))
                {
                    lista[lista.Count - 1].typ = PozycjaZamowienia.typSprzetu.procesor;
                    lista[lista.Count - 1].sprzet = pobierzProcesor(Convert.ToInt32(dr["IDSprzetu"]));
                }
                else if (czyRAM(Convert.ToInt32(dr["IDSprzetu"])))
                {
                    lista[lista.Count - 1].typ = PozycjaZamowienia.typSprzetu.RAM;
                    lista[lista.Count - 1].sprzet = pobierzPamiecRAM(Convert.ToInt32(dr["IDSprzetu"]));
                }
            }
        
            Connect.Close();
            return lista;
        }

        public DyskTwardy pobierzDyskTwardy(int IDsprzet)
        {
            return null;
        }

        public Procesor pobierzProcesor(int IDsprzet)
        {
            return null;
        }

        public PamiecRam pobierzPamiecRAM(int IDsprzet)
        {
            return null;
        }

        public Boolean czyDysk(int id)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT COUNT(*) " +
                                                  "FROM DyskTwardy WHERE FK_IDSprzet = @id ", Connect);

            czytajnik.Parameters.AddWithValue("@id", id);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            dr.Read();
            int no = Convert.ToInt32(dr[0]);

            Connect.Close();

            if (no == 1)
                return true;
            else
                return false;
        }
        public Boolean czyProcesor(int id)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT COUNT(*) " +
                                                  "FROM Procesor WHERE FK_IDSprzet = @id ", Connect);

            czytajnik.Parameters.AddWithValue("@id", id);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            dr.Read();
            int no = Convert.ToInt32(dr[0]);

            Connect.Close();

            if (no == 1)
                return true;
            else
                return false;
        }
        public Boolean czyRAM(int id)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT COUNT(*) " +
                                                  "FROM PamiecRam WHERE FK_IDSprzet = @id ", Connect);

            czytajnik.Parameters.AddWithValue("@id", id);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            dr.Read();
            int no = Convert.ToInt32(dr[0]);

            Connect.Close();

            if (no == 1)
                return true;
            else
                return false;
        }

        public int pobierzLiczbeZamowien(string Login)
        {
            ObservableCollection<Uzytkownik> lista = new ObservableCollection<Uzytkownik>();

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT COUNT(*) " +
                                                  "FROM Zamowienie WHERE FK_KlientID = @FK_KlientID ", Connect);

            czytajnik.Parameters.AddWithValue("@FK_KlientID", pobierzIDKlienta(Login));

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            dr.Read();
            int no = Convert.ToInt32(dr[0]);

            Connect.Close();

            return no;
        }

        #endregion

        //Do testowania jakby cos nie pyklo
        public void wypiszCoPrzeczytales(SqlDataReader dr)
        {
            for (int i = 0; i < dr.FieldCount; i++)
                Debug.Write(dr.GetName(i) + ": " + dr[i].ToString() + " ");
            Debug.WriteLine("");     
        }
    }
}
