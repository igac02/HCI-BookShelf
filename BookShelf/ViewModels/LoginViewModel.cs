using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using BookShelf.Core;
using BookShelf.Models;
using BookShelf.Services;

namespace BookShelf.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly DataAccess _dataAccess;
        private readonly NavigationService _navigationService;
        private string _email;
        private string _password;
        private string _errorMessage;

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand OpenRegisterCommand { get; }

        public LoginViewModel() { }

        public LoginViewModel(DataAccess dataAccess, NavigationService navigationService)
        {
            _dataAccess = dataAccess;
            _navigationService = navigationService;
            LoginCommand = new RelayCommand(Login, CanLogin);
            // Vraćamo komandu na njenu originalnu funkcionalnost.
            OpenRegisterCommand = new RelayCommand(p => _navigationService.ShowRegistrationWindow());
        }

        private bool CanLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        }

        private void Login(object parameter)
        {
            try
            {
                User user = _dataAccess.GetUserByEmail(Email);

                if (user != null && user.PasswordHash == Password)
                {
                    ErrorMessage = string.Empty;
                    _navigationService.ShowMainWindow(user, parameter as Window);
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred during login: {ex.Message}\n{ex.StackTrace}";
            }
        }
    }
}
