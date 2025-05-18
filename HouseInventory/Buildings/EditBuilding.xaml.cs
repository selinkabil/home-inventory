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

namespace HouseInventory.Buildings
{
    public partial class EditBuilding : Window
    {
        private readonly Building _building;

        public EditBuilding(Building building)
        {
            InitializeComponent();
            _building = building;
            BuildingNameTextBox.Text = _building.BuildingName;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var newName = BuildingNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Building name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DatabaseService.Instance.UpdateBuilding(_building.BuildingID, newName);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
