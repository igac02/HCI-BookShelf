using System.Windows;
using BookShelf.Service;
using BookShelf.Services;
using BookShelf.Views;

namespace BookShelf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create shared services that the entire application will use.
            var cartService = new ShoppingCartService();
            var navigationService = new NavigationService(cartService);

            // The application starts by showing the LoginView.
            navigationService.ShowLoginWindow();
        }
    }
}
