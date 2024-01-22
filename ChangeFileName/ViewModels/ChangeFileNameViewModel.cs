using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using ChangeFileName.ViewModels;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.ComponentModel;

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
        private string _NewFileName;
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

        public ChangeFileNameViewModel()
        {
            ChangeFileNameCommand = new RelayCommand(ChangeShopDrawingFile);
        }
        private void ChangeShopDrawingFile()
        {

            MessageBox.Show("Hello" + NewFileName);
        }

    }
}
