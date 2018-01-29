using Dialogowe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Dialogowe.BazaDanych
{
    class AktualizacjaBazyDanych
    {
        public void aktualizujDane(ObservableCollection<PozycjaZamowienia> lista)
        {
            foreach (PozycjaZamowienia z in lista)
            {
                aktualizujSprzet(z.sprzet.idSprzetu, new CzytajZBazy().pobierzSprzet(z.sprzet.idSprzetu).iloscSztuk - z.sprzet.iloscSztuk);
            }
        }

        private void aktualizujSprzet(int id, int liczba)
        {
            SqlConnection Connect = new SqlConnection(Polaczenie.connString);
           // new SqlCommand(@"Update Series set Banner = @Banner WHERE Id = @SeriesId", Connect);
            SqlCommand Command = new SqlCommand(@"Update Sprzet set Magazyn = @liczba
                                                 WHERE IDSprzet = @id", Connect);
            Command.Parameters.AddWithValue("@liczba", liczba);
            Command.Parameters.AddWithValue("@id", id);

            Connect.Open();
            Command.ExecuteNonQuery();
            Connect.Close();
        }
    }
}
