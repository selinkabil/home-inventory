using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HouseInventory.Models;

namespace HouseInventory
{
    public interface IDatabaseService
    {
        void AddRoom(string name, int userId);
        public void AddItem(Item newItem);
    }

}
