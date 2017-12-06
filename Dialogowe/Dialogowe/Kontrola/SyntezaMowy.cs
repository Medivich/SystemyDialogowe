using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dialogowe.Kontrola
{
    class SyntezaMowy
    {
        SpeechSynthesizer synth = new SpeechSynthesizer();

        public SyntezaMowy()
        {
            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
        }

        public void Mow(string text)
        { 
            synth.Speak(text);
        }
    }
}
