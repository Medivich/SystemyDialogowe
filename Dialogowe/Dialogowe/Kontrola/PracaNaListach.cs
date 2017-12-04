using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dialogowe.Model;

namespace Dialogowe.Kontrola
{
    class PracaNaListach
    {
        public void CzytajDane(ObservableCollection<Sprzet> lista)
        {
            foreach (Sprzet s in lista)
                s.Czytaj();
        }
    }
}
