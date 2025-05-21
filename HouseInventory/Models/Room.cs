using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseInventory.Models
{
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
    }
}

