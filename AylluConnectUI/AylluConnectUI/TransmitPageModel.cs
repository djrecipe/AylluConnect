using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AylluConnectUI
{
    public class SettingsPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            return;
        }
    }
}
