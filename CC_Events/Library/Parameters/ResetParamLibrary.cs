﻿using System.Reflection;
using System.IO;
using Autodesk.Revit.DB;
using CC_Library;
using CC_Library.Parameters;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    public static class Transactions
    {
        public delegate void DocCommand(Document doc);
        public delegate string DocStringCommand(Document doc);
        public delegate string StringBasedDocCommand(Document doc, string s);
        public delegate void ParamDocCommand(Document doc);
        public static void Run(ParamDocCommand pdc, Document doc) wher
        {
            using (Transaction trans = new Transaction(doc, "Run Command"))
            {
                trans.Start();
                try
                {
                    pdc(p, doc);
                    trans.Commit();
                }
                catch
                {
                    trans.RollBack();
                }
            }
        }
        public static void Transact(DocCommand dc, Document doc)
        {
            using(Transaction t = new Transaction(doc, "Run Command"))
            {
                t.Start();
                try
                {
                    dc(doc);
                    t.Commit();
                }
                catch
                {
                    t.RollBack();
                }
            }
        }
        public static void Transact(DocStringCommand dc, Document doc)
        {
            using (Transaction t = new Transaction(doc, "Run Command"))
            {
                t.Start();
                try
                {
                    dc(doc);
                    t.Commit();
                }
                catch
                {
                    t.RollBack();
                }
            }
        }
        public static string Transact(StringBasedDocCommand dc, Document doc, string s)
        {
            using (Transaction t = new Transaction(doc, "Run Command"))
            {
                t.Start();
                try
                {
                    dc(doc, s);
                    t.Commit();
                }
                catch
                {
                    t.RollBack();
                }
            }
            return s;
        }
    }
    public static class ResetParamLibrary
    {
        private static string Location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string SharedParams = Location + "\\CC_SharedParams.txt";

        public static void Run()
        {
            if (File.Exists(SharedParams))
            {
                File.Delete(SharedParams);
            }
            using (FileStream stream = File.Create(SharedParams))
            {
                stream.Close();
            }
        }
    }
}
