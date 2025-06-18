using System.Windows;
using BookShelf.Services;
using BookShelf.ViewModels;

namespace BookShelf.Views
{
    /// <summary>
    /// Interaction logic for AdminOrdersView.xaml
    /// </summary>
    public partial class AdminOrdersView : Window
    {
        public AdminOrdersView()
        {
            InitializeComponent();

            string connectionString = "Server=localhost;Port=3306;Database=bookshelf;Uid=root;Pwd=root;";
            DataAccess dataAccess = new DataAccess(connectionString);

            this.DataContext = new AdminOrdersViewModel(dataAccess);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the current window
        }
    }
}
