-- Kreiranje šeme
CREATE SCHEMA IF NOT EXISTS bookshelf DEFAULT CHARACTER SET utf8mb4;
USE BookShelf;

-- Tabela Authors
CREATE TABLE IF NOT EXISTS Authors (
  AuthorID INT NOT NULL AUTO_INCREMENT,
  FirstName VARCHAR(50) NOT NULL,
  LastName VARCHAR(50) NOT NULL,
  Biography TEXT NULL,
  PRIMARY KEY (AuthorID)
) ENGINE = InnoDB;

-- Tabela Publishers
CREATE TABLE IF NOT EXISTS Publishers (
  PublisherID INT NOT NULL AUTO_INCREMENT,
  Name VARCHAR(100) NOT NULL,
  PRIMARY KEY (PublisherID)
) ENGINE = InnoDB;

-- Tabela Categories
CREATE TABLE IF NOT EXISTS Categories (
  CategoryID INT NOT NULL AUTO_INCREMENT,
  Name VARCHAR(50) NOT NULL,
  Description VARCHAR(255) NULL,
  PRIMARY KEY (CategoryID)
) ENGINE = InnoDB;

-- Tabela Users
CREATE TABLE IF NOT EXISTS Users (
  UserID INT NOT NULL AUTO_INCREMENT,
  FirstName VARCHAR(50) NOT NULL,
  LastName VARCHAR(50) NOT NULL,
  Email VARCHAR(100) NOT NULL,
  PasswordHash VARCHAR(255) NOT NULL,
  Role VARCHAR(20) NOT NULL DEFAULT 'Customer',
  RegistrationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PreferredTheme VARCHAR(20) NOT NULL DEFAULT 'Light',
  PRIMARY KEY (UserID),
  UNIQUE INDEX Email_UNIQUE (Email ASC)
) ENGINE = InnoDB;

