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
using FindReferTitleID.UtilsMethod;
using System.IO;


using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace FindReferTitleID
{
    public class FindRefer : UtilsMethod.UtilsMethod
    {
        private class BlockData
        {
            public ObjectId ObjectId { get; set; }
            public Point2d Position { get; set; }
        }

        private static List<BlockData> blockDataList = new List<BlockData>();
        private static ListView listView;


        [CommandMethod("FindRefer")]
        public void CSS_FindRefer()
        {
            Document currentDocument = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = currentDocument.Database;
            Editor editor = currentDocument.Editor;

            ObjectId blockReferenceId = GetObjectID("TitleID");
            ObjectId mTextObjectIdOrigin = GetMTextId("Sheet number");

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
                }
                //Get Coordinate of block reference
                if (blockReference != null)
                {
                    Point3d blockPosition = blockReference.Position;
                    Point2d blockPosition2D = new Point2d(blockPosition.X, blockPosition.Y);
                    BlockData blockData = new BlockData
                    {
                        ObjectId = blockReferenceId,
                        Position = blockPosition2D
                    };
                    blockDataList.Add(blockData);
                }
                editor.WriteMessage("Done");
                // Commit the transaction
                transaction.Commit();
            }

        }
        //Show Table display List Data
        [CommandMethod("ShowFindRefer")]
        public static void ShowFindRefe()
        {
            Document currentDocument = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database currentDatabase = currentDocument.Database;
            Editor editor = currentDocument.Editor;
            //Show List Data
            if (blockDataList.Count > 0)
            {
                if (listView == null || listView.IsDisposed)
                    CreateListViewForm();

                //RefreshListView();
            }
            else
            {
                editor.WriteMessage("\nNo TitleID found.");
            }
        }
        private static void CreateListViewForm()
        {
            Form form = new Form();
            form.Text = "Block Data List";
            form.Size = new System.Drawing.Size(400, 300);

            listView = new ListView();
            listView.Dock = DockStyle.Fill;
            listView.View = View.Details;
            listView.Columns.Add("Object ID", 200);
            listView.Columns.Add("Position X", 100);
            listView.Columns.Add("Position Y", 100);

            form.Controls.Add(listView);

            form.FormClosing += (sender, e) =>
            {
                listView = null;
                blockDataList.Clear();
            };

            form.Show();
        }
        private static void RefreshListView()
        {
            //listView.Items.Clear();

            foreach (BlockData blockData in blockDataList)
            {
                ListViewItem item = new ListViewItem(blockData.ObjectId.ToString());
                item.SubItems.Add(blockData.Position.X.ToString());
                item.SubItems.Add(blockData.Position.Y.ToString());

                listView.Items.Add(item);
            }

            ListViewItem lastItem = listView.Items.Count > 0 ? listView.Items[listView.Items.Count - 1] : null;
            if (lastItem != null)
                lastItem.EnsureVisible();
        }
    }
}


