using System;
using CC_Library;
using CC_Library.Predictions;
using CC_Library.Datatypes;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace DataAnalysis
{
    class Program
    {
        public static string Write(string wo)
        {
            Console.WriteLine(wo);
            return wo;
        }
        [STAThread]
        static void Main(string[] args)
        {
            /*
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch(Exception exc) { exc.OutputError(); }
            */
            bool x = true;
            while(x)
            {
                Console.WriteLine("Enter the command to run");
                Console.WriteLine("Commands are : Train, Sort, Test, Show, Revise");
                string line = Console.ReadLine();
                line = line.ToUpper();
                try
                {
                    switch (line)
                    {
                        default:
                            break;
                        case "SOFTMAX":
                            double[] outputs = new double[3] {0.7, 0.1, 0.2};
                            double[] dvalues = new double[3] {0, 0, 1};
                            var t1 = Activations.InverseCrossEntropy(dvalues, outputs);
                            var t2 = Activations.InverseSoftMax(t1, outputs);
                            var t3 = Activations.InverseCombinedCrossEntropySoftmax(dvalues, outputs);
                            t1.WriteArray("Test 1, Cross Entropy", Write);
                            t2.WriteArray("Test 2, SoftMax", Write);
                            t3.WriteArray("Test 3, Combined CES", Write);
                            break;
                        case "TRAIN":
                            bool c = true;
                            while(c)
                            {
                                Datasets.RunPredictions(Write);
                            }
                            break;
                        case "TEST":
                            Console.WriteLine("Enter a Datatype to Predict");
                            string l2 = Console.ReadLine();
                            bool d = true;
                            while (d)
                            {
                                Console.WriteLine("Enter a phrase to predict.");
                                string l3 = Console.ReadLine();
                                PredTest.TestPredictions(l3, l2, new WriteToCMDLine(Write));
                                Console.WriteLine("Continue? Y / N");
                                string l4 = Console.ReadLine().ToUpper();
                                if (l4 == "N")
                                    c = false;
                                Console.WriteLine("");
                            }
                            break;
                        case "SHOW":
                            string s = "";
                            while(s != "X")
                            {
                                s = Console.ReadLine();
                                s.TestChars(new WriteToCMDLine(Write));
                            }
                            break;
                        case "COPY":
                            Sample samp = new Sample(Datatype.Masterformat);
                            samp.ReadSamples(new WriteToCMDLine(Write));
                            break;
                        case "READ":
                            ReadSample.Read(new WriteToCMDLine(Write));
                            break;
                        case "EXIT":
                        case "E":
                        case "X":
                            x = false;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    WriteError(ex);
                }
            }
        }
        static void WriteError(Exception ex)
        {
            string filePath = "Error.txt";
            string fullpath = filePath.GetMyDocs();

            using (StreamWriter writer = new StreamWriter(fullpath, true))
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
    }
}
