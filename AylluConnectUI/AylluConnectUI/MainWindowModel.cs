using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System.IO;

namespace AylluConnectUI
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Uri _CurrentPageUri = new Uri("SettingsPage.xaml", UriKind.Relative);
        public Uri CurrentPageUri
            {
            get
            {
                return this._CurrentPageUri;
            }
            set
            {
                this._CurrentPageUri = value;
                this.OnPropertyChanged(nameof(this.CurrentPageUri));
            }
        }
        public ICommand CommandNavigateToPage { get; private set;}
        public MainWindowModel()
        {

            this.CommandNavigateToPage = new RelayCommand<UiPages>(this.ExecuteCommandCommandNavigateToPage, this.CanExecuteCommandNavigateToPage);
        }
        private bool CanExecuteCommandNavigateToPage(UiPages page)
        {
            return true;
        }
        private void ExecuteCommandCommandNavigateToPage(UiPages page)
        {
            switch(page)
            {
                case UiPages.SettingsPage:
                    this.CurrentPageUri = new Uri("SettingsPage.xaml", UriKind.Relative);
                    break;
                case UiPages.TransmitPage:
                    this.CurrentPageUri = new Uri("TransmitPage.xaml", UriKind.Relative);
                    break;

            }
        }
        private void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            return;
        }

    }
}
