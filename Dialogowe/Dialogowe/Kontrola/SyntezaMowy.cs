using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dialogowe.Kontrola
{
    class SyntezaMowy
    {
        #region Singleton
        //Atrybuty dla klasy singleton
        private SyntezaMowy() {
            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
        }
        private static SyntezaMowy instancjaSingleton;
        public static SyntezaMowy obiekt {
            get {
                if (instancjaSingleton == null) {
                    instancjaSingleton = new SyntezaMowy();
                }
                return instancjaSingleton;
            }
        }
        #endregion

        SpeechSynthesizer synth;
        public void Mow(string text)
        {
            try
            {
                synth.Speak(text);
            }
            catch(Exception e)
            {
                Debug.WriteLine("Koniec gadania");
            }
        }

        public void zatrzymaj()
        {
            synth.SpeakAsyncCancelAll();
        }
    }
}
