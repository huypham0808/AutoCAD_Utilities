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
using System.Windows.Forms;

namespace FindReferTitleID.MethodUtilities
{
    internal class ReferMethod
    {
        private class BlockData
        {
            public ObjectId ObjectId { get; set; }
            public Point2d Position { get; set; }
        }
        public void CSS_FindRefer()
        {
            Document currentDocument = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = currentDocument.Database;
            Editor editor = currentDocument.Editor;

            // Prompt the user to select the block reference
            PromptEntityOptions entityOptions = new PromptEntityOptions("\nSelect the block reference: ");
            entityOptions.SetRejectMessage("\nInvalid selection. Please select a block reference.");
            entityOptions.AddAllowedClass(typeof(BlockReference), true);

            PromptEntityResult entityResult = editor.GetEntity(entityOptions);
            if (entityResult.Status != PromptStatus.OK)
                return;

            ObjectId blockReferenceId = entityResult.ObjectId;

            // Prompt the user to select the MText object
            PromptEntityOptions mTextOptions = new PromptEntityOptions("\nSelect the MText (Sheet number) object: ");
            mTextOptions.SetRejectMessage("\nInvalid selection. Please select an MText object.");
            mTextOptions.AddAllowedClass(typeof(MText), false);

            PromptEntityResult mTextResult = editor.GetEntity(mTextOptions);
            if (mTextResult.Status != PromptStatus.OK)
                return;
            ObjectId mTextObjectIdOrigin = new ObjectId();
            mTextObjectIdOrigin = mTextResult.ObjectId;

            string mTextObjectId = mTextObjectIdOrigin.ToString();
            mTextObjectId = mTextObjectId.Replace("(", "").Replace(")", "");
            // Start the transaction
            using (Transaction transaction = currentDatabase.TransactionManager.StartTransaction())
            {
                // Open the block reference and its attribute collection
                BlockReference blockReference = transaction.GetObject(blockReferenceId, OpenMode.ForRead) as BlockReference;
                AttributeCollection attributeCollection = blockReference.AttributeCollection;

                // Find the text attribute with tag "S1"
                foreach (ObjectId attributeId in attributeCollection)
                {
                    AttributeReference attribute = transaction.GetObject(attributeId, OpenMode.ForRead) as AttributeReference;
                    if (attribute != null && attribute.Tag.Equals("S1", StringComparison.OrdinalIgnoreCase))
                    {
                        // Create the field expression
                        string fieldExpression = "%<\\AcObjProp Object(%<\\_ObjId " + mTextObjectId.ToString() + ">%).TextString>%";
                        using (AttributeReference attributeToModify = transaction.GetObject(attributeId, OpenMode.ForWrite) as AttributeReference)
                        {
                            if (attributeToModify.Tag == "S1")
                            {
                                attribute.UpgradeOpen();
                                attributeToModify.TextString = fieldExpression;
                                attribute.DowngradeOpen();
                            }
                        }
                    }
                }
                editor.Regen();
                editor.WriteMessage("Done");
                transaction.Commit();
            }
        }
        private static List<BlockData> blockDataList = new List<BlockData>();
        private static ListView listView;

        public static void ShowFindRefe()
        {
            Document currentDocument = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = currentDocument.Database;
            Editor editor = currentDocument.Editor;
            //Show List Data
            if (blockDataList.Count > 0)
            {
                if (listView == null || listView.IsDisposed) { }
                //CreateListViewForm();
            }
            else
            {
                editor.WriteMessage("\nNo TitleID found.");
            }
        }
        public void SmartRefer()
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
                                attribute.UpgradeOpen();
                                attributeToModify.TextString = fieldExpressionTag1;
                                attribute.DowngradeOpen();
                                break;
                            case "S1":
                                attribute.UpgradeOpen();
                                attributeToModify.TextString = fieldExpressionTagS;
                                attribute.DowngradeOpen();
                                break;
                        }
                    }
                }
                editor.Regen();
                editor.WriteMessage("Refer successfully");
                tr.Commit();
            }
        }
    }
}
