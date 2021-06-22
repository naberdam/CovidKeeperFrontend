using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        bool idWorkerIsGood = false;
        bool fullNameIsGood = false;
        bool emailAddressIsGood = false;
        bool imageIsGood = false;
        string idWorkerInDataTable = default;

        public ManageWorkersUserControl()
        {
            InitializeComponent();
        }
        public void ClearFields()
        {
            indexOfSelectedRow = -1;
            bitmapImage = default;
            if (gridRowSelected != default)
            {
                gridRowSelected.DetailsVisibility = Visibility.Collapsed;
            }
            rowViewSelected = default;
            gridRowSelected = default;
            detailsBtn = default;
            idWorkerIsGood = false;
            fullNameIsGood = false;
            emailAddressIsGood = false;
            imageIsGood = false;
        }
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            indexOfSelectedRow = gd.Items.IndexOf(gd.CurrentItem);
            DataRowView rowSelectedNow = gd.SelectedItem as DataRowView;
            string idWorker = rowSelectedNow["Id"].ToString();
            await (Application.Current as App).ManageWorkersViewModel.DeleteWorker(idWorker, indexOfSelectedRow);
            ClearFields();
        }
        
        private void SetImageFromTableToBitmapImage(DataRowView row)
        {
            byte[] imageArray = (byte[])row["Image"];
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
                await (Application.Current as App).ManageWorkersViewModel.InsertWorker(idWorker, fullName, emailAddress, bitmapImage);
                ClearFields();
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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = @"Select a picture",
                Filter = @"All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                ImageWorker.Source = bitmapImage;
                imageIsGood = true;
                if (fullNameIsGood && emailAddressIsGood && idWorkerIsGood)
                {
                    AddButton.IsEnabled = true;
                }
                else
                {
                    AddButton.IsEnabled = false;
                }
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
            string searchString = IdWorkerSearch.Text;
            if (searchString == "")
            {
                MessageBox.Show("Please enter a word for the search");
                return;
            }
            //CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
            if (Regex.IsMatch(searchString, @"^[a-zA-Z ]+$"))
            {
                (Application.Current as App).ManageWorkersViewModel.SearchByFullName(searchString);
            }
            else if (IsValidEmail(searchString))
            {
                (Application.Current as App).ManageWorkersViewModel.SearchByEmail(searchString);
            }
            else if (Regex.IsMatch(searchString, @"^[0-9_]+$"))
            {
                (Application.Current as App).ManageWorkersViewModel.SearchById(searchString);
            }
            else
            {
                MessageBox.Show("The value you entered was not recognized as an ID, full name or email address." +
                    "\nPlease re-enter the word you are looking for.");
            }
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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
                }
            }
            catch (System.Exception)
            {
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ManageWorkersViewModel.GetWorkersDetailsAfterRefresh();
        }

        private void IdWorker_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.IdWorkerRuleProperty, textBox.Text,
                ref idWorkerIsGood, fullNameIsGood, emailAddressIsGood, imageIsGood, ref AddButton);
        }

        private void FullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.FullNameRuleProperty, textBox.Text,
                ref fullNameIsGood, emailAddressIsGood, idWorkerIsGood, imageIsGood, ref AddButton);
        }

        private void EmailAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.EmailAddressRuleProperty, textBox.Text, 
                ref emailAddressIsGood, fullNameIsGood, idWorkerIsGood, imageIsGood, ref AddButton);
        }
        private void UpdateIsEnabledAddButton(string userDetailsProperty, string textBox, ref bool myValue, bool oneValue, 
            bool secondValue, bool thirdValue, ref Button buttonToEnable)
        {
            if (userDetailsProperty == null)
            {
                buttonToEnable.IsEnabled = false;
            }
            else if (userDetailsProperty.Length != textBox.Length)
            {
                buttonToEnable.IsEnabled = false;
            }
            else
            {
                myValue = true;
                if (oneValue && secondValue && thirdValue)
                {
                    buttonToEnable.IsEnabled = true;
                }
                else
                {
                    buttonToEnable.IsEnabled = false;
                }
            }
        }

        private void EditImageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string idWorker = IdWorkerUpdate.Text;
            string emailAddress = EmailAddressUpdate.Text;
            string fullName = FullNameUpdate.Text;
            SetImageFromTableToBitmapImage(rowViewSelected);
            (Application.Current as App).ManageWorkersViewModel.UpdateWorkerDetails(idWorkerInDataTable, idWorker, fullName, emailAddress, bitmapImage, indexOfSelectedRow);
            ClearFields();
            this.IsEnabled = true;
            SetIsEnabled(true);
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void CancelSaveUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            this.IsEnabled = true;
            SetIsEnabled(true);
            IdWorkerUpdate.Text = default;
            FullNameUpdate.Text = default;
            EmailAddressUpdate.Text = default;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            SetIsEnabled(false);
            rowViewSelected = WorkerDetailsTable.CurrentCell.Item as DataRowView;
            indexOfSelectedRow = WorkerDetailsTable.Items.IndexOf(WorkerDetailsTable.CurrentItem);
            idWorkerIsGood = false;
            fullNameIsGood = false;
            emailAddressIsGood = false;
            SetImageFromTableToBitmapImage(rowViewSelected);
            imageIsGood = true;
            IdWorkerUpdate.Text = rowViewSelected["Id"].ToString();
            FullNameUpdate.Text = rowViewSelected["Fullname"].ToString();
            EmailAddressUpdate.Text = rowViewSelected["Email_address"].ToString();            
            ImageWorkerUpdate.Source = bitmapImage;
            idWorkerInDataTable = rowViewSelected["Id"].ToString();
        }

        private void EmailAddressUpdate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.EmailAddressUpdateRuleProperty, textBox.Text,
                ref emailAddressIsGood, fullNameIsGood, idWorkerIsGood, imageIsGood, ref SaveUpdateButton);
        }

        private void FullNameUpdate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.FullNameUpdateRuleProperty, textBox.Text,
                ref fullNameIsGood, emailAddressIsGood, idWorkerIsGood, imageIsGood, ref SaveUpdateButton);
        }

        private void IdWorkerUpdate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.IdWorkerUpdateRuleProperty, textBox.Text,
                ref idWorkerIsGood, fullNameIsGood, emailAddressIsGood, imageIsGood, ref SaveUpdateButton);
        }
    }
}
