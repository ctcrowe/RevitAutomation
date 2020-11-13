using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class Datasets
    {
        public static void RunPredictions(WriteToCMDLine write, Hold hold)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "Select a csv file",
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Open csv file"
            };
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                var filepath = ofd.FileName;
                Random random = new Random();

                if (Enum.GetNames(typeof(Datatype)).Any(x => filepath.Contains(x)))
                {
                    Datatype datatype = (Datatype)Enum.Parse(typeof(Datatype), Enum.GetNames(typeof(Datatype)).Where(x => filepath.Contains(x)).First());
                    write("Network Type : " + datatype.ToString());
                    
                    Func<string, WriteToCMDLine, List<Entry>> GetEntries;
                    Func<Entry, List<string>, double[]> Input;
                    Func<Entry, List<string>, double[]> Output;
                    Func<double[], double[], double[]> Forward;
                    Func< double[], double[], double[]> Backward;
                    Func<List<Entry>, NeuralNetwork, List<string>, double[]> Accuracy;
                    NetworkPropogation.AccuracyOutput output;
                    NetworkPropogation.SaveNetwork Save;

                    switch (datatype)
                    {
                        default:
                        case Datatype.Masterformat:
                            GetEntries = MasterformatEntry.MFEntry;
                            Input = MasterformatOutput.MFInput;
                            Output = MasterformatOutput.MFOutput;
                            Forward = CategoricalCrossEntropy.Forward;
                            Backward = CategoricalCrossEntropy.Backward;
                            Accuracy = MasterformatAccuracy.MF_Accuracy;
                            output = MasterformatAccuracy.MFAcc_Output;
                            Save = NeuralNetwork.Save;
                            break;
                        case Datatype.Boundary:
                            GetEntries = BoundaryEntry.BEntry;
                            Input = Boundary_Output.BInput;
                            Output = Boundary_Output.BOutput;
                            Forward = CategoricalCrossEntropy.Forward;
                            Backward = CategoricalCrossEntropy.Backward;
                            Accuracy = BoundaryAccuracy.BAccuracy;
                            output = BoundaryAccuracy.BAcc_Output;
                            Save = NeuralNetwork.Save;
                            break;
                        case Datatype.Dictionary:
                            GetEntries = DictionaryEntry.DictEntry;
                            Input = DictionaryOutput.DictInput;
                            Output = DictionaryOutput.DictOutput;
                            Forward = CategoricalCrossEntropy.Forward;
                            Backward = CategoricalCrossEntropy.Backward;
                            Accuracy = DictionaryAccuracy.Dict_Accuracy;
                            output = DictionaryAccuracy.DictAcc_Output;
                            Save = CustomNeuralNet.SaveDict;
                            break;
                        case Datatype.Elevation:
                            GetEntries = ElevationEntry.ElevEntry;
                            Input = ElevationOutput.ElevInput;
                            Output = ElevationOutput.ElevOutput;
                            Forward = MeanSquared.Forward;
                            Backward = MeanSquared.Backward;
                            Accuracy = ElevationAccuracy.ElevAccuracy;
                            output = ElevationAccuracy.ElevAcc_Output;
                            Save = NeuralNetwork.Save;
                            break;
                        case Datatype.OccupantLoadFactor:
                            GetEntries = OLF_Entry.OLFEntry;
                            Input = OLF_Output.OLFInput;
                            Output = OLF_Output.OLFOutput;
                            Forward = CategoricalCrossEntropy.Forward;
                            Backward = CategoricalCrossEntropy.Backward;
                            Accuracy = OLFAccuracy.OLF_Accuracy;
                            output = OLFAccuracy.OLFAcc_Output;
                            Save = NeuralNetwork.Save;
                            break;
                    }
                    filepath.Propogate(datatype, GetEntries, Input, Output, Forward, Backward, Accuracy, output, Save, write);
                }
            }
        }
    }
}