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
        DataRowView rowViewSelected = default;
        MainWindowTemp mainWindowTemp = default;
        DataGridRow gridRowSelected = default;
        Button detailsBtn = default;

        public ManageWorkersUserControl()
        {
            InitializeComponent();
        }
        public void ClearFields()
        {
            indexOfSelectedRow = -1;
            bitmapImage = default;
            rowViewSelected = default;
            gridRowSelected = default;
            detailsBtn = default;
        }
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            indexOfSelectedRow = gd.Items.IndexOf(gd.CurrentItem);
            DataRowView rowSelectedNow = gd.SelectedItem as DataRowView;
            string idWorker = rowSelectedNow["Id"].ToString();
            await (Application.Current as App).WorkersTableViewModel.DeleteWorker(idWorker, indexOfSelectedRow);
            ClearFields();
        }
        private void WorkerDetailsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckChangeOfSelection(sender);
        }
        private void SetImageFromTableToBitmapImage()
        {
            byte[] imageArray = (byte[])rowViewSelected["Image"];
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
        private async void AddButton_Click(object sender, RoutedEventArgs e)
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
                await (Application.Current as App).WorkersTableViewModel.InsertWorker(idWorker, fullName, emailAddress, bitmapImage);
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
            
            DataRowView rowSelectedNow = gd.CurrentItem as DataRowView;
            //DataRowView rowSelectedNow = gd.SelectedItem as DataRowView;
            if (rowViewSelected == rowSelectedNow)
            {
                return;
            }
            else if (rowSelectedNow != null && rowViewSelected == default)
            {
                rowViewSelected = rowSelectedNow;
                indexOfSelectedRow = gd.Items.IndexOf(gd.CurrentItem);
            }
            else if (rowSelectedNow != null && rowViewSelected != default)
            {
                string idWorker = rowViewSelected["Id"].ToString();
                string emailAddress = rowViewSelected["Email_address"].ToString();
                string fullName = rowViewSelected["Fullname"].ToString();
                SetImageFromTableToBitmapImage();
                (Application.Current as App).WorkersTableViewModel.UpdateWorkerDetails(idWorker, fullName, emailAddress, bitmapImage, indexOfSelectedRow);
                indexOfSelectedRow = gd.Items.IndexOf(gd.CurrentItem);
                ClearFields();
                rowViewSelected = rowSelectedNow;
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // the original source is what was clicked.  For example 
                // a button.
                DependencyObject dep = (DependencyObject)e.OriginalSource;

                // iteratively traverse the visual tree upwards looking for
                // the clicked row.
                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                // if we found the clicked row
                if (dep != null && dep is DataGridRow)
                {
                    // get the row
                    DataGridRow gridRowSelectedNow = (DataGridRow)dep;
                    Button detailsBtnNow = (Button)sender;
                    if (gridRowSelected == gridRowSelectedNow)
                    {
                        detailsBtn.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown };
                        gridRowSelected.DetailsVisibility = Visibility.Collapsed;
                        gridRowSelected = default;
                        detailsBtn = default;
                        return;
                    }
                    else if (gridRowSelectedNow != null && gridRowSelected == default)
                    {
                        detailsBtnNow.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp };
                        detailsBtn = detailsBtnNow;
                        gridRowSelected = gridRowSelectedNow;
                        gridRowSelected.DetailsVisibility = Visibility.Visible;
                    }
                    else if (gridRowSelectedNow != null && gridRowSelected != default)
                    {
                        detailsBtn.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown };
                        gridRowSelected.DetailsVisibility = Visibility.Collapsed;
                        gridRowSelected = gridRowSelectedNow;
                        gridRowSelected.DetailsVisibility = Visibility.Visible;
                        detailsBtnNow.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp };
                        detailsBtn = detailsBtnNow;
                    }

                    // change the details visibility
                    /*if (gridRowSelected.DetailsVisibility == Visibility.Collapsed)
                    {
                        gridRowSelected.DetailsVisibility = Visibility.Visible;
                    }
                    else
                    {
                        gridRowSelected.DetailsVisibility = Visibility.Collapsed;
                    }*/
                }
            }
            catch (System.Exception)
            {
            }
        }
    }
}
