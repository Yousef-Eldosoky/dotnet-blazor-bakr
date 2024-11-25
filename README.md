# Inventory Management System

**Inventory Management System** is a comprehensive application built with **Blazor WebAssembly (WASM)** for the frontend and **.NET Minimal API** for the backend. The system is designed to simplify and streamline inventory management processes for businesses, providing an intuitive interface and robust features to help track, manage, and organize products efficiently.

## Features

- **Product Management**: Add, update, delete, and view product details such as name, price, quantity, and description.
- **Real-Time Synchronization**: Seamless data synchronization between the frontend and backend.
- **Search and Filter**: Quickly locate products with powerful search and filtering capabilities.(Up comming)
- **Authorization and Authentication**: Secure login and role-based access control to ensure data protection.
- **Analytics and Reporting**: Generate detailed reports to analyze inventory trends and performance.(Up comming)
- **Responsive Design**: Optimized for both desktop and mobile devices.
- **Invoices**: You can store and print invoices for each product you sell.(Under work on blazor but fininshed in the backend)

## Technologies Used

### Frontend:
- **Blazor WebAssembly**: Enables a fast, single-page application experience with a clean and modern UI.
- **Bootstrap**: Used for styling and responsive design.
- **JavaScript Interop**: Integrates JavaScript functionality when needed for enhanced UI features.

### Backend:
- **.NET Minimal API**: Provides lightweight and highly efficient endpoints for handling requests.
- **Entity Framework Core**: Manages database operations and data persistence.
- **SQLite/SQL Server**: Supports flexible database options based on business requirements.

### Additional:
- **Dependency Injection**: Ensures maintainable and testable code.
- **RESTful APIs**: Simplifies communication between the frontend and backend.

## Getting Started

### Prerequisites
- .NET 9 SDK or later
- A compatible code editor (e.g., Visual Studio Code, Visual Studio or Rider)
- A database management tool (e.g., SQL Server Management Studio, SQLite Browser)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Yousef-Eldosoky/dotnet-blazor-wasm-bakr.git
   cd dotnet-blazor-wasm-bakr
   ```

2. Navigate to the backend project directory and restore dependencies:
   ```bash
   cd Bakr
   dotnet restore
   ```

3. Build and run the backend API: (It will also run the frontend)
   ```bash
   dotnet run
   ```

4. Access the application in your browser at `https://localhost:7286`.(write your localhost link)

### Configuration
Update the `appsettings.json` file in the API project to configure database connections or other settings or you can user-secrets to store the connection.

## Future Enhancements
- **Barcode Scanning**: Enable barcode scanning for quicker product management.(Up comming)
- **Multi-Language Support**: Add localization for global users.(Up comming)
- **Cloud Integration**: All the data stored in the cloud.
- **Notifications**: Set up low-stock alerts or notifications for inventory updates.(Up comming)

## Contributing
Contributions are welcome! If you'd like to contribute, please fork the repository and create a pull request with your changes.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

