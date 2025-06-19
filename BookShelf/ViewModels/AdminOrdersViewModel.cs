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
        /// Reprezentuje trenutno selektovanu narudžbu u listi.
        /// Detalji će biti prikazani u desnom panelu.
        /// </summary>
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
                // LINIJA KOJA JE IZAZIVALA GREŠKU JE UKLONJENA OVDJE.
                // Vaš RelayCommand automatski osvježava stanje dugmadi putem CommandManager-a.
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Komanda za promjenu statusa selektovane narudžbe.
        /// </summary>
        public ICommand ChangeStatusCommand { get; }

        #endregion

        /// <summary>
        /// Konstruktor za Design-Time podatke u Visual Studio dizajneru.
        /// </summary>
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
                    User = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" }
                };
                sampleOrder.OrderItems.Add(new OrderItem { Book = new Book { Title = "Sample Book 1" }, Quantity = 1, PricePerItem = 29.99m });
                sampleOrder.OrderItems.Add(new OrderItem { Book = new Book { Title = "Sample Book 2" }, Quantity = 1, PricePerItem = 19.99m });
                Orders = new ObservableCollection<Order> { sampleOrder };
                SelectedOrder = sampleOrder;
            }
        }

        /// <summary>
        /// Konstruktor koji se poziva u toku izvršavanja aplikacije.
        /// </summary>
        public AdminOrdersViewModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;

            // Inicijalizacija komandi
            ChangeStatusCommand = new RelayCommand(ChangeStatus, CanChangeStatus);

            LoadData();
        }

        private void LoadData()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;

            Orders = new ObservableCollection<Order>(_dataAccess.GetAllOrders());
        }

        #region Command Logic

        /// <summary>
        /// Provjerava da li se komanda za promjenu statusa može izvršiti.
        /// </summary>
        private bool CanChangeStatus(object parameter)
        {
            // Omogući dugmad samo ako je neka narudžba selektovana.
            return SelectedOrder != null;
        }

        /// <summary>
        /// Izvršava logiku za promjenu statusa narudžbe.
        /// </summary>
        private void ChangeStatus(object parameter)
        {
            // Provjeri da li je proslijeđeni parametar string (npr. "Completed")
            if (parameter is string newStatus)
            {
                // Prikaži dijalog za potvrdu akcije
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to change the status for order #{SelectedOrder.OrderID} to '{newStatus}'?",
                    "Confirm Status Change",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // 1. Pozovi metodu iz DataAccess sloja da ažurira bazu podataka
                        _dataAccess.UpdateOrderStatus(SelectedOrder.OrderID, newStatus);

                        // 2. Ažuriraj status na objektu u memoriji.
                        // Pošto smo na Order klasi implementirali INotifyPropertyChanged,
                        // UI će se automatski osvježiti.
                        SelectedOrder.Status = newStatus;

                        MessageBox.Show("Order status updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        #endregion
    }
}