using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
namespace TestTemplate1.View
{
    /// <summary>
    /// Interaction logic for LayerList.xaml
    /// </summary>
    public partial class LayerList : Window
    {
        public LayerList()
        {
            InitializeComponent();
        }
        private string GetLayerName()
        {
            var db = AppCad.acDb2();
            var ed = AppCad.acEd();

            ObjectId layerId = db.LayerTableId;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                LayerTable layerTbl = trans.GetObject(layerId, OpenMode.ForRead) as LayerTable; // [Ép kiểu]: ép layerTbl về kiểu LayerTable

                //int count = 0;
                string layerName = "";
                foreach (ObjectId ob in layerTbl)
                {
                    LayerTableRecord layerTblRec = trans.GetObject(ob, OpenMode.ForRead) as LayerTableRecord;
                    layerName += "\n" + layerTblRec.Name;
                }
                trans.Commit();
                return layerName;
            }
        }
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnLayer_Click(object sender, RoutedEventArgs e)
        {
            this.tblLayerName.Text = GetLayerName();
        }
    }
}
