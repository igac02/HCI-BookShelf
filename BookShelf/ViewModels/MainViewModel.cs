using BookShelf.Core;
using BookShelf.Models;
using BookShelf.Service;
using BookShelf.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BookShelf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DataAccess _dataAccess;
        private readonly ShoppingCartService _cartService;
        private readonly NavigationService _navigationService;
        private readonly ThemeManager _themeManager;
        private List<Book> _allBooks;

        #region Properties for Data Binding

        // --- FIX: Full property implementation for UI notifications ---
        private ObservableCollection<Book> _books;
        public ObservableCollection<Book> Books
        {
            get => _books;
            set { _books = value; OnPropertyChanged(); }
        }

        public User LoggedInUser { get; set; }
        public bool IsAdmin => LoggedInUser?.Role == "Admin";

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private Author _selectedAuthor;
        public Author SelectedAuthor
        {
            get => _selectedAuthor;
            set { _selectedAuthor = value; OnPropertyChanged(); ApplyFilter(); }
        }

        // --- FIX: Full property implementation for UI notifications ---
        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Author> _authors;
        public ObservableCollection<Author> Authors
        {
            get => _authors;
            set { _authors = value; OnPropertyChanged(); }
        }

        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set { _selectedBook = value; OnPropertyChanged(); }
        }

        // Theme properties - modified to save to database when changed
        private ThemeType _currentTheme = ThemeType.Light;
        public ThemeType CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    OnPropertyChanged();
                    _themeManager.SetTheme(value);

                    // Sačuvaj preference teme u bazu podataka
                    SaveThemePreference(value);

                    // Notify theme-related properties
                    OnPropertyChanged(nameof(IsLightTheme));
                    OnPropertyChanged(nameof(IsDarkTheme));
                    OnPropertyChanged(nameof(IsCrazyTheme));
                }
            }
        }

        // Properties for radio buttons or UI binding
        public bool IsLightTheme
        {
            get => CurrentTheme == ThemeType.Light;
            set
            {
                if (value) CurrentTheme = ThemeType.Light;
            }
        }

        public bool IsDarkTheme
        {
            get => CurrentTheme == ThemeType.Dark;
            set
            {
                if (value) CurrentTheme = ThemeType.Dark;
            }
        }

        public bool IsCrazyTheme
        {
            get => CurrentTheme == ThemeType.Crazy;
            set
            {
                if (value) CurrentTheme = ThemeType.Crazy;
            }
        }

        // Available themes for ComboBox
        public ObservableCollection<ThemeType> AvailableThemes { get; } = new ObservableCollection<ThemeType>
        {
            ThemeType.Light,
            ThemeType.Dark,
            ThemeType.Crazy
        };
        #endregion

        #region Commands
        public ICommand ClearFiltersCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand ViewCartCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand OpenAdminBooksCommand { get; }
        public ICommand OpenAdminUsersCommand { get; }
        public ICommand OpenAdminOrdersCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ToggleThemeCommand { get; } // Zadržano za kompatibilnost
        public ICommand SetLightThemeCommand { get; }
        public ICommand SetDarkThemeCommand { get; }
        public ICommand SetCrazyThemeCommand { get; }
        #endregion

        // Design-time constructor
        public MainViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            Books = new ObservableCollection<Book> { new Book { Title = "Design-Time Book", Author = new Author { FirstName = "Design", LastName = "Author" } } };
        }

        // Runtime constructor
        public MainViewModel(DataAccess dataAccess, ShoppingCartService cartService, NavigationService navigationService, User loggedInUser)
        {
            _dataAccess = dataAccess;
            _cartService = cartService;
            _navigationService = navigationService;
            _themeManager = new ThemeManager();
            LoggedInUser = loggedInUser;

            ClearFiltersCommand = new RelayCommand(p => ClearFilters());
            AddToCartCommand = new RelayCommand(p => AddToCart(), p => CanAddToCart());
            ViewCartCommand = new RelayCommand(p => _navigationService.ShowShoppingCart(LoggedInUser));
            OpenAdminBooksCommand = new RelayCommand(p => _navigationService.ShowAdminBooks());
            OpenAdminUsersCommand = new RelayCommand(p => _navigationService.ShowAdminUsers(LoggedInUser));
            OpenAdminOrdersCommand = new RelayCommand(p => _navigationService.ShowAdminOrders());
            LogoutCommand = new RelayCommand(p => Logout(p as Window));
            ViewDetailsCommand = new RelayCommand(p => ViewBookDetails(), p => CanViewBookDetails());

            // Theme commands
            ToggleThemeCommand = new RelayCommand(p => ToggleTheme()); // Zadržano za kompatibilnost
            SetLightThemeCommand = new RelayCommand(p => CurrentTheme = ThemeType.Light);
            SetDarkThemeCommand = new RelayCommand(p => CurrentTheme = ThemeType.Dark);
            SetCrazyThemeCommand = new RelayCommand(p => CurrentTheme = ThemeType.Crazy);

            LoadData();
            LoadUserThemePreference(); // Učitaj korisničku preference teme
        }

        #region Theme Management Methods

        /// <summary>
        /// Učitava korisničku preference teme iz baze podataka
        /// </summary>
        private void LoadUserThemePreference()
        {
            if (LoggedInUser != null)
            {
                // Učitaj najnovije podatke korisnika iz baze (uključujući PreferredTheme)
                var userFromDb = _dataAccess.GetUserById(LoggedInUser.UserID);
                if (userFromDb != null)
                {
                    LoggedInUser.PreferredTheme = userFromDb.PreferredTheme;

                    // Postavi temu bez okidanja save metode
                    _currentTheme = LoggedInUser.PreferredThemeType;
                    OnPropertyChanged(nameof(CurrentTheme));
                    OnPropertyChanged(nameof(IsLightTheme));
                    OnPropertyChanged(nameof(IsDarkTheme));
                    OnPropertyChanged(nameof(IsCrazyTheme));

                    // Primeni temu
                    _themeManager.SetTheme(_currentTheme);
                }
            }
        }

        /// <summary>
        /// Čuva korisničku preference teme u bazu podataka
        /// </summary>
        /// <param name="theme">Nova tema</param>
        private void SaveThemePreference(ThemeType theme)
        {
            if (LoggedInUser != null)
            {
                try
                {
                    LoggedInUser.PreferredThemeType = theme;
                    _dataAccess.UpdateUserThemePreference(LoggedInUser.UserID, theme.ToString());
                }
                catch (System.Exception ex)
                {
                    // Log greške ili prikaži poruku korisniku
                    MessageBox.Show($"Could not save theme preference: {ex.Message}", "Warning",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        #endregion

        // Methods for the new command
        private bool CanViewBookDetails() => SelectedBook != null;
        private void ViewBookDetails()
        {
            _navigationService.ShowBookDetailsWindow(SelectedBook, LoggedInUser);
        }

        private void ToggleTheme()
        {
            // Ciklično menjanje tema: Light -> Dark -> Crazy -> Light
            switch (CurrentTheme)
            {
                case ThemeType.Light:
                    CurrentTheme = ThemeType.Dark;
                    break;
                case ThemeType.Dark:
                    CurrentTheme = ThemeType.Crazy;
                    break;
                case ThemeType.Crazy:
                    CurrentTheme = ThemeType.Light;
                    break;
                default:
                    CurrentTheme = ThemeType.Light;
                    break;
            }
        }

        private bool CanAddToCart() => SelectedBook != null && SelectedBook.StockQuantity > 0;
        private void AddToCart()
        {
            _cartService.AddItem(SelectedBook);
            MessageBox.Show($"'{SelectedBook.Title}' has been added to your cart.", "Item Added");
        }

        private void Logout(Window currentWindow)
        {
            _navigationService.ShowLoginWindow();
            currentWindow?.Close();
        }

        #region Data and Filter Logic
        private void LoadData()
        {
            _allBooks = _dataAccess.GetBooks();
            Books = new ObservableCollection<Book>(_allBooks);
            Authors = new ObservableCollection<Author>(_dataAccess.GetAuthors());
            Categories = new ObservableCollection<Category>(_dataAccess.GetCategories());
            Authors.Insert(0, new Author { AuthorID = 0, FirstName = "All" });
            Categories.Insert(0, new Category { CategoryID = 0, Name = "All" });
        }

        private void ApplyFilter()
        {
            if (_allBooks == null) return;
            IEnumerable<Book> filteredBooks = _allBooks;
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string lowerCaseSearch = SearchText.ToLower();
                filteredBooks = filteredBooks.Where(b => b.Title.ToLower().Contains(lowerCaseSearch) || b.Author.FullName.ToLower().Contains(lowerCaseSearch));
            }
            if (SelectedCategory != null && SelectedCategory.CategoryID != 0)
            {
                filteredBooks = filteredBooks.Where(b => b.CategoryID == SelectedCategory.CategoryID);
            }
            if (SelectedAuthor != null && SelectedAuthor.AuthorID != 0)
            {
                filteredBooks = filteredBooks.Where(b => b.AuthorID == SelectedAuthor.AuthorID);
            }
            Books = new ObservableCollection<Book>(filteredBooks.ToList());
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            SelectedCategory = null;
            SelectedAuthor = null;
        }
        #endregion
    }
}