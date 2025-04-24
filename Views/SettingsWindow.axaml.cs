using Avalonia.Controls;
using Library.Models;
using Library.Persistence;
using Library.Service;
using MsgBox;
using Tmds.DBus.Protocol;

namespace Library;

public partial class SettingsWindow : Window
{
    private readonly int userID;
    private readonly IAppDbContext _dbContext;
    private readonly UserService _userService;
    private readonly User _user;
    public SettingsWindow(IAppDbContext dbContext, int userId)
    {
        _dbContext = dbContext;
        userID = userId;
        _userService = new UserService(_dbContext);
        _user = _userService.FindUser(userID);
        InitializeComponent();
        LoadData();
        WireUpEvents();
    }
    
    private void LoadData()
    {
        UsernameTextBox.Text = _user.Username;
        EmailTextBox.Text = _user.EmailAddress;
        PhoneNumberTextBox.Text = _user.PhoneNumber;
    }

    private void WireUpEvents()
    {
        UsernameTextBox.KeyUp += TextBox_KeyUp;
        EmailTextBox.KeyUp += TextBox_KeyUp;
        PasswordBox1.KeyUp += PasswordBox_KeyUp;
        PasswordBox2.KeyUp += PasswordBox_KeyUp;
        PasswordBox3.KeyUp += PasswordBox_KeyUp;
        PhoneNumberTextBox.KeyUp += TextBox_KeyUp;
    }

    private void TextBox_KeyUp(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (UsernameTextBox.Text != null && EmailTextBox.Text != null && PhoneNumberTextBox.Text != null)
        {
            if ((UsernameTextBox.Text != _user.Username || EmailTextBox.Text != _user.EmailAddress || PhoneNumberTextBox.Text != _user.PhoneNumber) && UsernameTextBox.Text.Length >= 8 && EmailTextBox.Text.Length >= 8 && PhoneNumberTextBox.Text.Length >= 8)
            {
                UpdateButton.IsEnabled = true;
            }
            else
            {
                UpdateButton.IsEnabled = false;
            }
        }
    }
    
    private void PasswordBox_KeyUp(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {   
        if(PasswordBox1.Text != null && PasswordBox2.Text != null && PasswordBox3.Text != null) 
        {
            if (PasswordBox1.Text.Length < 8 || PasswordBox2.Text != PasswordBox1.Text || PasswordBox3.Text.Length < 8)
            {
                UpdatePasswordButton.IsEnabled = false;
            }
            else
            {
                UpdatePasswordButton.IsEnabled = true;
            }
        }   
    }
    private async void UpdateButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var userName = UsernameTextBox.Text ;
        var email = EmailTextBox.Text;
        var phoneNumber = PhoneNumberTextBox.Text;
        if (userName != null && email != null && phoneNumber != null)
        {
            await _userService.UpdateUser(userID, userName, email, phoneNumber);
            await MessageBox.Show(this, "Succesfully changed user data", "Success", MessageBox.MessageBoxButtons.Ok);
            Close();

        }
        else
        {
            await MessageBox.Show(this, "Error during update", "Error", MessageBox.MessageBoxButtons.Ok);
        }
    }
    private void BackButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
    private async void UpdatePasswordButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var password = PasswordBox1.Text;
        if (PasswordBox3.Text == null || password == null)
        {
            return;
        }
        if(password == PasswordBox3.Text) 
        {
            await MessageBox.Show(this, "Same Password", "Error", MessageBox.MessageBoxButtons.Ok);
        }
        else if(PasswordHelper.Verify(PasswordBox3.Text, _user.PasswordHash))
        {
            await _userService.UpdatePasswordUser(userID, password);
            await MessageBox.Show(this, "Succesfully changed password", "Success", MessageBox.MessageBoxButtons.Ok);
            Close();
        }
        else
        {
            await MessageBox.Show(this, "Wrong Password", "Error", MessageBox.MessageBoxButtons.Ok);
        }
    }

}