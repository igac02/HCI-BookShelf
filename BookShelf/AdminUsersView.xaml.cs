using System.Windows;
using System.Windows.Controls;
using BookShelf.Models;
using BookShelf.Services;
using BookShelf.ViewModel;

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

            string connectionString = "Server=localhost;Port=3306;Database=bookshelf;Uid=root;Pwd=root;";
            DataAccess dataAccess = new DataAccess(connectionString);

            this.DataContext = new AdminUsersViewModel(dataAccess, loggedInUser);
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
