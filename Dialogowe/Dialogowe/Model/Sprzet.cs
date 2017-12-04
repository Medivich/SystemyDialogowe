using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dialogowe.Model
{
    class Sprzet
    {
        public int cena;
        public int iloscSztuk;
        public int idSprzetu;
        public int idPodzespolu;

        public virtual void Czytaj()
        {
            Debug.WriteLine(" Cena: " + cena + " Magazyn: " + iloscSztuk + " idSprzetu: "
                + idSprzetu + " idPodzespolu: " + idPodzespolu + " ");
        }
    }

    class Procesor : Sprzet
    {
        public int liczbaRdzeni;
        public string producent;

        public override void Czytaj()
        {
            Debug.Write("Procesor: liczba Rdzeni: " + liczbaRdzeni + " producent: " + producent);
            base.Czytaj();
        }
    }

    class PamiecRam : Sprzet
    {
        public float taktowanie;
        public int pojemnosc;

        public override void Czytaj()
        {
            Debug.Write("RAM: pojemnosc: " + pojemnosc + " taktowanie: " + taktowanie);
            base.Czytaj();
        }
    }

    class DyskTwardy : Sprzet
    {
        public int pojemnosc;

        public override void Czytaj()
        {
            Debug.Write("Dysk: pojemnosc: " + pojemnosc);
            base.Czytaj();
        }
    }
}
