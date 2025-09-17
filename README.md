Bank API (ASP.NET Core + EF Core)

A simple banking API built with ASP.NET Core and Entity Framework Core using the Repository + Service pattern.

The API supports:

Creating accounts

Deposits, withdrawals, transfers

PIN management (change PIN)

Transaction history (Deposit, Withdrawal, TransferIn, TransferOut)

Features

1. Account creation with a unique 10-digit account number

2. BCrypt-based PIN hashing for security

3. Transfer validation (balance check, PIN verification)

4. Soft-delete accounts

5. Transaction logging with optional descriptions

6. Clean architecture: Controller → Service → Repository → DbContext

Tech Stack

1. ASP.NET Core 8.0 (Web API)

2. Entity Framework Core (ORM)

3. SQLite

4. BCrypt.Net (PIN hashing)

set up, 
1. Clone the repo

2. Install dependencies

dotnet restore

3.Apply migrations & create DB

dotnet ef database update


Run the project

dotnet run


you can test the end point using swagger

Swagger UI available at:

https://localhost:5274/swagger



POST /api/accounts → Create account

{
  "firstName": "Uchenna",
  "lastName":"John",
  "Balance": 23000,
  "phoneNumber":"09014214236",
  "pin": "4321",
  "email": "uchenna@example.com",
  "address": "Abia, Nigeria",
  "bvn": "1234567887",
  "nin": "0987654337",
  "accountType": "Savings"
}


GET /api/accounts/{accountNumber} → Get account by number

GET /api/accounts → Get all accounts

GET /api/accounts/{accountNumber}/transactions → Get transaction history


Cash Deposit
POST /api/accounts/deposit
{
  "accountNumber": "8088275412",
  "amount": 15000
}


Cash Withdrawal
POST /api/accounts/withdraw
{
  "accountNumber": "8088275412",
  "pinHash": "4321",
  "amount": 1500
}



Cash Transfer
POST /api/accounts/transfer
{
  "fromAccountNumber": "8088275412",
  "toAccountNumber": "9829588341",
  "fromPin": "4321",
  "amount": 20
}



Update Account Details
PUT /api/accounts/{accountNumber}
{
  "firstName": "Remi",
  "lastName":"Sobanjo",
  "phoneNumber":"08034904256",
  "email": "remi@example.com",
  "address": "Ibadan, Nigeria",
  "accountType": "Current"
}
