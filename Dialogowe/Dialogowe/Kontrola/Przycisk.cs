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
            this.main = main;
        }

        public void Execute(object parameter)
        {
            try
            {
                main.zatrzymaj();
                main.rozpoznawanieMowy.SRE.RecognizeAsyncStop();
                main.rozpoznawanieMowy.SRE.EmulateRecognizeAsync(main.Fraza);
            }
            catch(Exception e)
            {
                Debug.WriteLine("Jeszce nie");
            }
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
