using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Dialogowe.VoiceXML {
    public class ParserVXML {
        #region Singleton
        //Atrybuty dla klasy singleton
        private ParserVXML() { }
        private static ParserVXML instancjaSingleton;
        public static ParserVXML obiekt {
            get {
                if (instancjaSingleton == null)
                    instancjaSingleton = new ParserVXML();
                return instancjaSingleton;
            }
        }
        #endregion

        private XmlDocument doc;
        

        public VoiceXML parsuj(string nazwaPliku) {
            doc = new XmlDocument();
            string prompt="", grammar = "", id = "";
            string path = "../../VoiceXML/"+nazwaPliku;//Path.Combine(Environment.CurrentDirectory, @"VoiceXML\Powitanie.vxml");
            doc.Load(path);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
                if (node.Name.Equals("form")) {
                    foreach(XmlAttribute attr in node.Attributes) {
                        if (attr.Name.Equals("id"))
                            id = attr.InnerText;
                    }
                    foreach (XmlNode innerNode in node) {
                        if (innerNode.Name.Equals("prompt")) {
                            prompt = innerNode.InnerText;
                        }
                        else if (innerNode.Name.Equals("grammar")) {
                            grammar = innerNode.InnerText;
                        }
                    }
                }
            }
            return new VoiceXML(id, prompt, grammar);
        }
    }
}
