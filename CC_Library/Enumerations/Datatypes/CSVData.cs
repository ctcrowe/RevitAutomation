using System;
using System.IO;
using System.Collections.Generic;

namespace CC_Library.Datatypes
{
    public class CSVData
    {
        public string ID { get; }
        public string Time { get; }
        public Datatype datatype { get; }
        public string Name { get; }
        public string Data { get; }

        public CSVData(Datatype dt, string n, string s)
        {
            this.ID = Guid.NewGuid().ToString();
            this.datatype = dt;
            this.Name = n;
            this.Data = s;
            this.Time = DateTime.Now.ToString("yyyyMMddhhmmss");
        }

        public CSVData(string fn)
        {
            string[] lines = File.ReadAllLines(fn);
            this.ID = fn;
            this.Time = lines[0];
            Datatype dt = (Datatype)Enum.Parse(typeof(Datatype), lines[1]);
            this.Name = lines[2];
            this.Data = lines[3];
        }

        public void Write()
        {
            string f = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string folder = f + "\\" + datatype.ToString();
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string fn = folder + "\\" + ID + ".csv";
            string[] lines = new string[4]
            {
                Time,
                datatype.ToString(),
                Name,
                Data
            };
            File.WriteAllLines(fn, lines);
        }
    }
    public static class GetAllData
    {
        public static List<CSVData> GetData(this Datatype dt)
        {
            var data = new List<CSVData>();
            string mydocs = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string folder = mydocs + "\\" + dt.ToString();
            if(Directory.Exists(folder))
            {
                foreach(string f in Directory.GetFiles(folder))
                {
                    try { data.Add(new CSVData(f)); }
                    catch { }
                }
            }
            return data;
        }
        public static CSVData CreateData(this Datatype dt, string name, string datapt)
        {
            CSVData data = new CSVData(dt, name, datapt);
            data.Write();
            return data;
        }
    }
}