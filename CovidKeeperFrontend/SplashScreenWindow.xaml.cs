using CovidKeeperFrontend.HelperClasses;
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
using System.Windows.Shapes;

namespace CovidKeeperFrontend
{
    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : Window
    {
        private const string LOAD_SCREEN_IMAGE_NAME = "load_screen_image.jpeg";
        public SplashScreenWindow()
        {
            InitializeComponent();
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.Length - 4);
            var image = new BitmapImage(new Uri(path + GlobalVariables.FILES_FOLDER_PATH + LOAD_SCREEN_IMAGE_NAME));
            ImageLoadScreen.ImageSource = image;
        }

        public double Progress
        {
            get { return progressBar.Value; }
            set { progressBar.Value = value; }
        }
    }
}
