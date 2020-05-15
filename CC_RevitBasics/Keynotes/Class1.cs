using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_RevitBasics.Keynotes
{
    class Class1
    {
        public static void test(Document doc)
        {
            KeynoteTable.GetKeynoteTable(doc).GetExternalResourceReferences();
        }
    }
}
