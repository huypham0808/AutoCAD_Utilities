using System.Windows.Input;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.ComponentModel;
using System.IO;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Windows.Forms;
using Clipboard = System.Windows.Forms.Clipboard;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Text;
using ChangeFileName.Utilities;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ChangeFileName.ViewModels
{
    public class ChangeFileNameViewModel : INotifyPropertyChanged
    {
        public ICommand ChangeFileNameCommand { get; }
        public ICommand ConverTextCommand { get; }
        public ICommand CopyTextCommand { get; }
        public ICommand GetCurrentLocalFileDwgCommand { get; }
        public ICommand GetSDriveProjectFolderCommand { get; }
        public ICommand PushFileToSDriveCommand { get; }
        public ICommand SaveDataToTextFileCommand { get; }
        public ICommand CreateTextStyleCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Define Properties
        private string _NewFileName;
        private string _revisionNumber;
        private string _inputText;
        private string _resultText;

        private string _contentCopyButton;
        private string _contentBeforeCopy = "Copy";
        private string _contentAfterCopy = "Copied";
        private Brush _colorTextOriginal;
        private string _copyTextClipboard;

        private string _currentLocalFile;
        private string _sdriveCompany;
        private ObservableCollection<string> _filePathsToListView = new ObservableCollection<string>();
        private bool _isExpandedHistoryPath;
        private readonly string directoryHistoryFolder = @"C:\FRP-ST-SST-Plugin\Data\Data Project Path";
        private readonly string historyDataFileName = "\\History project folder directory.xml";
        private const int originalNumber = 16;
        public string ContentCopyButton
        {
            get { return _contentCopyButton; }
            set
            {
                _contentCopyButton = value;
                OnPropertyChanged(nameof(ContentCopyButton));
            }
        }
        public string ContentBeforeCopy
        {
            get { return _contentBeforeCopy; }
            set
            {
                _contentBeforeCopy = value;
                OnPropertyChanged(nameof(ContentBeforeCopy));
            }
        }
        public string ContentAfterCopy
        {
            get { return _contentAfterCopy; }
            set
            {
                _contentAfterCopy = value;
                OnPropertyChanged(nameof(ContentAfterCopy));
            }
        }
        public string CopyTextClipboard
        {
            get { return _copyTextClipboard; }
            set
            {
                _copyTextClipboard = value;
                OnPropertyChanged(nameof(CopyTextClipboard));
            }
        }
        public Brush ColorTextOriginal
        {
            get { return _colorTextOriginal; }
            set
            {
                if(ResultText == "Text will be converted to UPPER here!")
                {
                    _colorTextOriginal = Brushes.DarkGray;
                }
                else
                {
                    _colorTextOriginal = value;
                    OnPropertyChanged(nameof(ColorTextOriginal));
                }
;
            }
        }

        public bool IsButtonEnabled => !string.IsNullOrEmpty(InputText);
        public bool IsExpandedHistoryPath
        {
            get { return _isExpandedHistoryPath; }
            set
            {
                _isExpandedHistoryPath = value;
                OnPropertyChanged(nameof(IsExpandedHistoryPath));

            }
        }
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
        public string InputText
        {
            get { return _inputText; }
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }
        public string ResultText
        {
            get { return _resultText; }
            set
            {
                _resultText = value;
                OnPropertyChanged(nameof(ResultText));
            }
        }
        public string CurrentLocalFilePath
        {
            get { return _currentLocalFile; }
            set
            {   
                _currentLocalFile = value;
                OnPropertyChanged(nameof(CurrentLocalFilePath));
            }
        }
        public string SDriveCompanyPath
        {
            get { return _sdriveCompany; }
            set
            {
                _sdriveCompany = value;
                OnPropertyChanged(nameof(SDriveCompanyPath));
            }
        }
        public ObservableCollection<string> FilePathToListView
        {
            get { return _filePathsToListView; }
            set
            {
                _filePathsToListView = value;
                OnPropertyChanged(nameof(FilePathToListView));
            }
        }
        private string _textStyleName;

        public string TextStyleName
        {
            get { return _textStyleName; }
            set 
            { 
                _textStyleName = value;
                OnPropertyChanged(nameof(TextStyleName));
            }
        }
        //Constructor
        public ChangeFileNameViewModel ()
        {
            RevisionNumber = "0";
            ContentCopyButton = ContentBeforeCopy;
            ResultText = "Text will be converted to UPPER here!";
            ColorTextOriginal = Brushes.Blue;
            ChangeFileNameCommand = new RelayCommand(ChangeShopDrawingFile);
            ConverTextCommand = new RelayCommand(ConvertToUpper);
            CopyTextCommand = new RelayCommand(CopyText);
            GetCurrentLocalFileDwgCommand = new RelayCommand(GetCurrentLocalFilePath);
            GetSDriveProjectFolderCommand = new RelayCommand(GetSDriveProjectPath);
            PushFileToSDriveCommand = new RelayCommand(PushFileToSDrive);
            IsExpandedHistoryPath = false;
            FilePathToListView = new ObservableCollection<string>();
            SaveDataToTextFileCommand = new RelayCommand(SaveListViewDataToXml);
            CreateTextStyleCommand = new RelayCommand(CreateNewTextStyle);
            LoadDataFromXmlFile();
            
            
        }
        //Method
        private void ChangeShopDrawingFile()
        {
            Document doc = UtilMethod.AcadDoc();

            string activeDate = DateTime.Now.ToString("yyyy.MM.dd");
            string projectName = string.IsNullOrEmpty(NewFileName) ? "Project name" : NewFileName;
            string newFileName = activeDate + "-" + projectName + "-" + "FRP Shop Drawings" + "-"+ "Rev." +RevisionNumber + ".dwg";
            try
            {
                // Get the current document name
                string currentFileName = doc.Name;
                // Generate the new file path
                string newFilePath = currentFileName.Replace(Path.GetFileName(currentFileName), newFileName);
                // Rename the file
                doc.Database.SaveAs(newFilePath, true, DwgVersion.Current, doc.Database.SecurityParameters);
                File.Delete(currentFileName);
                MessageBox.Show("Rename file successfully!", "AutoCAD", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                return;
            }
        }
        private void ConvertToUpper()
        {
            ColorTextOriginal = Brushes.Gray;
            if (!string.IsNullOrEmpty(InputText))
            {
                ResultText = InputText.ToUpper();
                ContentCopyButton = ContentBeforeCopy;
            }
            else
            {
                MessageBox.Show("Please input text before to covert!", "AutoCAD Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
        private void CopyText()
        {
            if(ResultText == "Text will be converted to UPPER here!")
            {
                MessageBox.Show("Please input text to convert!", "AutoCAD", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Clipboard.SetText(ResultText);
            ContentCopyButton = ContentAfterCopy;
            ColorTextOriginal = Brushes.LightGreen;

        }
        private void GetCurrentLocalFilePath()
        {
            string dwgFileName = (string)Application.GetSystemVariable("DWGNAME");
            string dwgPath = (string)Application.GetSystemVariable("DWGPREFIX");
            CurrentLocalFilePath = dwgPath + dwgFileName;
        }
        private void GetSDriveProjectPath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select project folder",
                CheckFileExists = false,
                FileName = "Select Folder",

                ValidateNames = false,
                CheckPathExists = true,
                Filter = "Folders|no.files"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string folderSDrivePath = Path.GetDirectoryName(openFileDialog.FileName);
                SDriveCompanyPath = folderSDrivePath;

                if (FilePathToListView.Count < 3)
                {
                    FilePathToListView.Add(SDriveCompanyPath);
                }
                else
                {
                    FilePathToListView.RemoveAt(0);
                    FilePathToListView.Add(SDriveCompanyPath);
                }               
            }
            else
            {
                MessageBox.Show("No folder selected!", "AutoCAD", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void PushFileToSDrive()
        {
            if(string.IsNullOrEmpty(CurrentLocalFilePath) || string.IsNullOrEmpty(SDriveCompanyPath))
            {
                UtilMethod.WarningMessageBox(@"Current local drive or S:\ ...drive NOT allow empty", "AutoCAD");
                return;
            }
            string sourceDir = CurrentLocalFilePath;
            string sourceDirPDF = sourceDir;
            string pdfDrawingDir = Path.ChangeExtension(sourceDirPDF, ".pdf");
            string fileName = Path.GetFileName(sourceDir);
            string pdfFileName = Path.GetFileName(pdfDrawingDir);
            string sDriveFolderPathDwg = SDriveCompanyPath + "\\" + fileName;
            string sDriveFolderPathPdf = SDriveCompanyPath + "\\" + pdfFileName;

            try
            {
                if (File.Exists(sDriveFolderPathDwg))
                {
                    File.Copy(sourceDir, sDriveFolderPathDwg, true);
                    File.Copy(pdfDrawingDir, sDriveFolderPathPdf, true);
                    UtilMethod.WarningMessageBox("Push file successfully!", "AutoCAD");
                }
                else if (!File.Exists(pdfDrawingDir))
                {
                    UtilMethod.WarningMessageBox("Print PDF before push file!", "AutoCAD");
                    return;
                }
                else
                {
                    File.Copy(sourceDir, sDriveFolderPathDwg);
                    File.Copy(pdfDrawingDir, sDriveFolderPathPdf);
                    UtilMethod.WarningMessageBox("Push file successfully!", "AutoCAD");
                }
            }
            catch
            {
                UtilMethod.WarningMessageBox("File is opening by someone", "AutoCAD");
                return;
            }
            IsExpandedHistoryPath = true;
        }
        private void LoadDataFromXmlFile()
        {
            string xmlFilePath = directoryHistoryFolder + historyDataFileName;
            try
            {
                using (StreamReader sr = new StreamReader(xmlFilePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        FilePathToListView.Add(line);
                    }
                }
            }
            catch
            {
                return;
            }
        }
        //Not in use
        private void SaveListViewDataToXml()
        {
            if (Directory.Exists(directoryHistoryFolder))
            {
                string xmlFilePath = directoryHistoryFolder + historyDataFileName;
                using (StreamWriter writer = new StreamWriter(xmlFilePath))
                {
                    foreach (var item in FilePathToListView)
                    {
                        writer.WriteLine(item);
                    }
                }
                return;
                //UtilMethod.WarningMessageBox("Saved current folder successfully!", "AutoCAD");
            }
        }
        private void CreateNewTextStyle()
        {
            Document doc = UtilMethod.AcadDoc();
            Database db = UtilMethod.AcadDb();

            try
            {
                using(var trans = db.TransactionManager.StartTransaction())
                {
                    doc.LockDocument();
                    SymbolTable st = (SymbolTable)trans.GetObject(db.TextStyleTableId, OpenMode.ForRead);

                    //Check text style isExisting
                    if(st.Has(TextStyleName))
                    {
                        UtilMethod.WarningMessageBox($"Text style {TextStyleName} is existing already!", "AutoCAD");
                        return;
                    }
                    ObjectId style16ID = st["16"];
                    TextStyleTableRecord style16 = (TextStyleTableRecord)trans.GetObject(style16ID, OpenMode.ForRead);
                
                    //Create new text style base on style 16
                    TextStyleTableRecord newStyle = new TextStyleTableRecord();
                    newStyle.Name = TextStyleName;
                    
                    int textStyleNum = Convert.ToInt32(newStyle.Name);

                    //Clone properties of Style 16
                    newStyle.FileName = style16.FileName;
                    newStyle.BigFontFileName = style16.BigFontFileName;
                    newStyle.FlagBits = style16.FlagBits;
                    newStyle.ObliquingAngle = style16.ObliquingAngle;
                    newStyle.TextSize = 1 * ((double)textStyleNum / originalNumber);
                    
                    //Add new text style to drawing
                    st.UpgradeOpen();
                    ObjectId newTextStyleID = st.Add(newStyle);
                    trans.AddNewlyCreatedDBObject(newStyle, true);

                    //-----------------------Create new dimension style base on style 16
                    DimStyleTable dst = (DimStyleTable)trans.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                    //Check dimension style isExisting
                    if (dst.Has(TextStyleName))
                    {
                        UtilMethod.WarningMessageBox($"Dimension style {TextStyleName} is existing already!", "AutoCAD");
                        return;
                    }
                    ObjectId dimStyleID16 = dst["Scale 16"];
                    DimStyleTableRecord dimStyle16 =  (DimStyleTableRecord)trans.GetObject(dimStyleID16, OpenMode.ForRead);

                    //Create new dimension style base on style 16
                    DimStyleTableRecord newDimStyle = new DimStyleTableRecord();
                    newDimStyle.Name = "Scale " + TextStyleName;
                    //Clone properties of Style 16
                    newDimStyle.CopyFrom(dimStyle16);
                    //newDimStyle.Dimgap = dimStyle16.Dimgap;
                    newDimStyle.Dimscale = textStyleNum;
                    
                    //Ad dimension style to table 
                    dst.UpgradeOpen();
                    ObjectId newDimStyleID = dst.Add(newDimStyle);
                    trans.AddNewlyCreatedDBObject(newDimStyle, true);

                    UtilMethod.WarningMessageBox("Create successfully", "AutoCAD");
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                UtilMethod.WarningMessageBox($"An error occurs: {ex.Message}","Error");
                return;
            }
        }
    }
}
