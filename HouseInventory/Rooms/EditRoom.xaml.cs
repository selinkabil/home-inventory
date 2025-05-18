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
namespace HouseInventory.Rooms
{
    public partial class EditRoom : Window
    {
      
        private readonly int _roomID;
        private readonly int _userID;

        public EditRoom(Room room)
        {
            InitializeComponent();
            _roomID = room.RoomID;
            LoadBuildings();
            RoomNameTextBox.Text = room.RoomName;
            BuildingComboBox.SelectedValue = room.BuildingID;
        }

        private void LoadBuildings()
        {
            var buildings = DatabaseService.Instance.GetBuildings();
            BuildingComboBox.ItemsSource = buildings;
         
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var roomName = RoomNameTextBox.Text.Trim();
            var selectedBuilding = BuildingComboBox.SelectedValue;

            if (string.IsNullOrWhiteSpace(roomName) || selectedBuilding == null)
            {
                MessageBox.Show("Please enter a room name and select a building.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DatabaseService.Instance.UpdateRoom(_roomID, roomName, (int)selectedBuilding);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}