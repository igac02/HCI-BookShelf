using System.Windows;
using BookShelf.Services;
using BookShelf.ViewModels;

namespace BookShelf.Views
{
    /// <summary>
    /// Interaction logic for AdminBooksView.xaml
    /// </summary>
    public partial class AdminBooksView : Window
    {
        public AdminBooksView()
        {
            InitializeComponent();

            string connectionString = "Server=localhost;Port=3306;Database=bookshelf;Uid=root;Pwd=root;";
            DataAccess dataAccess = new DataAccess(connectionString);

            this.DataContext = new AdminBooksViewModel(dataAccess);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Zatvara trenutni prozor
        }
    }
}
