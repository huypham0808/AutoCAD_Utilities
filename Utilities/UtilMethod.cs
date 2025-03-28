using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ChangeFileName.Utilities
{
    public class UtilMethod
    {
        public static void AppendTextToXmlFile(string filePath, string text)
        {
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(text);
            }
        }
        public static Document AcadDoc()
        {
            return Application.DocumentManager.MdiActiveDocument;
        }
        public static Database AcadDb()
        {
            return Application.DocumentManager.MdiActiveDocument.Database;
        }
        public static void WarningMessageBox (string contentMess, string titleMess)
        {
            MessageBox.Show(contentMess, titleMess, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
