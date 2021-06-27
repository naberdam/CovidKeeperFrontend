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
    /// Interaction logic for HelpUserControl.xaml
    /// </summary>
    public partial class HelpUserControl : UserControl
    {
        //Const variables for the location of the files
        private const string fileLocation = "CovidKeeperFrontend\\Files\\";
        private const string helpVideoName = "help_video.mp4";
        private const string helpImageName = "help_image.jpg";
        public HelpUserControl()
        {
            InitializeComponent();
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.Length - 4);
            string videoPath = path + fileLocation + helpVideoName;
            string imagePath = path + fileLocation + helpImageName;
            var image = new BitmapImage(new Uri(imagePath));
            //Initial QuestionMarkImage and HelpVideo
            HelpVideo.Source = new Uri(videoPath);
            QuestionMarkImage.Source = image;
        }

        //Function that reset the values 
        public void ClearFields()
        {
            try
            {
                HelpVideo.Stop();
            }
            catch (Exception) {}
            
            QuestionMarkImage.Visibility = Visibility.Visible;
            HelpVideo.Visibility = Visibility.Hidden;
            //Set PlayButton the IsEnabled to true and the others to false
            UpdateIsEnabledButtons(true, false, false);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            QuestionMarkImage.Visibility = Visibility.Hidden;
            HelpVideo.Visibility = Visibility.Visible;
            HelpVideo.Play();
            //Set PlayButton the IsEnabled to false and the others to true
            UpdateIsEnabledButtons(false, true, true);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            HelpVideo.Stop();
            //Set PlayButton the IsEnabled to true and the others to false
            UpdateIsEnabledButtons(true, false, false);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            HelpVideo.Pause();
            //Set PauseButton the IsEnabled to false and the others to true
            UpdateIsEnabledButtons(true, false, true);
        }
        private void UpdateIsEnabledButtons(bool playBool, bool pauseBool, bool stopBool)
        {
            PlayButton.IsEnabled = playBool;
            PauseButton.IsEnabled = pauseBool;
            StopButton.IsEnabled = stopBool;
        }
    }
}
