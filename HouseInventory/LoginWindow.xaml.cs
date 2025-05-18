using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static HouseInventory.DatabaseService;
namespace HouseInventory
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegistrationWindow();
            registerWindow.Owner = this;
            registerWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            registerWindow.ShowDialog();
        }
        private bool AuthenticateUser(string username, string password, out string roleName)
        {
            roleName = null;

            // Get all users from DB
            var users = DatabaseService.Instance.GetUsers();

            // Find user with matching username & password
            var user = users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password); // ideally, password is hashed and compared safely

            if (user == null)
                return false;

            // Get role for the found user
            var role = DatabaseService.Instance.GetRoleByUserId(user.UserID);
            if (role == null)
                return false;

            roleName = role.RoleName;
            return true;
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password; 
            if (AuthenticateUser(username, password, out string role))
            {
                var mainWindow = new Dashboard.MainWindow(username, password, role);
                mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
