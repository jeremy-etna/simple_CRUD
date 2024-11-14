# Quest Web API in .NET 6
This project is a RESTful CRUD API built with .NET 6, designed to manage user authentication, address information, and user data. The API is secured with JWT tokens and provides endpoints for creating, retrieving, updating, and deleting resources.

# Features
- User Management: Register, authenticate, and manage user profiles.
- Address Management: Create, retrieve, update, and delete address data linked to users.
- JWT Authentication: Secure endpoints and verify user roles for authorization.

# Prerequisites
- .NET 6 SDK
- A SQL database (e.g., MySQL)
- Postman or a similar tool to test the API endpoints

# Setup
Clone the project:
```
git clone https://github.com/your-username/quest_web_dotnet.git
```
```
cd quest_web_dotnet
```
Configure the database connection in appsettings.json:
```
"ConnectionStrings": {
    "DefaultConnection": "server=127.0.0.1;database=<dataBaseName>;user=<userName>;password=<password>"
}
```

# Run the Project
Run the project with:
```
dotnet run
```
The API will be available at http://localhost:8090.

# API Endpoints
## Authentication
- POST /register : Register a new user.
- POST /authenticate : Authenticate an existing user and receive a JWT.
- GET /me : Retrieve the authenticated user’s details.
- DELETE /me : Delete the authenticated user’s account.
## User Management
- GET /user : Retrieve all users (Admin only).
- GET /user/{id} : Retrieve a user by ID.
- GET /user/{id}/addresses : Retrieve addresses associated with a user.
- PUT /user/{id} : Update a user's username (Admin can also update roles).
- DELETE /user/{id} : Delete a user by ID (Admin only or self-deletion).
## Address Management
- POST /address : Create a new address linked to the authenticated user.
- POST /user/{id}/address : Create an address for a specified user (Admin only or self).
- GET /address : Retrieve all addresses (Admin only) or the authenticated user’s addresses.
- GET /address/{id} : Retrieve an address by ID.
- PUT /address/{id} : Update an address (Admin only or owner).
- DELETE /address/{id} : Delete an address by ID (Admin only or owner).
## Default Endpoints (Testing)
- GET /testSuccess : Returns 200 OK to test API success.
- GET /testNotFound : Returns 404 Not Found to test missing resources.
- GET /testError : Returns 500 Internal Server Error to test error handling.

# Technologies Used
.NET 6: Web API framework
Entity Framework Core: ORM for database management
MySQL: Relational database
BCrypt: Password hashing
JWT: JSON Web Tokens for secure authentication
