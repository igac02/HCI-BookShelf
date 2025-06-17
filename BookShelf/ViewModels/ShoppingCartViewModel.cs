using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BookShelf.Core;
using BookShelf.Models;
using BookShelf.Service;
using BookShelf.Services;

namespace BookShelf.ViewModel
{
    public class ShoppingCartViewModel : ViewModelBase
    {
        private readonly DataAccess _dataAccess;
        private readonly ShoppingCartService _cartService;
        private readonly User _currentUser;

        // We expose the Items and TotalPrice directly from the service
        // so any changes are automatically reflected here.
        public ObservableCollection<OrderItem> Items => _cartService.Items;
        public decimal TotalPrice => _cartService.TotalPrice;

        #region Commands
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand CheckoutCommand { get; }
        #endregion

        public ShoppingCartViewModel(DataAccess dataAccess, ShoppingCartService cartService, User currentUser)
        {
            _dataAccess = dataAccess;
            _cartService = cartService;
            _currentUser = currentUser;

            // We need to listen for changes in the service to update our view.
            _cartService.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(ShoppingCartService.TotalPrice))
                {
                    OnPropertyChanged(nameof(TotalPrice));
                }
            };

            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity, CanModifyQuantity);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity, CanModifyQuantity);
            RemoveItemCommand = new RelayCommand(RemoveItem, CanModifyQuantity);
            CheckoutCommand = new RelayCommand(Checkout, CanCheckout);
        }

        #region Command Methods

        private bool CanModifyQuantity(object parameter)
        {
            return parameter is OrderItem;
        }

        private void IncreaseQuantity(object parameter)
        {
            var item = parameter as OrderItem;
            if (item != null)
            {
                _cartService.AddItem(item.Book); // The service handles the logic
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private void DecreaseQuantity(object parameter)
        {
            var item = parameter as OrderItem;
            if (item != null && item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                // If quantity is 1, decreasing it is the same as removing
                RemoveItem(item);
            }
            _cartService.CalculateTotal(); // Manually recalculate
            OnPropertyChanged(nameof(TotalPrice));
        }

        private void RemoveItem(object parameter)
        {
            var item = parameter as OrderItem;
            _cartService.RemoveItem(item);
        }

        private bool CanCheckout(object parameter)
        {
            return Items.Any(); // Can only checkout if the cart is not empty
        }

        private void Checkout(object parameter)
        {
            var order = new Order
            {
                UserID = _currentUser.UserID,
                OrderDate = DateTime.Now,
                Status = "Processing",
                TotalPrice = TotalPrice,
                OrderItems = new List<OrderItem>(Items)
            };

            try
            {
                _dataAccess.CreateOrder(order);
                MessageBox.Show("Your order has been placed successfully!", "Order Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);
                _cartService.ClearCart();
                // Here you would typically close the cart window.
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error placing your order. Please try again.", "Order Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Log exception details: ex.Message
            }
        }

        #endregion
    }
}
