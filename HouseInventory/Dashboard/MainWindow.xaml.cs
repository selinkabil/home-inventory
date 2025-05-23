using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SQLite;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DotNetEnv;
using HouseInventory.Dashboard;
using static HouseInventory.DatabaseService;
using HouseInventory.Models;
namespace HouseInventory.Dashboard
{
    public partial class MainWindow : Window
    {
        public int UserID { get; private set; }
        public string Username { get; private set; }
        public string Role { get; private set; }

        public string password { get; private set; }

        //Search variables
        private ObservableCollection<Item> Items;
        private ICollectionView ItemsView;

        //Filter variables
        private string _currentSortProperty = null;
        private ListSortDirection _currentSortDirection = ListSortDirection.Ascending;


        public MainWindow(string username, string password, string role)
        {
            InitializeComponent();
            UpdateSortOrderIcon();
            // Load environment variables
            Env.Load();
            string unsplashAccessKey = Env.GetString("UNSPLASH_ACCESS_KEY");
            string unsplashSecretKey = Env.GetString("UNSPLASH_SECRET_KEY");

            // Authenticate user
            var user = DatabaseService.Instance.GetUserByCredentials(username, password);
            if (user == null)
            {
                MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserID = user.UserID;
            Username = user.Username;

            // Get Role name by RoleID
            var urole = DatabaseService.Instance.GetRoles().Find(r => r.RoleID == user.RoleID);
            Role = urole?.RoleName ?? "Unknown";

            UsernameLabel.Content = Username;
            List<Item> dbItems = DatabaseService.Instance.GetItems(UserID);

            // Initialize the class-level ObservableCollection with dbItems
            Items = new ObservableCollection<Item>(dbItems);
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsListView.ItemsSource = ItemsView;



            var excludedProperties = new HashSet<string> { "UserID", "RoomID", "ItemID", "ImagePath" };

            var sortableProperties = typeof(Item).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => (p.PropertyType == typeof(string) || p.PropertyType == typeof(int) || p.PropertyType == typeof(double) || p.PropertyType == typeof(DateTime))
                            && !excludedProperties.Contains(p.Name))
                .Select(p => p.Name)
                .ToList();

            SortComboBox.ItemsSource = sortableProperties;


        }
        public MainWindow(string username, int userID)
        {
            InitializeComponent();
            UpdateSortOrderIcon();
            // Load environment variables
            Env.Load();
            string unsplashAccessKey = Env.GetString("UNSPLASH_ACCESS_KEY");
            string unsplashSecretKey = Env.GetString("UNSPLASH_SECRET_KEY");

            // Authenticate user
            var user = DatabaseService.Instance.GetUserByCredentials(username, password);
            if (user == null)
            {
                MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UserID = user.UserID;
            Username = user.Username;

            // Get Role name by RoleID
            var urole = DatabaseService.Instance.GetRoles().Find(r => r.RoleID == user.RoleID);
            Role = urole?.RoleName ?? "Unknown";

            UsernameLabel.Content = Username;
            List<Item> dbItems = DatabaseService.Instance.GetItems(UserID);

            // Initialize the class-level ObservableCollection with dbItems
            Items = new ObservableCollection<Item>(dbItems);
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsListView.ItemsSource = ItemsView;



            var excludedProperties = new HashSet<string> { "UserID", "RoomID", "ItemID", "ImagePath" };

            var sortableProperties = typeof(Item).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => (p.PropertyType == typeof(string) || p.PropertyType == typeof(int) || p.PropertyType == typeof(double) || p.PropertyType == typeof(DateTime))
                            && !excludedProperties.Contains(p.Name))
                .Select(p => p.Name)
                .ToList();

            SortComboBox.ItemsSource = sortableProperties;


        }

