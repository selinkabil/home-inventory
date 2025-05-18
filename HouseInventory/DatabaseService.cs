using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Windows;

namespace HouseInventory
{
    public sealed class DatabaseService
    {
        private static readonly DatabaseService _instance = new DatabaseService();
        private readonly SQLiteConnection _connection;

        private const string ConnectionString = "Data Source=C:\\Users\\Lenovo RYZEN 7\\source\\repos\\HouseInventory\\inv.db;Version=3;";

        // Singleton database class
        private DatabaseService()
        {
            _connection = new SQLiteConnection(ConnectionString);
            _connection.Open();
        }
        

        public static DatabaseService Instance => _instance;
        public SQLiteConnection Connection => _connection;


        //ITEMS
        public void AddItem(Item newItem)
        {
            string sql = @"
            INSERT INTO Items 
            (ItemName, Description, Category, PurchaseDate, RoomID, Value, Quantity, ImagePath, UserID)
            VALUES
            (@ItemName, @Description, @Category, @PurchaseDate, @RoomID, @Value, @Quantity, @ImagePath, @UserID)";

            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@ItemName", newItem.ItemName);
            cmd.Parameters.AddWithValue("@Description", (object)newItem.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Category", (object)newItem.Category ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PurchaseDate", (object)newItem.PurchaseDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RoomID", newItem.RoomID > 0 ? (object)newItem.RoomID : DBNull.Value);
            cmd.Parameters.AddWithValue("@Value", newItem.Value);
            cmd.Parameters.AddWithValue("@Quantity", newItem.Quantity);
            cmd.Parameters.AddWithValue("@ImagePath", (object)newItem.ImagePath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserID", newItem.UserID);

            cmd.ExecuteNonQuery();
        }

        public void DeleteItem(int itemId)
        {
            const string sql = "DELETE FROM Items WHERE ItemID = @ItemID";

            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@ItemID", itemId);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected == 0)
            {
                throw new Exception($"Item with ID {itemId} not found or could not be deleted.");
            }
        }

        public void UpdateItem(Item item)
        {
            var command = new SQLiteCommand(
                "UPDATE Items SET ItemName = @ItemName, Description = @Description, Category = @Category, " +
                "PurchaseDate = @PurchaseDate, RoomID = @RoomID, Value = @Value, Quantity = @Quantity, " +
                "ImagePath = @ImagePath, UserID = @UserID WHERE ItemID = @ItemID", _connection);

            command.Parameters.AddWithValue("@ItemName", item.ItemName);
            command.Parameters.AddWithValue("@Description", item.Description);
            command.Parameters.AddWithValue("@Category", item.Category);

            if (string.IsNullOrWhiteSpace(item.PurchaseDate))
                command.Parameters.AddWithValue("@PurchaseDate", DBNull.Value);
            else
                command.Parameters.AddWithValue("@PurchaseDate", item.PurchaseDate);

            command.Parameters.AddWithValue("@RoomID", item.RoomID);
            command.Parameters.AddWithValue("@Value", item.Value);
            command.Parameters.AddWithValue("@Quantity", item.Quantity);
            command.Parameters.AddWithValue("@ImagePath", item.ImagePath);
            command.Parameters.AddWithValue("@UserID", item.UserID);
            command.Parameters.AddWithValue("@ItemID", item.ItemID);

            command.ExecuteNonQuery();
        }

        public List<Item> GetItems(int userId)
        {


            var items = new List<Item>();
            const string query = @"
        SELECT Items.ItemID, Items.ItemName, Items.Description, Items.Category,
               Items.PurchaseDate, Items.RoomID, Items.Value, Items.Quantity, Items.ImagePath,
               Rooms.RoomName, Buildings.BuildingName
        FROM Items
        LEFT JOIN Rooms ON Items.RoomID = Rooms.RoomID
        LEFT JOIN Buildings ON Rooms.BuildingID = Buildings.BuildingID
        WHERE Items.UserID = @UserID;
    ";

            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@UserID", userId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new Item
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
                    BuildingName = reader.IsDBNull(10) ? null : reader.GetString(10),
                    UserID = userId
                });
            }


