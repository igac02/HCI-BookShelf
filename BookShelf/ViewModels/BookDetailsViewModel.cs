using System;
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
    public class BookDetailsViewModel : ViewModelBase
    {
        private readonly DataAccess _dataAccess;
        private readonly User _currentUser;

        #region Bound Properties
        private Book _book;
        public Book Book
        {
            get => _book;
            set { _book = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Review> _reviews;
        public ObservableCollection<Review> Reviews
        {
            get => _reviews;
            set { _reviews = value; OnPropertyChanged(); }
        }

        // Properties for the new review form
        private int _newRating = 5; // Default to 5 stars
        public int NewRating
        {
            get => _newRating;
            set
            {
                _newRating = value;
                OnPropertyChanged();
                // Update star selection properties
                OnPropertyChanged(nameof(IsRating1Selected));
                OnPropertyChanged(nameof(IsRating2Selected));
                OnPropertyChanged(nameof(IsRating3Selected));
                OnPropertyChanged(nameof(IsRating4Selected));
                OnPropertyChanged(nameof(IsRating5Selected));
                OnPropertyChanged(nameof(SelectedRating));
            }
        }

        private string _newComment;
        public string NewComment
        {
            get => _newComment;
            set { _newComment = value; OnPropertyChanged(); }
        }

        // Star rating properties for UI binding
        public bool IsRating1Selected => NewRating >= 1;
        public bool IsRating2Selected => NewRating >= 2;
        public bool IsRating3Selected => NewRating >= 3;
        public bool IsRating4Selected => NewRating >= 4;
        public bool IsRating5Selected => NewRating >= 5;
        public int SelectedRating => NewRating;

        // Review text property for the form
        private string _reviewText;
        public string ReviewText
        {
            get => _reviewText;
            set { _reviewText = value; OnPropertyChanged(); }
        }

        // Average rating calculation
        public double AverageRating
        {
            get
            {
                if (Reviews == null || Reviews.Count == 0)
                    return 0;
                return Reviews.Average(r => r.Rating);
            }
        }

        public int ReviewCount => Reviews?.Count ?? 0;
        #endregion

        #region Commands
        public ICommand SubmitReviewCommand { get; }
        public ICommand SetRatingCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand AddToCartCommand { get; }
        #endregion

        public BookDetailsViewModel(Book book, User currentUser, DataAccess dataAccess)
        {
            _book = book;
            _currentUser = currentUser;
            _dataAccess = dataAccess;

            SubmitReviewCommand = new RelayCommand(SubmitReview, CanSubmitReview);
            SetRatingCommand = new RelayCommand(SetRating);
            CloseCommand = new RelayCommand(CloseWindow);
            AddToCartCommand = new RelayCommand(AddToCart);

            LoadReviews();
        }

        /// <summary>
        /// Ovaj konstruktor se koristi ISKLJUČIVO od strane XAML dizajnera za prikaz podataka u Design modu.
        /// </summary>
        public BookDetailsViewModel()
        {
            // =================================================================
            // 1. KREIRANJE OGLEDNOG (MOCK) OBJEKTA 'BOOK'
            // =================================================================
            Book = new Book
            {
                Title = "Gospodar prstenova: Prstenova družina",
                Description = "U drevna vremena, vilenjaci-kovači su iskovali Prstenove Moći. Međutim, Gospodar Tame, Sauron, potajno je iskovao Vrhovni Prsten kako bi vladao svima ostalima. Ali Prsten mu je oduzet i, nakon mnogo godina, došao je u ruke hobita po imenu Frodo Baggins.",
                PublicationYear = 1954,
                Price = 34.95m,
                StockQuantity = 25,
                CoverImagePath = null,

                Author = new Author
                {
                    FirstName = "J. R. R.",
                    LastName = "Tolkien",
                    Biography = "John Ronald Reuel Tolkien bio je engleski pisac, pjesnik, filolog i akademik."
                },

                Publisher = new Publisher
                {
                    Name = "Allen & Unwin"
                },

                Category = new Category
                {
                    Name = "Epska fantastika",
                    Description = "Podžanr fantastike koji karakterišu epske teme i borba između dobra i zla."
                }
            };

            // =================================================================
            // 2. KREIRANJE OGLEDNE LISTE RECENZIJA ('REVIEWS')
            // =================================================================
            Reviews = new ObservableCollection<Review>
            {
                new Review
                {
                    Rating = 5,
                    Comment = "Apsolutno remek-djelo! Svijet je nevjerovatno detaljan, a likovi su nezaboravni. Preporučujem svima.",
                    ReviewDate = DateTime.Now.AddDays(-10),
                    User = new User { FirstName = "Ana", LastName = "Anić", Role = "Customer" }
                },
                new Review
                {
                    Rating = 4,
                    Comment = "Knjiga je fantastična, ali malo spora na početku. Ipak, isplati se biti strpljiv.",
                    ReviewDate = DateTime.Now.AddDays(-5),
                    User = new User { FirstName = "Marko", LastName = "Marković", Role = "Customer" }
                }
            };

            // =================================================================
            // 3. POSTAVLJANJE VRIJEDNOSTI ZA FORMU ZA UNOS RECENZIJE
            // =================================================================
            NewRating = 5;
            NewComment = "";
            ReviewText = "";

            // Initialize commands for design mode
            SubmitReviewCommand = new RelayCommand(o => { });
            SetRatingCommand = new RelayCommand(o => { });
            CloseCommand = new RelayCommand(o => { });
            AddToCartCommand = new RelayCommand(o => { });
        }

        private void LoadReviews()
        {
            Reviews = new ObservableCollection<Review>(_dataAccess.GetReviewsForBook(Book.BookID));
            OnPropertyChanged(nameof(AverageRating));
            OnPropertyChanged(nameof(ReviewCount));
        }

        private bool CanSubmitReview(object parameter)
        {
            return !string.IsNullOrWhiteSpace(ReviewText) && _currentUser != null;
        }

        private void SubmitReview(object parameter)
        {
            var newReview = new Review
            {
                BookID = this.Book.BookID,
                UserID = _currentUser.UserID,
                Rating = this.NewRating,
                Comment = this.ReviewText,
                ReviewDate = DateTime.Now
            };

            try
            {
                _dataAccess.AddReview(newReview);
                MessageBox.Show("Thank you for your review!", "Review Submitted", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh the list and clear the form
                LoadReviews();
                ReviewText = string.Empty;
                NewRating = 5;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while submitting your review.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetRating(object parameter)
        {
            if (int.TryParse(parameter?.ToString(), out int rating))
            {
                NewRating = rating;
            }
        }

        private void CloseWindow(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        private void AddToCart(object parameter)
        {
            // Implementirajte logiku za dodavanje u korpu
            MessageBox.Show($"Book '{Book.Title}' added to cart!", "Added to Cart", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}