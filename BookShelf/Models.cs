﻿using BookShelf.Core;
using BookShelf.Services;
using System;
using System.Collections.Generic;

// All classes are within the BookShelf.Models namespace
// for better project organization.

namespace BookShelf.Models
{
    /// <summary>
    /// Represents an Author from the Authors table.
    /// </summary>
    public class Author
    {
        public int AuthorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Biography { get; set; }

        // This is a helper property to display the full name easily.
        public string FullName => $"{FirstName} {LastName}";
    }

    /// <summary>
    /// Represents a Publisher from the Publishers table.
    /// </summary>
    public class Publisher
    {
        public int PublisherID { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents a Category (genre) from the Categories table.
    /// </summary>
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Represents a User from the Users table.
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string PasswordHash { get; set; } // We will never store the actual password
        public string Role { get; set; } // e.g., "Admin", "Customer"
        public DateTime RegistrationDate { get; set; }

        // Nova property za čuvanje preference teme
        public string PreferredTheme { get; set; } = "Light"; // Default theme

        // Helper property za lakše rukovanje
        public ThemeType PreferredThemeType
        {
            get => Enum.TryParse<ThemeType>(PreferredTheme, out var theme) ? theme : ThemeType.Light;
            set => PreferredTheme = value.ToString();
        }
    }

    /// <summary>
    /// Represents a Book from the Books table.
    /// This is the central model of our application.
    /// </summary>
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PublicationYear { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string CoverImagePath { get; set; }

        // Foreign Keys linking to other tables
        public int AuthorID { get; set; }
        public int PublisherID { get; set; }
        public int CategoryID { get; set; }

        // Navigation Properties - these hold the actual objects
        // for easier data access in the application.
        // These will be populated when we retrieve data from the database.
        public Author Author { get; set; }
        public Publisher Publisher { get; set; }
        public Category Category { get; set; }
    }

    /// <summary>
    /// Represents an Order from the Orders table.
    /// </summary>
    public class Order : ViewModelBase // <-- NASLIJEDITE ViewModelBase
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); } // <-- OnPropertyChanged OBAVEZAN!
        }

        // Foreign Key
        public int UserID { get; set; }

        // Navigation Property
        public User User { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    /// <summary>
    /// Represents an OrderItem from the OrderItems junction table.
    /// It specifies which book is in which order and in what quantity.
    /// </summary>
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerItem { get; set; } // Price at the time of purchase

        // Foreign Keys
        public int OrderID { get; set; }
        public int BookID { get; set; }

        // Navigation Property
        public Book Book { get; set; }
    }

    /// <summary>
    /// Represents a Review from the Reviews table.
    /// </summary>
    public class Review
    {
        public int ReviewID { get; set; }
        public int Rating { get; set; } // e.g., 1 to 5 stars
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        // Foreign Keys
        public int BookID { get; set; }
        public int UserID { get; set; }

        // Navigation Properties
        public User User { get; set; }
    }
}
