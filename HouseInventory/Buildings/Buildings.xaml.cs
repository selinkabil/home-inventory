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
using HouseInventory.Models;

namespace HouseInventory.Buildings
{
    /// <summary>
    /// Interaction logic for Buildings.xaml
    /// </summary>
    public partial class Buildings : Window
    {
        private readonly int _userID;
        private readonly string Username;

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
            var dashboard = new HouseInventory.Dashboard.MainWindow(Username, _userID)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            dashboard.Show();
            LoadSidebarBuildings();
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

        public Buildings(string username, int userID)
        {
            InitializeComponent();
            _userID = userID;
            LoadSidebarBuildings();
            UsernameLabel.Content = username;
            Username = username;
            
        }

        private void LoadSidebarBuildings()
        {
            var buildings = DatabaseService.Instance.GetBuildings(_userID);
            BuildingsListView.ItemsSource = buildings;
        }

        private void AddBuildingButton_Click(object sender, RoutedEventArgs e)
        {
            var addBuildingWindow = new AddBuilding(_userID);
            addBuildingWindow.ShowDialog();
            LoadSidebarBuildings();
        }

        private void EditBuildingButton_Click(object sender, RoutedEventArgs e)
        {
            if (BuildingsListView.SelectedItem is Building selectedBuilding)
            {
                var editBuildingWindow = new EditBuilding(selectedBuilding);
                editBuildingWindow.ShowDialog();
                LoadSidebarBuildings();
            }
            else
            {
                MessageBox.Show("Please select a building to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteBuildingButton_Click(object sender, RoutedEventArgs e)
        {
            if (BuildingsListView.SelectedItem is Building selectedBuilding)
            {
                DatabaseService.Instance.DeleteBuilding(selectedBuilding.BuildingID, _userID);
                LoadSidebarBuildings();
            }
            else
            {
                MessageBox.Show("Please select a building to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenCategories();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenDashboard();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e) => OpenDashboard();
        private void CategoriesButton_Click(object sender, RoutedEventArgs e) => OpenCategories();
        private void BuildingsButton_Click(object sender, RoutedEventArgs e) => OpenBuildings();
        private void RoomsButton_Click(object sender, RoutedEventArgs e) => OpenRooms();
        private void ReportsButton_Click(object sender, RoutedEventArgs e) => OpenReports();
    }

}