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
using System.Windows;
using System.Windows.Input;

namespace Dialogowe {
    class MainViewModel : INotifyPropertyChanged {


        /*
         * Problemy
         * odczytHistorii - czyta jedno zamiast jeden itd. (jak i oczekuje), ale dwa to dwa xD 
         * 
         * 
         */



        private ObservableCollection<Uzytkownik> userzy;
        private DodajDoBazy obiektZapisuDoBazy;


        public StanyRozmowy stanRozmowy = StanyRozmowy.Powitanie;//enumeracja możliwych stanów rozmowy

        #region Referencje do singletonow
        public RozpoznawanieMowy rozpoznawanieMowy = RozpoznawanieMowy.obiekt;
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

        public PozycjaZamowienia wybranySprzet = null;

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

            obiektZapisuDoBazy.dodajProcesor(1, "Intel", 1000, 2, new DodawanieZdjec().dodajZdjecie("i3.png"));
            obiektZapisuDoBazy.dodajProcesor(5, "AMD", 1000, 5, new DodawanieZdjec().dodajZdjecie("AMD1.png"));
            obiektZapisuDoBazy.dodajProcesor(6, "AMD", 1300, 5, new DodawanieZdjec().dodajZdjecie("AMD2.png"));
            obiektZapisuDoBazy.dodajProcesor(6, "Intel", 3000, 2, new DodawanieZdjec().dodajZdjecie("i5.png"));
            obiektZapisuDoBazy.dodajProcesor(8, "AMD", 2500, 5, new DodawanieZdjec().dodajZdjecie("AMD2.png"));
            obiektZapisuDoBazy.dodajProcesor(8, "Intel", 4000, 9, new DodawanieZdjec().dodajZdjecie("i7.png"));
            obiektZapisuDoBazy.dodajProcesor(2, "Zhaoxin", 850, 4, new DodawanieZdjec().dodajZdjecie("chin.png"));

            obiektZapisuDoBazy.dodajDysk(500, 300, 5, new DodawanieZdjec().dodajZdjecie("dysk1.png"));
            obiektZapisuDoBazy.dodajDysk(1500, 700, 7, new DodawanieZdjec().dodajZdjecie("dysk2.png"));
            obiektZapisuDoBazy.dodajDysk(600, 350, 5, new DodawanieZdjec().dodajZdjecie("dysk3.png"));
            obiektZapisuDoBazy.dodajDysk(700, 500, 2, new DodawanieZdjec().dodajZdjecie("dysk4.png"));
            obiektZapisuDoBazy.dodajDysk(200, 50, 3, new DodawanieZdjec().dodajZdjecie("dysk4.png"));

            obiektZapisuDoBazy.dodajRAM(1.2F, 4, 200, 5, new DodawanieZdjec().dodajZdjecie("ram1.png"));
            obiektZapisuDoBazy.dodajRAM(1.066F, 2, 50, 5, new DodawanieZdjec().dodajZdjecie("ram1.png"));
            obiektZapisuDoBazy.dodajRAM(2.0F, 8, 300, 2, new DodawanieZdjec().dodajZdjecie("ram2.png"));
            obiektZapisuDoBazy.dodajRAM(2.0F, 4, 500, 3, new DodawanieZdjec().dodajZdjecie("ram3.png"));
            obiektZapisuDoBazy.dodajRAM(3.0F, 2, 900, 7, new DodawanieZdjec().dodajZdjecie("ram3.png"));

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



        //Jakbyśmy chcieli zacząć od danego miejsca
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
            stanRozmowy = StanyRozmowy.WyborTrybu;
        }

