using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using CC_Library.Predictions;

namespace CC_Library
{
    public static class CMDLibrary
    {
        public static int Abs(this int x)
        {
            if (x < 0)
                return x * -1;
            return x;
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }
        public static double PowX(this int x, int y)
        {
            double z = x;
            for(int i = 1; i < y.Abs(); i++)
            {
                z *= x;
            }
            if(x == 0)
                return 1;
            if(x > 0)
                return z;
            else
                return 1 / z;
        }
        public static double PowTen(this int x)
        {
            double v = 10;
            for (int i = 0; i < x.Abs(); i++)
            {
                v *= 10;
            }
            if (x == 0)
                return 1;
            if (x > 0)
                return v;
            else
                return 1 / v;
        }
        public static List<string> SplitOnCaps(this string s)
        {
            List<string> strings = new List<string>();
            int j = 0;
            char p = ' ';
            string outputstring = "";
            foreach(var c in s)
            {
                if (char.IsUpper(c) && !char.IsUpper(p) && p != ' ')
                {
                    strings.Add(outputstring.ToUpper());
                    outputstring = "" + c;
                }
                else
                {
                    outputstring += c;
                }
                if(j == s.Count() - 1)
                {
                    strings.Add(outputstring.ToUpper());
                }
                p = c;
                j++;
            }
            return strings;
        }
        public static List<string> SplitTitle(this string s)
        {
            List<string> data = new List<string>();
            char[] delimitters = { ',', '.', ' ', '_', '-' , '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            List<string> Array = s.Split(delimitters).ToList();
            foreach(string a in Array)
            {
                data.AddRange(a.SplitOnCaps());
            }
            return data;
        }
        public static double Sigmoid(this double x)
        {
            double Top = Math.Pow(Math.E, x);
            double Bottom = Top + 1;
            return Top / Bottom;
        }
        public static string SimplifyTitle(this string s)
        {
            char[] delimitters = { ',', '.', ' ', '-' , '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            List<string> Array = s.Split(delimitters).ToList();
            string x = string.Empty;
            foreach(string a in Array)
                x += a;
            return x;
        }
        public static double[] Multiply(this double[] matrix, double x)
        {
            double[] NewMatrix = new double[matrix.Count()];
            for(int i = 0; i < matrix.Count(); i++)
            {
                NewMatrix[i] = matrix[i] * x;
            }
            return NewMatrix;
        }
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
        public static KeyValuePair<TKey, TValue> GetEntry<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
        }
        public static void Divide(this double[] array, int a)
        {
            for(int i = 0; i < array.Count(); i++)
            {
                array[i] /= a;
            }
        }
        public static void WriteArray<t>(this t[] values, string label, WriteToCMDLine write)
        {
            string s = label + " : " + values.FirstOrDefault();
            for(int i = 1; i < values.Count(); i++)
            {
                s += ", " + values[i];
            }
            write(s);
        }
    }
}
