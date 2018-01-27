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
using Dialogowe.VoiceXML;
using System.Collections.ObjectModel;
using Microsoft.Speech.Recognition;
using System.Speech;
using System.Windows.Threading;

namespace Dialogowe {
    class MainViewModel : INotifyPropertyChanged {
        
        private ObservableCollection<Uzytkownik> userzy;
        private DodajDoBazy obiektZapisuDoBazy;


        private StanyRozmowy stanRozmowy = StanyRozmowy.Powitanie;//enumeracja możliwych stanów rozmowy
        #region Referencje do singletonow
        private RozpoznawanieMowy rozpoznawanieMowy = RozpoznawanieMowy.obiekt;
        private SyntezaMowy synteza = SyntezaMowy.obiekt;
        private ParserVXML parserXML = ParserVXML.obiekt;

        #endregion
        private VoiceXML.VoiceXML odpowiedzSystemu;

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


            ObservableCollection<PozycjaZamowienia> lista = new ObservableCollection<PozycjaZamowienia>
            {
                new PozycjaZamowienia
                {
                    liczba = 5,
                    sprzet = new Sprzet
                    {
                        idSprzetu = 10
                    }
                },

                new PozycjaZamowienia
                {
                    liczba = 3,
                    sprzet = new Sprzet
                    {
                        idSprzetu = 13
                    }
                },

                new PozycjaZamowienia
                {
                    liczba = 10,
                    sprzet = new Sprzet
                    {
                        idSprzetu = 2
                    }
                }
            };

            obiektZapisuDoBazy.dodajZamowienie(lista, new CzytajZBazy().pobierzIDKlienta("Artur"));
            obiektZapisuDoBazy.dodajZamowienie(lista, new CzytajZBazy().pobierzIDKlienta("Artur"));

            lista.Add(new PozycjaZamowienia
            {
                liczba = 2,
                sprzet = new Sprzet
                {
                    idSprzetu = 9
                }
            });

            obiektZapisuDoBazy.dodajZamowienie(lista, new CzytajZBazy().pobierzIDKlienta("Artur"));
        }

        private void przeskoczPoczatek()
        {
            userzy = new CzytajZBazy().pobierzUzytkownikow();
            foreach (Uzytkownik user in userzy)
            {
                user.imie = user.imie.Replace(" ", string.Empty);
                user.haslo = user.haslo.Replace(" ", string.Empty);
                rozpoznawanieMowy.dodajSlowa(user.imie);
            }
            uzytkownik = new Uzytkownik();
            uzytkownik.imie = "Artur";
            uzytkownik.haslo = "2";
            uzytkownik.id = new CzytajZBazy().pobierzIDKlienta("Artur");
            stanRozmowy = StanyRozmowy.HistoriaZamowien;
        }

