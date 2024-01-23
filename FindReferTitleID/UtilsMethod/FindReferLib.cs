using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;


namespace FindReferTitleID.UtilsMethod
{
    public class FindReferLib
    {
        public static Document FindReferDoc () => Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        public static Database FindReferDb() => FindReferDoc().Database;
        public static Editor FindReferEd() => FindReferDoc().Editor;
    }
}
