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
    public class Init : AppCad, IExtensionApplication 
    {
        #region MyfirstCommand
        [CommandMethod("MYFIRSTCOMMAND")]
        public void MyFirstCommand()
        {
            var doCu = AppCad.acDoc();
            var dbApp = AppCad.acDb();
            var edApp = AppCad.acEd();
            if (IsSavedFile())
            {
                edApp.WriteMessage("Drawing is saved");
            } else
            {
                edApp.WriteMessage("Drawing is NO saved");
                edApp.WriteMessage("\nDrawing name is: " + doCu.Name);
            }


        }
         
        #endregion
        #region Collectio
        [CommandMethod("TESTCOLLECTION")]
        public void TestCollection ()
        {
            var edApp = AppCad.acEd();
            AcAp.MainWindow.Text = "HuyPham";
            List<int> danhSach = new List<int>();
            danhSach.Add(1);
            danhSach.Add(2);
            danhSach.Add(3);

            foreach(int item in danhSach)
            {
                edApp.WriteMessage("\n" + item);
            }
        }

        [CommandMethod("TESTARRAY")]
        public void TestArray()
        {
            var edApp = AppCad.acEd();
            int[] array1 = new int[5];
            array1[0] = 1;
            array1[1] = 2;
            array1[2] = 3;
            array1[3] = 4;
            array1[4] = 5;

            for(int i = 0; i < array1.Length; i++)
            {
                try
                {
                    if(i == 4)
                    {
                        throw new Autodesk.AutoCAD.Runtime.Exception(ErrorStatus.AnonymousEntry, "Khong chap nhan so 5");
                    } 
                    else
                    {
                        edApp.WriteMessage("\n" + array1[i]);
                    }
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Application.ShowAlertDialog("Bat loi khi gia tri bang 5: " + ex.Message);
                }

            }
        }

        [CommandMethod("TESTCOUNTDOCS")]
        public void TestCountDocs()
        {
            var doc = AppCad.acDoc();
            var ed = AppCad.acEd();
            ed.WriteMessage("\nCo " + Application.DocumentManager.Count + " bang ve dang mo");
        }

        [CommandMethod("CSS_TEMPLATE")]
        public void CSSTemplate()
        {
            DocumentCollection docColl = Application.DocumentManager;
            var ed = AppCad.acEd(); 
            var db = AppCad.acDb();
            Document newDoc = docColl.Add(@"D:\HUY\CSS_Template.dwt");
            docColl.MdiActiveDocument = newDoc;
          
        }

        [CommandMethod("TESTOBJECTID")] 
        public void TestObjectID()
        {
            var db = AppCad.acDb2();
            var ed = AppCad.acEd();

            ObjectId layerId = db.LayerTableId;

            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable layerTbl = trans.GetObject(layerId, OpenMode.ForRead) as LayerTable; // [Ép kiểu]: ép layerTbl về kiểu LayerTable

                int count = 0;
                foreach(Object ob in layerTbl)
                {
                    count += 1;
                }

               if(count > 1)
               {
                   ed.WriteMessage("Co tat ca " + count + " layers");
               }
               else if (count <= 1 && count > 0)
               {
                   ed.WriteMessage("Co tat ca " + count + "layer");
               }
                trans.Commit();
            }
        }
        [CommandMethod("TESTCREATELAYER")]
        public void TestCreateLayer()
        {
            var db = AppCad.acDb2();
            var ed = AppCad.acEd();

            AppCad.CreateLayer("CSS_01");
            AppCad.CreateLayer("CSS_02");
        }
        #endregion
        #region SupportFunction
        public Boolean IsSavedFile()
        {
            
            int factor = System.Convert.ToInt16(Application.GetSystemVariable("DWGTITLED"));
            if (factor != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion
        public void Initialize()
        {
            //Dung de goi cac phuong thuc khi Open AutoCAD
            var db = AppCad.acDb();
            var ed = AppCad.acEd();
            AcAp.ShowAlertDialog("Plugin vua duoc load vao");

            ed.WriteMessage("Plugin vua duoc load vao");
        }

        public void Terminate()
        {
            //throw new NotImplementedException();
        }
    }

}
