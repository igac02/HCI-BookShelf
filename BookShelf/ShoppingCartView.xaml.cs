using System.Windows;
using BookShelf.Models;
using BookShelf.Service;
using BookShelf.Services;
using BookShelf.ViewModel;
using BookShelf.ViewModels;

namespace BookShelf.Views
{
    /// <summary>
    /// Interaction logic for ShoppingCartView.xaml
    /// </summary>
    public partial class ShoppingCartView : Window
    {
        public ShoppingCartView(DataAccess dataAccess, ShoppingCartService cartService, User currentUser)
        {
            InitializeComponent();
            this.DataContext = new ShoppingCartViewModel(dataAccess, cartService, currentUser);
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the current window
        }
    }
}
