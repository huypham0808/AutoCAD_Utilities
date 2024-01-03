using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

public class CADUtils
{
	public APIUtils()
	{
			
	}
	//Select filter Block Reference	
	public void SelectBlock()
    {
        Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;

        // Create a TypedValue array to define the filter criteria
        TypedValue[] acTypValAr = new TypedValue[1];
        acTypValAr.SetValue(new TypedValue((int)DxfCode.BlockName, "FASU"), 2);

        // Assign the filter criteria to a SelectionFilter object
        SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);

        // Request for objects to be selected in the drawing area
        PromptSelectionResult acSSPrompt;
        acSSPrompt = acDocEd.GetSelection(acSelFtr);

        // If the prompt status is OK, objects were selected
        if (acSSPrompt.Status == PromptStatus.OK)
        {
            SelectionSet acSSet = acSSPrompt.Value;

            Application.ShowAlertDialog("Number of objects selected: " +
                                        acSSet.Count.ToString());
        }
        else
        {
            Application.ShowAlertDialog("Number of objects selected: 0");
        }

    }
}
