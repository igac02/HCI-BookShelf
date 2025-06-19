using BookShelf.Services;
using BookShelf.ViewModels;
using System.Configuration;
using System.Windows;

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

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            DataAccess dataAccess = new DataAccess(connectionString);

            this.DataContext = new AdminBooksViewModel(dataAccess);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Zatvara trenutni prozor
        }
    }
}
