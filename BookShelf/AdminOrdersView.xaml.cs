using BookShelf.Services;
using BookShelf.ViewModels;
using System.Configuration;
using System.Windows;

namespace BookShelf.Views
{
    public partial class AdminOrdersView : Window
    {
        public AdminOrdersView()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
            DataAccess dataAccess = new DataAccess(connectionString);

            this.DataContext = new AdminOrdersViewModel(dataAccess);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}