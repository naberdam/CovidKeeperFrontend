using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        int time_camera = 0;
        int time_send = 0;
        public Settings()
        {
            InitializeComponent();
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            MainWindow view = new MainWindow();
            view.Show();
            this.Close();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        private void send_message(String message)
        {

            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 8082);
                NetworkStream nwStream = client.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);

                //---send the text---
                Console.WriteLine("Sending : " + message);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            }
            catch
            {
                // Do nothing
            }
        }

        private void Button_Click_Time_Cameras(object sender, RoutedEventArgs e)
        {
            send_message("BREAK_CAMERAS:" + time_camera.ToString());
        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Button_Click_Time_Send(object sender, RoutedEventArgs e)
        {
            send_message("TIME_SEND:" + time_send.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Slider_ValueChanged_Time_sends(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            // ... Get Value.
            int value = Convert.ToInt32(slider.Value);
            time_send = value;
        }

        private void Slider_ValueChanged_Time_Cameras(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            // ... Get Value.
            int value = Convert.ToInt32(slider.Value);
            time_camera = value;
        }
    }
}
