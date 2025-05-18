using System;
using System.Windows;
using System.Windows.Media.Imaging;
using static HouseInventory.DatabaseService;
namespace HouseInventory.Dashboard
{
    public partial class ImagePreviewDialog : Window
    {
        public bool UserConfirmed { get; private set; }

        public ImagePreviewDialog(string imageUrl)
        {
            InitializeComponent();
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageUrl);
                bitmap.EndInit();
                PreviewImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load image: {ex.Message}");
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmed = true;
            this.DialogResult = true;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmed = false;
            this.DialogResult = false;
            this.Close();
        }
    }
}
