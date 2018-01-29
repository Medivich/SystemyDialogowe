using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dialogowe.Model;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

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

            int i = 1;
            while (dr.Read())
            {
                Procesor s = new Procesor();
                s.iloscSztuk = Convert.ToInt32(dr["Magazyn"]);
                s.cena = Convert.ToInt32(dr["Cena"]);
                s.idSprzetu = Convert.ToInt32(dr["IDSprzet"]);
                s.idPodzespolu = Convert.ToInt32(dr["ID"]);

                s.producent = dr["Producent"].ToString();
                s.liczbaRdzeni = Convert.ToInt32(dr["LiczbaRdzeni"]);
                s.zdjecie = (byte[])dr["Zdjecie"];
                s.text = new CzytajZBazy().pobierzProcesor(s.idSprzetu).PobierzOpis();
                s.numerNaLiscie = i++;
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

            int i = 1;
            while (dr.Read())
            {
                PamiecRam s = new PamiecRam();
                s.iloscSztuk = Convert.ToInt32(dr["Magazyn"]);
                s.cena = Convert.ToInt32(dr["Cena"]);
                s.idSprzetu = Convert.ToInt32(dr["IDSprzet"]);
                s.idPodzespolu = Convert.ToInt32(dr["ID"]);

                s.pojemnosc = Convert.ToInt32(dr["Pojemnosc"]);
                s.taktowanie = Convert.ToInt32(dr["Taktowanie"]);
                s.zdjecie = (byte[])dr["Zdjecie"];
                s.text = new CzytajZBazy().pobierzProcesor(s.idSprzetu).PobierzOpis();
                s.numerNaLiscie = i++;

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

            int i = 1;
            while (dr.Read())
            {
                DyskTwardy s = new DyskTwardy();
                s.iloscSztuk = Convert.ToInt32(dr["Magazyn"]);
                s.cena = Convert.ToInt32(dr["Cena"]);
                s.idSprzetu = Convert.ToInt32(dr["IDSprzet"]);
                s.idPodzespolu = Convert.ToInt32(dr["ID"]);

                s.pojemnosc = Convert.ToInt32(dr["Pojemnosc"]);
                s.zdjecie = (byte[])dr["Zdjecie"];
                s.text = new CzytajZBazy().pobierzProcesor(s.idSprzetu).PobierzOpis();
                s.numerNaLiscie = i++;
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
                    lista[lista.Count - 1].sprzet.text = pobierzDyskTwardy(Convert.ToInt32(dr["IDSprzetu"])).PobierzOpis();
                }
                else if (czyProcesor(Convert.ToInt32(dr["IDSprzetu"])))
                {
                    lista[lista.Count - 1].typ = PozycjaZamowienia.typSprzetu.procesor;
                    lista[lista.Count - 1].sprzet = pobierzProcesor(Convert.ToInt32(dr["IDSprzetu"]));
                    lista[lista.Count - 1].sprzet.text = pobierzProcesor(Convert.ToInt32(dr["IDSprzetu"])).PobierzOpis();
                }
                else if (czyRAM(Convert.ToInt32(dr["IDSprzetu"])))
                {
                    lista[lista.Count - 1].typ = PozycjaZamowienia.typSprzetu.RAM;
                    lista[lista.Count - 1].sprzet = pobierzPamiecRAM(Convert.ToInt32(dr["IDSprzetu"]));
                    lista[lista.Count - 1].sprzet.text = pobierzPamiecRAM(Convert.ToInt32(dr["IDSprzetu"])).PobierzOpis();

                }

                lista[lista.Count - 1].sprzet.idSprzetu = pobierzSprzet(Convert.ToInt32(dr["IDSprzetu"])).idSprzetu;
                lista[lista.Count - 1].sprzet.cena = pobierzSprzet(Convert.ToInt32(dr["IDSprzetu"])).cena;
                lista[lista.Count - 1].sprzet.zdjecie = pobierzSprzet(Convert.ToInt32(dr["IDSprzetu"])).zdjecie;
            }
        
            Connect.Close();
            return lista;
        }

        public Sprzet pobierzSprzet(int IDsprzet)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT * " +
                                                  "FROM Sprzet WHERE IDsprzet = @IDsprzet ", Connect);

            czytajnik.Parameters.AddWithValue("@IDsprzet", IDsprzet);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            dr.Read();
            Sprzet sprz = new Sprzet
            {
                cena = Convert.ToInt32(dr["Cena"]),
                iloscSztuk = Convert.ToInt32(dr["Magazyn"]),
                zdjecie = (byte[]) dr["Zdjecie"],
                idSprzetu = IDsprzet
            };

            Connect.Close();

            return sprz;
        }

        public DyskTwardy pobierzDyskTwardy(int IDsprzet)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT * " +
                                                  "FROM DyskTwardy WHERE FK_IDSprzet = @IDsprzet ", Connect);

            czytajnik.Parameters.AddWithValue("@IDsprzet", IDsprzet);

           

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            dr.Read();
            DyskTwardy dysk = new DyskTwardy
            {
                pojemnosc = Convert.ToInt32(dr["Pojemnosc"]),
                idPodzespolu = Convert.ToInt32(dr["ID"]),
            };

            Connect.Close();

            return dysk;
        }

        public Procesor pobierzProcesor(int IDsprzet)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT * " +
                                                  "FROM Procesor WHERE FK_IDSprzet = @IDsprzet ", Connect);

            czytajnik.Parameters.AddWithValue("@IDsprzet", IDsprzet);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            dr.Read();
            Procesor procek = new Procesor
            {
                idPodzespolu = Convert.ToInt32(dr["ID"]),
                producent = dr["Producent"].ToString(),
                liczbaRdzeni = Convert.ToInt32(dr["LiczbaRdzeni"])
            };

            Connect.Close();

            return procek;
        }

        public PamiecRam pobierzPamiecRAM(int IDsprzet)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT * " +
                                                  "FROM PamiecRam WHERE FK_IDSprzet = @IDsprzet ", Connect);

            czytajnik.Parameters.AddWithValue("@IDsprzet", IDsprzet);

            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            dr.Read();
            PamiecRam ram = new PamiecRam
            {
                idPodzespolu = Convert.ToInt32(dr["ID"]),
                pojemnosc = Convert.ToInt32(dr["Pojemnosc"]),
                taktowanie = Convert.ToInt32(dr["Taktowanie"])
            };

            Connect.Close();

            return ram;
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
