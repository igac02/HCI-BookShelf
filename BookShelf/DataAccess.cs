using BookShelf.Models; // We need to use our model classes
using Dapper; // This comes from the Dapper NuGet package
using MySql.Data.MySqlClient; // This comes from the MySql.Data NuGet package
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace BookShelf.Services
{
    /// <summary>
    /// This class handles all database communication.
    /// It connects to the MySQL database and executes queries.
    /// </summary>
    public class DataAccess
    {
        // This is a placeholder for your connection string.
        // You will provide the actual connection string later.
        // Example: "Server=localhost;Database=BookShelf;Uid=root;Pwd=your_password;"
        private readonly string _connectionString;

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates and opens a connection to the MySQL database.
        /// </summary>
        private IDbConnection GetConnection()
        {
            IDbConnection connection = new MySqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

        #region Book Methods

        /// <summary>
        /// Retrieves a list of all books from the database.
        /// This query joins multiple tables to get all necessary data at once.
        /// </summary>
        public List<Book> GetBooks()
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = @"
                    SELECT 
                        b.*, 
                        a.*, 
                        p.*, 
                        c.*
                    FROM Books b
                    JOIN Authors a ON b.AuthorID = a.AuthorID
                    JOIN Publishers p ON b.PublisherID = p.PublisherID
                    JOIN Categories c ON b.CategoryID = c.CategoryID";

                var books = db.Query<Book, Author, Publisher, Category, Book>(
                    sql,
                    (book, author, publisher, category) =>
                    {
                        book.Author = author;
                        book.Publisher = publisher;
                        book.Category = category;
                        return book;
                    },
                    splitOn: "AuthorID,PublisherID,CategoryID"
                ).ToList();

                return books;
            }
        }

        /// <summary>
        /// Inserts a new book into the database.
        /// </summary>
        public void InsertBook(Book book)
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "INSERT INTO Books (Title, Description, PublicationYear, Price, StockQuantity, AuthorID, PublisherID, CategoryID, CoverImagePath) VALUES (@Title, @Description, @PublicationYear, @Price, @StockQuantity, @AuthorID, @PublisherID, @CategoryID, @CoverImagePath);";
                db.Execute(sql, book);
            }
        }

        /// <summary>
        /// Updates an existing book in the database.
        /// </summary>
        public void UpdateBook(Book book)
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "UPDATE Books SET Title = @Title, Description = @Description, PublicationYear = @PublicationYear, Price = @Price, StockQuantity = @StockQuantity, AuthorID = @AuthorID, PublisherID = @PublisherID, CategoryID = @CategoryID, CoverImagePath = @CoverImagePath WHERE BookID = @BookID;";
                db.Execute(sql, book);
            }
        }

        /// <summary>
        /// Deletes a book from the database.
        /// </summary>
        public void DeleteBook(int bookId)
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "DELETE FROM Books WHERE BookID = @BookID;";
                db.Execute(sql, new { BookID = bookId });
            }
        }

        #endregion

        #region User Methods

        /// <summary>
        /// Retrieves a user by their email address. Useful for the login process.
        /// </summary>
        public User GetUserByEmail(string email)
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "SELECT * FROM Users WHERE Email = @UserEmail";
                return db.QueryFirstOrDefault<User>(sql, new { UserEmail = email });
            }
        }

        /// <summary>
        /// Retrieves a list of all users from the database.
        /// </summary>
        public List<User> GetAllUsers()
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "SELECT * FROM Users ORDER BY LastName, FirstName";
                return db.Query<User>(sql).ToList();
            }
        }

        /// <summary>
        /// Inserts a new user into the database.
        /// </summary>
        public void InsertUser(User user)
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Role) VALUES (@FirstName, @LastName, @Email, @PasswordHash, @Role);";
                db.Execute(sql, user);
            }
        }

        /// <summary>
        /// Updates a user's details (without changing the password).
        /// </summary>
        public void UpdateUser(User user)
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Role = @Role WHERE UserID = @UserID;";
                db.Execute(sql, user);
            }
        }

        /// <summary>
        /// Updates only the password hash for a specific user.
        /// </summary>
        public void UpdateUserPassword(int userId, string passwordHash)
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserID = @UserID;";
                db.Execute(sql, new { PasswordHash = passwordHash, UserID = userId });
            }
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        public void DeleteUser(int userId)
        {
            using (IDbConnection db = GetConnection())
            {
                // Note: You might need to handle related records (like orders) first,
                // depending on your database constraints.
                string sql = "DELETE FROM Users WHERE UserID = @UserID;";
                db.Execute(sql, new { UserID = userId });
            }
        }

        #endregion

        #region Lookup (Šifarnici) Methods


        // Unutar DataAccess.cs klase

        /// <summary>
        /// Ažurira status određene narudžbe u bazi podataka.
        /// </summary>
        /// <param name="orderId">ID narudžbe koja se mijenja.</param>
        /// <param name="newStatus">Novi status (npr. "Completed", "Cancelled").</param>
        public void UpdateOrderStatus(int orderId, string newStatus)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Orders SET Status = @Status WHERE OrderID = @OrderID;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", newStatus);
                        command.Parameters.AddWithValue("@OrderID", orderId);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    // Ovdje možete dodati logiranje greške ako želite
                    MessageBox.Show($"An error occurred while updating the order status: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Retrieves a list of all authors from the database.
        /// </summary>
        public List<Author> GetAuthors()
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "SELECT * FROM Authors ORDER BY LastName, FirstName";
                return db.Query<Author>(sql).ToList();
            }
        }

        /// <summary>
        /// Retrieves a list of all publishers from the database.
        /// </summary>
        public List<Publisher> GetPublishers()
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "SELECT * FROM Publishers ORDER BY Name";
                return db.Query<Publisher>(sql).ToList();
            }
        }

        /// <summary>
        /// Retrieves a list of all categories from the database.
        /// </summary>
        public List<Category> GetCategories()
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "SELECT * FROM Categories ORDER BY Name";
                return db.Query<Category>(sql).ToList();
            }
        }

        /// <summary>
        /// Retrieves a list of all categories from the database.
        /// </summary>
        public List<Order> GetOrders()
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "SELECT * FROM Orders";
                return db.Query<Order>(sql).ToList();
            }
        }

        public List<Order> GetAllOrders()
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = @"
                    SELECT 
                        o.*, 
                        u.*, 
                        oi.*, 
                        b.*
                    FROM Orders o
                    JOIN Users u ON o.UserID = u.UserID
                    JOIN OrderItems oi ON o.OrderID = oi.OrderID
                    JOIN Books b ON oi.BookID = b.BookID
                    ORDER BY o.OrderDate DESC;";

                var orderLookup = new Dictionary<int, Order>();

                db.Query<Order, User, OrderItem, Book, Order>(
                    sql,
                    (order, user, orderItem, book) =>
                    {
                        if (!orderLookup.TryGetValue(order.OrderID, out Order currentOrder))
                        {
                            currentOrder = order;
                            currentOrder.User = user;
                            currentOrder.OrderItems = new List<OrderItem>();
                            orderLookup.Add(currentOrder.OrderID, currentOrder);
                        }

                        orderItem.Book = book;
                        currentOrder.OrderItems.Add(orderItem);
                        return currentOrder;
                    },
                    splitOn: "UserID,OrderItemID,BookID"
                );
                return orderLookup.Values.ToList();
            }
        }

        #endregion

        #region Order and Review Methods

        /// <summary>
        /// Creates a new order in a single transaction to ensure data integrity.
        /// </summary>
        public void CreateOrder(Order order)
        {
            using (IDbConnection db = GetConnection())
            {
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string orderSql = "INSERT INTO Orders (OrderDate, Status, TotalPrice, UserID) VALUES (@OrderDate, @Status, @TotalPrice, @UserID); SELECT LAST_INSERT_ID();";
                        int newOrderId = db.ExecuteScalar<int>(orderSql, order, transaction);

                        foreach (var item in order.OrderItems)
                        {
                            item.OrderID = newOrderId;
                            string itemSql = "INSERT INTO OrderItems (OrderID, BookID, Quantity, PricePerItem) VALUES (@OrderID, @BookID, @Quantity, @PricePerItem);";
                            db.Execute(itemSql, item, transaction);

                            string updateStockSql = "UPDATE Books SET StockQuantity = StockQuantity - @Quantity WHERE BookID = @BookID;";
                            db.Execute(updateStockSql, new { item.Quantity, item.BookID }, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all reviews for a specific book.
        /// </summary>
        public List<Review> GetReviewsForBook(int bookId)
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = @"
                    SELECT r.*, u.*
                    FROM Reviews r
                    JOIN Users u ON r.UserID = u.UserID
                    WHERE r.BookID = @BookID
                    ORDER BY r.ReviewDate DESC";

                return db.Query<Review, User, Review>(
                    sql,
                    (review, user) => {
                        review.User = user;
                        return review;
                    },
                    new { BookID = bookId },
                    splitOn: "UserID"
                ).ToList();
            }
        }

        /// <summary>
        /// Inserts a new review for a book.
        /// </summary>
        public void AddReview(Review review)
        {
            using (IDbConnection db = GetConnection())
            {
                string sql = "INSERT INTO Reviews (Rating, Comment, ReviewDate, BookID, UserID) VALUES (@Rating, @Comment, @ReviewDate, @BookID, @UserID);";
                db.Execute(sql, review);
            }
        }

        #endregion
    }
}