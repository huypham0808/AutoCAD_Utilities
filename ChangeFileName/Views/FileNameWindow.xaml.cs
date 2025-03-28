using System;
using System.Collections.Generic;
using System.IO;
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
using ChangeFileName.ViewModels;
using ChangeFileName.Utilities;

namespace ChangeFileName.Views
{
    /// <summary>
    /// Interaction logic for FileNameWindow.xaml
    /// </summary>
    public partial class FileNameWindow : Window
    {
        private readonly string directoryHistoryFolder = @"C:\FRP-ST-SST-Plugin\Data\Data Project Path";
        private readonly string historyDataFileName = "\\History project folder directory.xml";
        public FileNameWindow()
        {
            InitializeComponent();
            DataContext = new ChangeFileNameViewModel();           
        }
        private void ListViewHistoryPath_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                if(DataContext is ChangeFileNameViewModel changeFileNameViewModel)
                {
                    changeFileNameViewModel.SDriveCompanyPath = e.AddedItems[0] as string;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to close the window?", "Close Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true; // Cancel the closing event
            }
            else if (result == MessageBoxResult.Yes)
            {
                if (Directory.Exists(directoryHistoryFolder))
                {
                    string xmlFilePath = directoryHistoryFolder + historyDataFileName;
                    using (StreamWriter writer = new StreamWriter(xmlFilePath))
                    {
                        if (DataContext is ChangeFileNameViewModel changeFileNameViewModel)
                        {
                            foreach (var item in changeFileNameViewModel.FilePathToListView)
                            {
                                writer.WriteLine(item);
                            }
                        }
                    }                                
                }
                return;
            }
        }
    }
}
