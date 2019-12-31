using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
namespace CC_Events
{
    class TextUpdater : IUpdater
    {
        static AddInId addId;
        static UpdaterId upId;
        public TextUpdater(AddInId id)
        {
            addId = id;
            upId = new UpdaterId(addId, new Guid("de390bfb-591c-483d-ab51-39946c874cca"));
        }
        public void Execute(UpdaterData data)
        {

        }
        public string GetAdditionalInformation()
        {
            return "Logs all text in text notes and who writes the text.";
        }
        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Annotations;
        }
        public UpdaterId GetUpdaterId()
        {
            return upId;
        }
        public string GetUpdaterName()
        {
            return "Text Note Updater";
        }
    }
}
