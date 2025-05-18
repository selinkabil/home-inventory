using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static HouseInventory.DatabaseService;
using ClosedXML.Excel;
using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace HouseInventory.Reports
{
    public partial class Reports : Window
    {
        private readonly int _userID;
        private readonly string _username;

        public Reports(string username, int userID)
        {
            InitializeComponent();
            _username = username;
            _userID = userID;

            // Load all reports on startup (optional, or just the first tab)
            LoadInventorySummary();
            LoadCategoryBreakdown();
            LoadRoomUsage();
            LoadTotalValue();
        }

        private void InventorySummary_Click(object sender, RoutedEventArgs e)
        {
            LoadInventorySummary();
        }

        private void LoadInventorySummary()
        {
            var items = DatabaseService.Instance.GetInventorySummary(_userID);
            InventorySummaryGrid.ItemsSource = items;
        }

        private void CategoryBreakdown_Click(object sender, RoutedEventArgs e)
        {
            LoadCategoryBreakdown();
        }

        private void LoadCategoryBreakdown()
        {
            var breakdown = DatabaseService.Instance.GetCategoryBreakdown(_userID)
                .Select(x => new
                {
                    Category = x.Category,
                    ItemCount = x.Count,
                    TotalValue = x.TotalValue
                }).ToList();
            CategoryBreakdownGrid.ItemsSource = breakdown;
        }

        private void RoomsUsage_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomUsage();
        }

        private void LoadRoomUsage()
        {
            var usage = DatabaseService.Instance.GetRoomUsage(_userID)
                .Select(x => new
                {
                    RoomName = x.RoomName,
                    BuildingName = x.BuildingName,
                    ItemCount = x.Count,
                    TotalValue = x.TotalValue
                }).ToList();
            RoomUsageGrid.ItemsSource = usage;
        }

        private void ValueReport_Click(object sender, RoutedEventArgs e)
        {
            LoadTotalValue();
        }

        private void LoadTotalValue()
        {
            double totalValue = DatabaseService.Instance.GetTotalInventoryValue(_userID);
            TotalValueText.Text = $"Total Inventory Value: {totalValue:C}";
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
            var dashboard = new HouseInventory.Dashboard.MainWindow(_username, _userID)
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
            var categories = new HouseInventory.Categories.Categories.Categories(_username, _userID)
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
            var buildings = new HouseInventory.Buildings.Buildings(_username, _userID)
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
            var rooms = new HouseInventory.Rooms.Rooms(_username, _userID)
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
            var reports = new HouseInventory.Reports.Reports(_username, _userID)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            reports.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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

        private void ExportDataGridToExcel(DataGrid dataGrid, string defaultFileName)
        {
            if (dataGrid.ItemsSource == null) return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = defaultFileName
            };

            if (saveFileDialog.ShowDialog() != true)
                return;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Report");

                // Write headers
                var properties = dataGrid.Columns
                    .Where(c => c.Visibility == Visibility.Visible)
                    .Select(c => c.Header.ToString())
                    .ToList();

                for (int i = 0; i < properties.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = properties[i];
                }

                // Write data
                int row = 2;
                foreach (var item in dataGrid.ItemsSource)
                {
                    int col = 1;
                    foreach (var prop in properties)
                    {
                        var value = item.GetType().GetProperty(prop)?.GetValue(item, null);
                        if (value == null)
                        {
                            // Try to get value by matching property name case-insensitively
                            var propertyInfo = item.GetType().GetProperties()
                                .FirstOrDefault(p => string.Equals(p.Name, prop.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));
                            value = propertyInfo?.GetValue(item, null);
                        }
                        worksheet.Cell(row, col).Value = value?.ToString() ?? "";
                        col++;
                    }
                    row++;
                }

                workbook.SaveAs(saveFileDialog.FileName);
            }

            MessageBox.Show("Exported successfully!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportInventorySummary_Click(object sender, RoutedEventArgs e)
        {
            ExportDataGridToExcel(InventorySummaryGrid, "InventorySummary.xlsx");
        }

        private void ExportCategoryBreakdown_Click(object sender, RoutedEventArgs e)
        {
            ExportDataGridToExcel(CategoryBreakdownGrid, "CategoryBreakdown.xlsx");
        }

        private void ExportRoomUsage_Click(object sender, RoutedEventArgs e)
        {
            ExportDataGridToExcel(RoomUsageGrid, "RoomUsage.xlsx");
        }

        private void ExportTotalValue_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "TotalValue.xlsx"
            };

            if (saveFileDialog.ShowDialog() != true)
                return;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Total Value");
                worksheet.Cell(1, 1).Value = "Total Inventory Value";
                worksheet.Cell(1, 2).Value = TotalValueText.Text;
                workbook.SaveAs(saveFileDialog.FileName);
            }

            MessageBox.Show("Exported successfully!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e) => OpenDashboard();
        private void CategoriesButton_Click(object sender, RoutedEventArgs e) => OpenCategories();
        private void BuildingsButton_Click(object sender, RoutedEventArgs e) => OpenBuildings();
        private void RoomsButton_Click(object sender, RoutedEventArgs e) => OpenRooms();
        private void ReportsButton_Click(object sender, RoutedEventArgs e) => OpenReports();
    }
}
