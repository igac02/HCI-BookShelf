using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            set { _newRating = value; OnPropertyChanged(); }
        }

        private string _newComment;
        public string NewComment
        {
            get => _newComment;
            set { _newComment = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand SubmitReviewCommand { get; }
        #endregion

        public BookDetailsViewModel(Book book, User currentUser, DataAccess dataAccess)
        {
            _book = book;
            _currentUser = currentUser;
            _dataAccess = dataAccess;

            SubmitReviewCommand = new RelayCommand(SubmitReview, CanSubmitReview);

            LoadReviews();
        }

        private void LoadReviews()
        {
            Reviews = new ObservableCollection<Review>(_dataAccess.GetReviewsForBook(Book.BookID));
        }

        private bool CanSubmitReview(object parameter)
        {
            // Enable button only if there is a comment and a valid user.
            return !string.IsNullOrWhiteSpace(NewComment) && _currentUser != null;
        }

        private void SubmitReview(object parameter)
        {
            var newReview = new Review
            {
                BookID = this.Book.BookID,
                UserID = _currentUser.UserID,
                Rating = this.NewRating,
                Comment = this.NewComment,
                ReviewDate = DateTime.Now
            };

            try
            {
                _dataAccess.AddReview(newReview);
                MessageBox.Show("Thank you for your review!", "Review Submitted", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh the list and clear the form
                LoadReviews();
                NewComment = string.Empty;
                NewRating = 5;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while submitting your review.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
