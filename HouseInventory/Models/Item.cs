using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HouseInventory.Models
{
    public class Item
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string PurchaseDate { get; set; }
        public int RoomID { get; set; }
        public double Value { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }
        public string RoomName { get; set; }
        public string BuildingName { get; set; }
        public int UserID { get; set; }
    }
}
