namespace CC_Library
{
    public class PredictionElement
    {
        public string Word { get; }
        public int[] Predictions { get; set; }

        public PredictionElement(string s)
        {
            this.Word = s;
            this.Predictions = new int[26];
        }
        
        public static List<PredictionElement> GetData(string folder)
        {
            string[] Files = Directory.GetFiles(folder);
            var data = new List<string>();

            if (File.Exists(xfile))
            {
                XDocument doc = XDocument.Load(xfile);
                foreach (XElement ele in doc.Root.Elements())
                {
                    data.Add(new PreddictionElement(ele.Attribute("Value").Value));
                }
            }
            foreach (string f in Files)
            {
                XDocument doc = XDocument.Load(f);
                if (doc.Root.Attribute("Name") != null)
                {
                    string ele = doc.Root.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(ele))
                    {
                        List<string> title = SplitTitle(ele);
                        foreach (string s in title)
                            if (!data.Any(x => x.Word == s))
                                data.Add(new PredictionElement(s));
                    }
                }
            }
            return data;
        }
        
        public List<PredictionElement> SplitTitle()
        {
            var data = new List<PredictionElement>();
            int b = 0;
            char[] cs = this.Title.ToCharArray();
            for (int i = 1; i < cs.Count(); i++)
            {
                if (!char.IsLetter(cs[i]))
                {
                    if (i > b && b < cs.Count())
                    {
                        string z = string.Empty;
                        for (int j = b; j < i; j++)
                        {
                            z += cs[j];
                        }
                        data.Add(new PredictionElement(z));
                    }
                    b = i + 1;
                }
                else
                {
                    if (char.IsUpper(cs[i]) && !char.IsUpper(cs[i - 1]))
                    {
                        if (i > b && b < cs.Count())
                        {
                            string z = string.Empty;
                            for (int j = b; j < i; j++)
                            {
                                z += cs[j];
                            }
                            data.Add(new PredictionElement(z));
                        }
                        b = i;
                    }
                }
            }
            return data;
        }
        public static List<PredictionElement> SplitTitle(string s)
        {
            var data = new List<PredictionElement>();
            int b = 0;
            char[] cs = s.ToCharArray();
            for (int i = 1; i < cs.Count(); i++)
            {
                if (!char.IsLetter(cs[i]))
                {
                    if (i > b && b < cs.Count())
                    {
                        string z = string.Empty;
                        for (int j = b; j < i; j++)
                        {
                            z += cs[j];
                        }
                        data.Add(new PredictionElement(z));
                    }
                    b = i + 1;
                }
                else
                {
                    if (char.IsUpper(cs[i]) && !char.IsUpper(cs[i - 1]))
                    {
                        if (i > b && b < cs.Count())
                        {
                            string z = string.Empty;
                            for (int j = b; j < i; j++)
                            {
                                z += cs[j];
                            }
                            data.Add(new PredictionElement(z));
                        }
                        b = i;
                    }
                }
            }
            return data;
        }
    }
}
