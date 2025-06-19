using System.Windows;
using System.Configuration;
using BookShelf.Models;
using BookShelf.Service;
using BookShelf.Services;
using BookShelf.ViewModel;
using BookShelf.ViewModels;

namespace BookShelf.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(User loggedInUser, ShoppingCartService cartService, NavigationService navigationService)
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            DataAccess dataAccess = new DataAccess(connectionString);

            this.DataContext = new MainViewModel(dataAccess, cartService, navigationService, loggedInUser);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BooksDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
