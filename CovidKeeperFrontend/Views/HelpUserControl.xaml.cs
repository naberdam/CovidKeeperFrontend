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
        public HelpUserControl()
        {
            InitializeComponent();
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.Length - 4);
            string videoPath = path + "CovidKeeperFrontend\\Files\\help_video.mp4";
            string imagePath = path + "CovidKeeperFrontend\\Files\\help_image.jpg";
            HelpVideo.Source = new Uri(videoPath);
            var image = new BitmapImage(new Uri(imagePath));
            QuestionMarkImage.Source = image;
        }
        public void ClearFields()
        {
            try
            {
                HelpVideo.Stop();
            }
            catch (Exception) {}
            
            QuestionMarkImage.Visibility = Visibility.Visible;
            HelpVideo.Visibility = Visibility.Hidden;
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            StopButton.IsEnabled = false;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            QuestionMarkImage.Visibility = Visibility.Hidden;
            HelpVideo.Visibility = Visibility.Visible;
            HelpVideo.Play();
            PlayButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
            StopButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            HelpVideo.Stop();
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            StopButton.IsEnabled = false;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            HelpVideo.Pause();
            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }
    }
}
