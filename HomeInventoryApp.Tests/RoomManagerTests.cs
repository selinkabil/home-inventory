using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HouseInventory.Services;

namespace HomeInventoryApp.Tests
{
     [TestFixture]
    public class RoomManagerTests
    {
        [Test]
        public void AddRoom_ShouldCall_DatabaseService()
        {
            var fakeDb = new FakeDatabaseService();
            var manager = new RoomManager(fakeDb);

            manager.AddRoom("Living Room", 1);

            Assert.IsTrue(fakeDb.AddRoomCalled);
        }
    }
}
