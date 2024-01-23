using System.Windows.Input;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.ComponentModel;
using System.IO;
using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace ChangeFileName.ViewModels
{
    public class ChangeFileNameViewModel : INotifyPropertyChanged
    {
        public ICommand ChangeFileNameCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Define Properties
        private string _NewFileName;
        private string _revisionNumber;


        public string NewFileName
        {
            get { return _NewFileName; }
            set
            {
                if (_NewFileName != value)
                {
                    _NewFileName = value;
                    OnPropertyChanged(nameof(NewFileName));
                }

            }
        }
        public string RevisionNumber
        {
            get { return _revisionNumber; }
            set
            {
                _revisionNumber = value;
                OnPropertyChanged(nameof(RevisionNumber));
            }
        }

        //Constructor
        public ChangeFileNameViewModel ()
        {
            RevisionNumber = "0";
            
            ChangeFileNameCommand = new RelayCommand(ChangeShopDrawingFile);
        }
        //public ICommand UpdateCommand => new RelayCommand(() => ChangeShopDrawingFile());
        //Define method
        private void ChangeShopDrawingFile()
        {
            string activeDate = DateTime.Now.ToString("yyyy.MM.dd");
            string projectName = NewFileName;
            string newFileName = activeDate + "-" + projectName + "-" + "FRP Shop Drawings" + "-"+ "Rev." +RevisionNumber;
            try
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Editor editor = doc.Editor;

                // Get the current document name
                string currentFileName = doc.Name;

                // Generate the new file path
                string newFilePath = currentFileName.Replace(Path.GetFileName(currentFileName), newFileName);

                // Rename the file
                doc.Database.SaveAs(newFilePath, true, DwgVersion.Current, doc.Database.SecurityParameters);
                // Update the document name
                doc.CloseAndDiscard();
                Document newDoc = Application.DocumentManager.Open(newFilePath);
                editor.SwitchToPaperSpace();
                NewFileName = string.Empty;
                editor.WriteMessage("Rename Successfully");
            }
            catch
            {
                return;
            }
        }
    }
}
