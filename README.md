# HouseInventory

## Introduction

HouseInventory is a WPF desktop application for managing and tracking household items, rooms, buildings, and categories. It allows users to add, edit, delete, and organize inventory items, assign them to rooms and buildings, and generate reports. The application is designed for personal or small business use to keep track of assets and their locations.

## Features

- User authentication and roles
- Add, edit, and delete items, rooms, buildings, and categories
- Image support for items (including Unsplash integration)
- Search, filter, and sort inventory
- Inventory summary and reporting
- Data stored in a local SQLite database

## Installation Instructions

1. **Prerequisites**
   - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Windows OS (WPF support required)
   - [Visual Studio 2022 or later](https://visualstudio.microsoft.com/)

2. **Clone the Repository**

3. **Restore NuGet Packages**
- Open the solution in Visual Studio.
- Right-click the solution and select __Restore NuGet Packages__.

4. **Set Up the Database**
- The app uses a local SQLite database (`inv.db`). By default, it expects the database at:
  ```
  C:\Users\<YourUser>\source\repos\HouseInventory\inv.db
  ```
- If the database does not exist, create it and ensure the required tables (`Items`, `Rooms`, `Buildings`, `Categories`, `Users`, `Roles`) are present.

5. **Environment Variables**
   - Create a `.env` file in the project root with your Unsplash API keys:
     ```
     UNSPLASH_ACCESS_KEY=your_access_key
     UNSPLASH_SECRET_KEY=your_secret_key
     ```

6. **Build and Run**
   - Set `HouseInventory` as the startup project.
   - Press __F5__ or click __Start__ in Visual Studio to build and run the application.

## Notes

- For best results, use the application on Windows 10 or later.
- Make sure the database path in `DatabaseService.cs` matches your environment.
- If you encounter issues with missing images, ensure the image resources are included in the project.
