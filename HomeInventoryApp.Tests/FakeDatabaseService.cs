using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HouseInventory;
using HouseInventory.Models;
using System.Collections.Generic;

namespace HomeInventoryApp.Tests
{
    internal class FakeDatabaseService : IDatabaseService
    {
        public bool AddRoomCalled { get; private set; }
        public bool AddItemCalled { get; private set; }

        public void AddRoom(string roomName, int buildingId)
        {
            AddRoomCalled = true;
        }

        public void AddItem(Item item)
        {
            AddItemCalled = true;
        }
    }
}
