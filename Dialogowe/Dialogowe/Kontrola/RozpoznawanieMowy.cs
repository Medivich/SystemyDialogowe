using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dialogowe.Kontrola
{
    class RozpoznawanieMowy
    {
        #region Singleton
        //Atrybuty dla klasy singleton
        private RozpoznawanieMowy() {
            SRE = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pl-PL"));
            SRE.SetInputToDefaultAudioDevice();
        }
        private static RozpoznawanieMowy instancjaSingleton;
        public static RozpoznawanieMowy obiekt {
            get {
                if (instancjaSingleton == null) {
                    instancjaSingleton = new RozpoznawanieMowy();
                }
                return instancjaSingleton;
            }
        }
        #endregion

        public SpeechRecognitionEngine SRE;
        Choices Slowa = new Choices();

        public void czyscSlownik() {
            Slowa = new Choices();//czysc slownik
            //czysc stara obsluge zdarzen
            SRE.Dispose();
            SRE = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pl-PL"));
            SRE.SetInputToDefaultAudioDevice();
            //SRE.SpeechRecognized += (object sender, SpeechRecognizedEventArgs e) => { };
            //SRE.SpeechRecognitionRejected += (object sender, SpeechRecognitionRejectedEventArgs e) => { };
            //SRE.SpeechHypothesized += (object sender, SpeechHypothesizedEventArgs e) => { };
        }

        public void dodajSlowa(string slowo)
        {
            //dodajn slowo do slownika
            Slowa.Add(slowo);
        }

        public void dodajSlowa(string[] slowa)
        {
            //dodajn slowa do slownika
            Slowa.Add(slowa);
        }

        public void rozpoznajSlowaZeSlownika()
        {
            GrammarBuilder gramBuild = new GrammarBuilder();
            gramBuild.Append(Slowa);
            Grammar gramSRE = new Grammar(gramBuild);

            SRE.LoadGrammar(gramSRE);
            SRE.RecognizeAsync(RecognizeMode.Multiple);//podobno ta metoda rozpoznaje wiele slow i zwraca kazde oddzielnie, czyli wola wiele eventow recogninzed
        }

        public void rozpoznajSlowoZeSlownika() {
            GrammarBuilder gramBuild = new GrammarBuilder();
            gramBuild.Append(Slowa);
            Grammar gramSRE = new Grammar(gramBuild);

            SRE.LoadGrammar(gramSRE);
            SRE.RecognizeAsync(RecognizeMode.Single);//podobno ta metoda rozpoznaje jedno slowo
        }
    }
}
