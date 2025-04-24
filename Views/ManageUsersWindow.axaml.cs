using Avalonia.Controls;
using Avalonia.Interactivity;
using Library.Models;
using Library.Persistence;
using System.Collections.Generic;
using Library.Service;
using MsgBox;
using Library.Views;
using System;
using System.Linq;
using Avalonia.Input;
using Library.Services;

namespace Library;

public partial class ManageUsersWindow : Window
{
    private readonly IAppDbContext _dbContext;
    private readonly UserService _userService;
    public IEnumerable<User> Users { get; set; }
    private readonly int UserId;
    public ManageUsersWindow(IAppDbContext dbContext, int userId)
    {
        _dbContext = dbContext;
        InitializeComponent();
        LoadData();
        LoadSortOptions();
        UserId = userId;
        _userService = new UserService(_dbContext);
    }

    private void LoadSortOptions()
    {
        string[] sortOptions = ["Username", "Email", "Phone Number", "Role"];
        string[] sortMethods = ["asc", "desc"];
        SortOptionComboBox.ItemsSource = sortOptions;
        SortOptionComboBox.SelectedIndex = 0;
        SortMethodComboBox.ItemsSource = sortMethods;
        SortMethodComboBox.SelectedIndex = 0;
    }

    private void LoadData()
    {
        Users = _dbContext.Users;
        UsersListBox.ItemsSource = null;
        UsersListBox.ItemsSource = Users;
    }

    private void SortOptionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            if (SortOptionComboBox.SelectedItem != null && SortMethodComboBox.SelectedItem != null)
            {
                string selectedSortOption = (string)SortOptionComboBox.SelectedItem;
                string selectedMethodOption = (string)SortMethodComboBox.SelectedItem;

                switch (selectedSortOption)
                {
                    case "Username":
                        if (selectedMethodOption == "asc")
                        {
                            Users = Users.OrderBy(user => user.Username);
                        }
                        else
                        {
                            Users = Users.OrderByDescending(user => user.Username);
                        }
                        break;
                    case "Email":
                        if (selectedMethodOption == "asc")
                        {
                            Users = Users.OrderBy(user => user.EmailAddress);
                        }
                        else
                        {
                            Users = Users.OrderByDescending(user => user.EmailAddress);
                        }
                        break;
                    case "Phone Number":
                        if (selectedMethodOption == "asc")
                        {
                            Users = Users.OrderBy(user => user.PhoneNumber);
                        }
                        else
                        {
                            Users = Users.OrderByDescending(user => user.PhoneNumber);
                        }
                        break;
                    case "Role":
                        if (selectedMethodOption == "asc")
                        {
                            Users = Users.OrderBy(user => user.Role);
                        }
                        else
                        {
                            Users = Users.OrderByDescending(user => user.Role);
                        }
                        break;
                    default:
                        break;
                }

                UsersListBox.ItemsSource = Users;
            }
        }
    }


    private async void MakeAdminButton_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var data = (User)button.DataContext;
        var user = await _dbContext.Users.FindAsync(data.Id);
        
        if (user != null)
        {
            user.Role = UserRole.Admin;

            await _dbContext.SaveChangesAsync();
            LoadData();
        }
    }
    private async void MakeUserButton_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var user = (User)button.DataContext;

        user = await _dbContext.Users.FindAsync(user.Id);

        if (user != null)
        {
            user.Role = UserRole.User;
            await _dbContext.SaveChangesAsync();

            if (user.Id != UserId)
            {
                LoadData();
            }

            else
            {
                await MessageBox.Show(this, "GL HF", "[*]", MessageBox.MessageBoxButtons.Ok);
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }

        }      
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

            var UsersToShow = Users.Where(user =>
                user.Username.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                (user.Id).ToString() == searchText
            ).ToList();

            UsersListBox.ItemsSource = null;
            UsersListBox.ItemsSource = UsersToShow;
        }
    }

    private async void DeleteUserButton_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var user = (User)button.DataContext;
        await _userService.DeleteUser(user.Id);
        await MessageBox.Show(this, "Succesfully delete user", "Success", MessageBox.MessageBoxButtons.Ok);

        LoadData();
    }
    
    private async void AddUserButton_Click(object sender, RoutedEventArgs e)
    {
        var registerWindow = new RegisterWindow(_dbContext);
        registerWindow.Closed += RegisterWindow_Closed;
        registerWindow.Show();
    }

    private void RegisterWindow_Closed(object sender, EventArgs e)
    {
        LoadData();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        var adminWindow = new AdminWindow(_dbContext,UserId);
        adminWindow.Show();
        Close();
    }
}

