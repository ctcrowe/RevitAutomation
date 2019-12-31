using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

using System.IO;
using System.Reflection;

namespace CC_Plugin
{
    internal class ExDefOptions
    {
        public ExternalDefinitionCreationOptions opt { get; set; }
        public ExDefOptions(Param p)
        {
            switch(p.type)
            {
                default: case ParamType.Int:
                    opt = new ExternalDefinitionCreationOptions(p.name, ParameterType.Integer);
                    break;
                case ParamType.String:
                    opt = new ExternalDefinitionCreationOptions(p.name, ParameterType.Text);
                    break;
                case ParamType.Double:
                    opt = new ExternalDefinitionCreationOptions(p.name, ParameterType.Number);
                    break;
                case ParamType.Bool:
                    opt = new ExternalDefinitionCreationOptions(p.name, ParameterType.YesNo);
                    break;
                case ParamType.Length:
                    opt = new ExternalDefinitionCreationOptions(p.name, ParameterType.Length);
                    break;
                case ParamType.Material:
                    opt = new ExternalDefinitionCreationOptions(p.name, ParameterType.Material);
                    break;
                case ParamType.Area:
                    opt = new ExternalDefinitionCreationOptions(p.name, ParameterType.Area);
                    break;
            }                if (!p.Vis)
                {
                    opt.Visible = false;
                }
                if (p.Description != null)
                {
                    opt.Description = p.Description;
                }
                if (!p.UsrMod)
                {
                    opt.UserModifiable = false;
                }
                opt.GUID = p.ID;
        }
    }
}
