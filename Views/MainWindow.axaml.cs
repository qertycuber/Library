using Avalonia.Controls;
using Avalonia.Interactivity;
using Library.Persistence;
using Library.Service;
using Microsoft.EntityFrameworkCore;
using MsgBox;
using System;
using System.Linq;

namespace Library.Views
{
    public partial class MainWindow : Window
    {
        //private readonly string connectionString = $"Server={Environment.MachineName};Database=Library;Integrated Security=True;TrustServerCertificate=True;";
        //private string connectionString = "Server=localhost\\SQLEXPRESS;Database=NotesApp;Integrated Security=True;";
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=NotesApp;Integrated Security=True;Encrypt=False;";


        private readonly AppDbContext _dbContext;
        public MainWindow()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            _dbContext = new AppDbContext(optionsBuilder.Options);
            InitializeComponent();
            UsernameTextBox.KeyUp += TextBox_GotFocus;
            PasswordTextBox.KeyUp += TextBox_GotFocus;
        }

        private async void Login_ClickAsync(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && PasswordHelper.Verify(password, user.PasswordHash))
            {   
                if (user.Role == Models.UserRole.Admin)
                {

                    var adminWindow = new AdminWindow(_dbContext, user.Id);
                    adminWindow.Show();
                }
                else
                {
                    var bookWindow = new BookWindow(_dbContext,user.Id);
                    bookWindow.Show();
                }
                Close();
            }
            else
            {
                await MessageBox.Show(this, "Invalid username or password.", "Error", MessageBox.MessageBoxButtons.Ok);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_dbContext);
            registerWindow.Show();
        }

        private void TextBox_GotFocus(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(UsernameTextBox.Text) &&
                !string.IsNullOrWhiteSpace(PasswordTextBox.Text) &&
                UsernameTextBox.Text.Length >= 8 &&
                PasswordTextBox.Text.Length >= 8
               )
            {
                LoginButton.IsEnabled = true;
            }

            else
            {
                LoginButton.IsEnabled = false;
            }
        }
    }
}