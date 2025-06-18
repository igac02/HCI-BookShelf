using System.Windows;
using BookShelf.Models;
using BookShelf.Services;
using BookShelf.ViewModels;

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

            string connectionString = "Server=localhost;Port=3360;Database=bookshelf;Uid=root;Pwd=root;";
            DataAccess dataAccess = new DataAccess(connectionString);

            this.DataContext = new BookDetailsViewModel(book, currentUser, dataAccess);
        }
    }
}
