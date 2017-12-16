using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
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
            synth.Speak(text);
        }
    }
}
