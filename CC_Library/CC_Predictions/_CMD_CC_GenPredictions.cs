using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

/*
1) Collect an ordered dictionary of words
2) Add an array of words that is compared to each word (can be an array of numbers)
3) Attach an array of outputs to each word
4) For each output, add negative inputs
    4a) foreach phrase, look at where outputs are positive in other options.
    4b) foreach positive output, reduce the corresponding word pair when the output isnt the positive output.
FINAL OUTPUT
Word
    Number of times the word shows up
    Arrayed number of times the word shows up for each possible output (the words positive connotation)
    Arrayed number of times the word is subtracted when added to other words (the words negative connotation)
    
    internal class PredictionOption
        public string Name { get; }
        public int Positive { get; set; }
        public int Negative { get; set; }
*/

namespace CC_Library
{
    public class GenPredictions
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string Dataset = directory + "\\CC_XMLData";
        private static string FileName = directory + "\\CC_MFData.xml";
        
        public static void Run()
        {
            if(Directory.Exists(Dataset))
            {
                List<PredictionOption> po = new List<PredictionOption>();
                List<PredictionElement> pe = new List<PredictionElement>();
                List<PredictionPhrase> pp = new List<PredictionPhrase>();
                XDocument xfinsihed = new XDocument(new XElement("MASTERFORMAT")) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
                
                string[] files = Directory.GetFiles(folder);
                foreach(string f in files)
                {
                    XDocument doc = XDocument.Load(f);
                    if(doc.Root.Attribute("MFSection") != null)
                    {
                        PredictionPhrase phrase = new PredictionPhrase(doc.Root.Attribute("Name").Value, doc.Root.Attribute("MFSection").Value);
                        pp.Add(phrase);
                        List<string> Elements = SplitTitle.Run(phrase.Phrase);
                        foreach(string e in Elements)
                        {
                            if(pe.Any(x => x.Word == e))
                            {
                                pe.Where(x => x.Word == e).First().AddOption(phrase.Prediction);
                            }
                            else   
                            {
                                var o = new PredictionElement(e);
                                o.AddOption(phrase.Prediction);
                                pe.Add(o);
                            }
                        }
                    }
                }
                foreach(var p in pp)
                {
                    List<string> Elements = SplitTitle.Run(phrase.Phrase);
                    foreach(string e in Elements)
                    {
                        if(pe.Any(x => x.Word == e))
                        {
                            PredictionElement ele = pe.Where(x => x.Word == e).First();
                            
                        }
                        else
                        {
                        }
                    }
                }
            }
        }
    }
}