        private void Kontroler() {
            dodajDane();
            przeskoczPoczatek();

            while ((int)stanRozmowy < (int)StanyRozmowy.Pozegnanie) {//rozmowa trwa az wejdziemy w stan zakonczenia jej
                switch (stanRozmowy) {
                    case StanyRozmowy.Powitanie: //witamy usera
                        odpowiedzSystemu = parserXML.parsuj("Powitanie.vxml");
                        Powiedz(odpowiedzSystemu.Prompt);
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

                    #region bledny login haslo
                    case StanyRozmowy.BlednyLogin:
                        pobierzNazweUsera();//w tej metodzie nadpisujemy stan rozmowy jak zwroci ona wynik
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;//to bedzie nadpisane jak metoda (nie)rozpozna
                        break;
                    case StanyRozmowy.BledneHaslo:
                        pobierzHaslo();
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    case StanyRozmowy.NieRozpoznanyLogin:
                        pobierzNazweUsera();//w tej metodzie nadpisujemy stan rozmowy jak zwroci ona wynik
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;//to bedzie nadpisane jak metoda (nie)rozpozna
                        break;
                    case StanyRozmowy.NieRozpoznaneHaslo:
                        pobierzHaslo();
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    #endregion

                    case StanyRozmowy.WyborTrybu:
                        wyborTrybu((int) StanyRozmowy.WyborTrybu);
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    case StanyRozmowy.HistoriaZamowien:
                        odczytHistorii((int)StanyRozmowy.HistoriaZamowien);
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;

                    case StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze://nic nie robimy jak czekamy
                        Thread.Sleep(10);
                        break;
                    default:
                        break;
                }
            }

            if (stanRozmowy == StanyRozmowy.Pozegnanie) {
                odpowiedzSystemu = parserXML.parsuj("Pozegnanie.vxml");
                string nazwa = "";
                try {
                    nazwa = uzytkownik.imie;
                }
                catch { }
                Powiedz(odpowiedzSystemu.Prompt + nazwa);
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() => System.Windows.Application.Current.Shutdown()));
            }
            else if (stanRozmowy == StanyRozmowy.BrakOdpowiedzi) {

            }
            else if (stanRozmowy == StanyRozmowy.PrzerwanoRozmowe) {

            }
        }

        private void pobierzNazweUsera() {
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz" });

            userzy = new CzytajZBazy().pobierzUzytkownikow();
            foreach (Uzytkownik user in userzy) {
                user.imie = user.imie.Replace(" ", string.Empty);
                user.haslo = user.haslo.Replace(" ", string.Empty);
                rozpoznawanieMowy.dodajSlowa(user.imie);
            }

            
            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {
                Debug.WriteLine("Przyszedl event");
                string imie = e.Result.Text;
                if (string.Compare(e.Result.Text, "Wyjdź") == 0) {
                    stanRozmowy = StanyRozmowy.Pozegnanie;
                }
                else if (string.Compare(e.Result.Text, "Powtórz") == 0) {
                    stanRozmowy = StanyRozmowy.ZapytanieOLogin;
                }else {
                    foreach (Uzytkownik user in userzy) {
                        if (string.Compare(user.imie, imie) == 0) {
                            uzytkownik = user;
                            stanRozmowy = StanyRozmowy.ZapytanieOHaslo;//ustawienie stanu rozmowy
                            break;//jak znalazlem to koncze szukanie
                        }
                    }
                    if (stanRozmowy != StanyRozmowy.ZapytanieOHaslo) {//ja poprzenida petla nie znalazla usera to znaczy ze go nima
                        odpowiedzSystemu = parserXML.parsuj("LoginNieOk.vxml");
                        Powiedz(odpowiedzSystemu.Prompt);
                        stanRozmowy = StanyRozmowy.BlednyLogin;//ustawienie stanu rozmowy
                    }
                    else if (stanRozmowy == StanyRozmowy.ZapytanieOHaslo) {
                        odpowiedzSystemu = parserXML.parsuj("LoginOk.vxml");
                        Powiedz(odpowiedzSystemu.Prompt);
                        stanRozmowy = StanyRozmowy.ZapytanieOHaslo;
                    }
                }
            };

            //dodanie obsługi jak się nie udało rozpoznać usera
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                odpowiedzSystemu = parserXML.parsuj("NieRozpoznano.vxml");
                Powiedz(odpowiedzSystemu.Prompt);
                stanRozmowy = StanyRozmowy.NieRozpoznanyLogin;//ustawienie stanu rozmowy
                Debug.WriteLine("Nie rozpoznano usera " + e.ToString());
            };

            odpowiedzSystemu = parserXML.parsuj("ZapytajLogin.vxml");
            Powiedz(odpowiedzSystemu.Prompt);

            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//start metody rozpoznawania jednego słowa ze słownika
        }

        private void pobierzHaslo() {
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[]{ "Wyjdź", "Powtórz"});
            foreach (Uzytkownik user in userzy) {
                rozpoznawanieMowy.dodajSlowa(user.haslo);
            }
            //rozpoznawanieMowy.dodajSlowa(uzytkownik.haslo);//czytaj hasla z bazy danych

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {

                if (string.Compare(e.Result.Text, "Wyjdź") == 0) {
                    stanRozmowy = StanyRozmowy.Pozegnanie;
                }
                else if (string.Compare(e.Result.Text, "Powtórz") == 0) {
                    stanRozmowy = StanyRozmowy.ZapytanieOHaslo;
                }
                else {
                    if (string.Compare(uzytkownik.haslo, e.Result.Text) == 0) {
                        odpowiedzSystemu = parserXML.parsuj("HasloOk.vxml");
                        Powiedz(odpowiedzSystemu.Prompt);
                        stanRozmowy = StanyRozmowy.WyborTrybu;
                    }
                    else {
                        odpowiedzSystemu = parserXML.parsuj("HasloNieOk.vxml");
                        Powiedz(odpowiedzSystemu.Prompt);
                        stanRozmowy = StanyRozmowy.BledneHaslo;

                    }
                }
                    
            };

            //dodanie obsługi jak się nie udało rozpoznać
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                odpowiedzSystemu = parserXML.parsuj("NieRozpoznano.vxml");
                Powiedz(odpowiedzSystemu.Prompt);
                stanRozmowy = StanyRozmowy.NieRozpoznaneHaslo;//ustawienie stanu rozmowy
                Debug.WriteLine("Nie rozpoznano usera " + e.ToString());
            };

            //rozpoznawanieMowy.SRE.

            odpowiedzSystemu = parserXML.parsuj("ZapytajHaslo.vxml");
            Powiedz("Witaj " + uzytkownik.imie + " " + odpowiedzSystemu.Prompt);
            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//rozpoznaj slowo

        }

        private void wyborTrybu(int ja)
        {
            Debug.WriteLine("Wybor trybu");
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz" });

            rozpoznawanieMowy.dodajSlowa("Nowe");
            rozpoznawanieMowy.dodajSlowa("Historia");

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {
                Debug.WriteLine("Wybor trybu " + e.Result.Text);
                if (string.Compare(e.Result.Text, "Wyjdź") == 0)
                {
                    stanRozmowy = StanyRozmowy.Pozegnanie;
                }
                else if (string.Compare(e.Result.Text, "Powtórz") == 0)
                {
                    stanRozmowy = (StanyRozmowy) ja;
                }
                else
                {
                    if (string.Compare("Nowe", e.Result.Text) == 0)
                    {
                        odpowiedzSystemu = parserXML.parsuj("NoweZamowienie.vxml");
                        Powiedz(odpowiedzSystemu.Prompt);
                        stanRozmowy = StanyRozmowy.NoweZamowienie;
                    }
                    else if (string.Compare("Historia", e.Result.Text) == 0)
                    {
                        odpowiedzSystemu = parserXML.parsuj("HistoriaZamowien.vxml");
                        Powiedz(odpowiedzSystemu.Prompt);
                        stanRozmowy = StanyRozmowy.HistoriaZamowien;
                    }
                }
            };


            odpowiedzSystemu = parserXML.parsuj("WyborTrybu.vxml");
            Powiedz(odpowiedzSystemu.Prompt);
            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//rozpoznaj slowo
        }

        private void odczytHistorii(int ja)
        {
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz" });

            int liczbaZamowien = new CzytajZBazy().pobierzLiczbeZamowien(uzytkownik.imie);
            Debug.WriteLine("Widze " + liczbaZamowien + " zamowienia");
            for (int i = 1; i <= liczbaZamowien; i++)
                rozpoznawanieMowy.dodajSlowa(i + ""); //czyta jedno zamiast 1, lel

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {

                Debug.WriteLine("Rozpoznalem " + e.Result.Text);

                if (string.Compare(e.Result.Text, "Wyjdź") == 0)
                {
                    stanRozmowy = StanyRozmowy.Pozegnanie;
                }
                else if (string.Compare(e.Result.Text, "Powtórz") == 0)
                {
                    stanRozmowy = (StanyRozmowy)ja;
                }
                else
                {
                    for (int i = 1; i <= liczbaZamowien; i++)
                        if (string.Compare(i.ToString(), e.Result.Text) == 0)
                        {
                            odpowiedzSystemu = parserXML.parsuj("WyborHistoria.vxml");
                            Powiedz(odpowiedzSystemu.Prompt + i);
                            Powiedz("Skład zamówienia:");
                            ObservableCollection<Zamowienie> lista = new ObservableCollection<Zamowienie>();
                            lista = new CzytajZBazy().pobierzZamowienia(uzytkownik.imie);

                            for (int j = 0; j < lista.Count; j++)
                            {
                                Debug.WriteLine("Czytam liste: " + j);
                                lista[j].czytajListe();
                            }

                            
                            stanRozmowy = StanyRozmowy.WyborTrybu;
                        }
                    }
            };

            Powiedz("Obecnie posiadasz " + new CzytajZBazy().pobierzLiczbeZamowien(uzytkownik.imie) + " Ktore zamowienie cie interesuje");

            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//rozpoznaj slowo
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
