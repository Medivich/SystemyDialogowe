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
using Microsoft.Speech.Recognition;

namespace Dialogowe {
    class MainViewModel : INotifyPropertyChanged {

        private ObservableCollection<Uzytkownik> userzy;
        private DodajDoBazy obiektZapisuDoBazy;

        private StanyRozmowy stanRozmowy = StanyRozmowy.Powitanie;//enumeracja możliwych stanów rozmowy
        #region Referencje do singletonow
        private RozpoznawanieMowy rozpoznawanieMowy = RozpoznawanieMowy.obiekt;
        private SyntezaMowy synteza = SyntezaMowy.obiekt;
        #endregion

        public MainViewModel() {
            if (!_startKontrolera) {
                _startKontrolera = true;
                Thread t0 = new Thread(Kontroler);
                t0.IsBackground = true;
                t0.Start();
            }
        }

        public Uzytkownik uzytkownik = null;
        public bool _startKontrolera = false;
        private string _rozpoznanyTekst = "";
        public string RozpoznanyTekst {
            get {
                return _rozpoznanyTekst;
            }
            set {
                _rozpoznanyTekst = value;
                OnPropertyChanged("TekstWiadomosci");
            }
        }

        public string _tekstWiadomosci = "";

        public string TekstWiadomosci {
            set {
                this._tekstWiadomosci = value;
                OnPropertyChanged("TekstWiadomosci");
            }
            get {
                return this._tekstWiadomosci;
            }
        }

        private void dodajDane() {
            obiektZapisuDoBazy = new DodajDoBazy();

            obiektZapisuDoBazy.dodajProcesor(5, "Samsung", 1000, 5);
            obiektZapisuDoBazy.dodajProcesor(1, "Intel", 1000, 5);
            obiektZapisuDoBazy.dodajProcesor(1, "Intel", 500, 7);
            obiektZapisuDoBazy.dodajProcesor(5, "AMD", 1000, 5);
            obiektZapisuDoBazy.dodajProcesor(6, "AMD", 1300, 5);
            obiektZapisuDoBazy.dodajProcesor(6, "Intel", 3000, 2);

            obiektZapisuDoBazy.dodajDysk(500, 300, 5);
            obiektZapisuDoBazy.dodajDysk(1500, 700, 7);
            obiektZapisuDoBazy.dodajDysk(600, 350, 5);
            obiektZapisuDoBazy.dodajDysk(700, 500, 2);
            obiektZapisuDoBazy.dodajDysk(200, 50, 3);

            obiektZapisuDoBazy.dodajRAM(1.2F, 4, 200, 5);
            obiektZapisuDoBazy.dodajRAM(1.066F, 2, 50, 5);
            obiektZapisuDoBazy.dodajRAM(2.0F, 8, 300, 2);
            obiektZapisuDoBazy.dodajRAM(2.0F, 4, 500, 3);

            obiektZapisuDoBazy.dodajUzytkownika("Konrad", "1");
            obiektZapisuDoBazy.dodajUzytkownika("Artur", "2");
        }

        private void Kontroler() {
            dodajDane();
            //pobierzNazweUsera();
            //podajHaslo();

            while ((int)stanRozmowy < 99) {//rozmowa trwa az wejdziemy w stan zakonczenia jej
                switch (stanRozmowy) {
                    case StanyRozmowy.Powitanie: //witamy usera
                        Powiedz("Witaj w sklepie komputerowym neutron, podaj swoje imie");
                        stanRozmowy = StanyRozmowy.ZapytanieOLogin;
                        break;
                    case StanyRozmowy.ZapytanieOLogin://zapytanie o login
                        pobierzNazweUsera();//w tej metodzie nadpisujemy stan rozmowy jak zwroci ona wynik
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;//to bedzie nadpisane jak metoda (nie)rozpozna
                        break;
                    case StanyRozmowy.ZapytanieOHaslo://pytamy o haslo
                        pobierzHaslo();
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;



                    case StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze://nic nie robimy jak czekamy
                        break;
                    default:
                        break;
                }
            }

            if (stanRozmowy == StanyRozmowy.Pozegnanie) {

            }
            else if (stanRozmowy == StanyRozmowy.BrakOdpowiedzi) {

            }
            else if (stanRozmowy == StanyRozmowy.PrzerwanoRozmowe) {

            }
        }

        private void pobierzNazweUsera() {
            userzy = new CzytajZBazy().pobierzUzytkownikow();

            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            foreach (Uzytkownik user in userzy) {
                user.imie = user.imie.Replace(" ", string.Empty);
                user.haslo = user.haslo.Replace(" ", string.Empty);
                rozpoznawanieMowy.dodajSlowa(user.imie);
            }

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {
                string imie = e.Result.Text;

                foreach (Uzytkownik user in userzy) {
                    if (string.Compare(user.imie, imie) == 0) {
                        uzytkownik = user;
                        stanRozmowy = StanyRozmowy.ZapytanieOHaslo;//ustawienie stanu rozmowy
                        break;//jak znalazlem to koncze szukanie
                    }
                }
                if(stanRozmowy != StanyRozmowy.ZapytanieOHaslo)//ja poprzenida petla nie znalazla usera to znaczy ze go nima
                    stanRozmowy = StanyRozmowy.BlednyLogin;//ustawienie stanu rozmowy
            };

            //dodanie obsługi jak się nie udało rozpoznać usera
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                stanRozmowy = StanyRozmowy.NieRozpoznanyLogin;//ustawienie stanu rozmowy
                Debug.WriteLine("Nie rozpoznano usera " + e.ToString());
            };

            //obsluga jak sterownik nie jest pewny co rozpoznal?
            rozpoznawanieMowy.SRE.SpeechHypothesized += (object sender, SpeechHypothesizedEventArgs e) => {
                Debug.WriteLine("Hipotetycznie: " + e.ToString());
            };

            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//start metody rozpoznawania jednego słowa ze słownika
        }

        private void pobierzHaslo() {
            Powiedz("Witaj " + uzytkownik.imie + " podaj swoje haslo");
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(uzytkownik.haslo);
            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {
                if (string.Compare(uzytkownik.haslo, e.Result.Text) == 0) {
                    Powiedz("Haslo poprawne");
                    stanRozmowy = StanyRozmowy.RozpoznanoHaslo;
                }
                else
                    stanRozmowy = StanyRozmowy.BledneHaslo;
            };

            //dodanie obsługi jak się nie udało rozpoznać
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                stanRozmowy = StanyRozmowy.NieRozpoznaneHaslo;//ustawienie stanu rozmowy
                Debug.WriteLine("Nie rozpoznano usera " + e.ToString());
            };
            
        }

        private void Powiedz(string s) {
            TekstWiadomosci = s;
            synteza.Mow(s);
        }

        // Odświeżanie kontrolek
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(params string[] update) {
            if (PropertyChanged != null)
                foreach (string up in update)
                    PropertyChanged(this, new PropertyChangedEventArgs(up));
        }
    }
}
