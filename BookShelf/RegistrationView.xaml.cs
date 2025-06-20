using System.Windows;
using System.Windows.Controls;
using BookShelf.Service;
using BookShelf.Services;
using BookShelf.ViewModels;

namespace BookShelf.Views
{
    public partial class RegistrationView : Window
    {
        public RegistrationView(DataAccess dataAccess, NavigationService navigationService)
        {
            InitializeComponent();
            this.DataContext = new RegistrationViewModel(dataAccess, navigationService);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RegistrationViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RegistrationViewModel viewModel)
            {
                viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }
    }
}
