using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dialogowe.Model {
    enum StanyRozmowy {
        OczekiwanieNaRozpoznanieLubSynteze = 0,
        Powitanie,
        ZapytanieOLogin,//pytamy usera o podanie loginu - imienia
        ZapytanieOHaslo,//pytamy usera o haslo
        BlednyLogin,//ponawiamy pytanie o login
        BledneHaslo,//ponawiamy pytanie o haslo
        NieRozpoznanyLogin,//sterownik rozpoznal login ale nie ma go w slowniku, ponawiamy pytanie o login
        NieRozpoznaneHaslo,//sterownik rozpoznal haslo ale nie ma go w slowniku, ponawiamy pytanie o haslo
        RozpoznanoHaslo,



        Pozegnanie = 99,
        PrzerwanoRozmowe = 100,
        BrakOdpowiedzi = 101//mozliwe ze ten stan nie konczy rozmowy, do zmiany kiedys
    }
}
