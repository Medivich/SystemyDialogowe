using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Speech.Synthesis;

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

        private void Kontroler()
        {

            //synteza.Speak("Witaj w automatycznym sklepie komputerowym Neutron, jak się nazywasz");
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            synth.Speak("artur ty pałko");
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
