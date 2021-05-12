using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Interaction logic for WorkersTableUserControl.xaml
    /// </summary>
    public partial class WorkersTableUserControl : UserControl
    {
        int indexOfSelectedRow = -1;
        BitmapImage bitmapImage = default;
        public WorkersTableUserControl()
        {
            InitializeComponent();
        }
        public void ClearFields()
        {
            IdWorker.Text = "";
            FullName.Text = "";
            EmailAddress.Text = "";
            ImageWorker.Source = default;
            indexOfSelectedRow = -1;
            bitmapImage = default;            
        }

        private void WorkerDetailsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            indexOfSelectedRow = gd.Items.IndexOf(gd.CurrentItem);
            DataRowView rowSelected = gd.SelectedItem as DataRowView;
            if (rowSelected != null)
            {
                IdWorker.Text = rowSelected["Id"].ToString();
                FullName.Text = rowSelected["Fullname"].ToString();
                EmailAddress.Text = rowSelected["Email_address"].ToString();
                byte[] imageArray = (byte[])rowSelected["Image"];
                if (imageArray.Length == 0)
                {
                    ImageWorker.Source = default;
                }
                else
                {
                    bitmapImage = LoadImage(imageArray);
                    ImageWorker.Source = bitmapImage;
                }
            }
        }
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullName.Text;
            string emailAddress = EmailAddress.Text;
            string idWorker = IdWorker.Text;
            if (idWorker.Equals("") || fullName.Equals("") || emailAddress.Equals("") || bitmapImage == default || indexOfSelectedRow == -1)
            {
                MessageBox.Show("You have to insert name, email and image");
            }
            else
            {
                (Application.Current as App).WorkersTableViewModel.UpdateWorkerDetails(idWorker, fullName, emailAddress, bitmapImage, indexOfSelectedRow);
                ClearFields();
                MessageBox.Show("The worker " + idWorker + " " + fullName + " updated successfully.\nThe email is: " + emailAddress);                
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullName.Text;
            string emailAddress = EmailAddress.Text;
            string idWorker = IdWorker.Text;
            if (idWorker.Equals("") || fullName.Equals("") || emailAddress.Equals("") || bitmapImage == default)
            {
                MessageBox.Show("You have to insert name, email and image");
            }
            else
            {
                (Application.Current as App).WorkersTableViewModel.InsertWorker(idWorker, fullName, emailAddress, bitmapImage);
                ClearFields();
                MessageBox.Show("The worker " + idWorker + " " + fullName + " uploaded successfully.\nThe email is: " + emailAddress);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = @"Select a picture";
            openFileDialog.Filter = @"All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                ImageWorker.Source = bitmapImage;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullName.Text;
            string emailAddress = EmailAddress.Text;
            string idWorker = IdWorker.Text;
            if (idWorker.Equals("") || fullName.Equals("") || emailAddress.Equals("") || bitmapImage == default || indexOfSelectedRow == -1)
            {
                MessageBox.Show("You have to insert name, email and image");
            }
            else
            {
                (Application.Current as App).WorkersTableViewModel.DeleteWorker(idWorker, indexOfSelectedRow);
                ClearFields();
                MessageBox.Show("The worker " + idWorker + " " + fullName + " deleted successfully.\nThe email is: " + emailAddress);
            }
        }
    }
}
