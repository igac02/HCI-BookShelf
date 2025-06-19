using BookShelf.Models;
using BookShelf.Services;
using BookShelf.ViewModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace BookShelf.Views
{
    /// <summary>
    /// Interaction logic for AdminUsersView.xaml
    /// </summary>
    public partial class AdminUsersView : Window
    {
        public AdminUsersView(User loggedInUser)
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            DataAccess dataAccess = new DataAccess(connectionString);

            this.DataContext = new AdminUsersViewModel(dataAccess, loggedInUser);
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the current window
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is AdminUsersViewModel viewModel)
            {
                viewModel.NewPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
