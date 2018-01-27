using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dialogowe.Model
{
    class PozycjaZamowienia
    {
        public enum typSprzetu
        {
            procesor,
            dyskTwardy,
            RAM
        }

        public Sprzet sprzet;
        public int liczba;
        public int id;
        public typSprzetu typ;

        public void wypisz()
        {
            Debug.WriteLine("ID pozycji " + id + " liczba sztuk " + liczba);
            if(sprzet != null)
                Debug.WriteLine("Sprzet: cena" + sprzet.cena + " id: " + sprzet.idPodzespolu + " idSprzetu " + sprzet.idSprzetu + " ilosc sztuk " + sprzet.iloscSztuk);
        }
    }
}
