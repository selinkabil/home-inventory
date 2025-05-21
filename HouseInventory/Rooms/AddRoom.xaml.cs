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
using HouseInventory.Services;
using static HouseInventory.DatabaseService;

namespace HouseInventory.Rooms
{
    public partial class AddRoom : Window
    {
        private readonly int _userID;
        private readonly RoomManager _roomManager;
        public AddRoom(int userID)
        {
            InitializeComponent();
            _userID = userID;
            _roomManager = new RoomManager(DatabaseService.Instance);
            LoadBuildings();
        }

        private void LoadBuildings()
        {
            // Get the list of buildings from the database
            var buildings = DatabaseService.Instance.GetBuildings(_userID);
            BuildingComboBox.ItemsSource = buildings;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string roomName = RoomNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(roomName) || BuildingComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please enter a room name and select a building.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int buildingId = (int)BuildingComboBox.SelectedValue;
            _roomManager.AddRoom(roomName, buildingId);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}