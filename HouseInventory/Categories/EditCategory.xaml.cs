using System;
using System.Windows;
using static HouseInventory.DatabaseService;

namespace HouseInventory.Categories
{
    public partial class EditCategory : Window
    {
        private Category _category;

        public EditCategory(Category categoryToEdit)
        {
            InitializeComponent();
            _category = categoryToEdit;
            CategoryNameTextBox.Text = _category.CategoryName;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                MessageBox.Show("Category name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _category.CategoryName = CategoryNameTextBox.Text.Trim();

            try
            {
                DatabaseService.Instance.UpdateCategory(_category);
                MessageBox.Show("Category updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
