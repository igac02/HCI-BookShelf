using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BookShelf.Core;
using BookShelf.Models;

namespace BookShelf.Service
{
    /// <summary>
    /// Manages the state of the user's shopping cart.
    /// This service will be shared across different ViewModels.
    /// </summary>
    public class ShoppingCartService : ViewModelBase
    {
        private ObservableCollection<OrderItem> _items = new ObservableCollection<OrderItem>();
        public ObservableCollection<OrderItem> Items
        {
            get => _items;
            set { _items = value; OnPropertyChanged(); }
        }

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set { _totalPrice = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Adds a book to the shopping cart.
        /// If the book is already in the cart, it increases the quantity.
        /// </summary>
        public void AddItem(Book book)
        {
            if (book == null || book.StockQuantity <= 0) return;

            var existingItem = Items.FirstOrDefault(i => i.BookID == book.BookID);

            if (existingItem != null)
            {
                // Increase quantity if there is enough stock
                if (existingItem.Quantity < book.StockQuantity)
                {
                    existingItem.Quantity++;
                }
            }
            else
            {
                // Add new item to the cart
                var newItem = new OrderItem
                {
                    BookID = book.BookID,
                    Book = book,
                    Quantity = 1,
                    PricePerItem = book.Price
                };
                Items.Add(newItem);
            }

            CalculateTotal();
        }

        /// <summary>
        /// Removes an item completely from the cart.
        /// </summary>
        public void RemoveItem(OrderItem item)
        {
            if (item != null)
            {
                Items.Remove(item);
                CalculateTotal();
            }
        }

        /// <summary>
        /// Clears all items from the cart.
        /// </summary>
        public void ClearCart()
        {
            Items.Clear();
            CalculateTotal();
        }

        public void CalculateTotal()
        {
            TotalPrice = Items.Sum(item => item.Quantity * item.PricePerItem);
        }
    }
}
