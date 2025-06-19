using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookShelf.Core;
using BookShelf.Models;
using BookShelf.Services;

namespace BookShelf.ViewModels
{
    public class AdminBooksViewModel : ViewModelBase
    {
        private readonly DataAccess _dataAccess;

        #region Bound Properties

        private ObservableCollection<Book> _books;
        public ObservableCollection<Book> Books
        {
            get => _books;
            set { _books = value; OnPropertyChanged(); }
        }

        private Book _selectedBook;
        /// <summary>
        /// Represents the currently selected book in the DataGrid.
        /// When a book is selected, its details are copied to the form fields.
        /// </summary>
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                // Copy selected book data to the form fields for editing
                CopyBookToForm(_selectedBook);
                OnPropertyChanged();
            }
        }

        // Properties for the Add/Edit form fields
        private Book _bookInForm;
        public Book BookInForm
        {
            get => _bookInForm;
            set { _bookInForm = value; OnPropertyChanged(); }
        }

        // Collections for ComboBoxes
        public ObservableCollection<Author> Authors { get; set; }
        public ObservableCollection<Publisher> Publishers { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        #endregion

        #region Commands
        public ICommand AddNewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        #endregion

        public AdminBooksViewModel()
        {
            // Design-time data
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Books = new ObservableCollection<Book> { new Book { Title = "Sample Book", Price = 25 } };
                BookInForm = new Book();
            }
        }

        public AdminBooksViewModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            BookInForm = new Book();

            AddNewCommand = new RelayCommand(AddNewBook);
            SaveCommand = new RelayCommand(SaveBook, CanSaveBook);
            DeleteCommand = new RelayCommand(DeleteBook, CanDeleteBook);

            LoadData();
        }

        private void LoadData()
        {
            Books = new ObservableCollection<Book>(_dataAccess.GetBooks());
            Authors = new ObservableCollection<Author>(_dataAccess.GetAuthors());
            Publishers = new ObservableCollection<Publisher>(_dataAccess.GetPublishers());
            Categories = new ObservableCollection<Category>(_dataAccess.GetCategories());
        }

        /// <summary>
        /// Copies the details of a book to the form fields.
        /// A new, separate object is created to avoid modifying the original in the list directly.
        /// </summary>
        private void CopyBookToForm(Book book)
        {
            if (book != null)
            {
                BookInForm = new Book
                {
                    BookID = book.BookID,
                    Title = book.Title,
                    Description = book.Description,
                    PublicationYear = book.PublicationYear,
                    Price = book.Price,
                    StockQuantity = book.StockQuantity,
                    CoverImagePath = book.CoverImagePath,
                    AuthorID = book.AuthorID,
                    PublisherID = book.PublisherID,
                    CategoryID = book.CategoryID
                };
            }
            else
            {
                BookInForm = new Book(); // Prepare for a new book
            }
        }

        #region Command Methods

        /// <summary>
        /// Prepares the form for adding a new book.
        /// </summary>
        private void AddNewBook(object parameter)
        {
            SelectedBook = null; // Deselect any book in the list
            BookInForm = new Book(); // Clear the form fields
        }

        private bool CanSaveBook(object parameter)
        {
            // Enable save button only if the form has a valid title and other required fields.
            // Shneiderman's Rule: Offer simple error handling.
            return BookInForm != null && !string.IsNullOrWhiteSpace(BookInForm.Title) && BookInForm.AuthorID > 0 && BookInForm.CategoryID > 0 && BookInForm.PublisherID > 0;
        }

        /// <summary>
        /// Saves the book (either creates a new one or updates an existing one).
        /// </summary>
        private void SaveBook(object parameter)
        {
            if (BookInForm.BookID == 0) // It's a new book
            {
                _dataAccess.InsertBook(BookInForm);
                MessageBox.Show("Book successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else // It's an existing book
            {
                _dataAccess.UpdateBook(BookInForm);
                MessageBox.Show("Book successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            LoadData(); // Refresh the list
            AddNewBook(null); // Clear the form
        }

        private bool CanDeleteBook(object parameter)
        {
            // Enable delete button only if a book is selected.
            return SelectedBook != null;
        }

        /// <summary>
        /// Deletes the selected book after confirmation.
        /// </summary>
        private void DeleteBook(object parameter)
        {
            // Shneiderman's Rule: Permit easy reversal of actions (via confirmation).
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete '{SelectedBook.Title}'?",
                                                       "Confirm Deletion",
                                                       MessageBoxButton.YesNo,
                                                       MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _dataAccess.DeleteBook(SelectedBook.BookID);
                MessageBox.Show("Book deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData(); // Refresh the list
                AddNewBook(null); // Clear the form
            }
        }

        #endregion
    }
}
