using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Library.Models;
using Library.Persistence;
using Library.Service;
using Library.Services;
using Library.Views;
using System.Collections.Generic;
using System.Linq;

namespace Library
{
    public partial class BookWindow : Window
    {
        private readonly IAppDbContext _dbContext;
        private readonly BookService _bookService;
        private readonly CheckOutService _checkOutService;
        public IEnumerable<Book>? AvailableBooks { get; set; }
        public IEnumerable<Book>? UserBooks { get; set; }
        private readonly int _currentUserId;

        public BookWindow(IAppDbContext dbContext, int currentUserId)
        {
            _dbContext = dbContext;
            _bookService = new BookService(dbContext, currentUserId);
            _checkOutService = new CheckOutService(dbContext);
            InitializeComponent();
            _currentUserId = currentUserId;
            LoadData();
            LoadSortOptions();
        }

        private void LoadData()
        {
            AvailableBooks = _bookService.GetAllAvailableBooksForUser();
            UserBooks = _bookService.GetUserBorrowedBooks();           
            AvailableBooksListBox.ItemsSource = AvailableBooks;
            UserBooksListBox.ItemsSource = UserBooks;
        }

        private void SearchBooksTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string searchText;
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    searchText = textBox.Text.ToLower();

                }
                else
                {
                    searchText = "";
                }

                AvailableBooks = BookService.FilterBooks(_bookService.GetAllAvailableBooksForUser(), searchText);
                AvailableBooksListBox.ItemsSource = AvailableBooks;

                UserBooks = BookService.FilterBooks(_bookService.GetUserBorrowedBooks(), searchText);
                UserBooksListBox.ItemsSource = UserBooks;
            }
        }   

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
            ClearCachedData();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_dbContext,_currentUserId);
            settingsWindow.Show();
            
        }

        private void LoadSortOptions()
        {
            string[] sortOptions = ["Title", "Author", "Category"];
            string[] sortMethods = ["asc", "desc"];
            SortOptionComboBox.ItemsSource = sortOptions;
            SortOptionComboBox.SelectedIndex = 0;
            SortMethodComboBox.ItemsSource = sortMethods;
            SortMethodComboBox.SelectedIndex = 0;
        }

        private void SortOptionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (SortOptionComboBox.SelectedItem != null && SortMethodComboBox.SelectedItem != null && AvailableBooks != null && UserBooks != null)
                {
                    string selectedSortOption = (string)SortOptionComboBox.SelectedItem;
                    string selectedMethodOption = (string)SortMethodComboBox.SelectedItem;

                    switch (selectedSortOption)
                    {
                        case "Title":
                            if (selectedMethodOption == "asc")
                            {
                                AvailableBooks = AvailableBooks.OrderBy(book => book.Title);
                                UserBooks = UserBooks.OrderBy(book => book.Title);
                            }
                            else
                            {
                                AvailableBooks = AvailableBooks.OrderByDescending(book => book.Title);
                                UserBooks = UserBooks.OrderByDescending(book => book.Title);
                            }
                            break;
                        case "Author":
                            if (selectedMethodOption == "asc")
                            {
                                AvailableBooks = AvailableBooks.OrderBy(book => book.Author);
                                UserBooks = UserBooks.OrderBy(book => book.Author);
                            }
                            else
                            {
                                AvailableBooks = AvailableBooks.OrderByDescending(book => book.Author);
                                UserBooks = UserBooks.OrderByDescending(book => book.Author);
                            }
                            break;
                        case "Category":
                            if (selectedMethodOption == "asc")
                            {
                                AvailableBooks = AvailableBooks.OrderBy(book => book.Category);
                                UserBooks = UserBooks.OrderBy(book => book.Category);
                            }
                            else
                            {
                                AvailableBooks = AvailableBooks.OrderByDescending(book => book.Category);
                                UserBooks = UserBooks.OrderByDescending(book => book.Category);
                            }
                            break;
                        default:
                            break;
                    }

                    AvailableBooksListBox.ItemsSource = AvailableBooks;
                    UserBooksListBox.ItemsSource = UserBooks;
                }
                return;
            }
        }


        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if ((Book)button.DataContext != null)
            {
                var book = (Book)button.DataContext;
                if (book != null && AvailableBooks != null && UserBooks != null)
                {
                    _checkOutService.AddCheckOut(book.Id, _currentUserId);

                    var selectedBook = AvailableBooks.FirstOrDefault(b => b.Id == book.Id);
                    if (selectedBook != null)
                    {
                        AvailableBooks = AvailableBooks.Where(b => b.Id != selectedBook.Id).ToList();
                        AvailableBooksListBox.ItemsSource = AvailableBooks;

                        var userBookList = UserBooks.ToList();
                        userBookList.Add(selectedBook);
                        UserBooks = userBookList;
                        UserBooksListBox.ItemsSource = UserBooks;
                    }
                }
            }
        }

        private void ClearCachedData()
        {
            AvailableBooks = null;
            UserBooks = null;
            AvailableBooksListBox.ItemsSource = null;
            UserBooksListBox.ItemsSource = null;
        }
    }
}
