namespace CC_Library
{
    public class TitleAnalysis
    {
        public string Title { get; }
        public int Section { get; }

        public TitleAnalysis(string s, int i)
        {
            this.Title = s;
            this.Section = i;
        }
        public List<string> SplitTitle()
        {
            List<string> data = new List<string>();
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
                        data.Add(z);
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
                            data.Add(z);
                        }
                        b = i;
                    }
                }
            }
            return data;
        }
        public static List<string> SplitTitle(string s)
        {
            List<string> data = new List<string>();
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
                        data.Add(z);
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
                            data.Add(z);
                        }
                        b = i;
                    }
                }
            }
            return data;
        }
        public static List<PredictionElement> GetData(string folder)
        {
            string[] Files = Directory.GetFiles(folder);
            var data = new List<string>();

            foreach (string f in Files)
            {
                XDocument doc = XDocument.Load(f);
                if (doc.Root.Attribute("Name") != null)
                {
                    string ele = doc.Root.Attribute("Name").Value;
                    if (!string.IsNullOrEmpty(ele))
                    {
                        List<string> title = TitleAnalysis.SplitTitle(ele);
                        foreach (string s in title)
                            if (!data.Contains(s))
                                data.Add(s);
                    }
                }
            }
            if (File.Exists(xfile))
            {
                XDocument doc = XDocument.Load(xfile);
                foreach (XElement ele in doc.Root.Elements())
                {
                    data.Add(ele.Attribute("Value").Value);
                }
            }
            return data;
        }
    }
}
