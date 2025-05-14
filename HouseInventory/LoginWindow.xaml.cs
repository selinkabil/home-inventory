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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Owner = this;
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.ShowDialog();
        }
    }
}
