using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HouseInventory
{
    
    public partial class MainWindow : Window
    {
        private SQLiteDatabase db;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the database connection
            db = new SQLiteDatabase();

            // Fetch items from the database
            List<Item> items = db.GetItems();
            ItemsListView.ItemsSource = items;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var addItemWindow = new AddItem();
            addItemWindow.Owner = this;
            addItemWindow.WindowStartupLocation = WindowStartupLocation.Manual;

            // Position: center right of the owner window
            if (this.WindowState == WindowState.Maximized)
            {
                // For maximized, use screen size
                var screenWidth = SystemParameters.WorkArea.Width;
                var screenHeight = SystemParameters.WorkArea.Height;
                addItemWindow.Left = screenWidth - addItemWindow.Width - 50;
                addItemWindow.Top = (screenHeight - addItemWindow.Height) / 2;
            }
            else
            {
                addItemWindow.Left = this.Left + this.Width - addItemWindow.Width - 20;
                addItemWindow.Top = this.Top + (this.Height - addItemWindow.Height) / 2;
            }

            addItemWindow.ShowDialog();
        }
    }
}