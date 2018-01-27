using Dialogowe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public void dodajZamowienie(ObservableCollection<PozycjaZamowienia> lista, int idUzytkownika)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand Command = new SqlCommand(@"Insert Into Zamowienie(FK_KlientID, Koszt) output INSERTED.ID 
                                                Values(@FK_KlientID, @Koszt)", Connect);
            int koszt = 0;

            foreach(PozycjaZamowienia p in lista)
                koszt += p.liczba * p.sprzet.cena;
            

            Command.Parameters.AddWithValue("@FK_KlientID", idUzytkownika);
            Command.Parameters.AddWithValue("@Koszt", koszt);

            Connect.Open();
            int idZamowienia = (int)Command.ExecuteScalar();
            Connect.Close();

            foreach (PozycjaZamowienia p in lista)
                dodajPozycjeZamowienia(p.sprzet, idZamowienia, p.liczba);
        }

        public void dodajPozycjeZamowienia(Sprzet s, int IDZamowienie, int liczba)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand Command = new SqlCommand(@"Insert Into PozycjaZamowienia(IDSprzetu, Liczba, FK_IDZamowienie) 
                                                Values(@IDSprzetu, @Liczba, @FK_IDZamowienie)", Connect);

            Command.Parameters.AddWithValue("@Liczba", liczba);
            Command.Parameters.AddWithValue("@IDSprzetu", s.idSprzetu);
            Command.Parameters.AddWithValue("@FK_IDZamowienie", IDZamowienie);

            Connect.Open();
            Command.ExecuteNonQuery();
            Connect.Close();
        }

        public void dodajUzytkownika(string imie, string haslo)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand Command = new SqlCommand(@"Insert Into Klient(Haslo, Imie) 
                                                Values(@Haslo, @Imie)", Connect);

            Command.Parameters.AddWithValue("@imie", imie);
            Command.Parameters.AddWithValue("@haslo", haslo);

            Connect.Open();
            Command.ExecuteNonQuery();
            Connect.Close();
        }

        //zwraca id sprzetu
        private int dodajSprzet(int Cena, int Magazyn)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
            SqlCommand Command = new SqlCommand(@"Insert Into Sprzet(Cena, Magazyn) output INSERTED.IDSprzet
                                                Values(@Cena, @Magazyn)", Connect);

            Command.Parameters.AddWithValue("@Cena", Cena);
            Command.Parameters.AddWithValue("@Magazyn", Magazyn);

            Connect.Open();
            int idSprzetu = (int)Command.ExecuteScalar();
            Connect.Close();

            return idSprzetu;
        }
    }
}
