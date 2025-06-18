using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using BookShelf.Core;
using BookShelf.Models;
using BookShelf.Service;
using BookShelf.Services;

namespace BookShelf.ViewModels
{
    public class RegistrationViewModel : ViewModelBase
    {
        private readonly DataAccess _dataAccess;
        private readonly NavigationService _navigationService;

        #region Bound Properties
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        #endregion

        public ICommand RegisterCommand { get; }

        public RegistrationViewModel()
        {
            // For XAML designer only
        }

        public RegistrationViewModel(DataAccess dataAccess, NavigationService navigationService)
        {
            _dataAccess = dataAccess;
            _navigationService = navigationService;
            RegisterCommand = new RelayCommand(Register, CanRegister);
        }

        private bool CanRegister(object parameter)
        {
            // Enable Register button only if all fields are filled
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        private void Register(object parameter)
        {

            try
            {
                // --- Validation ---
                if (Password != ConfirmPassword)
                {
                    MessageBox.Show("Passwords do not match.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!_email.Contains("@") || !_email.Contains("."))
                {
                    MessageBox.Show("Please enter a valid email address.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Check if user with this email already exists
                if (_dataAccess.GetUserByEmail(Email) != null)
                {
                    MessageBox.Show("A user with this email address already exists.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // --- Create User ---
                var newUser = new User
                {
                    FirstName = this.FirstName,
                    LastName = this.LastName,
                    Email = this.Email,
                    Role = "Customer", // All new users are customers by default
                                       // *** SECURITY NOTE: HASH THE PASSWORD BEFORE SAVING ***
                    PasswordHash = this.Password // Placeholder - NOT SECURE!
                };

                _dataAccess.InsertUser(newUser);

                MessageBox.Show("Registration successful! You can now log in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Close the registration window
                (parameter as Window)?.Close();
            }
            catch (Exception ex)
            {
                string errorDetails = $"[{DateTime.Now}] ERROR: {ex.ToString()}{Environment.NewLine}";
                File.AppendAllText("error_log.txt", errorDetails); // Sprema u direktorij aplikacije

                MessageBox.Show($"An error occurred during registration:\n\n{ex.Message}", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
    }
}
