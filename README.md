# YourSouq

YourSouq is a full-stack e-commerce platform that allows customers to browse products, add them to a cart, and make secure payments. The project is built with ASP.NET Core for the backend API and Angular for the frontend client.

## Live Demos
- credentials :
```
email address: admin@test.com
password: Pa$$w0rd
Card for test payment :    5555555555554444 for success payment
                           4242424242424242 for success payment
                           4000000000009995 for declined payment
```
[View Live Demo](https://yoursouq.runasp.net/)

## Features

- Product catalog browsing
- Shopping cart management
- Secure user authentication and authorization
- Payment processing
- Admin and user roles

## Project Structure

```
YourSouq/
│
├── API/                  # ASP.NET Core Web API backend
│   ├── Controllers/      # API controllers
│   ├── Dtos/             # Data transfer objects
│   ├── Entities/         # Entity models
│   ├── Middleware/       # Custom middleware
│   ├── SignalR/          # Real-time communication
│   ├── wwwroot/          # Static files (Angular build output)
│   └── ...               # Other backend files
│
├── Core/                 # Core business logic and interfaces
│   ├── Entities/
│   ├── Interfaces/
│   └── ...
│
├── Infrastructure/       # Data access and external service implementations
│   └── ...
│
├── YourSouq.Client/      # Angular frontend application
│   └── src/
│       └── app/
│           └── layout/
│               └── footer/
│                   └── footer.component.html
│           └── ...
│
├── README.md             # Project documentation
└── YourSouq.sln          # Visual Studio solution file
```

## Getting Started

### Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download)
- [Node.js & npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)

### Setup

1. **Clone the repository:**

   ```sh
   git clone <repository-url>
   cd YourSouq
   ```

2. **Backend (API):**

   - Update `API/appsettings.json` with your database and Redis connection strings.
   - Run database migrations if needed.
   - Start the API:
     ```sh
     cd API
     dotnet run
     ```

3. **Frontend (Angular):**

   - Install dependencies and start the client:
     ```sh
     cd YourSouq.Client
     npm install
     ng serve
     ```

4. **Access the app:**
   - Frontend: [http://localhost:4200](http://localhost:4200)
   - API: [https://localhost:5001](https://localhost:5001) (or as configured)

## License

This project uses third-party libraries. See [`API/wwwroot/3rdpartylicenses.txt`](API/wwwroot/3rdpartylicenses.txt) for details.

---

_An E-commerce project to sell products online. Customers can browse items, add them to a cart, and make secure payments._

## Contact

For any questions or inquiries, please reach out via email at [ahmed.ali050607@gmail.com](mailto:ahmed.ali050607@gmail.com).
