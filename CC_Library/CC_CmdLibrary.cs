using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using CC_Library.Predictions;
using CC_Library.Datatypes;

namespace CC_Library
{
    public delegate string WriteToCMDLine(string s);
    public static class CMDLibrary
    {
        public static void Launch()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var Dir = Directory.GetParent(path);
            var target = Dir.GetFiles().Where(x => x.Name.Contains("CC_Learning.html")).First().FullName;
            System.Diagnostics.Process.Start(target);
        }
        public static double[,] Take(this double[,] x, int start, int count)
        {
            if(x.GetLength(0) < start + count)
            {
                Console.WriteLine("Error, tried to take " + count + ", start was " + start + ", x length is " + x.GetLength(0));
                throw new Exception("Out of Range Exception");
            }    
            double[,] result = new double[count, x.GetLength(1)];
            Parallel.For(start, start + count, i => result.SetRank(x.GetRank(1), i - start) );
            return result;
        }
        public static double[,] Append(this double[,] x, double[,] y)
        {
            if(x.GetLength(1) != y.GetLength(1))
            {
                Console.WriteLine("Error, x Count was : " + x.GetLength(0) + ", " + x.GetLength(1) + ", y length is : " + y.GetLength(0) + ", " + y.GetLength(1));
                throw new Exception("Element Null Exception");
            }
            double[,] result = new double[x.GetLength(0) + y.GetLength(0), x.GetLength(1)];
            Parallel.For(0, x.GetLength(0), i => result.SetRank(x.GetRank(i), i));
            Parallel.For(0, y.GetLength(0), i => result.SetRank(y.GetRank(i), i + x.GetLength(0)));
            return result;
        }
        public static double[,] Divide(this double[,] x, double divisor)
        {
            double[,] y = new double[x.GetLength(0), x.GetLength(1)];
            Parallel.For(0, x.GetLength(0), i =>
            {
                Parallel.For(0, x.GetLength(1), j => y[i, j] = x[i, j] / divisor);
            });
            return y;
        }
        public static double[] SumRange(this double[,] x)
        {
            double[] y = new double[x.GetLength(1)];
            Parallel.For(0, x.GetLength(0), j =>
            {
                Parallel.For(0, x.GetLength(1), i =>
                {
                    y[i] += x[j, i] / x.GetLength(0);
                });
            });
            return y;
        }
        public static void SetRandom(this double[,] x)
        {
            Random random = new Random();
            Parallel.For(0, x.GetLength(0), j =>
            {
                Parallel.For(0, x.GetLength(1), i =>
                {
                    x[j, i] = random.NextDouble() > 0.5 ?
                        random.NextDouble() / x.GetLength(1) : (-1 * random.NextDouble() / x.GetLength(1));
                });
            });
        }
        public static void Update(this double[,] set, double[,] dvalues, double rate, double L1Regularization = 1e-4, double L2Regularization = 1e-4)
        {
            if (set.GetLength(0) != dvalues.GetLength(0) || set.GetLength(1) != dvalues.GetLength(1))
            {
                Console.WriteLine("Error, x Count was : " + set.GetLength(0) + ", " + set.GetLength(1) + ", y length is : " + dvalues.GetLength(0) + ", " + dvalues.GetLength(1));
                throw new Exception("Element Null Exception");
            }
            Parallel.For(0, set.GetLength(0), j =>
            {
                Parallel.For(0, set.GetLength(1), k =>
                {
                    set[j, k] -= dvalues[j, k] * rate;
                });
            });
        }
        public static double[,] Transpose(this double[,] array)
        {
            var array2 = new double[array.GetLength(1), array.GetLength(0)];
            Parallel.For(0, array.GetLength(1), i =>
            {
                Parallel.For(0, array.GetLength(0), j =>
                {
                    array2[i, j] = array[j, i];
                });
            });
            return array2;
        }
        public static double[,] Ones(this double[,] Similar)
        {
            double[,] output = new double[Similar.GetLength(0), Similar.GetLength(1)];
            Parallel.For(0, output.GetLength(0), i =>
                         {
                             Parallel.For(0, output.GetLength(1), j => output[i, j] = 1);
                         });
            return output;
        }
        public static double[] Ones(this double[] Similar)
        {
            double[] output = new double[Similar.Count()];
            Parallel.For(0, output.Count(), i => output[i] = 1);
            return output;
        }
        public static double[] Ones(this int Count)
        {
            double[] output = new double[Count];
            Parallel.For(0, output.Count(), i => output[i] = 1);
            return output;
        }
        public static double[] RandomBinomial(this double[] Similar, double dropout)
        {
            Random r = new Random();
            double[] output = new double[Similar.Count()];
            double todropout = output.Count() * dropout;
            Parallel.For(0, output.Count(), i => output[i] = i + 1);
            for (int i = 0; i < todropout; i++)
            {
                var test = output.Where(x => x != 0).ToList();
                int numb = test.Count > 1 ? (int)test[r.Next(0, test.Count() - 1)] : 0;
                output[numb] = 0;
            }
            Parallel.For(0, output.Count(), i => output[i] = output[i] == 0 ? 0 : 1);
            return output;
        }
        public static double[] InverseDropOut(this double[] DValues, double[] DropOutRank)
        {
            double[] output = new double[DValues.Count()];
            for (int i = 0; i < DValues.Count(); i++)
            {
                output[i] = DropOutRank[i] == 0 ? 0 : DValues[i];
            }
            return output;
        }
        public static string GenText(this double[] x)
        {
            string s = x[0].ToString();
            for (int i = 1; i < x.Count(); i++)
            {
                s += ", " + x[i];
            }
            return s;
        }
        public static string WriteNull(string s)
        {
            return s;
        }
        public static double[] Duplicate(this double[] x)
        {
            double[] y = new double[x.Count()];
            for (int i = 0; i < x.Count(); i++)
            {
                y[i] = x[i];
            }
            return y;
        }
        public static double[] Clone(this double[] x)
        {
            double[] y = new double[x.Count()];
            for (int i = 0; i < x.Count(); i++)
            {
                y[i] = x[i];
            }
            return y;
        }
        public static void WriteToBinaryFile<T>(this string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Create(filePath))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }
        public static T ReadFromBinaryFile<T>(this string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
        public static double[] Clipped(this double[] X)
        {
            double[] result = new double[X.Count()];
            Parallel.For(0, X.Count(), i =>
                result[i] = X[i] <= 0 ? 1e-7 : X[i] >= 1 ? 1 - (1e-7) : X[i]);
            return result;
        }
        public static double[,] DiagFlat(this double[] X)
        {
            double[,] result = new double[X.Count(), X.Count()];
            for (int i = 0; i < X.Count(); i++)
            {
                result[i, i] = X[i];
            }
            return result;
        }
        public static string GetMyDocs(this string Subdir, WriteToCMDLine wo)
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string subdir = directory + "\\" + Subdir;
            wo(subdir);
            return subdir;
        }
        public static string GetMyDocs(this string Subdir)
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string subdir = directory + "\\" + Subdir;
            return subdir;
        }
        public static string GetDir(this string FolderName)
        {
            return !Directory.Exists(FolderName) ? Directory.CreateDirectory(FolderName).FullName : FolderName;
        }
        public static double[,] Dot(this double[,] x, double[,] y)
        {
            if (x.GetLength(1) != y.GetLength(0))
            {
                Console.WriteLine("Error, x Count was : " + x.GetLength(0) + ", " + x.GetLength(1) + ", y length is : " + y.GetLength(0) + ", " + y.GetLength(1));
                throw new Exception("Element Null Exception");
            }
            double[,] dot = new double[x.GetLength(0), y.GetLength(1)];

            Parallel.For(0, x.GetLength(0), i =>
            {
                Parallel.For(0, y.GetLength(1), j =>
                {
                    Parallel.For(0, y.GetLength(0), k => dot[i, j] += (x[i, k] * y[k, j]));
                });
            });
            return dot;
        }
        public static double[] Dot(this double[,] x, double[] y)
        {
            if (x.GetLength(1) != y.GetLength(0))
            {
                Console.WriteLine("Error, x Count was : " + x.GetLength(0) + ", " + x.GetLength(1) + ", y length is : " + y.GetLength(0));
                throw new Exception("Element Null Exception");
            }
            double[] z = new double[x.GetLength(0)];
            Parallel.For(0, x.GetLength(0), j =>
            {
                Parallel.For(0, x.GetLength(1), i => z[j] += x[j, i] * y[i]);
            });
            return z;
        }
        public static double[] Dot(this double[] x, double[] y)
        {
            if (x.Count() == y.Count())
            {
                double[] z = new double[x.Count()];
                Parallel.For(0, x.Count(), j => z[j] = x[j] * y[j]);
                return z;
            }
            return null;
        }
        public static double[,] Dot(this double[] x, double[,] y)
        {
            if (x.Count() != y.GetLength(1))
            {
                Console.WriteLine("Error, x Count was : " + x.Count() + ", y length 1 is : " + y.GetLength(1));
                throw new Exception("Element Null Exception");
            }
            double[,] z = new double[y.GetLength(0), y.GetLength(1)];
            Parallel.For(0, y.GetLength(0), j =>
            {
                Parallel.For(0, y.GetLength(1), k => z[j, k] = x[k] * y[j, k]);
            });
            return z;
        }
        public static double[] DotSwitch(this double[] x, double[] y)
        {
            if (x.GetLength(0) == y.GetLength(0))
            {
                double[] dot = new double[x.GetLength(0)];
                Parallel.For(0, x.GetLength(0), i =>
                {
                    dot[i] = x[i] * y[i];
                });
                return dot;
            }
            return null;
        }
        public static void OutputError(this Exception ex)
        {
            string f = "Error.txt";
            string filepath = f.GetMyDocs();

            using (StreamWriter writer = new StreamWriter(filepath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }
        public static List<string> OutputError(this Sample s, double[] e)
        {
            List<string> lines = new List<string>();
            lines.Add("-----------------------------------------------------------------------------");
            lines.Add("Date : " + DateTime.Now.ToString());
            lines.Add("Datatype : " + s.Datatype);
            lines.Add("Sample Text : " + s.TextInput);
            lines.Add("Desired Output : " + s.DesiredOutput.ToList().IndexOf(s.DesiredOutput.Max()));
            lines.Add("Error : " + e.SumError());
            return lines;
        }
        public static void ShowErrorOutput(this List<string> lines)
        {
            string f = "PredictionError.txt";
            string fp = f.GetMyDocs();
            File.WriteAllLines(fp, lines);
        }
        public static double[] GetRank(this double[,] D, int l)
        {
            if(l > D.GetLength(0))
            {
                Console.WriteLine("Error, x Count was : " + D.GetLength(0) + ", " + D.GetLength(1) + ", n is : " + l);
                throw new Exception("Indexing Exception");
            }
            double[] r = new double[D.GetLength(1)];
            Parallel.For(0, D.GetLength(1), j => r[j] = D[l, j]);
            return r;
        }
        public static void SetRank(this double[,] d, double[] r, int n)
        {
            if(d.GetLength(1) != r.GetLength(0))
            {
                Console.WriteLine("Error, x Count was : " + d.GetLength(0) + ", " + d.GetLength(1) + ", y length is : " + r.GetLength(0));
                throw new Exception("Size Difference Exception");
            }
            if(n > d.GetLength(0))
            {
                Console.WriteLine("Error, x Count was : " + d.GetLength(0) + ", " + d.GetLength(1) + ", n is : " + n);
                throw new Exception("Indexing Exception");
            }
            Parallel.For(0, d.GetLength(1), j => d[n, j] = r[j]);
        }
        public static double[] GetRank(this double[,,] D, int r1, int r2)
        {
            double[] r = new double[D.GetLength(2)];
            if (D.GetLength(0) > r1 && D.GetLength(1) > r2)
                Parallel.For(0, D.GetLength(2), j => r[j] = D[r1, r2, j]);
            return r;
        }
        public static void SetRank(this double[,,] d, double[] r, int r1, int r2)
        {
            if (d.GetLength(2) == r.GetLength(0) && r1 < d.GetLength(0) && r2 < d.GetLength(1))
            {
                Parallel.For(0, d.GetLength(1), j => d[r1, r2, j] = r[j]);
            }
        }
        public static double MeanLoss(this double[] F)
        {
            return F.Sum() / F.Count();
        }
        public static double SecH(this double x)
        {
            double a = 2 / (Math.Exp(x) + Math.Exp(-1 * x));
            return a;
        }
        public static double SumError(this double[] X)
        {
            double a = 0;
            for (int x = 0; x < X.Count(); x++)
            {
                a += X[x];
            }
            return a / X.Count();
        }
        public static int Abs(this int x) { return x < 0 ? x * -1 : x; }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
        }
        public static void Add(this double[,] X, double[,] Y)
        {
            if (X.GetLength(0) != Y.GetLength(0) || X.GetLength(1) != Y.GetLength(1))
            {
                Console.WriteLine("Error, x Count was : " + X.GetLength(0) + ", " + X.GetLength(1) + ", y length is : " + Y.GetLength(0) + ", " + Y.GetLength(1));
                throw new Exception("Size Difference Exception");
            }
            Parallel.For(0, X.GetLength(0), i =>
            {
                Parallel.For(0, X.GetLength(1), j => X[i, j] += Y[i, j]);
            });
        }
        public static double[] Add(this double[] X, double[] Y)
        {
            if (X.GetLength(0) != Y.GetLength(0))
            {
                throw new Exception("Size Difference Exception");
            }
            double[] Z = new double[X.GetLength(0)];
            Parallel.For(0, X.GetLength(0), j => Z[j] = X[j] + Y[j]);
            return Z;
        }
        public static void Divide(this double[] X, double Y)
        {
            for (int i = 0; i < X.Count(); i++)
            {
                X[i] /= Y;
            }
        }
        public static double PowX(this int x, int y)
        {
            double z = x;
            for (int i = 1; i < y.Abs(); i++)
            {
                z *= x;
            }
            if (x == 0)
                return 1;
            if (x > 0)
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
            foreach (var c in s)
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
                if (j == s.Count() - 1)
                {
                    strings.Add(outputstring.ToUpper());
                }
                p = c;
                j++;
            }
            return strings;
        }
        public static double Sigmoid(this double x)
        {
            double Top = Math.Pow(Math.E, x);
            double Bottom = Top + 1;
            return Top / Bottom;
        }
        public static string SimplifyTitle(this string s)
        {
            char[] delimitters = { ',', '.', ' ', '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            List<string> Array = s.Split(delimitters).ToList();
            string x = string.Empty;
            foreach (string a in Array)
                x += a;
            return x;
        }
        public static double[] Multiply(this double[] matrix, double x)
        {
            double[] NewMatrix = new double[matrix.Count()];
            for (int i = 0; i < matrix.Count(); i++)
            {
                NewMatrix[i] = matrix[i] * x;
            }
            return NewMatrix;
        }
        public static double[] Multiply(this double[,] a, double[] b)
        {
            double[] c = new double[a.GetLength(1)];
            if (a.GetLength(0) == b.GetLength(0))
            {
                for (int i = 0; i < a.GetLength(0); i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        c[j] += a[i, j] * b[i];
                    }
                }
            }
            return c;
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
            for (int i = 0; i < array.Count(); i++)
            {
                array[i] /= a;
            }
        }
        public static void WriteArray<t>(this t[] values, string label, WriteToCMDLine write)
        {
            string s = label + " : " + values.FirstOrDefault();
            for (int i = 1; i < values.Count(); i++)
            {
                s += ", " + values[i];
            }
            write(s);
        }
        public static double[,] Merge(this double[,] X, double [,] Y)
        {
            if (X.GetLength(0) != Y.GetLength(0))
            {
                Console.WriteLine("Error, x Count was : " + X.GetLength(0) + ", " + X.GetLength(1) + ", y length is : " + Y.GetLength(0) + ", " + Y.GetLength(1));
                throw new Exception("Size Difference Exception");
            }
            double[,] Z = new double[X.GetLength(0), X.GetLength(1) + Y.GetLength(1)];
            Parallel.For(0, X.GetLength(0), i =>
            {
                Parallel.For(0, X.GetLength(1), j => Z[i, j] = X[i, j]);
                Parallel.For(0, Y.GetLength(1), j => Z[i, X.GetLength(1) + j] = Y[i, j]);
            });
            return Z;
        }
        public static double[,] Normalize(this double[,] x)
        {
            double normalval = 0;
            for (int i = 0; i <= x.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= x.GetUpperBound(1); j++)
                {
                    normalval += Math.Pow(x[i, j], 2);
                }
            }
            double length = Math.Sqrt(normalval);
            double[,] resultant = new double[x.GetUpperBound(0), x.GetUpperBound(1)];
            for (int i = 0; i <= x.GetUpperBound(0); i++)
            {
                for (int j = 0; j < x.GetUpperBound(1); j++)
                {
                    resultant[i, j] = x[i, j] / resultant[i, j];
                }
            }
            return resultant;
        }
        public static double[] Normalize(this double[] x)
        {
            double normalval = 0;
            for (int i = 0; i < x.Count(); i++)
            {
                normalval += Math.Pow(x[i], 2);
            }
            double length = Math.Sqrt(normalval);
            double[] result = new double[x.Count()];
            for (int i = 0; i < x.Count(); i++)
            {
                result[i] = x[i] / length;
            }
            return result;
        }
        public static double VectorLength(this double[] vector)
        {
            double vec = 0;
            for(int i = 0; i < vector.Count(); i++)
            {
                double a = Math.Pow(vector[i], 2);
                vec += a;
            }
            double result = Math.Sqrt(vec);
            return result;
        }
        public static double[,] OuterProduct(this double[] x, double[] y)
        {
            double[,] z = new double[x.Count(), y.Count()];
            for(int i = 0; i < x.Count(); i++)
            {
                for(int j = 0; j < y.Count(); j++)
                {
                    z[i,j] += x[i] * y[j];
                }
            }
            return z;
        }
        public static double[,] Wedge(this double[] x, double[] y)
        {
            if (x.Count() == y.Count())
            {
                double[,] Wedge = new double[x.Count(), x.Count()];

                double[,] v1 = x.OuterProduct(y);
                double[,] v2 = y.OuterProduct(x);

                for (int i = 0; i < x.Count(); i++)
                {
                    for(int j = 0; j < y.Count(); j++)
                    {
                        Wedge[i, j] = v1[i, j] - v2[i, j];
                    }
                }
                return Wedge;
            }
            return null;
        }
    }
}
