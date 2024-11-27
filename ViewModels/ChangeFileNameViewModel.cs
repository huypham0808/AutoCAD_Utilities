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
        private string directoryHistoryFolder = @"C:\FRP-ST-SST-Plugin\Data\Data Project Path";
        private string historyDataFileName = "\\History project folder directory.xml";       

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
            }
            else
            {
                MessageBox.Show("No folder selected!", "AutoCAD", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void PushFileToSDrive()
        {
            if(string.IsNullOrEmpty(CurrentLocalFilePath))
            {
                UtilMethod.WarningMessageBox("Please get Current local drive!", "AutoCAD");
                return;
            }
            if (string.IsNullOrEmpty(SDriveCompanyPath))
            {
                UtilMethod.WarningMessageBox(@"Please get S:\ ...drive!", "AutoCAD");
                return;
            }
            string sourceDir = CurrentLocalFilePath;
            string fileName = Path.GetFileName(sourceDir);
            string sDriveFolderPath = SDriveCompanyPath + "\\" + fileName;
            string fullXmlFilePath = directoryHistoryFolder + historyDataFileName;
            try
            {
                if (File.Exists(sDriveFolderPath))
                {
                    File.Copy(sourceDir, sDriveFolderPath, true);
                    UtilMethod.WarningMessageBox("Push file done", "AutoCAD");
                }
                else
                {
                    File.Copy(sourceDir, sDriveFolderPath);
                    UtilMethod.WarningMessageBox("Push file done", "AutoCAD");
                }
            }
            catch
            {
                UtilMethod.WarningMessageBox("File is opening by another", "AutoCAD");
            }
            if(!FilePathToListView.Contains(SDriveCompanyPath))
            {
                if(FilePathToListView.Count < 3)
                {
                    FilePathToListView.Add(SDriveCompanyPath);
                }
                else
                {
                    FilePathToListView.RemoveAt(0);
                    FilePathToListView.Add(SDriveCompanyPath);
                }
                if (Directory.Exists(directoryHistoryFolder))
                {
                    UtilMethod.AppendTextToXmlFile(fullXmlFilePath, SDriveCompanyPath);
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(directoryHistoryFolder);
                    UtilMethod.AppendTextToXmlFile(fullXmlFilePath, SDriveCompanyPath);
                }        
            }
            OnPropertyChanged(nameof(FilePathToListView));
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
    }
}
