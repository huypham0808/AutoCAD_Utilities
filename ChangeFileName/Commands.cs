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
using ChangeFileName.Views;
using ChangeFileName.ViewModels;

namespace ChangeFileName
{
    public class Commands
    {
        [CommandMethod("Test")]
        public void Test()
        {
            ChangeFileNameViewModel vM = new ChangeFileNameViewModel();

            FileNameWindow fileNameWindow = new FileNameWindow();
            fileNameWindow.DataContext = vM;

            Autodesk.AutoCAD.ApplicationServices.Application.ShowModalWindow(fileNameWindow);
        }
    }
}
