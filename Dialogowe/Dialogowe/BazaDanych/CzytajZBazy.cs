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
        #region Pobieranie kolejnego ID

        public int pobierzIDSprzetu()
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand czytajnik = new SqlCommand("SELECT COUNT(*) FROM Sprzet", Connect);
     
            Connect.Open();
            SqlDataReader dr = czytajnik.ExecuteReader();

            dr.Read();
            int id = Convert.ToInt32(dr[0]);

            Connect.Close();
            return id;
        }

        #endregion

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

        public ObservableCollection<Sprzet> pobierzPamiecRam()
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
        #endregion

        //Do testowania jakby cos nie pyklo
        public void wypiszCoPrzeczytales(SqlDataReader dr)
        {
            while (dr.Read())
            {
                for (int i = 0; i < dr.FieldCount; i++)
                    Debug.Write(dr.GetName(i) + ": " + dr[i].ToString() + " ");
                Debug.WriteLine("");
            }
        }
    }
}
