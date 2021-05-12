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
    /// Interaction logic for SearchWorkersUserControl.xaml
    /// </summary>
    public partial class SearchWorkersUserControl : UserControl
    {
        BitmapImage bitmapImage = default;
        public SearchWorkersUserControl()
        {
            InitializeComponent();
        }
        public void ClearFields()
        {
            IdWorker.Text = "";
            FullName.Text = "";
            EmailAddress.Text = "";
            ImageWorker.Source = default;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullName.Text;
            string emailAddress = EmailAddress.Text;
            string idWorker = IdWorker.Text;
            if (idWorker.Equals("") && fullName.Equals("") && emailAddress.Equals(""))
            {
                MessageBox.Show("You have to insert at least id ,name or email");
            }
            // Check if id is not empty
            else if(!idWorker.Equals(""))
            {
                // Search only by id
                if (fullName.Equals("") && emailAddress.Equals(""))
                {
                    (Application.Current as App).SearchWorkersViewModel.SearchById(idWorker);
                    ClearFields();
                    MessageBox.Show("The worker " + idWorker);
                }
                // Search by id and fullName
                else if (!fullName.Equals("") && emailAddress.Equals(""))
                {
                    (Application.Current as App).SearchWorkersViewModel.SearchByIdAndFullName(idWorker, fullName);
                    ClearFields();
                    MessageBox.Show("The worker " + idWorker + " " + fullName);
                }
                // Search by id and emailAddress
                else if (fullName.Equals("") && !emailAddress.Equals(""))
                {
                    (Application.Current as App).SearchWorkersViewModel.SearchByIdAndEmail(idWorker, emailAddress);
                    ClearFields();
                    MessageBox.Show("The worker " + idWorker + " " + emailAddress);
                }
                // Search by id and fullName and emailAddress
                else
                {
                    (Application.Current as App).SearchWorkersViewModel.SearchByIdAndFullNameAndEmail(idWorker, fullName, emailAddress);
                    ClearFields();
                    MessageBox.Show("The worker " + idWorker + " " + fullName + " " + emailAddress);
                }

            }
            // Check if fullName is not empty
            else if (!fullName.Equals(""))
            {
                // Search only by fullName
                if (emailAddress.Equals(""))
                {
                    (Application.Current as App).SearchWorkersViewModel.SearchByFullName(fullName);
                    ClearFields();
                    MessageBox.Show("The worker " + fullName);
                }
                // Search by fullName and emailAddress
                else
                {
                    (Application.Current as App).SearchWorkersViewModel.SearchByFullNameAndEmail(fullName, emailAddress);
                    ClearFields();
                    MessageBox.Show("The worker " + fullName + " " + emailAddress);
                }

            }
            // Check if emailAddress is not empty and search only by emailAddress
            else if (!emailAddress.Equals(""))
            {
                (Application.Current as App).SearchWorkersViewModel.SearchByEmail(emailAddress);
                ClearFields();
                MessageBox.Show("The worker " + emailAddress);
            }
        }

        private void WorkerDetailsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            /*indexOfSelectedRow = gd.Items.IndexOf(gd.CurrentItem);*/
            DataRowView rowSelected = gd.SelectedItem as DataRowView;
            if (rowSelected != null)
            {
                IdWorker.Text = rowSelected["Id"].ToString();
                FullName.Text = rowSelected["FullName"].ToString();
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
    }
}
