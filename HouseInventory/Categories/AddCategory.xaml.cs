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
using static HouseInventory.DatabaseService;
namespace HouseInventory.Categories
{
    public partial class AddCategory : Window
    {
        private readonly string accessKey;
        private readonly int _userID;
        private string selectedImageUrl = null;

        public AddCategory(int userID)
        {
            InitializeComponent();
            _userID = userID;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string CategoryName = CategoryNameTextBox.Text.Trim();
         
            if (string.IsNullOrEmpty(CategoryName))
            {
                MessageBox.Show("Please enter a category name.");
                return;
            }
            try
            {
                DatabaseService.Instance.AddCategory(CategoryName,_userID);
                MessageBox.Show("Cateogry added successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}");
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
