using Avalonia.Controls;
using Avalonia.Interactivity;
using Library.Models;
using Library.Persistence;
using Library.Services;
using MsgBox;
using System.Collections.Generic;

namespace Library;

public partial class AddNewItemWindow : Window
{

    private readonly IAppDbContext _dbContext;
    private readonly BookService _bookService;
    private readonly int userID;
    private List<BookCategory> Category;
    public AddNewItemWindow(IAppDbContext dbContext, int userId)
    {
        _dbContext = dbContext;
        userID = userId;
        _bookService = new BookService(_dbContext, userID);
        InitializeComponent();
        WireUpEvents();
        LoadData();
    }

    private void WireUpEvents()
    {
        TitleTextBox.KeyUp += TextBox_KeyUp;
        AuthorTextBox.KeyUp += TextBox_KeyUp;
        CategoryTextBox.KeyUp += TextBox_KeyUp;
    }

    private void TextBox_KeyUp(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(TitleTextBox.Text) && !string.IsNullOrEmpty(AuthorTextBox.Text) && !string.IsNullOrEmpty(CategoryComboBox.SelectedItem as string))
        {
            AddBookButton.IsEnabled = true;
        }
        else
        {
            AddBookButton.IsEnabled = false;
        }
        if (!string.IsNullOrEmpty(CategoryTextBox.Text))
        {
            CategoryAddButton.IsEnabled = true;
        }
        else
        {
            CategoryAddButton.IsEnabled = false;
        }
    }

    private async void LoadData()
    {
        Category = await _bookService.GetAllCategories();
        if (Category != null && Category.Count > 0)
        {
            var categoryNames = new List<string>();
            foreach (var category in Category)
            {
                categoryNames.Add(category.Name);
            }
            CategoryComboBox.ItemsSource = categoryNames;
            CategoryComboBox.SelectedIndex = 0;
        }
    }



    private async void AddBookButton_Click(object sender, RoutedEventArgs e)
    {
        if (CategoryComboBox.SelectedItem is string categoryName && TitleTextBox.Text != null && AuthorTextBox.Text != null)
        {
            var catId = Category.Find(c => c.Name == categoryName)?.Id;
            if (catId != null)
            {
                var newBook = new Book
                {
                    Title = TitleTextBox.Text,
                    Author = AuthorTextBox.Text,
                    CategoryId = (int)catId,
                };
                _bookService.AddBook(newBook);
                await MessageBox.Show(this, "Book added succesfully", "Success", MessageBox.MessageBoxButtons.Ok);
            }
            else
            {
                await MessageBox.Show(this, "Category not found", "Error", MessageBox.MessageBoxButtons.Ok);
            }
        }

        Close();
    }
    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private async void CategoryAddButton_Click(object sender, RoutedEventArgs e)
    {
        var categoryName = CategoryTextBox.Text;
        if (categoryName != null)
        {
            var newCat = new BookCategory
            {
                Name = categoryName,
            };

            _bookService.AddCategory(newCat);
            await MessageBox.Show(this, "Category added succesfully", "Success", MessageBox.MessageBoxButtons.Ok);
        }
        Close();
    }
}