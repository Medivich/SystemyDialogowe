using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dialogowe.VoiceXML {
    class VoiceXML {

        //private string _formId;
        //private string _prompt;
        //private string _gramatyka;
        
        public string FormId {
            get; set;
        }
        public string Prompt {
            get;set;
        }
        public string Gramatyka {
            get; set;
        }

        public VoiceXML(string FormId, string Prompt, string Gramatyka) {
            this.FormId = FormId;
            this.Prompt = Prompt;
            this.Gramatyka = Gramatyka;
        }
    }
}
