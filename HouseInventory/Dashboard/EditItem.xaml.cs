using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DotNetEnv;
using Microsoft.Win32;
using static HouseInventory.DatabaseService;
namespace HouseInventory.Dashboard
{
    public partial class EditItem : Window
    {
        private readonly string accessKey;
        private string _imagePath;
        public int UserID { get; private set; }
        private Item _item;
        public EditItem(Item itemToEdit)
        {
            InitializeComponent();

            CategoryComboBox.ItemsSource = DatabaseService.Instance.GetCategories(UserID);
            CategoryComboBox.DisplayMemberPath = "CategoryName";
            CategoryComboBox.SelectedValuePath = "CategoryID";

            BuildingComboBox.ItemsSource = DatabaseService.Instance.GetBuildings();
            BuildingComboBox.DisplayMemberPath = "BuildingName"; 
            BuildingComboBox.SelectedValuePath = "BuildingID";

            _item = itemToEdit;
            UserID = itemToEdit.UserID;
            ItemNameTextBox.Text = _item.ItemName;
            DescriptionTextBox.Text = _item.Description;

            if (!string.IsNullOrEmpty(_item.Category))
                CategoryComboBox.SelectedItem = _item.Category;

            if (!string.IsNullOrEmpty(_item.BuildingName))
                BuildingComboBox.SelectedItem = _item.BuildingName;

            // Set PurchaseDatePicker selected date, assuming _item.PurchaseDate is string (parse to DateTime)
            if (DateTime.TryParse(_item.PurchaseDate, out DateTime purchaseDate))
                PurchaseDatePicker.SelectedDate = purchaseDate;
            else
                PurchaseDatePicker.SelectedDate = null;

            QuantityTextBox.Text = _item.Quantity.ToString();
            ValueTextBox.Text = _item.Value.ToString();

            _imagePath = _item.ImagePath;
            if (!string.IsNullOrWhiteSpace(_imagePath))
            {
                ItemImage.Source = new BitmapImage(new Uri(_imagePath, UriKind.Absolute));
            }
            else
            {
                ItemImage.Source = null;
            }

            // Set RoomComboBox selected item based on _item.RoomID
            var roomName = DatabaseService.Instance.GetRoomNameById(_item.RoomID);
            if (!string.IsNullOrEmpty(roomName))
                RoomComboBox.SelectedItem = roomName;

            Env.Load();
            accessKey = Env.GetString("UNSPLASH_ACCESS_KEY");
        }

        private void BuildingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BuildingComboBox.SelectedItem is Building selectedBuilding)
            {
                RoomComboBox.ItemsSource = DatabaseService.Instance.GetRoomsForBuilding(selectedBuilding.BuildingID);
                RoomComboBox.DisplayMemberPath = "RoomName";
                RoomComboBox.SelectedValuePath = "RoomID";
            }
            else
            {
                RoomComboBox.ItemsSource = null;
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

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                _imagePath = openFileDialog.FileName;
            }
        }
        private async void UploadRandomImageButton_Click(object sender, RoutedEventArgs e)
        {
            string itemName = ItemNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(itemName))
            {
                MessageBox.Show("Please enter an item name first.");
                return;
            }

            string imageUrl = await GetImageUrlFromUnsplash(itemName);

            if (string.IsNullOrEmpty(imageUrl))
            {
                MessageBox.Show("No image found.");
                return;
            }

            var previewDialog = new ImagePreviewDialog(imageUrl)
            {
                Owner = this
            };
            bool? dialogResult = previewDialog.ShowDialog();

            if (dialogResult == true)
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    try
                    {
                        ItemImage.Source = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
                    }
                    catch
                    {
                        // fallback image if file not found or invalid
                        ItemImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/no-image.png"));
                    }
                }
                ItemImage.Source = new BitmapImage(new Uri(imageUrl));
                MessageBox.Show("Random image applied.");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ItemNameTextBox.Text))
            {
                MessageBox.Show("Item name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                MessageBox.Show("Quantity must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(ValueTextBox.Text, out double value))
            {
                MessageBox.Show("Value must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string category = (CategoryComboBox.SelectedItem as Category)?.CategoryName ?? "";

            int roomId = 0;
            if (RoomComboBox.SelectedItem is Room selectedRoom)
            {
                roomId = selectedRoom.RoomID;
            }

            string purchaseDate = PurchaseDatePicker.SelectedDate?.ToString("yyyy-MM-dd") ?? "";

            _item.ItemName = ItemNameTextBox.Text.Trim();
            _item.Description = DescriptionTextBox.Text.Trim();
            _item.Category = category;
            _item.PurchaseDate = purchaseDate;
            _item.RoomID = roomId;
            _item.Value = value;
            _item.Quantity = quantity;
            _item.ImagePath = _imagePath;
            _item.UserID = this.UserID;
            _item.ItemID=_item.ItemID; // Assuming you have a way to set the ItemID

            try
            {
                DatabaseService.Instance.UpdateItem(_item);
                MessageBox.Show("Item updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
