using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;


namespace AutoRouting
{
    public class AutoRouting: APIUtils
    {
        [CommandMethod("ArentAutoRout")]
        public static void AutoRout()
        {
            APIUtils apiSelectBlock = new APIUtils();
            apiSelectBlock.SelectBlock();
        }
    }
}
