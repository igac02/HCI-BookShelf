using BookShelf.Models;
using BookShelf.Services;
using BookShelf.ViewModels;
using System.Configuration;
using System.Windows;

namespace BookShelf.Views
{
    /// <summary>
    /// Interaction logic for BookDetailsView.xaml
    /// </summary>
    public partial class BookDetailsView : Window
    {
        public BookDetailsView(Book book, User currentUser)
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString; 
            DataAccess dataAccess = new DataAccess(connectionString);

            this.DataContext = new BookDetailsViewModel(book, currentUser, dataAccess);
        }
    }
}
