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
            Document newDoc = docColl.Add(@"S:\@FRP\_SST-ST Alliance\3. CAD template\_Current template\2023-10-03_ST-SST_CAD TEMPLATE.dwt");
            docColl.MdiActiveDocument = newDoc;
            ed.WriteMessage("Template was loaded successfully!");
          
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

        [CommandMethod("TESTBLOCKATTRIBUTE")]
        public void TestBlockAttribute()
        {
            var db = AppCad.acDb2();
            var ed = AppCad.acEd();

            PromptEntityOptions options = new PromptEntityOptions("Select title of drawing");

            options.SetRejectMessage("Please select Title ID");
            options.AddAllowedClass(typeof(BlockReference), true);

            PromptEntityResult result = ed.GetEntity(options);
            if(result.Status == PromptStatus.OK)
            {
                ObjectId objectID = result.ObjectId;
                using(Transaction trans = db.TransactionManager.StartTransaction())
                {
                    BlockReference blockRef = trans.GetObject(objectID, OpenMode.ForRead) as BlockReference;
                    BlockTableRecord blockTblRec = trans.GetObject(objectID, OpenMode.ForRead) as BlockTableRecord;

                    foreach(ObjectId attributeID in blockRef.AttributeCollection)
                    {
                        AttributeReference attributeRef = trans.GetObject(attributeID, OpenMode.ForRead) as AttributeReference;
                        string attributeName = attributeRef.Tag;
                        string attributeValue = attributeRef.TextString;

                        ed.WriteMessage("\nTag: " + attributeName + " & Value: " + attributeValue + "\n");
                    }
                    trans.Commit();
                }
            }


        }

        [CommandMethod("TESTGETDISTANCE")]
        public void TestGetDistance()
        {
            var db = AppCad.acDb2();
            var ed = AppCad.acEd();

            PromptDistanceOptions option = new PromptDistanceOptions("Chon hoac chon khoang cach");
            option.AllowArbitraryInput = false;
            option.AllowNegative = false;
            option.AllowNone = true;
            option.AllowZero = false;
            option.DefaultValue = 1;
            option.Only2d = true;
            option.UseDefaultValue = true;
            option.Keywords.Add("Yes");
            option.Keywords.Add("No");


            PromptDoubleResult result = ed.GetDistance(option);
            if (result.Status == PromptStatus.Keyword)
            {
                ed.WriteMessage("\nKeyword nguoi dung da chon la: " + result.StringResult);
            } 
            else
            {
                AcAp.ShowAlertDialog("Distance is :" + Math.Round(result.Value, db.Lunits, MidpointRounding.AwayFromZero));
                ed.WriteMessage("Distance is: " + Math.Round(result.Value, db.Lunits,MidpointRounding.AwayFromZero));
            }
        }

        [CommandMethod("TESTGETENTITY")]
        public void TestGetEntity()
        {
            var db = AppCad.acDb2();
            var ed = AppCad.acEd();

            PromptEntityOptions getEntity = new PromptEntityOptions("\nChon doi tuong");
   
            getEntity.AllowNone = true;
            getEntity.AllowObjectOnLockedLayer = true;
            getEntity.SetRejectMessage("\nKhong phai duong thang CIRCL: "); //Hien thi warning neu ko phai Line           
            //Cho phep chon Layer bi khoa
            getEntity.AddAllowedClass(typeof(Circle), true); //Chi cho phep chon Line

            PromptEntityResult resultEntity = ed.GetEntity(getEntity);
            if (resultEntity.Status != PromptStatus.OK) return;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Circle lineOb = trans.GetObject(resultEntity.ObjectId, OpenMode.ForRead) as Circle;
                ed.WriteMessage("Chieu dai duong Line la: " + lineOb.Area);

                trans.Commit();
            }
        }
        [CommandMethod("TESTSELLECTIONSET")]
        public void TestSellectionSet()
        {
            var db = AppCad.acDb2();
            var ed = AppCad.acEd();

            PromptSelectionOptions sellectionObj = new PromptSelectionOptions();
            sellectionObj.AllowDuplicates = false;
            sellectionObj.MessageForAdding = "\nChon duong thang de do chieu dai: ";
            sellectionObj.MessageForRemoval = "\nChon duong thang de loai bo: ";
            sellectionObj.RejectObjectsFromNonCurrentSpace = false;
            sellectionObj.RejectObjectsOnLockedLayers = true;
            sellectionObj.RejectPaperspaceViewport = true;

            // Filter theo đối tượng theo Circle
            TypedValue[] typeValue = new TypedValue[1];
            typeValue[0] = new TypedValue((int)DxfCode.Start, "CIRCLE");
           
            SelectionFilter selFilter = new SelectionFilter(typeValue);

            PromptSelectionResult resultSel = ed.GetSelection(sellectionObj, selFilter);
            if (resultSel.Status != PromptStatus.OK) return;
            using(Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    SelectionSet selSet = resultSel.Value;
                    Circle cirObj;
                    double totalArea = 0.0;
                    foreach(SelectedObject selected in selSet)
                    {
                        cirObj = trans.GetObject(selected.ObjectId, OpenMode.ForRead) as Circle;
                        totalArea += cirObj.Area;
                    }
                    Application.ShowAlertDialog("\nTong dien tich cac hinh tron la: " + Math.Round(totalArea, db.Luprec,MidpointRounding.AwayFromZero));
                }
                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    Application.ShowAlertDialog("Khong the tinh tong Dien tich");
                }
                trans.Commit();
            }
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
            //AcAp.ShowAlertDialog("Plugin vua duoc load vao");
            
            ed.WriteMessage("Plugin vua duoc load vao");
        }

        public void Terminate()
        {
            //throw new NotImplementedException();
        }
    }

}
