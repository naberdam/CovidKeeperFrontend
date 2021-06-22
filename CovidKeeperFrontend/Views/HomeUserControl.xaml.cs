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

namespace CovidKeeperFrontend.Views
{
    /// <summary>
    /// Interaction logic for HomeUserControl.xaml
    /// </summary>
    public partial class HomeUserControl : UserControl
    {
        public HomeUserControl()
        {
            InitializeComponent();
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.Length - 4);
            path += "CovidKeeperFrontend\\people_with_mask.jpg";
            PeopleClickOnButton.Source = new BitmapImage(new Uri(path));
        }

        private async void ActiveButton_Click(object sender, RoutedEventArgs e)
        {
            await (Application.Current as App).HomeViewModel.ActiveButtonClicked();
        }
    }
}