        private T GetOpenWindow<T>() where T : Window
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is T)
                {
                    return window as T;
                }
            }
            return null;
        }

        private void OpenDashboard()
        {
            var existingWindow = GetOpenWindow<HouseInventory.Dashboard.MainWindow>();
            if (existingWindow != null)
            {
                existingWindow.Activate();
                existingWindow.Topmost = true;
                existingWindow.Topmost = false;
                return;
            }
            var dashboard = new HouseInventory.Dashboard.MainWindow(Username, UserID)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            dashboard.Show();
        }

        private void OpenCategories()
        {
            var existingWindow = GetOpenWindow<HouseInventory.Categories.Categories.Categories>();
            if (existingWindow != null)
            {
                existingWindow.Activate();
                existingWindow.Topmost = true;
                existingWindow.Topmost = false;
                return;
            }
            var categories = new HouseInventory.Categories.Categories.Categories(Username, UserID)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            categories.Show();
        }

        private void OpenBuildings()
        {
            var existingWindow = GetOpenWindow<HouseInventory.Buildings.Buildings>();
            if (existingWindow != null)
            {
                existingWindow.Activate();
                existingWindow.Topmost = true;
                existingWindow.Topmost = false;
                return;
            }
            var buildings = new HouseInventory.Buildings.Buildings(Username, UserID)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            buildings.Show();
        }

        private void OpenRooms()
        {
            var existingWindow = GetOpenWindow<HouseInventory.Rooms.Rooms>();
            if (existingWindow != null)
            {
                existingWindow.Activate();
                existingWindow.Topmost = true;
                existingWindow.Topmost = false;
                return;
            }
            var rooms = new HouseInventory.Rooms.Rooms(Username, UserID)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            rooms.Show();
        }

        private void OpenReports()
        {
            var existingWindow = GetOpenWindow<HouseInventory.Reports.Reports>();
            if (existingWindow != null)
            {
                existingWindow.Activate();
                existingWindow.Topmost = true;
                existingWindow.Topmost = false;
                return;
            }
            var reports = new HouseInventory.Reports.Reports(Username, UserID)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            reports.Show();
        }

        public void RefreshItems()
        {
            var newItems = DatabaseService.Instance.GetItems(UserID);
            Items.Clear();
            foreach (var item in newItems)
            {
                Items.Add(item);
            }
            ItemsView.Refresh();
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

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            var existingWindow = GetOpenWindow<AddItem>();
            if (existingWindow != null)
            {
                existingWindow.Activate();
                existingWindow.Topmost = true;  // optional — forces to front
                existingWindow.Topmost = false;
                return;
            }

            var addItemWindow = new AddItem(UserID)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.Manual
            };

            if (this.WindowState == WindowState.Maximized)
            {
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

            addItemWindow.Closed += AddItemWindow_Closed;
            addItemWindow.ShowDialog();
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (ItemsView == null) return;

            var tb = sender as System.Windows.Controls.TextBox;
            string filterText = tb.Text.Trim();

            if (string.IsNullOrWhiteSpace(filterText))
            {
                ItemsView.Filter = null;
            }
            else
            {
                ItemsView.Filter = obj =>
                {
                    if (obj is Item item)
                    {
                        // Example filter - check if ItemName or Description contains search text (case insensitive)
                        return item.ItemName?.IndexOf(filterText, System.StringComparison.OrdinalIgnoreCase) >= 0
                            || item.Description?.IndexOf(filterText, System.StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                    return false;
                };
            }
            ItemsView.Refresh();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedItem == null)
                return;

            string selectedProperty = SortComboBox.SelectedItem.ToString();

            if (_currentSortProperty == selectedProperty)
            {
                // Toggle sort direction
                _currentSortDirection = _currentSortDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                _currentSortProperty = selectedProperty;
                _currentSortDirection = ListSortDirection.Ascending;
            }

            ApplySorting();
            UpdateSortOrderIcon();
        }


        private void SortOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentSortProperty))
                return;

            _currentSortDirection = _currentSortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            ApplySorting();
            UpdateSortOrderIcon();
        }


        private void UpdateSortOrderIcon()
        {
            string iconPath;

            if (_currentSortDirection == ListSortDirection.Ascending)
            {
                iconPath = "/Dashboard/arrow-down.png";
            }
            else
            {
               iconPath = "/Dashboard/up-arrow.png";
            }

            SortOrderIcon.Source = new BitmapImage(new Uri(iconPath, UriKind.Relative));
        }

        private void ApplySorting()
        {
            if (string.IsNullOrEmpty(_currentSortProperty) || ItemsView == null)
                return;

            ItemsView.SortDescriptions.Clear();
            ItemsView.SortDescriptions.Add(new SortDescription(_currentSortProperty, _currentSortDirection));
            ItemsView.Refresh();
        }

        private void AddItemWindow_Closed(object sender, EventArgs e)
        {
            RefreshItems(); 
        }

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsListView.SelectedItem is Item selectedItem)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete \"{selectedItem.ItemName}\"?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseService.Instance.DeleteItem(selectedItem.ItemID);
                        MessageBox.Show("Item deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        RefreshItems();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            ItemsView.Refresh();
        }

      
        private void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (Item)ItemsListView.SelectedItem;
            if (selectedItem != null)
            {
                // Look up the RoomName by RoomID
                var room = DatabaseService.Instance.GetRooms().Find(r => r.RoomID == selectedItem.RoomID);
                string roomName = room?.RoomName ?? "Unknown";

                var editWindow = new EditItem(selectedItem);
                editWindow.Closed += AddItemWindow_Closed;
                editWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an item to edit.", "No Item Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            ItemsView.Refresh();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var existingWindow = GetOpenWindow<FilterWindow>();
            if (existingWindow != null)
            {
                existingWindow.Activate();
                existingWindow.Topmost = true;
                existingWindow.Topmost = false;
                return;
            }

            var filterWindow = new FilterWindow(Items.ToList());
            if (filterWindow.ShowDialog() == true)
            {
                var filteredItems = filterWindow.FilteredItems;
                ItemsView = CollectionViewSource.GetDefaultView(filteredItems);
                ItemsListView.ItemsSource = ItemsView;
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           Application.Current.Shutdown();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            var loginWindow = new LoginWindow();
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loginWindow.Show();
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e) => OpenDashboard();
        private void CategoriesButton_Click(object sender, RoutedEventArgs e) => OpenCategories();
        private void BuildingsButton_Click(object sender, RoutedEventArgs e) => OpenBuildings();
        private void RoomsButton_Click(object sender, RoutedEventArgs e) => OpenRooms();
        private void ReportsButton_Click(object sender, RoutedEventArgs e) => OpenReports();
    }
}
