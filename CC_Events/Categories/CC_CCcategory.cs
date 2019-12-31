namespace CC_Plugin
{
    internal class CCcategory
    {
        public string name { get; set; }
        public int lineweight { get; set; }
        public byte[] color { get; set; }

        public CCcategory(string n, int lw, byte[] c)
        {
            this.name = n;
            this.lineweight = lw;
            this.color = c;
        }
    }
}