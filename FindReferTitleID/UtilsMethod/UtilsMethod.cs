using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using FindReferTitleID.UtilsMethod;

namespace FindReferTitleID.UtilsMethod
{
    public class UtilsMethod
    {
        public static ObjectIdCollection GetObjectIDs (string objectName)
        {
            var ed = FindReferLib.FindReferEd(); ;

            // Prompt the user to select the block reference
            PromptSelectionOptions selectionOptions = new PromptSelectionOptions();
            selectionOptions.MessageForAdding = $"\nSelect the { objectName}:";
            selectionOptions.SingleOnly = false;

            PromptSelectionResult selectionResult = ed.GetSelection(selectionOptions);
            
            if (selectionResult.Status != PromptStatus.OK)          
               return new ObjectIdCollection();

            SelectionSet selectionSet = selectionResult.Value;
            ObjectIdCollection objectIDCollection = new ObjectIdCollection();
            foreach(SelectedObject selectedObject in selectionSet)
            {
                objectIDCollection.Add(selectedObject.ObjectId);
            }
            return objectIDCollection;
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
