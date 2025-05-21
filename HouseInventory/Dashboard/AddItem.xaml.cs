using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DotNetEnv;
using HouseInventory.Models;
using HouseInventory.Services;
using static HouseInventory.DatabaseService;

namespace HouseInventory.Dashboard
{
    public partial class AddItem : Window
    {
        private readonly string accessKey;
        private readonly int _userID;
        private string selectedImageUrl = null;
        private readonly ItemManager _itemManager;
        public AddItem(int userID)
        {
            InitializeComponent();
            _userID = userID;
            _itemManager = new ItemManager(DatabaseService.Instance);
            Env.Load();
            accessKey = Env.GetString("UNSPLASH_ACCESS_KEY");

            if (string.IsNullOrEmpty(accessKey))
            {
                MessageBox.Show("Missing API key.");
            }

            var buildings = DatabaseService.Instance.GetBuildings(_userID);
            BuildingComboBox.ItemsSource = buildings;
            BuildingComboBox.DisplayMemberPath = "BuildingName";
            BuildingComboBox.SelectedValuePath = "BuildingID";
            if (buildings.Count > 0)
            {
                 BuildingComboBox.SelectedIndex = 0;
            }
            var categories = DatabaseService.Instance.GetCategories(_userID);
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.DisplayMemberPath = "CategoryName";  
            CategoryComboBox.SelectedValuePath = "CategoryName";
            if (categories.Count > 0)
            {
                CategoryComboBox.SelectedIndex = 0;
            }

        }

        private async Task<string> GetImageUrlFromUnsplash(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrEmpty(accessKey))
            {
                return null;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"https://api.unsplash.com/photos/random?query={Uri.EscapeDataString(query)}&client_id={accessKey}";

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        // RootElement is an object, get "urls" -> "regular"
                        string imageUrl = doc.RootElement.GetProperty("urls").GetProperty("regular").GetString();
                        return imageUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching image: {ex.Message}");
                return null;
            }
        }
        //FIX LATER:fix this to get the selected room ID from the UI, double check with database tabel item
        private async void UploadRandomImageButton_Click(object sender, RoutedEventArgs e)
        {

            string itemName = ItemNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(itemName))
            {
                MessageBox.Show("Please enter an item name.");
                return;
            }

            string imageUrl = await GetImageUrlFromUnsplash(itemName);

            if (string.IsNullOrEmpty(imageUrl))
            {
                MessageBox.Show("No image found for the specified item.");
                return;
            }
            var previewDialog = new ImagePreviewDialog(imageUrl)
            {
                Owner = this
            };

            bool? dialogResult = previewDialog.ShowDialog();

            if (dialogResult == true)
            {
                ItemImage.Source = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
                selectedImageUrl = imageUrl;
                MessageBox.Show("Image selected!");
            }
            else
            {
                selectedImageUrl = null;
                MessageBox.Show("Image not used. You can try another image or change the item name.");
            }

        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFileName = openFileDialog.FileName;
                ItemImage.Source = new BitmapImage(new Uri(selectedFileName));
                selectedImageUrl = selectedFileName;
                MessageBox.Show("Image selected! ");
            }
        }



        private void BuildingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            if (BuildingComboBox.SelectedItem is Building selectedBuilding)
            {
                var allRooms = DatabaseService.Instance.GetRooms();
                var filteredRooms = allRooms.FindAll(r => r.BuildingID == selectedBuilding.BuildingID);

                RoomComboBox.ItemsSource = filteredRooms;
                RoomComboBox.DisplayMemberPath = "RoomName";
                RoomComboBox.SelectedValuePath = "RoomID";

                if (filteredRooms.Count > 0)
                {
                    RoomComboBox.SelectedIndex = 0;
                }
            }

    }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string itemName = ItemNameTextBox.Text.Trim();
            string category = CategoryComboBox.Text.Trim();
            string purchaseDate = PurchaseDatePicker.Text.Trim();
            string valueText = ValueTextBox.Text.Trim();
            string quantityText = QuantityTextBox.Text.Trim();

         
            if (string.IsNullOrEmpty(itemName))
            {
                MessageBox.Show("Please enter an item name.");
                return;
            }

        
            if (!(RoomComboBox.SelectedItem is Room selectedRoom))
            {
                MessageBox.Show("Please select a room.");
                return;
            }

            if (string.IsNullOrEmpty(category))
            {
                MessageBox.Show("Please select a category.");
                return;
            }

            if (!string.IsNullOrEmpty(purchaseDate) && !DateTime.TryParse(purchaseDate, out _))
            {
                MessageBox.Show("Please enter a valid purchase date or leave it empty.");
                return;
            }

            if (!int.TryParse(quantityText, out int quantity) || quantity < 1)
            {
                MessageBox.Show("Please enter a valid quantity of 1 or more.");
                return;
            }


            if (!double.TryParse(valueText, out double value) || value < 0)
            {
                MessageBox.Show("Please enter a valid non-negative value.");
                return;
            }

           
            
            int selectedRoomID = selectedRoom.RoomID;

            var newItem = new Item
            {
                ItemName = itemName,
                Description = DescriptionTextBox.Text, 
                Category = category,
                PurchaseDate = purchaseDate,
                RoomID = selectedRoomID,
                Value = value,
                Quantity = quantity,
                ImagePath = selectedImageUrl,
                UserID = _userID
            };

            try
            {
                _itemManager.AddItem(newItem);
                MessageBox.Show("Item added successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding item: {ex.Message}");
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
