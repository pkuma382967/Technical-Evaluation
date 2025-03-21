# Technical-Evaluation

## Getting Started

### 🔧 Step 1: Update Connection String
Open `appsettings.json` and update the connection string:
```json
"ConnectionStrings": {
  "SearchAppDBConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SearchAPI;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 🔧 Step 2: Run EF Core Migration
Use the following commands in the **Package Manager Console**:
```powershell
Add-Migration InitialSetup
Update-Database
```
> 💡 This will create the database with the following tables:
> - `Product`
> - `User` (with a default admin user)

---

## 🔐 Authentication
To access the **Search API**, you need a valid **JWT token**. Use the following authentication endpoint:

### Request:
```http
POST {{SearchAPI_HostAddress}}/api/auth/login
Accept: application/json
Content-Type: application/json

{
  "username": "admin",
  "password": "password123"
}
```

### Response:
Returns a **JWT token**.

Use this token in the `Authorization` header of subsequent requests:
```http
Authorization: Bearer <your_token_here>
```

---

## 🚦 Rate Limiting
The project includes **request rate limiting** with the following settings:
```json
"RateLimiting": {
  "RequestLimit": 5,
  "TimeWindowSeconds": 10
},
"BlacklistedIPs": [
  //"::1",
  "10.0.0.50"
]
```
> ⚠️ Requests beyond the limit within the time window will be blocked.

---

## 📜 Logging with Serilog
**Serilog** is configured for structured logging of **requests** and **responses**.

### Logging Settings in `appsettings.json`:
```json
"Serilog": {
  "LogRequestBody": true,
  "LogResponseBody": true,
  "Using": [
    "Serilog.Sinks.Console",
    "Serilog.Sinks.Async"
  ],
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Default": "Information",
      "Microsoft": "Information",
      "System": "Information"
    }
  },
  "WriteTo": [
    {
      "Name": "File",
      "Args": {
        "path": "./logs/Information_.log",
        "outputTemplate": "{Timestamp:yyyy-MM-dd'T'HH:mm:ss.fff} {ApplicationName} [{Level}] {Message} {Information} {NewLine}",
        "fileSizeLimitBytes": "10485760",
        "rollingInterval": "Day",
        "retainedFileCountLimit": "150",
        "rollOnFileSizeLimit": "true",
        "shared": "true",
        "flushToDiskInterval": "00:00:01",
        "restrictedToMinimumLevel": "Information"
      }
    },
    {
      "Name": "File",
      "Args": {
        "path": "./logs/Error_.log",
        "outputTemplate": "{Timestamp:yyyy-MM-dd'T'HH:mm:ss.fff} {ApplicationName} [{Level}] {Message} {Exception} {NewLine}",
        "fileSizeLimitBytes": "10485760",
        "rollingInterval": "Day",
        "retainedFileCountLimit": "70",
        "rollOnFileSizeLimit": "true",
        "shared": "true",
        "flushToDiskInterval": "00:00:01",
        "restrictedToMinimumLevel": "Error"
      }
    }
  ]
}
```
> ✅ Every request and response is logged using Serilog middleware.

---

## 🧠 Caching
The API supports **in-memory caching** with expiration controlled via:
```json
"CacheSettings": {
  "ExpirationMinutes": 5
}
```
> 🕒 This improves performance by reducing unnecessary DB hits.




 