        private void Kontroler() {
            dodajDane();
           // przeskoczPoczatek();

            while ((int)stanRozmowy < (int)StanyRozmowy.Pozegnanie) {//rozmowa trwa az wejdziemy w stan zakonczenia jej
                switch (stanRozmowy) {
                    case StanyRozmowy.Powitanie: //witamy usera
                        odpowiedzSystemu = parserXML.parsuj("Powitanie.vxml");
                        Powiedz(odpowiedzSystemu.Prompt);
                        stanRozmowy = StanyRozmowy.ZapytanieOLogin_Haslo;
                        break;
                    #region logowanie
                    case StanyRozmowy.ZapytanieOLogin_Haslo://zapytanie o login
                        PobierzWszystko();//w tej metodzie nadpisujemy stan rozmowy jak zwroci ona wynik
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;//to bedzie nadpisane jak metoda (nie)rozpozna
                        break;
                    case StanyRozmowy.ZapytanieOLogin://zapytanie o login
                        PobierzNazweUsera();//w tej metodzie nadpisujemy stan rozmowy jak zwroci ona wynik
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;//to bedzie nadpisane jak metoda (nie)rozpozna
                        break;
                    case StanyRozmowy.ZapytanieOHaslo://pytamy o haslo
                        PobierzHaslo();
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    #endregion
                    #region bledny login haslo
                    case StanyRozmowy.BlednyLogin:
                        PobierzNazweUsera();//w tej metodzie nadpisujemy stan rozmowy jak zwroci ona wynik
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;//to bedzie nadpisane jak metoda (nie)rozpozna
                        break;
                    case StanyRozmowy.BledneHaslo:
                        PobierzHaslo();
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    case StanyRozmowy.NieRozpoznanyLogin:
                        PobierzNazweUsera();//w tej metodzie nadpisujemy stan rozmowy jak zwroci ona wynik
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;//to bedzie nadpisane jak metoda (nie)rozpozna
                        break;
                    case StanyRozmowy.NieRozpoznaneHaslo:
                        PobierzHaslo();
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    #endregion

                    case StanyRozmowy.WyborTrybu:
                        Debug.WriteLine("STAN - WYBÓR TRYBU");
                        WyborTrybu((int) StanyRozmowy.WyborTrybu);
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    case StanyRozmowy.HistoriaZamowien:
                        OdczytHistorii((int)StanyRozmowy.HistoriaZamowien);
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    #region zamawianie
                    case StanyRozmowy.NoweZamowienie:
                        ZamawiamKoszyk((int)StanyRozmowy.NoweZamowienie);
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    case StanyRozmowy.WyborProcesora:
                        KupnoProcesora((int)StanyRozmowy.WyborProcesora);
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    case StanyRozmowy.WyborRAMu:
                        KupnoPamieciRam((int)StanyRozmowy.WyborRAMu);
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    case StanyRozmowy.WyborDysku:
                        KupnoDyskuTwardego((int)StanyRozmowy.WyborDysku);
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    case StanyRozmowy.LiczbaSztuk:
                        WyborLiczbySztuk((int)StanyRozmowy.LiczbaSztuk);
                        stanRozmowy = StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze;
                        break;
                    #endregion



                    case StanyRozmowy.OczekiwanieNaRozpoznanieLubSynteze://nic nie robimy jak czekamy
                        Thread.Sleep(100);
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

        #region logowanie

        private bool login = false;
        private bool haslo = false;
        private string hasloText = null;
        private string loginText = null;
        private int odczyt = 0;

        private void PobierzWszystko()
        {
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz", "Login", "Hasło" });

            userzy = new CzytajZBazy().pobierzUzytkownikow();
            foreach (Uzytkownik user in userzy)
            {
                user.imie = user.imie.Replace(" ", string.Empty);
                user.haslo = user.haslo.Replace(" ", string.Empty);
                rozpoznawanieMowy.dodajSlowa(user.imie);
                rozpoznawanieMowy.dodajSlowa(user.haslo);
            }

       


            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {

                string imie = e.Result.Text;
                if (string.Compare(e.Result.Text, "Wyjdź") == 0)
                {
                    stanRozmowy = StanyRozmowy.Pozegnanie;
                }
                else if (string.Compare(e.Result.Text, "Powtórz") == 0)
                {
                    stanRozmowy = StanyRozmowy.ZapytanieOLogin_Haslo;
                }
                else
                {
                    Debug.WriteLine(e.Result.Text);

                    if (string.Compare(e.Result.Text, "Login") == 0)
                        login = true;
                    else if (string.Compare(e.Result.Text, "Hasło") == 0)
                        haslo = true;
                    else if (login)
                        foreach (Uzytkownik user in userzy)
                        {
                            if (string.Compare(user.imie, imie) == 0)
                            {
                                uzytkownik = user;
                                loginText = imie;
                            }
                            login = false;
                        }
                    else if(haslo)
                    {
                        foreach (Uzytkownik user in userzy)
                        {
                            if (string.Compare(user.haslo, imie) == 0)
                            {
                                uzytkownik = user;
                                hasloText = imie;
                            }
                            haslo = false;
                        } 
                    }

                    if(hasloText != null && loginText != null)
                    {
                        if (string.Compare(uzytkownik.imie, loginText) == 0 && string.Compare(uzytkownik.haslo, hasloText) == 0)
                        {
                            Powiedz(odpowiedzSystemu.Prompt);
                            stanRozmowy = StanyRozmowy.WyborTrybu;
                        }
                        else
                        {
                            odpowiedzSystemu = parserXML.parsuj("BladInit.vxml");
                            Powiedz(odpowiedzSystemu.Prompt);
                            stanRozmowy = StanyRozmowy.ZapytanieOLogin;//ustawienie stanu rozmowy
                        }
                    }
                }
            };

            //dodanie obsługi jak się nie udało rozpoznać usera
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                odpowiedzSystemu = parserXML.parsuj("NieRozpoznano.vxml");
                Powiedz(odpowiedzSystemu.Prompt);
                stanRozmowy = StanyRozmowy.ZapytanieOLogin;//ustawienie stanu rozmowy
            };

            odpowiedzSystemu = parserXML.parsuj("Login_Haslo.vxml");
            Powiedz(odpowiedzSystemu.Prompt);

            rozpoznawanieMowy.rozpoznajSlowaZeSlownika();//start metody rozpoznawania jednego słowa ze słownika
        }

        private void PobierzNazweUsera() {
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

        private void PobierzHaslo() {
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

        #endregion

        private void WyborTrybu(int ja)
        {
            ListaZamowien = new ObservableCollection<PozycjaZamowienia>();
            ListaSprzetu = new ObservableCollection<Sprzet>();
            Debug.WriteLine("Wybor trybu");
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz" });

            rozpoznawanieMowy.dodajSlowa("Nowe");
            rozpoznawanieMowy.dodajSlowa("Historia");

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {
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

            //dodanie obsługi jak się nie udało rozpoznać usera
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                odpowiedzSystemu = parserXML.parsuj("NieRozpoznano.vxml");
                Powiedz(odpowiedzSystemu.Prompt);
                stanRozmowy = (StanyRozmowy) ja;
            };

            odpowiedzSystemu = parserXML.parsuj("WyborTrybu.vxml");
            Powiedz(odpowiedzSystemu.Prompt);
            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//rozpoznaj slowo
        }

        private void OdczytHistorii(int ja)
        {
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz" });

            int liczbaZamowien = new CzytajZBazy().pobierzLiczbeZamowien(uzytkownik.imie);

            for (int i = 1; i <= liczbaZamowien; i++)
                rozpoznawanieMowy.dodajSlowa(i + ""); //czyta jedno zamiast 1, lel

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {

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
                            Powiedz(odpowiedzSystemu.Prompt + " " + i);
                            Powiedz("Skład zamówienia:");
                            ObservableCollection<Zamowienie> lista = new ObservableCollection<Zamowienie>();
                            lista = new CzytajZBazy().pobierzZamowienia(uzytkownik.imie);
                            ObservableCollection<PozycjaZamowienia> pom = new ObservableCollection<PozycjaZamowienia>();

                            foreach (PozycjaZamowienia poz in lista[i - 1].lista)
                                pom.Add(poz);

                            ListaZamowien = pom;
                            Historia = Visibility.Visible;

                            foreach (PozycjaZamowienia poz in lista[i - 1].lista)
                                Powiedz("Zamówiłeś: " + poz.typ + " o parametrach " + poz.sprzet.text);

                            Historia = Visibility.Hidden;
                            stanRozmowy = StanyRozmowy.WyborTrybu;
                        }
                    }
            };

            //dodanie obsługi jak się nie udało rozpoznać usera
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                odpowiedzSystemu = parserXML.parsuj("NieRozpoznano.vxml");
                Powiedz(odpowiedzSystemu.Prompt);
                stanRozmowy = (StanyRozmowy)ja;
            };

            Powiedz("Obecnie posiadasz " + new CzytajZBazy().pobierzLiczbeZamowien(uzytkownik.imie) + " zamówienia. " +
                "Które cię interesuje");

            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//rozpoznaj slowo
        }

        #region kupowanie

        private void ZamawiamKoszyk(int ja)
        {
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz" });
            rozpoznawanieMowy.dodajSlowa(new string[] { "Procesor", "Dysk Twardy", "Pamięć RAM", "Koniec" });
            Historia = Visibility.Visible;
            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {

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
                    if (string.Compare("Koniec", e.Result.Text) == 0)
                    {
                        Powiedz("Dziękujemy, towary niedługo zostaną wydane");
                        new DodajDoBazy().dodajZamowienie(ListaZamowien, new CzytajZBazy().pobierzIDKlienta(uzytkownik.imie));
                        new AktualizacjaBazyDanych().aktualizujDane(ListaZamowien);
                        Historia = Visibility.Hidden;
                        stanRozmowy = StanyRozmowy.WyborTrybu;
                    }
                    else if (string.Compare(e.Result.Text, "Procesor") == 0)
                    {
                        Powiedz("Wybrales kupno procesora");
                        stanRozmowy = StanyRozmowy.WyborProcesora;
                    }
                    else if(string.Compare(e.Result.Text, "Dysk Twardy") == 0)
                    {
                        Powiedz("Wybrales kupno dysku twardego");
                        stanRozmowy = StanyRozmowy.WyborDysku;
                    }
                    else if (string.Compare(e.Result.Text, "Pamięć RAM") == 0)
                    {
                        Powiedz("Wybrales kupno pamieci ram");
                        stanRozmowy = StanyRozmowy.WyborRAMu;
                    }
                }
            };

            //dodanie obsługi jak się nie udało rozpoznać usera
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                odpowiedzSystemu = parserXML.parsuj("NieRozpoznano.vxml");
                Powiedz(odpowiedzSystemu.Prompt);
                stanRozmowy = (StanyRozmowy)ja;
            };

