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

            ObjectId mTextObjectId = mTextResult.ObjectId;

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
                        string fieldExpression = $"%<\\AcObjProp Object(%<\\_ObjId {mTextObjectId.Handle}>%).TextString>%";

                        // Set the attribute text as the field expression
                        attribute.UpgradeOpen();
                        attribute.TextString = fieldExpression;
                        attribute.DowngradeOpen();
                        break;
                    }
                }
                // Commit the transaction
                transaction.Commit();
            }
        }
    }
}
