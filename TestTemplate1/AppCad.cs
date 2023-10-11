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
using Autodesk.AutoCAD.Colors;

namespace TestTemplate1
{
    public class AppCad
    {
        public static Document acDoc()
        {
            return AcAp.DocumentManager.MdiActiveDocument;
        }
        //Lay Database Cach 1
        public static Database acDb()
        {
            return AcAp.DocumentManager.MdiActiveDocument.Database;
        }
        //Lay Datbase Cach 2
        public static Database acDb2()
        {
            return HostApplicationServices.WorkingDatabase;
        }
        public static Editor acEd()
        {
            return AcAp.DocumentManager.MdiActiveDocument.Editor;
        }
        public static void CreateLayer(string layerName)
        {
            var db = acDb2();

            ObjectId layerId = db.LayerTableId;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable layerTbl = trans.GetObject(layerId, OpenMode.ForWrite) as LayerTable; // [Ép kiểu]: ép layerTbl về kiểu LayerTable
                LayerTableRecord layerTblRc;

                if (layerTbl.Has(layerName) == false)
               {
                    layerTblRc = new LayerTableRecord();
                    layerTblRc.Name = layerName;
                    if (layerTbl.IsWriteEnabled == false) layerTbl.UpgradeOpen();
                    layerTbl.Add(layerTblRc);
                    trans.AddNewlyCreatedDBObject(layerTblRc, true);
               }
               else
               {
                    layerTblRc = trans.GetObject(layerTbl[layerName], OpenMode.ForWrite) as LayerTableRecord;
               }

               trans.Commit();
            }
        }
    }
}
