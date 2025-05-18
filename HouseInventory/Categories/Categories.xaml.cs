using System;
using System.Windows;
using static HouseInventory.DatabaseService;

namespace HouseInventory.Categories.Categories
{
    public partial class Categories : Window
    {
        private readonly int _userID;
        private readonly string Username;


        public Categories(string username, int userID)
        {
            InitializeComponent();
            _userID = userID;
            Username = username;
            LoadSidebarCategories();
            UsernameLabel.Content = username;
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
        private void LoadSidebarCategories()
        {
            var categories = DatabaseService.Instance.GetCategories(_userID);
            CategoriesListView.ItemsSource = categories;
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var addCategoryWindow = new AddCategory(_userID);
            addCategoryWindow.ShowDialog();
            LoadSidebarCategories();
        }

        private void EditCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesListView.SelectedItem is Category selectedCategory)
            {
                var editCategoryWindow = new EditCategory(selectedCategory);
                editCategoryWindow.ShowDialog();
                LoadSidebarCategories();
            }
            else
            {
                MessageBox.Show("Please select a category to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesListView.SelectedItem is Category selectedCategory)
            {
                var result = MessageBox.Show($"Are you sure you want to delete category '{selectedCategory.CategoryName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DatabaseService.Instance.DeleteCategory(selectedCategory.CategoryID);
                    LoadSidebarCategories();
                }
            }
            else
            {
                MessageBox.Show("Please select a category to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenDashboard();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenBuildings();
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
            var dashboard = new HouseInventory.Dashboard.MainWindow(Username, _userID)
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
            var categories = new HouseInventory.Categories.Categories.Categories(Username, _userID)
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
            var buildings = new HouseInventory.Buildings.Buildings(Username, _userID)
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
            var rooms = new HouseInventory.Rooms.Rooms(Username, _userID)
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
            var reports = new HouseInventory.Reports.Reports(Username, _userID)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            reports.Show();
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e) => OpenDashboard();
        private void CategoriesButton_Click(object sender, RoutedEventArgs e) => OpenCategories();
        private void BuildingsButton_Click(object sender, RoutedEventArgs e) => OpenBuildings();
        private void RoomsButton_Click(object sender, RoutedEventArgs e) => OpenRooms();
        private void ReportsButton_Click(object sender, RoutedEventArgs e) => OpenReports();
    }
}
