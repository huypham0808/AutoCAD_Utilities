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
        public ICommand CreateDimStyleCommand { get; }
        public ICommand CreateMultileaderCommand { get; }
        public ICommand CreatAllCommands { get; }

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
                if(ResultText == "Text will be converted to UPPER here!" || StatusCreateTextStyle == TextStyleFail || StatusCreateDimStyle == DimStyleFail)
                {
                    _colorTextOriginal = Brushes.Blue;
                }
                else
                {
                    _colorTextOriginal = Brushes.Pink;
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
        private string _statusCreateTextStyle;

        public string StatusCreateTextStyle
        {
            get { return _statusCreateTextStyle; }
            set 
            { 
                _statusCreateTextStyle = value;
                OnPropertyChanged(nameof(StatusCreateTextStyle));
            }
        }
        private string _statusCreateDimStyle;

        public string StatusCreateDimStyle
        {
            get { return _statusCreateDimStyle; }
            set { _statusCreateDimStyle = value; OnPropertyChanged(nameof(StatusCreateDimStyle)); }
        }


        private string _textStyleDone = "New Text style was created successfully!";
        private string _dimStyleDone = "New Dimension style was created successfully!";
        private string _multileaderStyleDone = "New Multileader style was created successfully!";

        public string TextStyleDone
        {
            get { return _textStyleDone; }
            set { _textStyleDone = value; OnPropertyChanged(nameof(TextStyleDone)); }
        }
        public string DimStyleDone
        {
            get { return _dimStyleDone; }
            set { _dimStyleDone = value; OnPropertyChanged(nameof(DimStyleDone)); }
        }
        public string MultileaderStyleDone
        {
            get { return _multileaderStyleDone; }
            set { _multileaderStyleDone = value; OnPropertyChanged(nameof(MultileaderStyleDone)); }
        }
        private string _textStyleFail = "Text style wasn't created!";
        private string _dimStyleFail = "Dimension style wasn't created!";
        private string _multileaderFail = "Multileader style wasn't created!";

        public string TextStyleFail
        {
            get { return _textStyleFail; }
            set { _textStyleFail = value; OnPropertyChanged(nameof(TextStyleFail)); }
        }


        public string DimStyleFail
        {
            get { return _dimStyleFail; }
            set { _dimStyleFail = value; OnPropertyChanged(nameof(DimStyleFail)); }
        }


        public string MultileaderFail
        {
            get { return _multileaderFail; }
            set { _multileaderFail = value; OnPropertyChanged(nameof(MultileaderFail)); }
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
            CreateDimStyleCommand = new RelayCommand(CreateNewDimStyle);
            CreateMultileaderCommand = new RelayCommand(CreateNewMultileader);
            LoadDataFromXmlFile();
            StatusCreateTextStyle = "None!";
            StatusCreateDimStyle = "None!";
            CreatAllCommands = new RelayCommand(CreateAlls);
            CreateMultileaderCommand = new RelayCommand(CreateNewMultileader);
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
                        StatusCreateTextStyle = TextStyleFail;
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
                    trans.Commit();
                    UtilMethod.WarningMessageBox($"Text style {TextStyleName} was created successfully!", "AutoCAD");
                    StatusCreateTextStyle = TextStyleDone;
                    db.Textstyle = newStyle.ObjectId;
                }
            }
            catch (Exception ex)
            {
                UtilMethod.WarningMessageBox($"An error occurs: {ex.Message}","Error");
                return;
            }
        }
        private void CreateNewDimStyle()
        {
            Document doc = UtilMethod.AcadDoc();
            Database db = UtilMethod.AcadDb();

            try
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    doc.LockDocument();
                    // Open the DimStyle table for read
                    DimStyleTable dst = (DimStyleTable)trans.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                    string strDimStyleName = "Scale " + TextStyleName;

                    DimStyleTableRecord acDimStyleTblRec;
                    ObjectId dimStyleID16 = dst["Scale 16"];
                    DimStyleTableRecord dimStyle16 = (DimStyleTableRecord)trans.GetObject(dimStyleID16, OpenMode.ForRead);

                    // Check to see if the dimension style exists or not
                    if (dst.Has(strDimStyleName) == false)
                    {
                        if (dst.IsWriteEnabled == false) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite);

                        acDimStyleTblRec = new DimStyleTableRecord();
                        acDimStyleTblRec.Name = strDimStyleName;

                        dst.Add(acDimStyleTblRec);
                        trans.AddNewlyCreatedDBObject(acDimStyleTblRec, true);
                    }
                    else
                    {
                        acDimStyleTblRec = trans.GetObject(dst[strDimStyleName],
                                                                OpenMode.ForWrite) as DimStyleTableRecord;
                    }
                    acDimStyleTblRec.CopyFrom(dimStyle16);
                    acDimStyleTblRec.Name = strDimStyleName;
                    int textStyleNum = Convert.ToInt32(TextStyleName);
                    acDimStyleTblRec.Dimscale = textStyleNum;
                    dimStyle16.Dispose();
                    trans.Commit();
                    UtilMethod.WarningMessageBox("Done", "AutoCAD");
                    StatusCreateDimStyle = DimStyleDone;
                    db.Dimstyle = acDimStyleTblRec.ObjectId;
                }
            }
            catch (Exception ex)
            {
                UtilMethod.WarningMessageBox($"An error occurs: {ex.Message}", "Error");
                StatusCreateDimStyle = DimStyleFail;
                return;
            }
        }
        private void CreateNewMultileader()
        {
            Document doc = UtilMethod.AcadDoc();
            Database db = UtilMethod.AcadDb();
            double textStyleNum = Convert.ToInt32(TextStyleName);
            try
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
 
                    ObjectId mlSTableId = db.MLeaderStyleDictionaryId;
                    DBDictionary mlSTable = (DBDictionary)trans.GetObject(mlSTableId, OpenMode.ForRead);

                    if (mlSTable.Contains(TextStyleName))
                    {
                        MessageBox.Show("Create multileader fail!", "AutoCAD Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    ObjectId standardMLeaderStyleId = (ObjectId)mlSTable["16"];
                    MLeaderStyle standardMLeaderStyle = (MLeaderStyle)trans.GetObject(standardMLeaderStyleId, OpenMode.ForRead);
                    //add a new mleader style...
                    MLeaderStyle newMleaderStyle = new MLeaderStyle();
                    newMleaderStyle.Name = TextStyleName;

                    newMleaderStyle.ContentType = standardMLeaderStyle.ContentType;
                    newMleaderStyle.ContentType = standardMLeaderStyle.ContentType;
                    newMleaderStyle.EnableDogleg = standardMLeaderStyle.EnableDogleg;
                    newMleaderStyle.LeaderLineColor = standardMLeaderStyle.LeaderLineColor;
                    newMleaderStyle.LeaderLineTypeId = standardMLeaderStyle.LeaderLineTypeId;
                    newMleaderStyle.LeaderLineWeight = standardMLeaderStyle.LeaderLineWeight;
                    newMleaderStyle.LeaderLineType = standardMLeaderStyle.LeaderLineType;
                    newMleaderStyle.TextAlignmentType = standardMLeaderStyle.TextAlignmentType;
                    newMleaderStyle.TextColor = standardMLeaderStyle.TextColor;
                    newMleaderStyle.Scale = standardMLeaderStyle.Scale;

                    ObjectId textStyleTableId = db.TextStyleTableId;
                    TextStyleTable textStyleTable = (TextStyleTable)trans.GetObject(textStyleTableId, OpenMode.ForRead);
                    ObjectId textStyleID = textStyleTable[TextStyleName];
                    newMleaderStyle.TextStyleId = textStyleID;
                    newMleaderStyle.MaxLeaderSegmentsPoints = 5;
                    newMleaderStyle.LandingGap = 1 / 2 * (textStyleNum / 16);
                    newMleaderStyle.ArrowSize = 1.25 * (textStyleNum / 16);
                    newMleaderStyle.ArrowSymbolId = standardMLeaderStyle.ArrowSymbolId;
                    newMleaderStyle.BreakSize = 11 / 16 * (textStyleNum / 16);
                    newMleaderStyle.ContentType = standardMLeaderStyle.ContentType;
                    newMleaderStyle.DoglegLength = 2 * textStyleNum / 16;
                    ObjectId mleaderStyleId = newMleaderStyle.PostMLeaderStyleToDb(db, TextStyleName);
                    mlSTable.UpgradeOpen();
                    trans.AddNewlyCreatedDBObject(newMleaderStyle, true);
                    mleaderStyleId = mlSTable.GetAt(TextStyleName);
                    trans.Commit();
                    MessageBox.Show("Create multileader!", "AutoCAD Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                UtilMethod.WarningMessageBox($"An error occurs: {ex.Message}", "Error");
                return;
            }
 
        }
        private void CreateAlls()
        {
            CreateNewTextStyle();
            CreateNewDimStyle();
            //CreateNewMultileader();
        }
    }
}
