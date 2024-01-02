using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace FindReferTitleID
{
    public class FindRefer
    {
        private class BlockData
        {
            public ObjectId ObjectId { get; set; }
            public Point2d Position { get; set; }
        }
        [CommandMethod("CSS_FindRefer")]
        public void CSS_FindRefer()
        {
            Document currentDocument = Application.DocumentManager.MdiActiveDocument;
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
            PromptEntityOptions mTextOptions = new PromptEntityOptions("\nSelect the MText object: ");
            mTextOptions.SetRejectMessage("\nInvalid selection. Please select an MText object.");
            mTextOptions.AddAllowedClass(typeof(MText), true);

            PromptEntityResult mTextResult = editor.GetEntity(mTextOptions);
            if (mTextResult.Status != PromptStatus.OK)
                return;
            ObjectId mTextObjectIdOrigin = new ObjectId();
            mTextObjectIdOrigin = mTextResult.ObjectId;
            List<BlockData> blockDataList = new List<BlockData>();
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
                            attribute.UpgradeOpen();
                            attributeToModify.TextString = fieldExpression;
                            attribute.DowngradeOpen();
                        }

                    }
                    //Get Coordinate of block reference
                    if (blockReference != null)
                    {
                        Point3d blockPosition = blockReference.Position;
                        Point2d blockPosition2D = new Point2d(blockPosition.X, blockPosition.Y);
                        //editor.WriteMessage($"\nBlock reference position: X = {blockPosition.X}, Y = {blockPosition.Y}, Z = {blockPosition.Z}");
                        BlockData blockData = new BlockData
                        {
                            ObjectId = entityResult.ObjectId,
                            Position = blockPosition2D
                        };
                        blockDataList.Add(blockData);
                    }
                }
                
                //editor.Regen();
                /*foreach (BlockData blockData in blockDataList)
                {
                    editor.WriteMessage($"\nBlock reference position: {blockData.ObjectId} X = {blockData.Position.X}, Y = {blockData.Position.Y}");
                }*/
                editor.WriteMessage("Done");
                // Commit the transaction
                transaction.Commit();
            }
        }
    }
}
