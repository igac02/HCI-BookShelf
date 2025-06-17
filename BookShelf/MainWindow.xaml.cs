using System.Windows;
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

            string connectionString = "Server=localhost;Database=bookshelf;Uid=root;Pwd=root;";
            DataAccess dataAccess = new DataAccess(connectionString);

            this.DataContext = new MainViewModel(dataAccess, cartService, navigationService, loggedInUser);
        }
    }
}
