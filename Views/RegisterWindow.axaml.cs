using Avalonia.Controls;
using Library.Models;
using Library.Persistence;
using Library.Service;
using MsgBox;
using System.Linq;
using System.Threading.Tasks;

namespace Library;

public partial class RegisterWindow : Window
{
    private readonly IAppDbContext _dbContext;

    public RegisterWindow(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
        InitializeComponent();
        WireUpEvents();
    }

    private void WireUpEvents()
    {
        UsernameTextBox.KeyUp += TextBox_KeyUp;
        PasswordBox1.KeyUp += TextBox_KeyUp;
        PasswordBox2.KeyUp += TextBox_KeyUp;
        EmailTextBox.KeyUp += TextBox_KeyUp;
        PhoneNumberTextBox.KeyUp += TextBox_KeyUp;
    }

    private void TextBox_KeyUp(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string[] fields =
        [
            UsernameTextBox.Text, PasswordBox1.Text, PasswordBox2.Text, EmailTextBox.Text, PhoneNumberTextBox.Text
        ];

        bool isUsernameValid = ValidateField(fields[0], 8);
        bool isPassword1Valid = ValidateField(fields[1], 8);
        bool isPassword2Valid = ValidateField(fields[2], 8);
        bool isEmailValid = !string.IsNullOrWhiteSpace(fields[3]);
        bool isPhoneValid = !string.IsNullOrWhiteSpace(fields[4]);

        RegisterButton.IsEnabled = isUsernameValid && isPassword1Valid && isPassword2Valid && isEmailValid && isPhoneValid;
    }

    private static bool ValidateField(string fieldValue, int minLength)
    {
        return !string.IsNullOrWhiteSpace(fieldValue) && fieldValue.Length >= minLength;
    }

    private async void Register_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string[] fields =
        [
            UsernameTextBox.Text,
            PasswordBox1.Text,
            PasswordBox2.Text,
            EmailTextBox.Text,
            PhoneNumberTextBox.Text
        ];

        if (fields.Any(string.IsNullOrWhiteSpace))
        {
            await ShowMessageBox("Please fill in all fields", "Error");
            return;
        }

        if (fields[1] != fields[2])
        {
            await ShowMessageBox("Different passwords", "Error");
            return;
        }

        if (!UserService.IsValidEmail(fields[3]))
        {
            await ShowMessageBox("Invalid email address", "Error");
            return;
        }

        if (!ValidateField(fields[0], 8) || !ValidateField(fields[1], 8))
        {
            await ShowMessageBox("Minimal length of 8 characters required", "Error");
            return;
        }

        if (_dbContext.UserExists(fields[0], fields[3]))
        {
            await ShowMessageBox("User with that username or email exists", "Error");
            return;
        }

        string hashedPassword = PasswordHelper.HashPas(fields[1]);

        var newUser = new User
        {
            Username = fields[0],
            PasswordHash = hashedPassword,
            EmailAddress = fields[3],
            PhoneNumber = fields[4]
        };

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        await ShowMessageBox("User registered successfully", "Success");
        Close();
    }

    private async Task ShowMessageBox(string message, string title)
    {
        await MessageBox.Show(this, message, title, MessageBox.MessageBoxButtons.Ok);
    }
}