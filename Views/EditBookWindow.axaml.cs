using Avalonia.Controls;
using Avalonia.Interactivity;
using Library.Models;
using Library.Persistence;
using Library.Services;
using MsgBox;
using System.Collections.Generic;
using System.Linq;

namespace Library;
public partial class EditBookWindow : Window
{
    private readonly int BookID;
    private readonly IAppDbContext _dbContext;
    private readonly BookService _bookService;
    private readonly int _currentUserId;
    private readonly Book _book;
    private List<BookCategory> Category;
    public EditBookWindow(int BookId, IAppDbContext dbContext, int userId)
    {
        _currentUserId = userId;
        _dbContext = dbContext;
        _bookService = new BookService(_dbContext, _currentUserId);
        BookID = BookId;
        _book = _bookService.GetBook(BookID);
        InitializeComponent();
        WireUpEvents();
        LoadData();
    }

    private void WireUpEvents()
    {
        TitleTextBox.KeyUp += TextBox_KeyUp;
        AuthorTextBox.KeyUp += TextBox_KeyUp;
        CategoryComboBox.SelectionChanged += TextBox_KeyUp;
    }

    private async void LoadData()
    {
        Category = await _bookService.GetAllCategories();
        if (Category != null && Category.Count > 0)
        {
            var CategoryNames = Category.Select(cat => cat.Name).ToList();
            CategoryComboBox.ItemsSource = CategoryNames;
        }

        TitleTextBox.Text = _book.Title;
        AuthorTextBox.Text = _book.Author;
        var categoryOfBook = Category.FirstOrDefault(cat => cat.Id == _book.CategoryId);
        if (categoryOfBook != null)
        {
            CategoryComboBox.SelectedItem = categoryOfBook.Name;
        }   
    }

    private void TextBox_KeyUp(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (TitleTextBox.Text != _book.Title || AuthorTextBox.Text != _book.Author || CategoryComboBox.SelectedItem as string != _book.Category.Name)
        {
            EditButton.IsEnabled = true;
        }
        else
        {
            EditButton.IsEnabled = false;
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var adminWindow = new AdminWindow(_dbContext, _currentUserId);
        adminWindow.Show();
        Close();
    }
    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (CategoryComboBox.SelectedItem is string categoryName)
        {
            var catId = Category.Find(c => c.Name == categoryName)?.Id;
            if (catId.HasValue && (TitleTextBox.Text != _book.Title || AuthorTextBox.Text != _book.Author || catId != _book.CategoryId))
            {
                await _bookService.EditBook(BookID, TitleTextBox.Text, AuthorTextBox.Text, (int)catId);
            }
            await MessageBox.Show(this, "Book updated succesfully", "Success", MessageBox.MessageBoxButtons.Ok);
        }

        var adminWindow = new AdminWindow(_dbContext, _currentUserId);
        adminWindow.Show();
        Close();
    }
}