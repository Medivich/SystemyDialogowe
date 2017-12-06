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
        SpeechRecognitionEngine SRE;        Choices Slowa = new Choices();        bool rozpoznano = false;        string rozpoznaneSlowo = null;        public RozpoznawanieMowy()
        {
            SRE = new SpeechRecognitionEngine(
                new System.Globalization.CultureInfo("pl-PL"));
            SRE.SetInputToDefaultAudioDevice();
        }        public void dodajSlowa(string slowo)
        {
            Slowa.Add(slowo);
        }

        public void dodajSlowa(string[] slowo)
        {
            Slowa.Add(slowo);
        }        public string rozpoznajZSlownika()
        {
            GrammarBuilder gramBuild = new GrammarBuilder();
            gramBuild.Append(Slowa);
            Grammar gramSRE = new Grammar(gramBuild);
            SRE.LoadGrammar(gramSRE);
            SRE.SpeechRecognized += new EventHandler
                <SpeechRecognizedEventArgs>(SRE_SpeechRecognized);
            SRE.RecognizeAsync(RecognizeMode.Multiple);

            while(!rozpoznano)
                Thread.Sleep(5);

            return rozpoznaneSlowo;
        }


        private void SRE_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            rozpoznaneSlowo = e.Result.Text;
            rozpoznano = true;
        }
    }
}
