using System;
using System.Collections.Generic;
using System.Text;
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
    /// <summary>
    /// Interaction logic for TransmitPage.xaml
    /// </summary>
    public partial class TransmitPage : Page
    {
        public TransmitPageModel Model { get; private set; } = new TransmitPageModel();
        public TransmitPage()
        {
            this.DataContext = this.Model;
            InitializeComponent();
        }
    }
}
