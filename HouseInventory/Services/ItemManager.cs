using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HouseInventory.Models;

namespace HouseInventory.Services
{
    public class ItemManager
    {
        private readonly IDatabaseService _database;

        public ItemManager(IDatabaseService database)
        {
            _database = database;
        }

        public void AddItem(Item item)
        {
            _database.AddItem(item);
        }
    }
}
