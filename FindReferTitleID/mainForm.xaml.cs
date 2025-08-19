using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FindReferTitleID
{
    /// <summary>
    /// Interaction logic for mainForm.xaml
    /// </summary>
    public partial class mainForm : UserControl
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void btnbtnTileIDSheet_Click(object sender, RoutedEventArgs e)
        {
            MultipleTitleIDSheet();
        }

        private void btnSectionTileID_Click(object sender, RoutedEventArgs e)
        {
            Document currentDocument = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = currentDocument.Database;
            Editor editor = currentDocument.Editor;

            // Prompt the user to select the TitleID block 1
            PromptEntityOptions blockSection = new PromptEntityOptions("\nSelect the block Section_IDN1: ");
            blockSection.SetRejectMessage("\nInvalid Block selection. Please select a block Section_IDN1.");
            blockSection.AddAllowedClass(typeof(BlockReference), false);

            PromptEntityResult blockSectionResult = editor.GetEntity(blockSection);
            if (blockSectionResult.Status != PromptStatus.OK)
                return;

            // Prompt the user to select the TitleID block 2
            PromptEntityOptions blockTitleID1 = new PromptEntityOptions("\nSelect the block TitleID1: ");
            blockTitleID1.SetRejectMessage("\nInvalid Block selection. Please select a block TitleID1.");
            blockTitleID1.AddAllowedClass(typeof(BlockReference), false);

            PromptEntityResult blockTitleID1Result = editor.GetEntity(blockTitleID1);
            if (blockSectionResult.Status != PromptStatus.OK)
                return;

            ObjectId blockTitleID1Id = blockTitleID1Result.ObjectId;
            ObjectId blockSectionId = blockSectionResult.ObjectId;

            using (DocumentLock docLock = currentDocument.LockDocument())
            using (Transaction tr = currentDatabase.TransactionManager.StartTransaction())
            {
                BlockReference br = tr.GetObject(blockTitleID1Id, OpenMode.ForRead) as BlockReference;
                AttributeCollection blockTitleID1Collection = br.AttributeCollection;
                if (br == null)
                {
                    editor.WriteMessage("\nNot a valid block reference.");
                    return;
                }

                // Bước 3: Duyệt qua các attribute
                ObjectId tag1Id = ObjectId.Null;
                ObjectId tagS1Id = ObjectId.Null;

                foreach (ObjectId attId in blockTitleID1Collection)
                {
                    AttributeReference attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                    if (attRef != null)
                    {
                        if (attRef.Tag.Equals("1", StringComparison.OrdinalIgnoreCase))
                        {
                            tag1Id = attId;
                        }
                        else if (attRef.Tag.Equals("S1", StringComparison.OrdinalIgnoreCase))
                        {
                            tagS1Id = attId;
                        }
                    }
                }
                string tag1IdConvert = tag1Id.ToString();
                tag1IdConvert = tag1IdConvert.Replace("(", "").Replace(")", "");
                string tagS1IdConvert = tagS1Id.ToString();
                tagS1IdConvert = tagS1IdConvert.Replace("(", "").Replace(")", "");

                string fieldExpressionTag1 = "%<\\AcObjProp Object(%<\\_ObjId " + tag1IdConvert.ToString() + ">%).TextString>%";
                string fieldExpressionTagS = "%<\\AcObjProp Object(%<\\_ObjId " + tagS1IdConvert.ToString() + ">%).TextString>%";

                BlockReference brSection = tr.GetObject(blockSectionId, OpenMode.ForRead) as BlockReference;
                AttributeCollection blockSectionCollection = brSection.AttributeCollection;
                if (brSection == null)
                {
                    editor.WriteMessage("\nNot a valid block reference.");
                    return;
                }
                foreach (ObjectId attributeId in blockSectionCollection)
                {
                    AttributeReference attribute = tr.GetObject(attributeId, OpenMode.ForRead) as AttributeReference;
                    // Create the field expression
                    using (AttributeReference attributeToModify = tr.GetObject(attributeId, OpenMode.ForWrite) as AttributeReference)
                    {
                        switch (attributeToModify.Tag)
                        {
                            case "1":
                                attributeToModify.UpgradeOpen();
                                attributeToModify.TextString = fieldExpressionTag1;
                                attributeToModify.DowngradeOpen();
                                break;
                            case "S1":
                                attributeToModify.UpgradeOpen();
                                attributeToModify.TextString = fieldExpressionTagS;
                                attributeToModify.DowngradeOpen();
                                break;
                        }
                    }
                }
                editor.Regen();
                editor.WriteMessage("Refer successfully");
                tr.Commit();
            }
        }

        private void btnbtnFindTitleID_Click(object sender, RoutedEventArgs e)
        {
            Document currentDocument = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = currentDocument.Database;
            Editor editor = currentDocument.Editor;

            // Prompt the user to select the TitleID block 1
            PromptEntityOptions blockSection = new PromptEntityOptions("\nSelect the block Section_IDN1: ");
            blockSection.SetRejectMessage("\nInvalid Block selection. Please select a block Section_IDN1.");
            blockSection.AddAllowedClass(typeof(BlockReference), false);

            PromptEntityResult blockSectionResult = editor.GetEntity(blockSection);
            if (blockSectionResult.Status != PromptStatus.OK)
                return;

            ObjectId blockSectionId = blockSectionResult.ObjectId;
            string valueTag1 = string.Empty;
            string valueTagS1 = string.Empty;
            ObjectId blockIDResult = ObjectId.Null;
            using (Transaction tr = currentDatabase.TransactionManager.StartTransaction())
            {
                BlockReference br = tr.GetObject(blockSectionId, OpenMode.ForRead) as BlockReference;
                AttributeCollection blockSectionCollection = br.AttributeCollection;
                if (br == null)
                {
                    editor.WriteMessage("\nNot a valid block reference.");
                    return;
                }
                foreach (ObjectId attId in blockSectionCollection)
                {
                    AttributeReference attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                    if (attRef != null)
                    {
                        switch (attRef.Tag)
                        {
                            case "1":
                                valueTag1 = attRef.TextString;
                                break;
                            case "S1":
                                valueTagS1 = attRef.TextString;
                                break;
                        }
                    }
                }
                tr.Commit();
            }
            blockIDResult = FindBlockReference(currentDatabase, "Title ID2", valueTag1, valueTagS1);
            ZoomHighlight(blockIDResult);
            //SelectObjectById(editor, blockIDResult);
        }
        private void btnReferCallout_Click(object sender, RoutedEventArgs e)
        {
            GetMLeaderMTextContent();
        }
        private void MultipleTitleIDSheet()
        {
            Document currentDocument = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = currentDocument.Database;
            Editor editor = currentDocument.Editor;

            // Prompt the user to select multiple block references
            PromptSelectionOptions selOptions = new PromptSelectionOptions();
            selOptions.MessageForAdding = "\nSelect block references: ";

            // Filter chỉ chọn BlockReference
            SelectionFilter filter = new SelectionFilter(new TypedValue[]
            {
                new TypedValue((int)DxfCode.Start, "INSERT") // INSERT = BlockReference
            });

            // Chọn nhiều block
            PromptSelectionResult selRes = editor.GetSelection(selOptions, filter);
            if (selRes.Status != PromptStatus.OK)
                return;

            ObjectId[] blockReferenceIds = selRes.Value.GetObjectIds();

            // Prompt the user to select the MText object
            PromptEntityOptions mTextOptions = new PromptEntityOptions("\nSelect the MText object: ");
            mTextOptions.SetRejectMessage("\nInvalid selection. Please select an MText object.");
            mTextOptions.AddAllowedClass(typeof(MText), false);

            PromptEntityResult mTextResult = editor.GetEntity(mTextOptions);
            if (mTextResult.Status != PromptStatus.OK)
                return;

            ObjectId mTextObjectIdOrigin = mTextResult.ObjectId;

            string mTextObjectId = mTextObjectIdOrigin.ToString();
            mTextObjectId = mTextObjectId.Replace("(", "").Replace(")", "");

            // Create the field expression
            string fieldExpression = "%<\\AcObjProp Object(%<\\_ObjId " + mTextObjectId + ">%).TextString>%";

            // Start the transaction
            using (DocumentLock docLock = currentDocument.LockDocument())
            using (Transaction transaction = currentDatabase.TransactionManager.StartTransaction())
            {
                foreach (ObjectId blockReferenceId in blockReferenceIds)
                {
                    BlockReference blockReference = transaction.GetObject(blockReferenceId, OpenMode.ForRead) as BlockReference;
                    if (blockReference == null) continue;

                    AttributeCollection attributeCollection = blockReference.AttributeCollection;

                    // Find the text attribute with tag "S1"
                    foreach (ObjectId attributeId in attributeCollection)
                    {
                        AttributeReference attribute = transaction.GetObject(attributeId, OpenMode.ForRead) as AttributeReference;
                        if (attribute != null && attribute.Tag.Equals("S1", StringComparison.OrdinalIgnoreCase))
                        {
                            AttributeReference attributeToModify = transaction.GetObject(attributeId, OpenMode.ForWrite) as AttributeReference;
                            if (attributeToModify != null)
                            {
                                attributeToModify.TextString = fieldExpression;
                                attributeToModify.DowngradeOpen();
                            }
                        }
                    }
                }

                editor.Regen();
                editor.WriteMessage("\nDone updating multiple blocks.");
                transaction.Commit();
            }

        }
        public static void ZoomHighlight(ObjectId id)
        {
            // Lấy tài liệu hiện hành
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = doc.Database;
            Editor editor = doc.Editor;

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                try
                {
                    // Lấy đối tượng từ ObjectId
                    Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                    if (ent != null)
                    {
                        // Lấy giới hạn hình học của đối tượng
                        Extents3d extents = ent.GeometricExtents;

                        Point3d minPoint = extents.MinPoint;
                        Point3d maxPoint = extents.MaxPoint;

                        Point2d center = new Point2d(
                            (minPoint.X + maxPoint.X) / 2.0,
                            (minPoint.Y + maxPoint.Y) / 2.0
                        );

                        double height = maxPoint.Y - minPoint.Y;
                        double width = maxPoint.X - minPoint.X;
                        var cview = editor.GetCurrentView();
                        //editor.WriteMessage("\n Chiều cao của view trước zoom: " + cview.Height);
                        // Tính tỷ lệ khung hình của viewport hiện tại
                        double aspectRatio = cview.Width / cview.Height;

                        // Điều chỉnh chiều cao và chiều rộng để phù hợp với khung nhìn
                        if (height * aspectRatio < width)
                        {
                            height = width / aspectRatio;
                        }
                        // đặt scale nếu muốn zoom nhỏ hơn object một ít
                        double scale = 1.5;


                        cview.CenterPoint = center;
                        cview.Height = height * scale;
                        cview.Width = height * aspectRatio * scale;
                        // Thiết lập view cho editor
                        editor.SetCurrentView(cview);
                        ent.Highlight();
                        //editor.WriteMessage("\n Chiều cao của view sau zoom: " + cview.Height);
                    }
                    else
                    {
                        editor.WriteMessage("\nKhông thể lấy đối tượng từ ObjectId.");
                    }

                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    editor.WriteMessage($"\nLỗi Zoom: {ex.Message}");
                }
            }
        }
        public ObjectId FindBlockReference(Database db, string blockName, string textValueSection1, string textValueSection2)
        {
            ObjectId result = ObjectId.Null;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Lấy blockTableRecord của model space
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                // Duyệt toàn bộ đối tượng trong model space
                foreach (ObjectId entId in modelSpace)
                {
                    BlockReference br = tr.GetObject(entId, OpenMode.ForRead) as BlockReference;
                    if (br == null) continue;

                    // Kiểm tra tên block
                    BlockTableRecord btr = tr.GetObject(br.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                    if (!btr.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Kiểm tra các attribute
                    string s1Value = null;
                    string s2Value = null;

                    foreach (ObjectId attId in br.AttributeCollection)
                    {
                        AttributeReference attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                        if (attRef == null) continue;

                        if (attRef.Tag.Equals("1", StringComparison.OrdinalIgnoreCase))
                            s1Value = attRef.TextString;
                        else if (attRef.Tag.Equals("S1", StringComparison.OrdinalIgnoreCase))
                            s2Value = attRef.TextString;
                    }

                    // Nếu thỏa điều kiện cả S1 và S2 thì return
                    if (s1Value == textValueSection1 && s2Value == textValueSection2)
                    {
                        result = br.ObjectId;
                        break;
                    }
                }

                tr.Commit();
            }
            return result;
        }
        public static void SelectObjectById(Editor ed, ObjectId objectId)
        {
            if (!objectId.IsNull && objectId.IsValid)
            {
                // Tạo SelectionSet từ ObjectId
                ObjectId[] ids = new ObjectId[] { objectId };
                ed.SetImpliedSelection(ids);
                //ed.WriteMessage($"\nObject {objectId} selected.");
            }
            else
            {
                ed.WriteMessage("\nInvalid ObjectId.");
            }
        }
        public void GetMLeaderMTextContent()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            // Prompt chỉ chọn Multileader
            PromptEntityOptions peo = new PromptEntityOptions("\nSelect FRP Callout: ");
            peo.SetRejectMessage("\nInvalid selection. Please select FRP Callout only.");
            peo.AddAllowedClass(typeof(MLeader), false);

            PromptEntityResult per = ed.GetEntity(peo);
            if (per.Status != PromptStatus.OK) return;

            //Yeu cau nguoi dung chon Title ID1.
            PromptEntityOptions blockTitleID1 = new PromptEntityOptions("\nSelect the block TitleID1: ");
            blockTitleID1.SetRejectMessage("\nInvalid Block selection. Please select a block TitleID1.");
            blockTitleID1.AddAllowedClass(typeof(BlockReference), false);

            PromptEntityResult blockTitleID1Result = ed.GetEntity(blockTitleID1);
            if (blockTitleID1Result.Status != PromptStatus.OK)
                return;

            ObjectId blockTitleID1Id = blockTitleID1Result.ObjectId;

            using (DocumentLock docLock = doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockReference br = tr.GetObject(blockTitleID1Id, OpenMode.ForRead) as BlockReference;
                AttributeCollection blockTitleID1Collection = br.AttributeCollection;
                if (br == null)
                {
                    ed.WriteMessage("\nNot a valid block reference.");
                    return;
                }

                // Bước 3: Duyệt qua các attribute
                ObjectId tag1Id = ObjectId.Null;
                ObjectId tagS1Id = ObjectId.Null;

                foreach (ObjectId attId in blockTitleID1Collection)
                {
                    AttributeReference attRef = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                    if (attRef != null)
                    {
                        if (attRef.Tag.Equals("1", StringComparison.OrdinalIgnoreCase))
                        {
                            tag1Id = attId;
                        }
                        else if (attRef.Tag.Equals("S1", StringComparison.OrdinalIgnoreCase))
                        {
                            tagS1Id = attId;
                        }
                    }
                }
                string tag1IdConvert = tag1Id.ToString();
                tag1IdConvert = tag1IdConvert.Replace("(", "").Replace(")", "");
                string tagS1IdConvert = tagS1Id.ToString();
                tagS1IdConvert = tagS1IdConvert.Replace("(", "").Replace(")", "");

                string fieldExpressionTag1 = "%<\\AcObjProp Object(%<\\_ObjId " + tag1IdConvert.ToString() + ">%).TextString>%";
                string fieldExpressionTagS = "%<\\AcObjProp Object(%<\\_ObjId " + tagS1IdConvert.ToString() + ">%).TextString>%";
                MLeader mLeader = tr.GetObject(per.ObjectId, OpenMode.ForWrite) as MLeader;

                if (mLeader != null && mLeader.ContentType == ContentType.MTextContent)
                {
                    MText mtext = mLeader.MText;
                    if (mtext != null)
                    {
                        string content = mtext.Text;
                        string newContent = content;
                        //Xu ly content 
                        string marker = "SEE DETAIL";
                        if (content.Contains(marker))
                        {
                            // Xóa phần A/B, thay bằng fieldExpressionTag1/fieldExpressionTagS
                            int index = content.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                            newContent = content.Substring(0, index + marker.Length).Trim()
                                         + " " + fieldExpressionTag1 + "/" + fieldExpressionTagS;
                            mtext.Contents = newContent;
                            mLeader.MText = mtext;
                        }                                
                        //Assign content cho mtex
                        ed.WriteMessage($"\nMLeader content:\n{content}");                     
                    }
                    else
                    {
                        ed.WriteMessage("\nThis FRP Callout has no MText content.");
                    }
                }
                else
                {
                    ed.WriteMessage("\nSelected FRP Callout does not contain MText content.");
                }
                tr.Commit();
                ed.Regen();
            }
        }
    }
}
