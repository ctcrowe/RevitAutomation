using System.Linq;
using System.Reflection;
using System.IO;
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace CC_Updater
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class CCUpdater : IExternalApplication
    {
        /* The Manifest File discussed below should look something like this:
                #This is the manifest file. This line is a header
                C:\Users\Charlie\Desktop\RevitPlugin\BCAG\DLL\
                C:\ProgramData\Autodesk\Revit\Addins\2017\CCPlugin\CC_Events.dll
         */
        public const string tabname = "CCrowe";
        private static string PLoc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string FLoc = PLoc + "\\CCPlugin\\";
        private static string Man = FLoc + "Manifest.txt";
        private static string GLoc = File.ReadAllLines(Man)[1];
        private static string RMethod = File.ReadAllLines(Man)[2];
        public static void RunMethod(string assemb, string met, UIControlledApplication uiApp)
        {
            Assembly CCAssembly;
            CCAssembly = Assembly.LoadFrom(assemb);
            Type t = CCAssembly.GetType("CC_Plugin.CCRibbon");
            if (t != null)
            {
                MethodInfo MI = t.GetMethod(met);
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
                        object[] PArray = new object[] { uiApp };
                        res = MI.Invoke(ClassInstance, PArray);
                    }
                }
            }
        }
        public Result OnStartup(UIControlledApplication uiApp)
        {
            if (Directory.Exists(FLoc))
            {
                try
                {
                    foreach(string file in Directory.GetFiles(GLoc))
                    {
                        string fn = file.Split('\\').Last();
                        if (File.Exists(file) && File.Exists(FLoc + fn))
                            File.Delete(FLoc + fn);
                        File.Copy(file, FLoc + fn);
                    }
                    RunMethod(RMethod, "OnStartup", uiApp);
                }
                catch
                { }
            }
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication uiApp)
        {
            try
            {
                RunMethod(RMethod, "OnShutdown", uiApp);
            }
            catch
            { }
            return Result.Succeeded;
        }
        private static string dllpath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = dir + "\\CC_Updater.dll";
            return dll;
        }
    }
}