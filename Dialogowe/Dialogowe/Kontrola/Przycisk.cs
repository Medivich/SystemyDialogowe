using Dialogowe.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Dialogowe.Kontrola
{
    class Przycisk : ICommand
    {
        private readonly MainViewModel main;

        public Przycisk(MainViewModel main)
        {
            this.main = main ?? throw new ArgumentNullException("Login Command");
        }

        public void Execute(object parameter)
        {
            // main.rozpoznawanieMowy.SRE.SpeechRecognized += SRE_SpeechRecognized;
            main.stanRozmowy = StanyRozmowy.WyborTrybu;
            /////////////////////////////////////////////TUTAJ MOŻNA WYWOŁAĆ EVENT
            Debug.WriteLine(main.Fraza);
        }

        private void SRE_SpeechRecognized(object sender, Microsoft.Speech.Recognition.SpeechRecognizedEventArgs e)
        {
            ;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
