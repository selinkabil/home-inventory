using System;
using System.Collections.Generic;
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

namespace HouseInventory.Rooms
{
    public partial class Rooms : Window
    {
 
        private readonly int _userID;
        private readonly string Username;
        public Rooms(string username, int userID)
        {
            InitializeComponent();
            LoadRooms();
            UsernameLabel.Content = username;
        }

        private void LoadRooms()
        {
            var rooms = DatabaseService.Instance.GetRooms();
            RoomsListView.ItemsSource = rooms;
        }

        private void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {
            var addRoomWindow = new AddRoom(_userID);
            addRoomWindow.ShowDialog();
            LoadRooms();
        }

        private void EditRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsListView.SelectedItem is Room selectedRoom)
            {
                var editRoomWindow = new EditRoom(selectedRoom);
                editRoomWindow.ShowDialog();
                LoadRooms();
            }
            else
            {
                MessageBox.Show("Please select a room to edit.", "No Room Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsListView.SelectedItem is Room selectedRoom)
            {
                var result = MessageBox.Show($"Are you sure you want to delete room '{selectedRoom.RoomName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    DatabaseService.Instance.DeleteRoom(selectedRoom.RoomID);
                    LoadRooms();
                }
            }
            else
            {
                MessageBox.Show("Please select a room to delete.", "No Room Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
