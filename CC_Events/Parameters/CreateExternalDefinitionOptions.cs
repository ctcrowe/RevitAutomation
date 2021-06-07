using System;
using Autodesk.Revit.DB;
using CC_Library.Parameters;

namespace CC_Plugin.Parameters
{
    internal static class CreateExternalDefinitionOptions
    {
        public static ExternalDefinitionCreationOptions CreateOptions(this Param param)
        {
            ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(param.Name, ParameterType.Text);
            switch (param.Type)
            {
                default:
                case ParamType.Text:
                    options.Type = ParameterType.Text;
                    break;
                case ParamType.Length:
                    options.Type = ParameterType.Length;
                    break;
                case ParamType.Bool:
                    options.Type = ParameterType.YesNo;
                    break;
                case ParamType.Int:
                    options.Type = ParameterType.Integer;
                    break;
                case ParamType.Material:
                    options.Type = ParameterType.Material;
                    break;
                case ParamType.Area:
                    options.Type = ParameterType.Area;
                    break;
            }
            options.GUID = param.Guid;
            options.UserModifiable = param.UserModifiable;
            return options;
        }
    }
}