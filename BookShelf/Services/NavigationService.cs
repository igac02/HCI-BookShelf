using BookShelf.Models;
using BookShelf.Service;
using BookShelf.Services;
using BookShelf.Views;
using System.Windows;

namespace BookShelf.Services
{
    /// <summary>
    /// A simple service to handle window navigation.
    /// </summary>
    public class NavigationService
    {
        private readonly ShoppingCartService _cartService;
        private readonly DataAccess _dataAccess;

        private const string ConnectionString = "Server=localhost;Database=bookshelf;id=root;Pwd=root;";

        public NavigationService(ShoppingCartService cartService)
        {
            _cartService = cartService;
            _dataAccess = new DataAccess(ConnectionString);
        }

        public void ShowMainWindow(User loggedInUser, Window currentWindowToClose)
        {
            var mainWindow = new MainWindow(loggedInUser, _cartService, this);
            mainWindow.Show();
            currentWindowToClose.Close();
        }

        public void ShowShoppingCart(User currentUser)
        {
            var cartWindow = new ShoppingCartView(_dataAccess, _cartService, currentUser);
            cartWindow.ShowDialog();
        }

        /// <summary>
        /// Opens the registration window.
        /// </summary>
        public void ShowRegistrationWindow()
        {
            var registrationView = new RegistrationView(_dataAccess, this);
            registrationView.ShowDialog(); // Show as a dialog
        }

        public void ShowAdminBooks()
        {
            var adminWindow = new AdminBooksView();
            adminWindow.Show();
        }

        public void ShowAdminUsers(User loggedInUser)
        {
            var adminWindow = new AdminUsersView(loggedInUser);
            adminWindow.Show();
        }

        public void ShowAdminOrders()
        {
            var adminWindow = new AdminOrdersView();
            adminWindow.Show();
        }

        public void ShowLoginWindow()
        {
            var loginView = new LoginView(this);
            loginView.Show();
        }
    }
}
