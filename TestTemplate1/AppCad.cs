using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace TestTemplate1
{
    public class AppCad
    {
        public static Document acDoc()
        {
            return AcAp.DocumentManager.MdiActiveDocument;
        }
        public static Database acDb()
        {
            return AcAp.DocumentManager.MdiActiveDocument.Database;
        }
        public static Editor acEd()
        {
            return AcAp.DocumentManager.MdiActiveDocument.Editor;
        }
    }
}
