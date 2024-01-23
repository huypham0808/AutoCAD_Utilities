using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using FindReferTitleID.UtilsMethod;

namespace FindReferTitleID.UtilsMethod
{
    public class UtilsMethod
    {
        public static ObjectId GetObjectID (string objectName)
        {
            var ed = FindReferLib.FindReferEd(); ;

            // Prompt the user to select the block reference
            PromptEntityOptions entityOptions = new PromptEntityOptions($"\nSelect the {objectName}: ");
            entityOptions.SetRejectMessage("\nInvalid selection. Please select a block reference.");
            entityOptions.AddAllowedClass(typeof(BlockReference), true);

            PromptEntityResult entityResult = ed.GetEntity(entityOptions);
            ObjectId blockObjectID = entityResult.ObjectId;
            if (entityResult.Status != PromptStatus.OK) return ObjectId.Null;
            return blockObjectID;

        }

        public static ObjectId GetMTextId (string mtextName)
        {
            var ed = FindReferLib.FindReferEd();

            PromptEntityOptions mTextOptions = new PromptEntityOptions($"\nSelect the {mtextName}: ");
            mTextOptions.SetRejectMessage("\nInvalid selection. Please select an Sheet number.");
            mTextOptions.AddAllowedClass(typeof(MText), true);

            PromptEntityResult mTextResult = ed.GetEntity(mTextOptions);
            ObjectId mTextObjectIdOrigin = mTextResult.ObjectId;
            if (mTextResult.Status != PromptStatus.OK) return ObjectId.Null;
            return mTextObjectIdOrigin;
        }
    }
}
