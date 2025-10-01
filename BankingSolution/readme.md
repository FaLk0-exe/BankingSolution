# BankingSolution

BankingSolution is a demo project of a banking API built with .NET , FastEndpoints and Domain-Driven Design (DDD) principles.  
The system allows managing users, accounts, and performing money transfers between accounts.

## Features
- Manage users (create, list)
- Manage accounts (create, get list, get details)
- Account operations: replenish, withdraw, transfer funds
- InMemory database (no setup required)
- Swagger UI for API exploration
- Integration tests with xUnit

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)

### Run the API
```
git clone https://github.com/FaLk0-exe/BankingSolution.git
cd BankingSolution
dotnet run --project BankingSolution
```

Swagger UI will be available at:  
http://localhost:5006/swagger  

The application uses InMemory database, so all data is reset after restart.  

## Why FastEndpoints
FastEndpoints is used because it provides less boilerplate code compared to MVC or Minimal APIs and has better runtime performance. It also keeps endpoint definitions clean and simple.  

## Why DDD
Domain-Driven Design is applied to separate business logic from the web layer.  
Entities, Value Objects, and Domain Services live inside the `Domain` project, which makes the business logic testable and independent from the API layer.  

## Running Tests
The project includes integration tests that start the application with InMemory database.  
Tests cover both positive and negative scenarios, such as:  
- Successful creation of users and accounts  
- Replenish, withdraw and transfer operations  
- Handling invalid IDs (404 Not Found)  
- Handling insufficient funds or conflicts (409 Conflict)  

### Run all tests
```bash
cd BankingSolution.Tests.Integration
dotnet test
```

