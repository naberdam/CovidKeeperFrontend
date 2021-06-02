using MaterialDesignThemes.Wpf;
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
    /// Interaction logic for ManageWorkersUserControl.xaml
    /// </summary>
    public partial class ManageWorkersUserControl : UserControl
    {
        int indexOfSelectedRow = -1;
        BitmapImage bitmapImage = default;
        DataRowView rowSelected = default;
        MainWindowTemp mainWindowTemp = default;

        public ManageWorkersUserControl()
        {
            InitializeComponent();
        }
        public void ClearFields()
        {
            indexOfSelectedRow = -1;
            bitmapImage = default;
            rowSelected = default;
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            indexOfSelectedRow = gd.Items.IndexOf(gd.CurrentItem);
            DataRowView rowSelectedNow = gd.SelectedItem as DataRowView;
            string idWorker = rowSelectedNow["Id"].ToString();
            (Application.Current as App).WorkersTableViewModel.DeleteWorker(idWorker, indexOfSelectedRow);
            ClearFields();
            MessageBox.Show("The worker " + idWorker + " deleted successfully");
        }
        private void WorkerDetailsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckChangeOfSelection(sender);
        }
        private void SetImageFromTableToBitmapImage()
        {
            byte[] imageArray = (byte[])rowSelected["Image"];
            if (imageArray.Length == 0)
            {
                bitmapImage = default;
            }
            bitmapImage = LoadImage(imageArray);
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
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string idWorker = IdWorker.Text;
            string emailAddress = EmailAddress.Text;
            string fullName = FullName.Text;
            if (idWorker.Equals("") || fullName.Equals("") || emailAddress.Equals("") || bitmapImage == default)
            {
                MessageBox.Show("You have to insert name, email and image");
            }
            else
            {
                (Application.Current as App).WorkersTableViewModel.InsertWorker(idWorker, fullName, emailAddress, bitmapImage);
                ClearFields();
                MessageBox.Show("The worker " + idWorker + " " + fullName + " uploaded successfully.\nThe email is: " + emailAddress);
                this.IsEnabled = true;
                SetIsEnabled(true);
                DialogHost.CloseDialogCommand.Execute(null, null);
            }
        }

        private void CancelAddWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            this.IsEnabled = true;
            SetIsEnabled(true);
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

        private void WorkerDetailsTable_CurrentCellChanged(object sender, EventArgs e)
        {
            CheckChangeOfSelection(sender);
        }
        private void CheckChangeOfSelection(object sender)
        {
            DataGrid gd = (DataGrid)sender;
            indexOfSelectedRow = gd.Items.IndexOf(gd.CurrentItem);
            DataRowView rowSelectedNow = gd.SelectedItem as DataRowView;
            if (rowSelected == rowSelectedNow)
            {
                return;
            }
            else if (rowSelectedNow != null && rowSelected == default)
            {
                rowSelected = rowSelectedNow;
            }
            else if (rowSelectedNow != null && rowSelected != default)
            {
                string idWorker = rowSelectedNow["Id"].ToString();
                string emailAddress = rowSelectedNow["Fullname"].ToString();
                string fullName = rowSelectedNow["Email_address"].ToString();
                SetImageFromTableToBitmapImage();
                (Application.Current as App).WorkersTableViewModel.UpdateWorkerDetails(idWorker, fullName, emailAddress, bitmapImage, indexOfSelectedRow);
                ClearFields();
                rowSelected = rowSelectedNow;
                MessageBox.Show("The worker " + idWorker + " " + fullName + " updated successfully.\nThe email is: " + emailAddress);
            }
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            SetIsEnabled(false);
        }
        public void SetMainWindow(MainWindowTemp mainWindowTemp)
        {
            this.mainWindowTemp = mainWindowTemp;
        }
        private void SetIsEnabled(bool isEnable)
        {
            if (mainWindowTemp != default)
            {
                mainWindowTemp.IsEnabled = isEnable;
            }
        }
    }
}
