using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dialogowe.Model
{
    class Sprzet
    {
        public int cena { get; set; }
        public int iloscSztuk { get; set; }
        public int idSprzetu { get; set; }
        public int idPodzespolu { get; set; }
        public string text { get; set; }
        public int numerNaLiscie { get; set; }

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

        public string PobierzOpis()
        {
            return "Liczba Rdzeni: " + liczbaRdzeni + " producent: " + producent;
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

        public string PobierzOpis()
        {
            return "Pojemnosc: " + pojemnosc + " GB taktowanie: " + taktowanie + " GHz";
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

        public string PobierzOpis()
        {
            return "Pojemnosc: " + pojemnosc + " GB";
        }
    }
}
