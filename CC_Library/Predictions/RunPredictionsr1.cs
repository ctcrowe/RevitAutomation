using System;
using System.Linq;
using System.Windows.Forms;

using CC_Library.Datatypes;
using CC_Library.Predictions.Masterformat;

namespace CC_Library.Predictions
{
    public static class Datasets
    {
        public static void RunPredictions(WriteToCMDLine write, Hold hold)
        {
            InitializeData init;
            GetEntry ge;
            Accuracy accuracy;
            ChangedElements ce;

            //Open the Dataset
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

                    switch(datatype)
                    {
                        default:
                        case Datatype.Masterformat:
                            init = new InitializeData(MasterformatInitialize.InitializeMF);
                            accuracy = new Accuracy(MasterformatAccuracy.MF_Accuracy);
                            ge = new GetEntry(MasterformatEntry.MFEntry);
                            ce = new ChangedElements(MasterformatInitialize.CollectChangedElements);
                            break;
                        case Datatype.OccupantLoadFactor:
                            init = new InitializeData(OLFInitialize.Initialize);
                            accuracy = new Accuracy(OLFAccuracy.Accuracy);
                            ge = new GetEntry(OLFGetEntry.OLFEntry);
                            ce = new ChangedElements(OLFInitialize.CollectChangedElements);
                            break;
                        case Datatype.StudLayer:
                            init = new InitializeData(Stud_Initialize.Initialize);
                            accuracy = new Accuracy(Stud_Accuracy.Accuracy);
                            ge = new GetEntry(Stud_Entry.StudEntry);
                            ce = new ChangedElements(Stud_Initialize.CollectChangedElements);
                            break;
                        case Datatype.OccupancyGroup:
                            init = new InitializeData(OCCInitialize.Initialize);
                            accuracy = new Accuracy(OCCAccuracy.Accuracy);
                            ge = new GetEntry(OCCGetEntry.OCCEntry);
                            ce = new ChangedElements(OCCInitialize.CollectChangedElements);
                            break;
                        case Datatype.RoomPrivacy:
                            init = new InitializeData(PrivacyInitialize.RPInitialize);
                            accuracy = new Accuracy(PrivacyAccuracy.RP_Accuracy);
                            ge = new GetEntry(PrivacyEntry.RPEntry);
                            ce = new ChangedElements(PrivacyInitialize.CollectChangedElements);
                            break;
                    }
                    filepath.AlternatingPredictions2(datatype, init, accuracy, ge, write, ce, hold);
                }
            }
        }
    }
}