            return items;
        }
        public Item GetItemById(int itemId)
        {
            const string query = @"
    SELECT Items.ItemID, Items.ItemName, Items.Description, Items.Category,
           Items.PurchaseDate, Items.RoomID, Items.Value, Items.Quantity, Items.ImagePath,
           Rooms.RoomName, Buildings.BuildingName
    FROM Items
    LEFT JOIN Rooms ON Items.RoomID = Rooms.RoomID
    LEFT JOIN Buildings ON Rooms.BuildingID = Buildings.BuildingID
    WHERE Items.ItemID = @ItemID;
    ";

            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@ItemID", itemId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Item
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
            }

            // If no item found, return null or throw exception depending on your preference
            return null;
        }

        public List<User> GetUsers()
        {
            var users = new List<User>();
            const string query = "SELECT UserID, Username, Password, RoleID FROM Users";

            using var cmd = new SQLiteCommand(query, _connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new User
                {
                    UserID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    RoleID = reader.GetInt32(3)
                });
            }

            return users;
        }

        //USERS

        public bool RegisterUser(string username, string password, int roleId = 2)
        {
            // Check if username already exists
            const string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            using (var checkCmd = new SQLiteCommand(checkQuery, _connection))
            {
                checkCmd.Parameters.AddWithValue("@Username", username);
                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                if (exists)
                    return false; // Username already exists
            }

            // Insert new user
            const string insertQuery = "INSERT INTO Users (Username, Password, RoleID) VALUES (@Username, @Password, @RoleID)";
            using (var insertCmd = new SQLiteCommand(insertQuery, _connection))
            {
                insertCmd.Parameters.AddWithValue("@Username", username);
                insertCmd.Parameters.AddWithValue("@Password", password);
                insertCmd.Parameters.AddWithValue("@RoleID", roleId); // Default to regular user
                insertCmd.ExecuteNonQuery();
            }
            return true;
        }

        public List<Role> GetRoles()
        {
            var roles = new List<Role>();
            const string query = "SELECT RoleID, RoleName FROM Roles";

            using var cmd = new SQLiteCommand(query, _connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                roles.Add(new Role
                {
                    RoleID = reader.GetInt32(0),
                    RoleName = reader.GetString(1)
                });
            }

            return roles;
        }

        public User GetUserByCredentials(string username, string password)
        {
            const string query = @"
        SELECT UserID, Username, Password, RoleID 
        FROM Users 
        WHERE Username = @username AND Password = @password
        LIMIT 1;
    ";

            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            using SQLiteDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    UserID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    RoleID = reader.GetInt32(3)
                };
            }
            else
            {
                // No user found
                return null;
            }
        }



        public Role GetRoleByUserId(int userId)
        {
            Role role = null;
            const string query = @"
                SELECT r.RoleID, r.RoleName
                FROM Roles r
                JOIN Users u ON r.RoleID = u.RoleID
                WHERE u.UserID = @userId";

            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                role = new Role
                {
                    RoleID = reader.GetInt32(0),
                    RoleName = reader.GetString(1)
                };
            }

            return role;
        }

        //ROOMS
        public List<Room> GetRooms()
        {
            var rooms = new List<Room>();
            const string query = @"
        SELECT Rooms.RoomID, Rooms.RoomName, Rooms.BuildingID, Buildings.BuildingName
        FROM Rooms
        LEFT JOIN Buildings ON Rooms.BuildingID = Buildings.BuildingID";

            using var cmd = new SQLiteCommand(query, _connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                rooms.Add(new Room
                {
                    RoomID = reader.GetInt32(0),
                    RoomName = reader.GetString(1),
                    BuildingID = reader.GetInt32(2),
                    BuildingName = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }

            return rooms;
        }

        public void AddRoom(string roomName, int buildingId)
        {
            const string sql = "INSERT INTO Rooms (RoomName, BuildingID) VALUES (@RoomName, @BuildingID)";
            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@RoomName", roomName);
            cmd.Parameters.AddWithValue("@BuildingID", buildingId);
            cmd.ExecuteNonQuery();
        }

        public void UpdateRoom(int roomId, string newRoomName, int buildingId)
        {
            const string sql = "UPDATE Rooms SET RoomName = @RoomName, BuildingID = @BuildingID WHERE RoomID = @RoomID";
            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@RoomName", newRoomName);
            cmd.Parameters.AddWithValue("@BuildingID", buildingId);
            cmd.Parameters.AddWithValue("@RoomID", roomId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteRoom(int roomId)
        {
            const string sql = "DELETE FROM Rooms WHERE RoomID = @RoomID";
            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@RoomID", roomId);
            cmd.ExecuteNonQuery();
        }

        public string GetRoomNameById(int roomId)
        {
            const string query = "SELECT RoomName FROM Rooms WHERE RoomID = @RoomID";
            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@RoomID", roomId);
            var result = cmd.ExecuteScalar();
            return result != null ? result.ToString() : string.Empty;
        }



        //CATEGORIES
        public List<Category> GetCategories(int userId)
        {
            var categories = new List<Category>();
            const string sql = "SELECT CategoryID, CategoryName, UserID FROM Categories WHERE UserID = @UserID";

            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@UserID", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(new Category
                {
                    CategoryID = Convert.ToInt32(reader["CategoryID"]),
                    CategoryName = reader["CategoryName"].ToString(),
                    UserID = Convert.ToInt32(reader["UserID"])
                });
            }

            return categories;
        }


        public void AddCategory(string categoryName, int userId)
        {
            const string sql = "INSERT INTO Categories (CategoryName, UserID) VALUES (@CategoryName, @UserID)";

            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@CategoryName", categoryName);
            cmd.Parameters.AddWithValue("@UserID", userId);

            cmd.ExecuteNonQuery();
        }

        public void UpdateCategory(Category category)
        {
            const string sql = "UPDATE Categories SET CategoryName = @CategoryName WHERE CategoryID = @CategoryID AND UserID = @UserID";
            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
            cmd.Parameters.AddWithValue("@CategoryID", category.CategoryID);
            cmd.Parameters.AddWithValue("@UserID", category.UserID);
            cmd.ExecuteNonQuery();
        }



        public void DeleteCategory(int categoryId)
        {
            const string sql = "DELETE FROM Categories WHERE CategoryID = @CategoryID";
            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@CategoryID", categoryId);
            cmd.ExecuteNonQuery();
        }
        public int GetCategoryIdByName(string categoryName)
        {
            const string sql = "SELECT CategoryID FROM Categories WHERE CategoryName = @CategoryName";
            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@CategoryName", categoryName);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        //BUILDINGS

        public List<Building> GetBuildings()
        {
            var buildings = new List<Building>();
            const string query = "SELECT BuildingID, BuildingName FROM Buildings";

            using var cmd = new SQLiteCommand(query, _connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                buildings.Add(new Building
                {
                    BuildingID = reader.GetInt32(0),
                    BuildingName = reader.GetString(1)
                });
            }

            return buildings;
        }

        public void AddBuilding(string buildingName)
        {
            const string sql = "INSERT INTO Buildings (BuildingName) VALUES (@BuildingName)";
            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@BuildingName", buildingName);
            cmd.ExecuteNonQuery();
        }

        public void UpdateBuilding(int buildingId, string newBuildingName)
        {
            const string sql = "UPDATE Buildings SET BuildingName = @BuildingName WHERE BuildingID = @BuildingID";
            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@BuildingName", newBuildingName);
            cmd.Parameters.AddWithValue("@BuildingID", buildingId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteBuilding(int buildingId)
        {
            const string sql = "DELETE FROM Buildings WHERE BuildingID = @BuildingID";
            using var cmd = new SQLiteCommand(sql, _connection);
            cmd.Parameters.AddWithValue("@BuildingID", buildingId);
            cmd.ExecuteNonQuery();
        }


        public List<Room> GetRoomsForBuilding(int buildingId)
        {
            var rooms = new List<Room>();
            const string query = "SELECT RoomID, RoomName, BuildingID FROM Rooms WHERE BuildingID = @BuildingID";

            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@BuildingID", buildingId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rooms.Add(new Room
                {
                    RoomID = reader.GetInt32(0),
                    RoomName = reader.GetString(1),
                    BuildingID = reader.GetInt32(2)
                });
            }
            return rooms;
        }

        public Building GetBuildingByRoomId(int roomId)
        {
            const string query = @"
        SELECT b.BuildingID, b.BuildingName
        FROM Buildings b
        JOIN Rooms r ON b.BuildingID = r.BuildingID
        WHERE r.RoomID = @RoomID
        LIMIT 1";

            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@RoomID", roomId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Building
                {
                    BuildingID = reader.GetInt32(0),
                    BuildingName = reader.GetString(1)
                };
            }
            return null; // or throw exception if preferred
        }

        // 1. Inventory Summary: Returns all items with room and building info
        public List<Item> GetInventorySummary(int userId)
        {
            var items = new List<Item>();
            const string query = @"
                SELECT Items.ItemID, Items.ItemName, Items.Description, Items.Category,
                    Items.PurchaseDate, Items.RoomID, Items.Value, Items.Quantity, Items.ImagePath,
                    Rooms.RoomName, Buildings.BuildingName, Items.UserID
                FROM Items
                LEFT JOIN Rooms ON Items.RoomID = Rooms.RoomID
                LEFT JOIN Buildings ON Rooms.BuildingID = Buildings.BuildingID
                WHERE Items.UserID = @UserID;
            ";
            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@UserID", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new Item
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
                    BuildingName = reader.IsDBNull(10) ? null : reader.GetString(10),
                    UserID = reader.IsDBNull(11) ? 0 : reader.GetInt32(11)
                });
            }
            return items;
        }

        // 2. Category Breakdown: Returns item count and total value per category
        public List<(string Category, int Count, double TotalValue)> GetCategoryBreakdown(int userId)
        {
            var result = new List<(string, int, double)>();
            const string query = @"
                SELECT Category, COUNT(*) AS ItemCount, SUM(Value * Quantity) AS TotalValue
                FROM Items
                WHERE UserID = @UserID
                GROUP BY Category
                ORDER BY Category;
            ";
            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@UserID", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add((
                    reader.IsDBNull(0) ? "Uncategorized" : reader.GetString(0),
                    reader.GetInt32(1),
                    reader.IsDBNull(2) ? 0 : reader.GetDouble(2)
                ));
            }
            return result;
        }

        // 3. Room Usage: Returns item count and total value per room
        public List<(string RoomName, string BuildingName, int Count, double TotalValue)> GetRoomUsage(int userId)
        {
            var result = new List<(string, string, int, double)>();
            const string query = @"
                SELECT Rooms.RoomName, Buildings.BuildingName, COUNT(Items.ItemID) AS ItemCount, SUM(Items.Value * Items.Quantity) AS TotalValue
                FROM Items
                LEFT JOIN Rooms ON Items.RoomID = Rooms.RoomID
                LEFT JOIN Buildings ON Rooms.BuildingID = Buildings.BuildingID
                WHERE Items.UserID = @UserID
                GROUP BY Rooms.RoomName, Buildings.BuildingName
                ORDER BY Buildings.BuildingName, Rooms.RoomName;
            ";
            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@UserID", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add((
                    reader.IsDBNull(0) ? "No Room" : reader.GetString(0),
                    reader.IsDBNull(1) ? "No Building" : reader.GetString(1),
                    reader.GetInt32(2),
                    reader.IsDBNull(3) ? 0 : reader.GetDouble(3)
                ));
            }
            return result;
        }

        // 4. Total Value: Returns the total value of all items
        public double GetTotalInventoryValue(int userId)
        {
            const string query = @"
                SELECT SUM(Value * Quantity)
                FROM Items
                WHERE UserID = @UserID;
            ";
            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@UserID", userId);
            var result = cmd.ExecuteScalar();
            return result != DBNull.Value && result != null ? Convert.ToDouble(result) : 0.0;
        }

        // 5. High-Value Items: Returns items sorted by value descending
        public List<Item> GetHighValueItems(int userId, int topN = 10)
        {
            var items = new List<Item>();
            string query = $@"
                SELECT Items.ItemID, Items.ItemName, Items.Description, Items.Category,
                    Items.PurchaseDate, Items.RoomID, Items.Value, Items.Quantity, Items.ImagePath,
                    Rooms.RoomName, Buildings.BuildingName, Items.UserID
                FROM Items
                LEFT JOIN Rooms ON Items.RoomID = Rooms.RoomID
                LEFT JOIN Buildings ON Rooms.BuildingID = Buildings.BuildingID
                WHERE Items.UserID = @UserID
                ORDER BY Items.Value * Items.Quantity DESC
                LIMIT {topN};
            ";
            using var cmd = new SQLiteCommand(query, _connection);
            cmd.Parameters.AddWithValue("@UserID", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new Item
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
                    BuildingName = reader.IsDBNull(10) ? null : reader.GetString(10),
                    UserID = reader.IsDBNull(11) ? 0 : reader.GetInt32(11)
                });
            }
            return items;
        }

        public class Building
        {
            public int BuildingID { get; set; }
            public string BuildingName { get; set; }
        }

        public class Category
        {
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }
            public int UserID { get; set; }
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
            public string BuildingName { get; set; }
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

            public int UserID { get; set; }

        }
    }
}