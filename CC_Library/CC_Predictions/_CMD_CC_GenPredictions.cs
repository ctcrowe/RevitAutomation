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
    Arrayed number of times the word changes another word away from a target
    
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
        private static string FileName = directory + "\\CC_MFData.xml";
        
        public static void Run()
        {
            XDocument xdoc = new XDocument(new XElement("MASTERFORMAT")) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            var pes = GetPEs();
            AdjustPredictions.Run(pes);
            /*
            foreach(PredictionElement pe in GetPEs())
            {
                XElement e = pe.CreateXML();
                xdoc.Root.Add(e);
            }
            xdoc.Save(FileName);
            */
        }
        
        private static List<PredictionElement> GetPEs()
        {
            var files = PredictionPhrase.GetData();
            var pe = new List<PredictionElement>();
            if(files.Count() > 0)
            {
                foreach(var f in files)
                {
                    foreach(var e in f.Elements)
                    {
                        if(pe.Any(x => x.Word == e))
                                pe.Where(x => x.Word == e).First().AddOption(f.Prediction);
                        else   
                        {
                            var o = new PredictionElement(e);
                            o.AddOption(f.Prediction);
                            pe.Add(o);
                        }
                    }
                }
                foreach(var f in files)
                {
                    var eles = pe.Where(x => f.Elements.Contains(x.Word));
                    foreach(var a in eles)
                    {
                        foreach(var o in a.Options)
                        {
                            foreach(var b in eles)
                            {
                                if(a.Word != b.Word)
                                {
                                    if(b.Options.Any(x => x.Name == o.Name))
                                    {
                                        if(b.Options.Where(x => x.Name == o.Name).First().Positive == 0)
                                        {
                                            pe.Where(x => x.Word == b.Word).First().SubtractOption(o.Name);
                                        }
                                    }
                                    else
                                    {
                                        pe.Where(x => x.Word == b.Word).First().SubtractOption(o.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(pe.Count() < 1)
                pe.Add(new PredictionElement("NULL"));
            return pe;
        }
    }
}
