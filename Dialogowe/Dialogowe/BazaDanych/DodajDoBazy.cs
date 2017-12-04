using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Dialogowe.BazaDanych
{
    class DodajDoBazy
    {
        //Dodaje procek do bazy
        public void dodajProcesor(int LiczbaRdzeni, string Producent, int cena, int iloscSztuk)
        {
            int idSprzetu = dodajSprzet(cena, iloscSztuk);

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand Command = new SqlCommand(@"Insert Into Procesor(Producent, LiczbaRdzeni, FK_IDSprzet) 
                                                Values(@Producent, @LiczbaRdzeni, @FK_IDSprzet)", Connect);
            Command.Parameters.AddWithValue("@Producent", Producent);
            Command.Parameters.AddWithValue("@LiczbaRdzeni", LiczbaRdzeni);
            Command.Parameters.AddWithValue("@FK_IDSprzet", idSprzetu);

            Connect.Open();
            Command.ExecuteNonQuery();
            Connect.Close();
        }

        public void dodajRAM(float taktowanie, int pojemnosc, int cena, int iloscSztuk)
        {
            int idSprzetu = dodajSprzet(cena, iloscSztuk);

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand Command = new SqlCommand(@"Insert Into PamiecRam(Taktowanie, Pojemnosc, FK_IDSprzet) 
                                                Values(@taktowanie, @pojemnosc, @FK_IDSprzet)", Connect);
            Command.Parameters.AddWithValue("@taktowanie", taktowanie);
            Command.Parameters.AddWithValue("@pojemnosc", pojemnosc);
            Command.Parameters.AddWithValue("@FK_IDSprzet", idSprzetu);

            Connect.Open();
            Command.ExecuteNonQuery();
            Connect.Close();
        }

        public void dodajDysk(int pojemnosc, int cena, int iloscSztuk)
        {
            int idSprzetu = dodajSprzet(cena, iloscSztuk);

            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand Command = new SqlCommand(@"Insert Into DyskTwardy(Pojemnosc, FK_IDSprzet) 
                                                Values(@pojemnosc, @FK_IDSprzet)", Connect);

            Command.Parameters.AddWithValue("@pojemnosc", pojemnosc);
            Command.Parameters.AddWithValue("@FK_IDSprzet", idSprzetu);

            Connect.Open();
            Command.ExecuteNonQuery();
            Connect.Close();
        }

        //zwraca id sprzetu
        private int dodajSprzet(int Cena, int Magazyn)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand Command = new SqlCommand(@"Insert Into Sprzet(Cena, Magazyn) 
                                                Values(@Cena, @Magazyn)", Connect);

            Command.Parameters.AddWithValue("@Cena", Cena);
            Command.Parameters.AddWithValue("@Magazyn", Magazyn);

            Connect.Open();
            Command.ExecuteNonQuery();
            Connect.Close();

            CzytajZBazy czytaj = new CzytajZBazy();
            return czytaj.pobierzIDSprzetu();
        }
    }
}
