using System.Reflection;
using System;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CC_Plugin
{
    internal class CCMethod
    {
        public static void Run(string assembly, string typename, string method, Document doc)
        {
            string FullAssembly = "C:\\ProgramData\\Autodesk\\Revit\\Addins\\2019\\CCPlugin\\" + assembly + ".dll";
            byte[] assembly_bytes = File.ReadAllBytes(FullAssembly);
            Assembly CCAssembly = Assembly.Load(assembly_bytes);
            Type t = CCAssembly.GetType(typename);
            if (t != null)
            {
                MethodInfo MI = t.GetMethod(method);
                if (MI != null)
                {
                    object res = null;
                    ParameterInfo[] PI = MI.GetParameters();
                    object ClassInstance = Activator.CreateInstance(t, null);
                    if (PI.Length == 0)
                    {
                        res = MI.Invoke(ClassInstance, null);
                    }
                    else
                    {
                        object[] PArray = new object[] { doc };
                        res = MI.Invoke(ClassInstance, PArray);
                    }
                }
            }
        }
        /*
            byte[] assembly_bytes = System.IO.File.ReadAllBytes(_path);
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(assembly_bytes);
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass && type.FullName == _class_full_name)
                {
                    data._instance = Activator.CreateInstance(type) as Autodesk.Revit.UI.IExternalCommand;
                    break;
                }
            }
         */
    }
}
