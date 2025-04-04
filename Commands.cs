using Autodesk.AutoCAD.Runtime;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using ChangeFileName.Views;
using ChangeFileName.ViewModels;

[assembly: CommandClass(typeof(ChangeFileName.Commands))]
namespace ChangeFileName
{
    public class Commands
    {
        [CommandMethod("CSS_UtilitiesTool_Beta", CommandFlags.Modal)]
        public void CallForm()
        {
            ChangeFileNameViewModel vM = new ChangeFileNameViewModel();

            FileNameWindow fileNameWindow = new FileNameWindow
            {
                DataContext = vM
            };
            AcAp.ShowModelessWindow(fileNameWindow);
        }
    }
}