-- Tabela Books
CREATE TABLE IF NOT EXISTS Books (
  BookID INT NOT NULL AUTO_INCREMENT,
  Title VARCHAR(255) NOT NULL,
  Description TEXT NULL,
  PublicationYear INT NULL,
  Price DECIMAL(10,2) NOT NULL,
  StockQuantity INT NOT NULL DEFAULT 0,
  CoverImagePath VARCHAR(255) NULL,
  AuthorID INT NOT NULL,
  PublisherID INT NOT NULL,
  CategoryID INT NOT NULL,
  PRIMARY KEY (BookID),
  INDEX fk_Books_Authors_idx (AuthorID ASC),
  INDEX fk_Books_Publishers_idx (PublisherID ASC),
  INDEX fk_Books_Categories_idx (CategoryID ASC),
  CONSTRAINT fk_Books_Authors
    FOREIGN KEY (AuthorID) REFERENCES Authors (AuthorID)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT fk_Books_Publishers
    FOREIGN KEY (PublisherID) REFERENCES Publishers (PublisherID)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT fk_Books_Categories
    FOREIGN KEY (CategoryID) REFERENCES Categories (CategoryID)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB;

-- Tabela Orders
CREATE TABLE IF NOT EXISTS Orders (
  OrderID INT NOT NULL AUTO_INCREMENT,
  OrderDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  Status VARCHAR(50) NOT NULL,
  TotalPrice DECIMAL(10,2) NOT NULL,
  UserID INT NOT NULL,
  PRIMARY KEY (OrderID),
  INDEX fk_Orders_Users_idx (UserID ASC),
  CONSTRAINT fk_Orders_Users
    FOREIGN KEY (UserID) REFERENCES Users (UserID)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB;

-- Tabela OrderItems
CREATE TABLE IF NOT EXISTS OrderItems (
  OrderItemID INT NOT NULL AUTO_INCREMENT,
  OrderID INT NOT NULL,
  BookID INT NOT NULL,
  Quantity INT NOT NULL,
  PricePerItem DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (OrderItemID),
  INDEX fk_OrderItems_Orders_idx (OrderID ASC),
  INDEX fk_OrderItems_Books_idx (BookID ASC),
  CONSTRAINT fk_OrderItems_Orders
    FOREIGN KEY (OrderID) REFERENCES Orders (OrderID)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_OrderItems_Books
    FOREIGN KEY (BookID) REFERENCES Books (BookID)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE = InnoDB;

-- Tabela Reviews
CREATE TABLE IF NOT EXISTS Reviews (
  ReviewID INT NOT NULL AUTO_INCREMENT,
  Rating INT NOT NULL,
  Comment TEXT NULL,
  ReviewDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  BookID INT NOT NULL,
  UserID INT NOT NULL,
  PRIMARY KEY (ReviewID),
  INDEX fk_Reviews_Books_idx (BookID ASC),
  INDEX fk_Reviews_Users_idx (UserID ASC),
  CONSTRAINT fk_Reviews_Books
    FOREIGN KEY (BookID) REFERENCES Books (BookID)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_Reviews_Users
    FOREIGN KEY (UserID) REFERENCES Users (UserID)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB;

-- Unos autora
INSERT INTO Authors (FirstName, LastName, Biography) VALUES
('George', 'Orwell', 'English novelist and essayist.'),
('Jane', 'Austen', 'Known for novels like Pride and Prejudice.'),
('Mark', 'Twain', 'American writer and humorist.'),
('Fyodor', 'Dostoevsky', 'Russian novelist and philosopher.'),
('J.K.', 'Rowling', 'Author of the Harry Potter series.'),
('Ernest', 'Hemingway', 'American novelist and short-story writer.'),
('Leo', 'Tolstoy', 'Russian writer.'),
('Mary', 'Shelley', 'Author of Frankenstein.'),
('Agatha', 'Christie', 'Queen of crime novels.'),
('Isaac', 'Asimov', 'Science fiction author.');

-- Unos izdavača
INSERT INTO Publishers (Name) VALUES
('Penguin Books'), ('HarperCollins'), ('Vintage'), ('Oxford Press'),
('Macmillan'), ('Simon & Schuster'), ('Bloomsbury'), ('Scholastic'),
('Tor Books'), ('Random House');

-- Unos kategorija
INSERT INTO Categories (Name, Description) VALUES
('Fiction', 'Fictional works'), ('Sci-Fi', 'Science fiction novels'),
('Biography', 'Biographies'), ('Mystery', 'Mystery and detective novels'),
('Romance', 'Romantic stories'), ('Fantasy', 'Fantasy novels'),
('History', 'Historical works'), ('Horror', 'Horror stories'),
('Philosophy', 'Philosophical texts'), ('Adventure', 'Adventure novels');

-- Unos korisnika
INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Role) VALUES
('Admin', 'Admin', 'admin@bookshelf.com', 'admin', 'Admin'),
('John', 'Doe', 'john@example.com', 'pass123', 'Customer'),
('Jane', 'Smith', 'jane@example.com', 'pass123', 'Customer'),
('Mike', 'Brown', 'mike@example.com', 'pass123', 'Customer'),
('Anna', 'Johnson', 'anna@example.com', 'pass123', 'Customer'),
('Tom', 'Davis', 'tom@example.com', 'pass123', 'Customer'),
('Lucy', 'Miller', 'lucy@example.com', 'pass123', 'Customer'),
('Peter', 'Wilson', 'peter@example.com', 'pass123', 'Customer'),
('Emma', 'Moore', 'emma@example.com', 'pass123', 'Customer'),
('Chris', 'Taylor', 'chris@example.com', 'pass123', 'Customer');

-- Unos knjiga
INSERT INTO Books (Title, Description, PublicationYear, Price, StockQuantity, AuthorID, PublisherID, CategoryID) VALUES
('1984', 'Dystopian novel', 1949, 15.99, 20, 1, 1, 1),
('Animal Farm', 'Political satire', 1945, 12.99, 15, 1, 1, 1),
('Pride and Prejudice', 'Romantic novel', 1813, 14.50, 10, 2, 2, 5),
('Emma', 'Comedy of manners', 1815, 13.75, 8, 2, 2, 5),
('Adventures of Huckleberry Finn', 'Adventure novel', 1884, 16.00, 12, 3, 3, 10),
('The Idiot', 'Philosophical novel', 1869, 17.00, 5, 4, 4, 9),
('Harry Potter 1', 'Fantasy novel', 1997, 19.99, 25, 5, 7, 6),
('Harry Potter 2', 'Fantasy sequel', 1998, 20.99, 25, 5, 7, 6),
('The Old Man and the Sea', 'Short novel', 1952, 11.99, 10, 6, 5, 1),
('War and Peace', 'Historical novel', 1869, 22.99, 6, 7, 4, 7),
('Frankenstein', 'Gothic novel', 1818, 10.99, 9, 8, 6, 8),
('Murder on the Orient Express', 'Detective novel', 1934, 13.49, 14, 9, 6, 4),
('Foundation', 'Science fiction', 1951, 15.00, 10, 10, 9, 2),
('I, Robot', 'Sci-Fi stories', 1950, 14.50, 13, 10, 9, 2),
('The Brothers Karamazov', 'Philosophical novel', 1880, 18.00, 7, 4, 4, 9),
('Sense and Sensibility', 'Romance', 1811, 14.00, 10, 2, 2, 5),
('Harry Potter 3', 'Fantasy third book', 1999, 21.99, 22, 5, 7, 6),
('Harry Potter 4', 'Fantasy fourth book', 2000, 22.99, 20, 5, 7, 6),
('Harry Potter 5', 'Fantasy fifth book', 2003, 23.99, 20, 5, 7, 6),
('Harry Potter 6', 'Fantasy sixth book', 2005, 24.99, 20, 5, 7, 6),
('Harry Potter 7', 'Fantasy final book', 2007, 25.99, 20, 5, 7, 6),
('Death on the Nile', 'Mystery novel', 1937, 12.99, 11, 9, 6, 4),
('A Tale of Two Cities', 'Historical fiction', 1859, 16.99, 12, 3, 8, 7),
('The Hobbit', 'Fantasy prequel', 1937, 18.99, 18, 3, 5, 6),
('The Great Gatsby', 'Classic fiction', 1925, 14.99, 17, 3, 5, 1),
('Brave New World', 'Dystopian fiction', 1932, 16.49, 16, 1, 1, 1),
('Dracula', 'Horror classic', 1897, 13.99, 8, 8, 6, 8),
('The Catcher in the Rye', 'Coming of age novel', 1951, 15.75, 10, 3, 5, 1),
('Crime and Punishment', 'Russian classic', 1866, 19.25, 9, 4, 4, 9),
('The Sun Also Rises', 'Novel by Hemingway', 1926, 14.75, 10, 6, 5, 1);

-- Unos narudžbi
INSERT INTO Orders (Status, TotalPrice, UserID) VALUES
('Completed', 45.97, 2), ('Pending', 29.99, 3), ('Cancelled', 0.00, 4),
('Completed', 60.50, 5), ('Pending', 22.99, 6), ('Completed', 35.00, 7),
('Completed', 41.75, 8), ('Pending', 19.25, 9), ('Completed', 28.49, 10), ('Pending', 18.00, 2);

-- Stavke narudžbi
INSERT INTO OrderItems (OrderID, BookID, Quantity, PricePerItem) VALUES
(1, 1, 1, 15.99), (1, 3, 2, 14.99),
(2, 5, 1, 29.99),
(4, 6, 2, 17.00), (4, 10, 1, 22.50),
(6, 13, 2, 15.00),
(7, 20, 1, 25.99), (7, 9, 1, 15.76),
(8, 29, 1, 19.25),
(9, 8, 1, 20.99), (9, 15, 1, 13.49),
(10, 6, 1, 18.00);

-- Recenzije
INSERT INTO Reviews (Rating, Comment, BookID, UserID) VALUES
(5, 'Amazing book!', 1, 2),
(4, 'Very good read', 3, 3),
(3, 'Not bad', 5, 4),
(5, 'Loved every page!', 7, 5),
(2, 'Expected more', 10, 6);
