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
using HouseInventory;
namespace HouseInventory.Dashboard;
using static HouseInventory.DatabaseService;

public partial class FilterWindow : Window
{
    public List<Item> FilteredItems { get; private set; } = new List<Item>();
    private List<Item> AllItems;

    public FilterWindow(List<Item> items)
    {
        InitializeComponent();
        AllItems = new List<Item>(items);

        var categories = AllItems.Select(i => i.Category)
                                 .Where(c => !string.IsNullOrWhiteSpace(c))
                                 .Distinct()
                                 .ToList();
        FilterCategory.ItemsSource = categories;

        var buildings = AllItems.Select(i => i.BuildingName)
                                .Where(b => !string.IsNullOrWhiteSpace(b))
                                .Distinct()
                                .ToList();
        FilterBuilding.ItemsSource = buildings;

        var rooms = AllItems.Select(i => i.RoomName)
                            .Where(r => !string.IsNullOrWhiteSpace(r))
                            .Distinct()
                            .ToList();
        FilterRoom.ItemsSource = rooms;
    }


    private void ApplyFilters_Click(object sender, RoutedEventArgs e)
    {
        var filtered = AllItems.Where(item =>
        {
            // Category
            if (FilterCategory.SelectedItem is string category &&
                !string.IsNullOrWhiteSpace(category) &&
                !string.Equals(item.Category, category, StringComparison.OrdinalIgnoreCase))
                return false;

            // Building Name
            if (!string.IsNullOrWhiteSpace(FilterBuilding.Text) &&
                !item.BuildingName.Contains(FilterBuilding.Text, StringComparison.OrdinalIgnoreCase))
                return false;

            // Value Range
            if (double.TryParse(ValueMinTextBox.Text, out double minValue) && item.Value < minValue)
                return false;

            if (double.TryParse(ValueMaxTextBox.Text, out double maxValue) && item.Value > maxValue)
                return false;

            // Quantity Range
            if (int.TryParse(QuantityMinTextBox.Text, out int minQty) && item.Quantity < minQty)
                return false;

            if (int.TryParse(QuantityMaxTextBox.Text, out int maxQty) && item.Quantity > maxQty)
                return false;

            // Purchase Date Range
            if (StartDatePicker.SelectedDate.HasValue)
            {
                if (DateTime.TryParse(item.PurchaseDate, out DateTime itemDate))
                {
                    if (itemDate < StartDatePicker.SelectedDate.Value)
                        return false;
                }
                else
                {
                    return false;
                }
            }

            if (EndDatePicker.SelectedDate.HasValue)
            {
                if (DateTime.TryParse(item.PurchaseDate, out DateTime itemDate))
                {
                    if (itemDate > EndDatePicker.SelectedDate.Value)
                        return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }).ToList();

        FilteredItems = filtered;
        DialogResult = true;
    }

    // Value sliders -> TextBoxes
    private void ValueMinSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ValueMinTextBox.Text = ((int)ValueMinSlider.Value).ToString();
        if (ValueMinSlider.Value > ValueMaxSlider.Value)
            ValueMaxSlider.Value = ValueMinSlider.Value;
    }

    private void ValueMaxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ValueMaxTextBox.Text = ((int)ValueMaxSlider.Value).ToString();
        if (ValueMaxSlider.Value < ValueMinSlider.Value)
            ValueMinSlider.Value = ValueMaxSlider.Value;
    }

    // Value TextBoxes -> sliders
    private void ValueMinTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(ValueMinTextBox.Text, out int value))
        {
            if (value > ValueMaxSlider.Value) value = (int)ValueMaxSlider.Value;
            ValueMinSlider.Value = value;
        }
    }

    private void ValueMaxTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(ValueMaxTextBox.Text, out int value))
        {
            if (value < ValueMinSlider.Value) value = (int)ValueMinSlider.Value;
            ValueMaxSlider.Value = value;
        }
    }

    // Quantity sliders -> TextBoxes
    private void QuantityMinSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        QuantityMinTextBox.Text = ((int)QuantityMinSlider.Value).ToString();
        if (QuantityMinSlider.Value > QuantityMaxSlider.Value)
            QuantityMaxSlider.Value = QuantityMinSlider.Value;
    }

    private void QuantityMaxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        QuantityMaxTextBox.Text = ((int)QuantityMaxSlider.Value).ToString();
        if (QuantityMaxSlider.Value < QuantityMinSlider.Value)
            QuantityMinSlider.Value = QuantityMaxSlider.Value;
    }

    // Quantity TextBoxes -> sliders
    private void QuantityMinTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(QuantityMinTextBox.Text, out int value))
        {
            if (value > QuantityMaxSlider.Value) value = (int)QuantityMaxSlider.Value;
            QuantityMinSlider.Value = value;
        }
    }

    private void QuantityMaxTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(QuantityMaxTextBox.Text, out int value))
        {
            if (value < QuantityMinSlider.Value) value = (int)QuantityMinSlider.Value;
            QuantityMaxSlider.Value = value;
        }
    }
    private void FilterWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Set Value sliders to their maximum
        ValueMinSlider.Value = ValueMinSlider.Minimum;
        ValueMaxSlider.Value = ValueMaxSlider.Maximum;

        // Set Value TextBoxes
        ValueMinTextBox.Text = ((int)ValueMinSlider.Minimum).ToString();
        ValueMaxTextBox.Text = ((int)ValueMaxSlider.Maximum).ToString();

        // Set Quantity sliders to their maximum
        QuantityMinSlider.Value = QuantityMinSlider.Minimum;
        QuantityMaxSlider.Value = QuantityMaxSlider.Maximum;

        // Set Quantity TextBoxes
        QuantityMinTextBox.Text = ((int)QuantityMinSlider.Minimum).ToString();
        QuantityMaxTextBox.Text = ((int)QuantityMaxSlider.Maximum).ToString();
    }





    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
