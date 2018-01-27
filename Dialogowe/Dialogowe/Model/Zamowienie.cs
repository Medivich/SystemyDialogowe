using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dialogowe.Model
{
    class Zamowienie
    {
        public ObservableCollection<PozycjaZamowienia> lista = new ObservableCollection<PozycjaZamowienia>();
        public int koszt;
        public int id;

        public void czytajListe()
        {
            foreach (PozycjaZamowienia p in lista)
                p.wypisz();
        }
    }
}
