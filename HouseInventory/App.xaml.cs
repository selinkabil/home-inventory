using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace HouseInventory
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }

    public class SQLiteDatabase
    {
        private const string ConnectionString = @"Data Source=inventory.db;Version=3;";

        // Function to get all users
        public List<User> GetUsers()
        {
            var users = new List<User>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT UserID, Username, Password, RoleID FROM Users";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new User
                        {
                            UserID = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Password = reader.GetString(2),
                            RoleID = reader.GetInt32(3)
                        };
                        users.Add(user);
                    }
                }
            }

            return users;
        }

        // Function to get all roles
        public List<Role> GetRoles()
        {
            var roles = new List<Role>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT RoleID, RoleName FROM Roles";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var role = new Role
                        {
                            RoleID = reader.GetInt32(0),
                            RoleName = reader.GetString(1)
                        };
                        roles.Add(role);
                    }
                }
            }

            return roles;
        }

        // Function to get all rooms
        public List<Room> GetRooms()
        {
            var rooms = new List<Room>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT RoomID, RoomName, BuildingID FROM Rooms";
                            
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var room = new Room
                        {
                            RoomID = reader.GetInt32(0),
                            RoomName = reader.GetString(1),
                            BuildingID = reader.GetInt32(2)
                        };
                        rooms.Add(room);
                    }
                }
            }

            return rooms;
        }

        // Function to get all items
        public List<Item> GetItems()
        {
            var items = new List<Item>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT \r\n    Items.ItemID,\r\n    Items.ItemName,\r\n    Items.Description,\r\n    Items.Category,\r\n    Items.PurchaseDate,\r\n    Items.RoomID, -- Added back here\r\n    Items.Value,\r\n    Items.Quantity,\r\n    Items.ImagePath,\r\n    Rooms.RoomName,\r\n    Buildings.BuildingName\r\nFROM Items\r\nLEFT JOIN Rooms ON Items.RoomID = Rooms.RoomID\r\nLEFT JOIN Buildings ON Rooms.BuildingID = Buildings.BuildingID;\r\n";
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Item
                        {
                            ItemID = reader.GetInt32(0),
                            ItemName = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                            PurchaseDate = reader.IsDBNull(4) ? null : reader.GetString(4),
                            RoomID = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                            Value = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                            Quantity = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                            ImagePath = reader.IsDBNull(8) ? null : reader.GetString(8),
                            RoomName = reader.IsDBNull(9) ? null : reader.GetString(9),
                            BuildingName = reader.IsDBNull(10) ? null : reader.GetString(10)
                        };
                        items.Add(item);
                    }

                }
            }

            return items;
        }

        // Function to get all categories
        public List<Category> GetCategories()
        {
            var categories = new List<Category>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT CategoryID, CategoryName FROM Categories";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var category = new Category
                        {
                            CategoryID = reader.GetInt32(0),
                            CategoryName = reader.GetString(1)
                        };
                        categories.Add(category);
                    }
                }
            }

            return categories;
        }

        // Function to get all buildings
        public List<Building> GetBuildings()
        {
            var buildings = new List<Building>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT BuildingID, BuildingName FROM Buildings";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var building = new Building
                        {
                            BuildingID = reader.GetInt32(0),
                            BuildingName = reader.GetString(1)
                        };
                        buildings.Add(building);
                    }
                }
            }

            return buildings;
        }

        // Function to get roles based on user ID (to get role name for a user)
        public Role GetRoleByUserId(int userId)
        {
            Role role = null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT r.RoleID, r.RoleName FROM Roles r JOIN Users u ON r.RoleID = u.RoleID WHERE u.UserID = @userId";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            role = new Role
                            {
                                RoleID = reader.GetInt32(0),
                                RoleName = reader.GetString(1)
                            };
                        }
                    }
                }
            }

            return role;
        }
    }

    // Models to represent the tables
    public class Building
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
    
    }

    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }

    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }

    public class Room
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public int BuildingID { get; set; }
    }

    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
    }

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
    }

}
