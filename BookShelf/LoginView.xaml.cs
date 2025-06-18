using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BookShelf.Service;
using BookShelf.Services;
using BookShelf.ViewModels;

namespace BookShelf.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView(NavigationService navigationService)
        {
            InitializeComponent();

            // This is the crucial part: connecting the View to the ViewModel.
            // In a real-world application, the connection string would come from a config file.
            string connectionString = "Server=localhost;Port=3306;Database=bookshelf;Uid=root;Pwd=root;";
            DataAccess dataAccess = new DataAccess(connectionString);

            // Set the DataContext of this window to a new instance of LoginViewModel.
            this.DataContext = new LoginViewModel(dataAccess, navigationService);
        }

        /// <summary>
        /// This event handler is a common workaround for the PasswordBox,
        /// which doesn't support direct data binding on its Password property for security reasons.
        /// We manually update the ViewModel's Password property whenever the password changes.
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // We ensure the DataContext is a LoginViewModel before trying to set the password.
            if (this.DataContext is LoginViewModel viewModel)
            {
                // The PasswordBox's SecurePassword property is used to get the password securely.
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}


namespace BookShelf.Converters
{
    /// <summary>
    /// This converter is used to show or hide the error message TextBlock in the LoginView.
    /// It converts a string value to a Visibility value. If the string is not empty,
    /// it returns Visibility.Visible; otherwise, it returns Visibility.Collapsed.
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the string is not null or empty, the UI element should be visible.
            return !string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter is only used for one-way binding, so ConvertBack is not needed.
            throw new NotImplementedException();
        }
    }
}