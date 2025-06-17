using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using BookShelf.Core;
using BookShelf.Models;
using BookShelf.Services;

namespace BookShelf.ViewModel
{
    public class AdminOrdersViewModel : ViewModelBase
    {
        private readonly DataAccess _dataAccess;

        #region Bound Properties

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set { _orders = value; OnPropertyChanged(); }
        }

        private Order _selectedOrder;
        /// <summary>
        /// Represents the currently selected order in the main list.
        /// Its details (items) will be shown in a separate view.
        /// </summary>
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set { _selectedOrder = value; OnPropertyChanged(); }
        }

        #endregion

        public AdminOrdersViewModel()
        {
            // Design-time data
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                var sampleOrder = new Order
                {
                    OrderID = 1,
                    OrderDate = DateTime.Now,
                    Status = "Shipped",
                    TotalPrice = 49.98m,
                    User = new User { FirstName = "John", LastName = "Doe" }
                };
                sampleOrder.OrderItems.Add(new OrderItem { Book = new Book { Title = "Sample Book 1" }, Quantity = 1, PricePerItem = 29.99m });
                sampleOrder.OrderItems.Add(new OrderItem { Book = new Book { Title = "Sample Book 2" }, Quantity = 1, PricePerItem = 19.99m });
                Orders = new ObservableCollection<Order> { sampleOrder };
                SelectedOrder = sampleOrder;
            }
        }

        public AdminOrdersViewModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            LoadData();
        }

        private void LoadData()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

            Orders = new ObservableCollection<Order>(_dataAccess.GetOrders());
        }
    }
}
