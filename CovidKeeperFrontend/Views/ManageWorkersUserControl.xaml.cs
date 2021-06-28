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
        //Varaible that representing the index of the selected row
        int indexOfSelectedRow = -1;
        //Variable that representing the image that client choose
        BitmapImage bitmapImage = default;
        //Variable that representing the row of datagrid as DataRowView for getting the information from the row
        DataRowView rowViewSelected = default;
        //Variable that representing the MainWindow
        MainMenu mainWindow = default;
        //Variable that representing the row of the datagrid as DataGridRow for getting the details for the DataTemplate
        DataGridRow gridRowSelected = default;
        //Variable that representing the last detailsBtn that the client chose
        Button detailsBtn = default;
        //Bool variables that are used for set IsEnabled of AddButton
        bool idWorkerIsGood = false;
        bool fullNameIsGood = false;
        bool emailAddressIsGood = false;
        bool imageIsGood = false;
        //Variable that representing the id before the client changes it
        string idWorkerInDataTable = default;

        public ManageWorkersUserControl()
        {
            InitializeComponent();
        }

        //Function that reset the values 
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
            //Get the index of the selected row
            indexOfSelectedRow = WorkerDetailsTable.Items.IndexOf(WorkerDetailsTable.CurrentItem);
            DataRowView rowSelectedNow = WorkerDetailsTable.CurrentCell.Item as DataRowView;
            string idWorker = rowSelectedNow["Id"].ToString();
            //Calling the delete function in the view model
            await (Application.Current as App).ManageWorkersViewModel.DeleteWorker(idWorker, indexOfSelectedRow);
            ClearFields();
        }
        
        //Function that convert the image from the table to BitmapImage
        private void SetImageFromTableToBitmapImage(DataRowView row)
        {
            byte[] imageArray = (byte[])row["Image"];
            if (imageArray.Length == 0)
            {
                bitmapImage = default;
            }
            bitmapImage = LoadImage(imageArray);
        }

        //Function that loads image
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
            bool checkIfInsertionSucceed = (Application.Current as App).ManageWorkersViewModel.InsertWorker(idWorker, fullName, 
                emailAddress, bitmapImage);
            //Check if insert worker succeed or not
            if (checkIfInsertionSucceed)
            {
                ClearFields();
                this.IsEnabled = true;
                SetIsEnabled(true);
                //Close DialoHost
                DialogHost.CloseDialogCommand.Execute(null, null);
            }            
        }

        private void CancelAddWorkerButton_Click(object sender, RoutedEventArgs e)
        {
            //Close DialoHost
            DialogHost.CloseDialogCommand.Execute(null, null);
            this.IsEnabled = true;
            SetIsEnabled(true);
        }

        //Function to load image from client's computer
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
                //Set the image that worker's image
                bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                ImageWorker.Source = bitmapImage;
                imageIsGood = true;
                //Check if all values are set properly for IsEnabled of AddButton
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
            //Disable all windows and user controls besides of the DialogHost of adding worker
            this.IsEnabled = false;
            SetIsEnabled(false);
        }

        //Function that sets the mainWindow
        public void SetMainWindow(MainMenu mainWindowTemp)
        {
            this.mainWindow = mainWindowTemp;
        }
        
        //Function that changes the IsEnabled according to the given isEnable
        private void SetIsEnabled(bool isEnable)
        {
            if (mainWindow != default)
            {
                mainWindow.IsEnabled = isEnable;
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
            //Check what exactly the client search
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

        //Function that checks if the email is valid
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
                //The original source is what was clicked.  For example 
                //A button.
                DependencyObject dep = (DependencyObject)e.OriginalSource;

                //Iteratively traverse the visual tree upwards looking for
                //The clicked row.
                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                //If we found the clicked row
                if (dep != null && dep is DataGridRow)
                {
                    //Get the row
                    DataGridRow gridRowSelectedNow = (DataGridRow)dep;
                    Button detailsBtnNow = (Button)sender;
                    //Check if the clients clicked on the same he clicked in the last time
                    if (gridRowSelected == gridRowSelectedNow)
                    {
                        detailsBtn.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowDown };
                        gridRowSelected.DetailsVisibility = Visibility.Collapsed;
                        gridRowSelected = default;
                        detailsBtn = default;
                        return;
                    }
                    //Check if it is the first time that the client clicked
                    else if (gridRowSelectedNow != null && gridRowSelected == default)
                    {
                        detailsBtnNow.Content = new MaterialDesignThemes.Wpf.PackIcon
                        { Kind = MaterialDesignThemes.Wpf.PackIconKind.ArrowUp };
                        detailsBtn = detailsBtnNow;
                        gridRowSelected = gridRowSelectedNow;
                        gridRowSelected.DetailsVisibility = Visibility.Visible;
                    }
                    //Check if the client clicked on a new row not like the last one
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

        //Function for changing the dataTable from search to represent all workers
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).ManageWorkersViewModel.GetWorkersDetailsAfterRefresh();
        }

        //Function that handling the text changing in IdWorker
        private void IdWorker_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.IdWorkerRuleProperty, textBox.Text,
                ref idWorkerIsGood, fullNameIsGood, emailAddressIsGood, imageIsGood, ref AddButton);
        }

        //Function that handling the text changing in FullName
        private void FullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.FullNameRuleProperty, textBox.Text,
                ref fullNameIsGood, emailAddressIsGood, idWorkerIsGood, imageIsGood, ref AddButton);
        }

        //Function that handling the text changing in EmailAddress
        private void EmailAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.EmailAddressRuleProperty, textBox.Text, 
                ref emailAddressIsGood, fullNameIsGood, idWorkerIsGood, imageIsGood, ref AddButton);
        }

        //Function that updates tje IsEnabled of the AddButton only after all boolean variables are true
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
                //Check if all boolean variables are true
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

        //Function that save the update 
        private void SaveUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string idWorker = IdWorkerUpdate.Text;
            string emailAddress = EmailAddressUpdate.Text;
            string fullName = FullNameUpdate.Text;
            SetImageFromTableToBitmapImage(rowViewSelected);
            bool checkIfUpdateSucceed;
            //Call update function from view model
            if (idWorker != idWorkerInDataTable)
            {
                checkIfUpdateSucceed = (Application.Current as App).ManageWorkersViewModel.UpdateWorkerDetailsWithNewId(idWorkerInDataTable, idWorker, fullName, emailAddress, bitmapImage, indexOfSelectedRow);
            }
            else
            {
                checkIfUpdateSucceed = (Application.Current as App).ManageWorkersViewModel.UpdateWorkerDetails(idWorker, fullName, emailAddress, bitmapImage, indexOfSelectedRow);
            }
            if (checkIfUpdateSucceed)
            {
                ClearFields();
                this.IsEnabled = true;
                SetIsEnabled(true);
                //Close DialogHost
                DialogHost.CloseDialogCommand.Execute(null, null);
            }            
        }

        private void CancelSaveUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            //Close DialogHost
            DialogHost.CloseDialogCommand.Execute(null, null);
            this.IsEnabled = true;
            SetIsEnabled(true);
            //Reset textboxes
            IdWorkerUpdate.Text = default;
            FullNameUpdate.Text = default;
            EmailAddressUpdate.Text = default;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            SetIsEnabled(false);
            //Get the row
            rowViewSelected = WorkerDetailsTable.CurrentCell.Item as DataRowView;
            //Get the index of the row that the client choose
            indexOfSelectedRow = WorkerDetailsTable.Items.IndexOf(WorkerDetailsTable.CurrentItem);
            //Reset boolean values for IsEnabled of UpdateButton
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

        //Function that handling the text changing in EmailAddress
        private void EmailAddressUpdate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.EmailAddressUpdateRuleProperty, textBox.Text,
                ref emailAddressIsGood, fullNameIsGood, idWorkerIsGood, imageIsGood, ref SaveUpdateButton);
        }

        //Function that handling the text changing in FullNameUpdate
        private void FullNameUpdate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.FullNameUpdateRuleProperty, textBox.Text,
                ref fullNameIsGood, emailAddressIsGood, idWorkerIsGood, imageIsGood, ref SaveUpdateButton);
        }

        //Function that handling the text changing in IdWorkerUpdate
        private void IdWorkerUpdate_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            UpdateIsEnabledAddButton((Application.Current as App).ManageWorkersViewModel.IdWorkerUpdateRuleProperty, textBox.Text,
                ref idWorkerIsGood, fullNameIsGood, emailAddressIsGood, imageIsGood, ref SaveUpdateButton);
        }
    }
}
