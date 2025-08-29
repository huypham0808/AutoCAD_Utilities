using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using AttributeCollection = Autodesk.AutoCAD.DatabaseServices.AttributeCollection;

[assembly: CommandClass(typeof(FindReferTitleID.FindRefer))]
namespace FindReferTitleID
{
    public class FindRefer
    {
        static PaletteSet palette;
        static bool wasVisible;

        [CommandMethod("CSS_SmartReferPalette", CommandFlags.Modal)]
        public void CallFormSmartRefer2()
        {
            if (palette == null)
            {
                palette = new PaletteSet("CSS SMART REFER", "CSS_SMARTREFER", new Guid("{3C23279E-18B9-4920-BA41-ECE7BCD0119F}"));
                palette.Style = PaletteSetStyles.ShowAutoHideButton |
                                PaletteSetStyles.ShowCloseButton |
                                PaletteSetStyles.ShowPropertiesMenu;

                palette.Size = new System.Drawing.Size(130, 250);
                
                palette.AddVisual("Tab 1", new mainForm());

                var docs = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;
                docs.DocumentBecameCurrent += (s, e) => palette.Visible = e.Document == null ? false : wasVisible;
                docs.DocumentCreated += (s, e) => palette.Visible = wasVisible;
                docs.DocumentToBeDeactivated += (s, e) => wasVisible = palette.Visible;
                docs.DocumentToBeDestroyed += (s, e) =>
                {
                    wasVisible = palette.Visible;
                    if (docs.Count == 1)
                        palette.Visible = false;
                };
            }
            
            palette.Visible = true;
            palette.DockEnabled = DockSides.None;
            palette.RolledUp = false;
        }
    }
}
