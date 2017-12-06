using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Speech.Synthesis;
using System.Diagnostics;
using Dialogowe.BazaDanych;
using Dialogowe.Kontrola;
using Dialogowe.Model;
using System.Collections.ObjectModel;

namespace Dialogowe
{
    class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            if (!_startKontrolera)
            {
                _startKontrolera = true;
                Thread t0 = new Thread(Kontroler);
                t0.IsBackground = true;
                t0.Start();
            }
        }

        public Uzytkownik uzytkownik = null;
        public bool _startKontrolera = false;
        public string _tekstWiadomosci = "Witaj, jak się nazywasz?";

        public string TekstWiadomosci
        {
            set
            {
                this._tekstWiadomosci = value;
                OnPropertyChanged("TekstWiadomosci");
            }
            get
            {
                return this._tekstWiadomosci;
            }
        }

        private void dodajDane()
        {
            CzytajZBazy czytaj = new CzytajZBazy();
            DodajDoBazy dodaj = new DodajDoBazy();

            dodaj.dodajProcesor(5, "Samsung", 1000, 5);
            dodaj.dodajProcesor(1, "Intel", 1000, 5);
            dodaj.dodajProcesor(1, "Intel", 500, 7);
            dodaj.dodajProcesor(5, "AMD", 1000, 5);
            dodaj.dodajProcesor(6, "AMD", 1300, 5);
            dodaj.dodajProcesor(6, "Intel", 3000, 2);

            dodaj.dodajDysk(500, 300, 5);
            dodaj.dodajDysk(1500, 700, 7);
            dodaj.dodajDysk(600, 350, 5);
            dodaj.dodajDysk(700, 500, 2);
            dodaj.dodajDysk(200, 50, 3);

            dodaj.dodajRAM(1.2F, 4, 200, 5);
            dodaj.dodajRAM(1.066F, 2, 50, 5);
            dodaj.dodajRAM(2.0F, 8, 300, 2);
            dodaj.dodajRAM(2.0F, 4, 500, 3);

            dodaj.dodajUzytkownika("Konrad", "1");
            dodaj.dodajUzytkownika("Artur", "2");
        }

        private void Kontroler()
        {
            dodajDane();
            pobierzNazweUsera();
            podajHaslo();
        }

        private void pobierzNazweUsera()
        {
            Powiedz("Witaj w sklepie komputerowym neutron, podaj swoje imie");
            RozpoznawanieMowy s = new RozpoznawanieMowy();
            ObservableCollection<Uzytkownik> userzy = new CzytajZBazy().pobierzUzytkownikow();
            foreach (Uzytkownik user in userzy)
            {
                user.imie = user.imie.Replace(" ", string.Empty);
                user.haslo = user.haslo.Replace(" ", string.Empty);
                s.dodajSlowa(user.imie);
            }

            string imie = s.rozpoznajZSlownika();

            foreach(Uzytkownik user in userzy)
            {
                if (String.Compare(user.imie, imie) == 0)
                    uzytkownik = user;
            }

        }

        private void podajHaslo()
        {
            Powiedz("Witaj " + uzytkownik.imie + " podaj swoje haslo");

            RozpoznawanieMowy s = new RozpoznawanieMowy();
            s.dodajSlowa(uzytkownik.haslo);

            if (String.Compare(uzytkownik.haslo, s.rozpoznajZSlownika()) == 0)
                Powiedz("Haslo poprawne");
        }

        private void Powiedz(string s)
        {
            TekstWiadomosci = s;
            SyntezaMowy synteza = new SyntezaMowy();
            synteza.Mow(s);
        }

        // Odświeżanie kontrolek
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(params string[] update)
        {
            if (PropertyChanged != null)
                foreach (string up in update)
                    PropertyChanged(this, new PropertyChangedEventArgs(up));
        }
    }
}
