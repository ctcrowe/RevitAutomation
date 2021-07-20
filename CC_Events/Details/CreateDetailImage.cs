using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
using CC_Plugin.Parameters;
using CC_Library;
using CC_Library.Parameters;
using Autodesk.Revit.Attributes;

/// <summary>
/// The details classes will have to do all of the following
/// 1) A class that stores XML Files containing information about all the objects in the detail (They should not include lines. PERIOD
/// Everything in this will be information about the family objects and their parameters including
/// XYZ location
/// Rotation / Orientation
/// Name
/// Type / Parameter information
/// The detail itself will need the following information as well.
/// JPG File Name for referencing (so it can be viewed in the "Open" menu.
/// Tag style detail information for searching such as "Door", "HM", Etc.
/// The detail itself will need a GUID for referencing as well.
/// This GUID will be added to the view as a parameter, so that updating is smooth and easy.
/// 
/// Once detail information is created it cannot be updated. Only deleted.
/// Only modifications to the XML file through revit will include adding tags, so you get more information to search and use.

///https://www.revitapidocs.com/2020/e87fc993-5dc8-bb39-b7a7-fe91d075489a.htm
///https://www.revitapidocs.com/2020/8c0c72db-2801-3642-72bb-108cfdff23e1.htm
///https://www.revitapidocs.com/2020/6aa52291-2f4b-27d7-b999-5b5755bd7235.htm

/// 
/// If I have time eventually and figure it out, it would be nice to save these details to the google cloud using the google api. that way they  can be referenced from anywhere.
/// </summary>

namespace CC_Plugin.Details
{
    public static class DetailImage
    {
        public static void CreateDetailImage(this Document doc, string vid)
        {
            View v = doc.ActiveView;
            if (v.ViewType == ViewType.DraftingView || v.ViewType == ViewType.Detail)
            {
                string dir = "Details".GetMyDocs();
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                ImageExportOptions options = new ImageExportOptions();

                options.FilePath = dir + "\\" + vid;
                options.ExportRange = ExportRange.CurrentView;
                options.ZoomType = ZoomFitType.Zoom;
                options.Zoom = 100;
                options.ImageResolution = ImageResolution.DPI_72;

                doc.ExportImage(options);
            }
        }
    }
}
