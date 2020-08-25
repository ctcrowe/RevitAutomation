using System;
using Autodesk.Revit.DB;
using CC_Library.Parameters;

namespace CC_RevitBasics
{
    internal static class ExDefOptions
    {
        public static ExternalDefinitionCreationOptions CreateOptions(this CCParameter par)
        {
            ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(par.ToString(), ParameterType.Text);
            int value = (int)par;
            int typevalue = Math.Abs((value / 100) % 10);
            switch (typevalue)
            {
                default:
                case 0:
                    options.Type = ParameterType.Text;
                    break;
                case 1:
                    options.Type = ParameterType.Length;
                    break;
                case 2:
                    options.Type = ParameterType.YesNo;
                    break;
                case 3:
                    options.Type = ParameterType.Integer;
                    break;
                case 4:
                    options.Type = ParameterType.Material;
                    break;
                case 5:
                    options.Type = ParameterType.Area;
                    break;
            }
            options.GUID = par.GetGUID();
            if (value % 10 == 0)
                options.UserModifiable = false;

            return options;
        }
    }
}