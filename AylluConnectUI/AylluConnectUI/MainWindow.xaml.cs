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

namespace AylluConnectUI
{
    public enum UiPages : int
    {
        SettingsPage = 0,
        TransmitPage = 1
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowModel Model { get; private set;} = new MainWindowModel();
        public MainWindow()
        {
            this.DataContext = this.Model;
            InitializeComponent();
        }
    }
}
