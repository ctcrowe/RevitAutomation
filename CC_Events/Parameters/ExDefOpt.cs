using Autodesk.Revit.DB;
using CC_Library.Parameters;

namespace CC_Plugin
{
    internal static class ExDefOptions
    {
        public static ExternalDefinitionCreationOptions CreateOptions(this CC_Library.Parameters.Param p)
        {
            ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(p.name, ParameterType.Integer);
            switch(p.type)
            {
                default: case ParamType.Int:
                        options.Type = ParameterType.Integer;
                        break;
                case ParamType.String:
                    options.Type = ParameterType.Text;
                    break;
                case ParamType.Double:
                    options.Type = ParameterType.Number;
                    break;
                case ParamType.Bool:
                    options.Type = ParameterType.YesNo;
                    break;
                case ParamType.Length:
                    options.Type = ParameterType.Length;
                    break;
                case ParamType.Material:
                    options.Type = ParameterType.Material;
                    break;
                case ParamType.Area:
                    options.Type = ParameterType.Area;
                    break;
            }
            if (!p.Vis)
                options.Visible = false;
            if (p.Description != null)
                options.Description = p.Description;
            if (!p.UsrMod)
                options.UserModifiable = false;
            options.GUID = p.ID;

            return options;
        }
    }
}
