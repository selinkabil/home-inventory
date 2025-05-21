using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HouseInventory.Models;

namespace HouseInventory.Services
{
    public class RoomManager
    {
        private readonly IDatabaseService _database;

        public RoomManager(IDatabaseService database)
        {
            _database = database;
        }

        public void AddRoom(string roomName, int buildingId)
        {
            _database.AddRoom(roomName, buildingId);
        }
    }
}
