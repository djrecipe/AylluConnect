using AbaciAndroid.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace AbaciAndroid.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}