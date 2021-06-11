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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CovidKeeperFrontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowTemp mainWindowTemp = new MainWindowTemp();
            mainWindowTemp.Show();
            this.Close();
            ///WpfDatabase b = new WpfDatabase();
        }

        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            send_message("start");

        }

        private void Button_Click_Add_Worker(object sender, RoutedEventArgs e)
        {
            /*AddWorker view = new AddWorker();
            // Change the boolean type to true for closing the program.

            this.Close();
            // Show the SubMainMenu.
            view.Show();*/
        }

        private void Button_Click_Close(object sender, RoutedEventArgs e)
        {
            send_message("close");
        }

        private void send_message(String message)
        {

            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 8080);
                NetworkStream nwStream = client.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);

                //---send the text---
                Console.WriteLine("Sending : " + message);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                client.Close();
            }
            catch
            {
                // Do nothing
            }
        }

        private void Button_Click_Settings(object sender, RoutedEventArgs e)
        {
            /*send_message("settings");

            Settings view = new Settings();
            // Change the boolean type to true for closing the program.

            this.Close();
            // Show the SubMainMenu.
            view.Show();*/
        }

        private void Button_Click_Delete_Worker(object sender, RoutedEventArgs e)
        {
            /*DeleteWorker view = new DeleteWorker();
            // Change the boolean type to true for closing the program.

            this.Close();
            // Show the SubMainMenu.
            view.Show();*/
        }
    }
}
