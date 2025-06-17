using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookShelf.Core;
using BookShelf.Models;
using BookShelf.Services;

namespace BookShelf.ViewModel
{
    public class AdminUsersViewModel : ViewModelBase
    {
        private readonly DataAccess _dataAccess;
        private User _currentlyLoggedInUser;

        #region Bound Properties
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                CopyUserToForm(_selectedUser);
                OnPropertyChanged();
            }
        }

        private User _userInForm;
        public User UserInForm
        {
            get => _userInForm;
            set { _userInForm = value; OnPropertyChanged(); }
        }

        public string NewPassword { get; set; }

        public ObservableCollection<string> Roles { get; } = new ObservableCollection<string> { "Customer", "Admin" };
        #endregion

        #region Commands
        public ICommand AddNewCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        #endregion

        /// <summary>
        /// Parameterless constructor FOR THE XAML DESIGNER ONLY.
        /// </summary>
        public AdminUsersViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Users = new ObservableCollection<User>
                {
                    new User { FirstName = "John", LastName = "Doe", Email = "john@example.com", Role = "Customer" },
                    new User { FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Role = "Admin" }
                };
                UserInForm = new User();
            }
        }

        /// <summary>
        /// This is the main constructor used at RUNTIME.
        /// </summary>
        public AdminUsersViewModel(DataAccess dataAccess, User loggedInUser)
        {
            _dataAccess = dataAccess;
            _currentlyLoggedInUser = loggedInUser;
            UserInForm = new User();

            AddNewCommand = new RelayCommand(AddNewUser);
            SaveCommand = new RelayCommand(SaveUser, CanSaveUser);
            DeleteCommand = new RelayCommand(DeleteUser, CanDeleteUser);

            LoadData();
        }

        private void LoadData()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            Users = new ObservableCollection<User>(_dataAccess.GetAllUsers());
        }

        private void CopyUserToForm(User user)
        {
            if (user != null)
            {
                UserInForm = new User
                {
                    UserID = user.UserID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                };
            }
            else
            {
                UserInForm = new User { Role = "Customer" };
            }
            NewPassword = "";
            OnPropertyChanged(nameof(NewPassword));
        }

        #region Command Methods
        private void AddNewUser(object parameter)
        {
            SelectedUser = null;
            UserInForm = new User { Role = "Customer" };
            NewPassword = "";
            OnPropertyChanged(nameof(NewPassword));
        }

        private bool CanSaveUser(object parameter)
        {
            if (UserInForm == null) return false;

            if (UserInForm.UserID == 0 && string.IsNullOrWhiteSpace(NewPassword))
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(UserInForm.FirstName) &&
                   !string.IsNullOrWhiteSpace(UserInForm.Email) &&
                   !string.IsNullOrWhiteSpace(UserInForm.Role);
        }

        private void SaveUser(object parameter)
        {
            if (!UserInForm.Email.Contains("@") || !UserInForm.Email.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (UserInForm.UserID == 0)
            {
                UserInForm.PasswordHash = NewPassword;
                _dataAccess.InsertUser(UserInForm);
                MessageBox.Show("User successfully added!", "Success");
            }
            else
            {
                _dataAccess.UpdateUser(UserInForm);
                if (!string.IsNullOrWhiteSpace(NewPassword))
                {
                    _dataAccess.UpdateUserPassword(UserInForm.UserID, NewPassword);
                }
                MessageBox.Show("User successfully updated!", "Success");
            }
            LoadData();
            AddNewUser(null);
        }

        private bool CanDeleteUser(object parameter)
        {
            if (SelectedUser == null) return false;

            if (_currentlyLoggedInUser != null && SelectedUser.UserID == _currentlyLoggedInUser.UserID)
            {
                return false;
            }
            return true;
        }

        private void DeleteUser(object parameter)
        {
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete user '{SelectedUser.Email}'?",
                                                       "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _dataAccess.DeleteUser(SelectedUser.UserID);
                MessageBox.Show("User deleted.", "Success");
                LoadData();
                AddNewUser(null);
            }
        }
        #endregion
    }
}
