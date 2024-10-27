# DogHouseService API

DogHouseService is a RESTful API built with ASP.NET Core, providing functionality to manage dog data. The API supports operations for retrieving, creating, and managing dogs stored in an MS SQL database. 

## Features

- **Ping Endpoint**: A health check endpoint.
- **Retrieve Dogs**: List dogs with sorting and pagination support.
- **Create Dog**: Add a new dog to the database with input validation.
- **Rate Limiting**: Protects the API from excessive requests by limiting requests per second.

## Technologies Used

- **ASP.NET Core Web API**
- **Entity Framework Core** (Code-First with MS SQL Database)
- **Rate Limiting** (for managing request throttling)
- **Unit Testing** (xUnit)
- **In-Memory Database** (for testing)

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Postman](https://www.postman.com/downloads/) (optional, for testing API endpoints)

## Getting Started

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd DogHouseService
```

### Step 2: Update Connection String

Update the connection string in `appsettings.json` to point to your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=<YOUR_SERVER>;Database=DogHouseDB;Trusted_Connection=True;"
}
```

### Step 3: Apply Database Migrations

Use the following commands to create the database and apply migrations:

```bash
dotnet ef database update
```

### Step 4: Run the API

Start the API using:

```bash
dotnet run
```

The API will be available at `http://localhost:5000`.

## Endpoints

### 1. Health Check

**GET** `/ping`  
Returns a service version message.

**Response:**

```json
"Dogshouseservice.Version1.0.1"
```

### 2. List Dogs

**GET** `/dogs`  
Retrieves a list of dogs with sorting and pagination options.

| Parameter    | Type   | Description                           |
|--------------|--------|---------------------------------------|
| `attribute`  | string | Attribute to sort by (e.g., `weight`). |
| `order`      | string | Sort order (`asc` or `desc`).        |
| `pageNumber` | int    | Page number for pagination.          |
| `pageSize`   | int    | Number of items per page.            |

**Example Request:**

```bash
curl -X GET "http://localhost:5000/dogs?attribute=weight&order=desc&pageNumber=1&pageSize=2"
```

**Response:**

```json
[
    {
        "name": "Neo",
        "color": "red&amber",
        "tail_length": 22,
        "weight": 32
    },
    {
        "name": "Jessy",
        "color": "black&white",
        "tail_length": 7,
        "weight": 14
    }
]
```

### 3. Create Dog

**POST** `/dog`  
Adds a new dog to the database.

**Request Body:**

```json
{
    "name": "Doggy",
    "color": "red",
    "tail_length": 17,
    "weight": 33
}
```

**Example Command:**

```bash
curl -X POST "http://localhost:5000/dog" -H "Content-Type: application/json" -d "{\"name\": \"Doggy\",\"color\":\"red\",\"tail_length\": 17,\"weight\": 33}"
```

### 4. Rate Limiting

If the number of requests exceeds the configured limit (e.g., 10 requests per second), the server will respond with:

```json
{
  "status": 429,
  "message": "Too many requests"
}
```

## Unit Testing

Unit tests are included in the **DogHouseService.Tests** project. To run the tests, use:

```bash
dotnet test
```

This will run all tests and display results in the terminal.

## Folder Structure

```
DogHouseService/
├── Controllers/            # API controllers
├── Data/                   # Database context and seed data
├── Models/                 # Data models
├── Program.cs              # Main entry point of the API
├── appsettings.json        # App settings (connection strings, etc.)
DogHouseService.Tests/
├── DogServiceTests.cs      # Unit tests for Dog service
└── ...
```

## Future Improvements

- **Additional Validation**: Enhance validation for more edge cases.
- **Logging**: Add structured logging for debugging and monitoring.
- **Caching**: Implement caching for frequently accessed data.

## License

This project is licensed under the MIT License.

---

