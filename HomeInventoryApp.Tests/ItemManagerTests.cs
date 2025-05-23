using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeInventoryApp.Tests;
using HouseInventory.Services;
using HouseInventory.Models;

    [TestFixture]
    public class ItemManagerTests
    {
        [Test]
        public void AddItem_ShouldCall_DatabaseService()
        {
            var fakeDb = new FakeDatabaseService();
            var manager = new ItemManager(fakeDb);
            var newItem = new Item { ItemName = "Table" };

            manager.AddItem(newItem);

            Assert.IsTrue(fakeDb.AddItemCalled);
        }
    }

