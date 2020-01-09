/// 1) Array dimensions comparing times of different objects to find related.
/// 2) Array dimensions reading room names to find relationships (Central File Name, Room Name, Room Number, Phase
///     Two rooms are only the same if all 3 are the smae
/// 3) Array dimension reading time in project when placed.
/// 4) Array dimension reading next object placed and previous object placed (relational objects).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace DataAnalysis
{
    public class Mastersection
    {
      public static Dictionary<string, string[]> GetData(string folder)
      {
        string[] Files = Directory.GetFiles(folder);
        Dictionary<string, string[]> data = new Dictionary<string, string[]>();
        foreach(string f in Files)
        {
          XDocument doc = XDocument.Load(f);
          if(doc.Root.Attribute("Name") != null)
          {
            string ele = doc.Root.Attribute("Name").Value;
            if(!string.IsNullOrEmpty(ele))
            {
              if(doc.Root.Attribute("Category") != null)
              {
                string cat = doc.Root.Attribute("Category").Value;
                data.Add(ele, new string[1] { cat });
              }
            }
          }
        }
        return data;
      }
    }
}