            Powiedz("Posiadasz w koszyku " + listaZamowien.Count + " zamówień. Chciałbyś zamowić dysk twardy," +
                " procesor czy Pamięć ram? Jeśli byś chciał zakończyć kupowanie powiedz koniec");

            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//rozpoznaj slowo
        }

        private void KupnoProcesora(int ja)
        {
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz" });

            ListaSprzetu = new CzytajZBazy().pobierzProcesory();

            for (int i = 0; i < ListaSprzetu.Count; i++)
            {
                ListaSprzetu[i].text = new CzytajZBazy().pobierzProcesor(ListaSprzetu[i].idSprzetu).PobierzOpis();
            }

            SprzetVisible = Visibility.Visible;

            for (int i = 1; i <= ListaSprzetu.Count; i++)
                rozpoznawanieMowy.dodajSlowa(i + ""); //czyta jedno zamiast 1, lel

            for (int i = 1; i <= ListaSprzetu.Count; i++)
                ListaSprzetu[i - 1].numerNaLiscie = i;

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {

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
                    for (int i = 1; i <= ListaSprzetu.Count; i++)
                        if (string.Compare(i.ToString(), e.Result.Text) == 0)
                        {
                            Powiedz("Wybrałeś procesor numer " + i);
                            //Dispatcher
                            PozycjaZamowienia poz = new PozycjaZamowienia
                            {
                                sprzet = ListaSprzetu[i - 1],
                                liczba = 1
                            };

                            wybranySprzet = poz;
                            SprzetVisible = Visibility.Hidden;
                            

                            stanRozmowy = StanyRozmowy.LiczbaSztuk;
                        }
                }
            };

            //dodanie obsługi jak się nie udało rozpoznać usera
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                odpowiedzSystemu = parserXML.parsuj("NieRozpoznano.vxml");
                Powiedz(odpowiedzSystemu.Prompt);
                stanRozmowy = (StanyRozmowy)ja;
            };

            Powiedz("Który procesor cię interesuje, podaj numer");

            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//rozpoznaj slowo
        }

        private void KupnoPamieciRam(int ja)
        {
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz" });

            ListaSprzetu = new CzytajZBazy().pobierzPamieciRam();

            for (int i = 0; i < ListaSprzetu.Count; i++)
            {
                ListaSprzetu[i].text = new CzytajZBazy().pobierzPamiecRAM(ListaSprzetu[i].idSprzetu).PobierzOpis();
            }

            SprzetVisible = Visibility.Visible;

            for (int i = 1; i <= ListaSprzetu.Count; i++)
                rozpoznawanieMowy.dodajSlowa(i + ""); //czyta jedno zamiast 1, lel

            for (int i = 1; i <= ListaSprzetu.Count; i++)
                ListaSprzetu[i - 1].numerNaLiscie = i;

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {
                
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
                    for (int i = 1; i <= ListaSprzetu.Count; i++)
                        if (string.Compare(i.ToString(), e.Result.Text) == 0)
                        {
                            Powiedz("Wybrałeś pamięć ram numer " + i);
                            //Dispatcher
                            PozycjaZamowienia poz = new PozycjaZamowienia
                            {
                                sprzet = ListaSprzetu[i - 1],
                                liczba = 1
                            };

                            wybranySprzet = poz;
                            SprzetVisible = Visibility.Hidden;


                            stanRozmowy = StanyRozmowy.LiczbaSztuk;
                        }
                }
            };

            //dodanie obsługi jak się nie udało rozpoznać usera
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                odpowiedzSystemu = parserXML.parsuj("NieRozpoznano.vxml");
                Powiedz(odpowiedzSystemu.Prompt);
                stanRozmowy = (StanyRozmowy)ja;
            };

            Powiedz("Która pamięć ram cię interesuje, podaj numer");

            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//rozpoznaj slowo
        }

        private void KupnoDyskuTwardego(int ja)
        {
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz" });

            ListaSprzetu = new CzytajZBazy().pobierzDyskiTwarde();

            for (int i = 0; i < ListaSprzetu.Count; i++)
            {
                ListaSprzetu[i].text = new CzytajZBazy().pobierzDyskTwardy(ListaSprzetu[i].idSprzetu).PobierzOpis();
            }

            SprzetVisible = Visibility.Visible;

            for (int i = 1; i <= ListaSprzetu.Count; i++)
                rozpoznawanieMowy.dodajSlowa(i + ""); //czyta jedno zamiast 1, lel

            for (int i = 1; i <= ListaSprzetu.Count; i++)
                ListaSprzetu[i - 1].numerNaLiscie = i;

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {

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
                    for (int i = 1; i <= ListaSprzetu.Count; i++)
                        if (string.Compare(i.ToString(), e.Result.Text) == 0)
                        {
                            Powiedz("Wybrałeś dysk numer " + i);
                            //Dispatcher
                            PozycjaZamowienia poz = new PozycjaZamowienia
                            {
                                sprzet = ListaSprzetu[i - 1],
                                liczba = 1
                            };

                            wybranySprzet = poz;
                            SprzetVisible = Visibility.Hidden;


                            stanRozmowy = StanyRozmowy.LiczbaSztuk;
                        }
                }
            };

            //dodanie obsługi jak się nie udało rozpoznać usera
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                odpowiedzSystemu = parserXML.parsuj("NieRozpoznano.vxml");
                Powiedz(odpowiedzSystemu.Prompt);
                stanRozmowy = (StanyRozmowy)ja;
            };

            Powiedz("Który dysk cię interesuje, podaj numer");

            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//rozpoznaj slowo
        }

        private void WyborLiczbySztuk(int ja)
        {
            rozpoznawanieMowy.czyscSlownik();//przed rozpoznawaniem czyscimy slownik
            rozpoznawanieMowy.dodajSlowa(new string[] { "Wyjdź", "Powtórz" });

            for (int i = 1; i <= new CzytajZBazy().pobierzSprzet(wybranySprzet.sprzet.idSprzetu).iloscSztuk; i++)
                rozpoznawanieMowy.dodajSlowa(i + ""); //czyta jedno zamiast 1, lel

            //dodanie obsługi udanego rozpoznania
            rozpoznawanieMowy.SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => {

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
                    for (int i = 1; i <= ListaSprzetu.Count; i++)
                        if (string.Compare(i.ToString(), e.Result.Text) == 0)
                        {
                            wybranySprzet.sprzet.iloscSztuk = i;
                            wybranySprzet.liczba = i;
                            Application.Current.Dispatcher.Invoke(new Action(() => ListaZamowien.Add(wybranySprzet)));
                            Application.Current.Dispatcher.Invoke(new Action(() => stanRozmowy = StanyRozmowy.NoweZamowienie));
                        }
                }
            };

            //dodanie obsługi jak się nie udało rozpoznać usera
            rozpoznawanieMowy.SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => {
                odpowiedzSystemu = parserXML.parsuj("NieRozpoznano.vxml");
                Powiedz(odpowiedzSystemu.Prompt);
                stanRozmowy = (StanyRozmowy)ja;
            };

            Powiedz("Ile chciałbyś sztuk sprzętu? Podaj liczbę z zakresu 1 - " + new CzytajZBazy().pobierzSprzet(wybranySprzet.sprzet.idSprzetu).iloscSztuk);

            rozpoznawanieMowy.rozpoznajSlowoZeSlownika();//rozpoznaj slowo
        }

        #endregion

        private void Powiedz(string s) {
            TekstWiadomosci = s;
            synteza.Mow(s);
        }

        public void zatrzymaj()
        {
            synteza.zatrzymaj();
        }

        // Odświeżanie kontrolek
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(params string[] update) {
            if (PropertyChanged != null)
                foreach (string up in update)
                    PropertyChanged(this, new PropertyChangedEventArgs(up));
        }


        public Visibility historia = Visibility.Hidden;

        public Visibility Historia
        {
            get
            {
                return this.historia;
            }
            set
            {
                this.historia = value;
                OnPropertyChanged("Historia");
            }
        }

        public Visibility sprzetVisible = Visibility.Hidden;

        public Visibility SprzetVisible
        {
            get
            {
                return this.sprzetVisible;
            }
            set
            {
                this.sprzetVisible = value;
                OnPropertyChanged("SprzetVisible");
            }
        }

        ObservableCollection<Sprzet> listaSprzetu = new ObservableCollection<Sprzet>();

        public ObservableCollection<Sprzet> ListaSprzetu
        {
            get
            {
                return this.listaSprzetu;
            }
            set
            {
                this.listaSprzetu = value;
                OnPropertyChanged("ListaSprzetu");
            }
        }

        public ObservableCollection<PozycjaZamowienia> listaZamowien = new ObservableCollection<PozycjaZamowienia>();

        public ObservableCollection<PozycjaZamowienia> ListaZamowien
        {
            get
            {
                return this.listaZamowien;
            }
            set
            {
                this.listaZamowien = value;
                OnPropertyChanged("ListaZamowien");
            }
        }

        private ICommand Przycisk;

        public ICommand Prz
        {
            get
            {
                if (Przycisk == null)
                    Przycisk = new Przycisk(this);
                return Przycisk;
            }
        }

        public string fraza = "Wpisz frazę";

        public string Fraza
        {
            get
            {
                return this.fraza;
            }
            set
            {
                this.fraza = value;
                OnPropertyChanged("Fraza");
            }
        }
    }
}
