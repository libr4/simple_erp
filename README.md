# Target Technical Assignment

## 🚀 Quick Start

### Option 1: Docker Compose (Recommended)

The fastest way to run the entire project locally:

```bash
MSSQL_SA_PASSWORD=Your_password123 docker compose up --build
```

**Access the API:**
- **Swagger UI (API Documentation):** [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)
- **Health Check:** [http://localhost:5000/health](http://localhost:5000/health)

**Optional React Frontend Interface:**
- **Interactive UI:** [http://localhost:3000](http://localhost:3000)

This setup automatically:
- ✅ Builds and starts the backend API (.NET 8)
- ✅ Starts the SQL Server 2019 database
- ✅ Builds and starts the frontend (React + Vite)
- ✅ Applies database migrations automatically

---

### Option 2: Local Development

To run the API and frontend locally on your machine, you must have **SQL Server installed** (or SQL Server Express).

#### Prerequisites
- .NET 8 SDK
- Node.js 16+
- SQL Server (local instance or running separately)

#### Setup API

```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

API will be available at `http://localhost:5000`

#### Setup Frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend will be available at `http://localhost:3000`

---

## 📋 API Documentation

### Overview

The API provides three main endpoints for managing sales commissions, inventory movements, and overdue fee calculations.

### Base URL
```
http://localhost:5000/api/v1
```

---

### 1. Commission Calculation

**Endpoint:** `POST /comissao`

Calculate commissions for sales representatives based on sales values with tiered commission rates.

**Commission Rules:**
- Sales < R$100: No commission
- Sales R$100–R$499.99: 1% commission
- Sales ≥ R$500: 5% commission

**Request:**
```json
{
  "vendas": [
    { "vendedor": "João Silva", "valor": 1200.50 },
    { "vendedor": "Maria Souza", "valor": 400.50 },
    { "vendedor": "Carlos Oliveira", "valor": 800.50 }
  ]
}
```

**Response (200 OK):**
```json
[
  {
    "vendedor": "João Silva",
    "totalVendas": 1200.50,
    "comissaoTotal": 60.03,
    "itens": [
      {
        "valor": 1200.50,
        "comissao": 60.03
      }
    ]
  },
  {
    "vendedor": "Maria Souza",
    "totalVendas": 400.50,
    "comissaoTotal": 4.01,
    "itens": [
      {
        "valor": 400.50,
        "comissao": 4.01
      }
    ]
  },
  {
    "vendedor": "Carlos Oliveira",
    "totalVendas": 800.50,
    "comissaoTotal": 40.03,
    "itens": [
      {
        "valor": 800.50,
        "comissao": 40.03
      }
    ]
  }
]
```

**Error Response (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "vendas": ["Lista de vendas não pode estar vazia."]
  }
}
```

---

### 2. Inventory Management

**Endpoint:** `POST /estoque`

Record inventory movements (entries, exits, or inventory adjustments) for products and retrieve updated stock levels.

**Movement Types:**
- `ENTRADA`: Stock entry
- `SAIDA`: Stock exit
- `INVENTARIO`: Inventory adjustment

**Request:**
```json
{
  "codigoProduto": 101,
  "tipo": "ENTRADA",
  "quantidade": 50,
  "descricao": "Reposição de estoque"
}
```

**Response (201 Created):**
```json
{
  "produto": {
    "codigo": 101,
    "descricao": "Caneta Azul",
    "estoque": 200
  },
  "movimentacoesRecentes": [
    {
      "id": 1,
      "codigoProduto": 101,
      "tipo": "ENTRADA",
      "quantidade": 50,
      "descricao": "Reposição de estoque",
      "data": "2024-12-01T10:30:00",
      "estoqueAntigo": 150,
      "estoqueNovo": 200
    }
  ]
}
```

**Error Responses:**
- `400 Bad Request`: Validation errors (invalid type, negative quantity, etc.)
- `404 Not Found`: Product code not found in database
- `409 Conflict`: Insufficient stock for exit operation

---

### 3. Overdue Fee Calculation

**Endpoint:** `GET /fees`

Calculate daily overdue fees (juros/multa) for a payment with a due date.

**Fee Rules:**
- Daily penalty rate: 2.5% per day
- Date format: `dd/MM/yyyy`
- Based on Brazil timezone (America/Sao_Paulo)

**Query Parameters:**
```
GET /fees?vencimento=01/12/2024&valor=1000.00
```

**Response (200 OK):**
```json
{
  "valorOriginal": 1000.00,
  "diasAtraso": 5,
  "juros": 125.00,
  "valorComJuros": 1125.00
}
```

**Error Response (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "vencimento": ["Data de vencimento inválida. Formato esperado: dd/MM/yyyy"]
  }
}
```

---

## 🏗️ Architecture & Design Decisions

### Clean Architecture
The project follows clean architecture principles with clear separation of concerns:

```
Controllers → Services → Repositories → Database
    ↓            ↓            ↓
  (API)     (Business)   (Persistence)
```

### Key Decisions

**1. Commission Calculation Strategy**
- **Simple Tiered Model**: Straightforward percentage-based tiers instead of complex progressive formulas
- **Grouped by Seller**: Commissions are calculated per seller, with individual item breakdown for transparency
- **Decimal Precision**: Uses `decimal` type with `AwayFromZero` rounding to ensure accurate financial calculations

**2. Inventory Management**
- **Unique Movement IDs**: Each stock movement has a unique identifier for audit trail
- **Recent Movements History**: Returns the last 10 movements per product for quick visibility
- **Concurrency Protection**: Validates sufficient stock before processing exits (prevents overselling)
- **Three Movement Types**: ENTRADA (entry), SAIDA (exit), INVENTARIO (adjustment) cover all common warehouse scenarios

**3. Overdue Fees Calculation**
- **Brazil Timezone Awareness**: All date calculations use America/Sao_Paulo timezone for consistency
- **Daily Simple Interest**: Simple daily rate (2.5%) without compound interest for clarity
- **Date Format Standardization**: Enforces `dd/MM/yyyy` format for Brazilian market consistency
- **Query Parameter Approach**: Uses GET with query parameters for stateless fee calculations

**4. Validation Strategy**
- **FluentValidation Framework**: Centralized validation using industry-standard FluentValidation library
- **Global Action Filter**: Validation runs automatically on all requests via middleware
- **Problem Details Standard**: All validation errors follow RFC 7231 Problem Details format for API consistency

**5. Error Handling**
- **Global Exception Middleware**: Centralized exception handling ensuring consistent error responses across all endpoints
- **Specific HTTP Status Codes**: Proper HTTP semantics (201 Created, 404 Not Found, 409 Conflict, etc.)
- **Normalized Error Responses**: All errors follow a consistent JSON structure

---

## 🛠️ Technology Stack

### Backend API

| Component | Technology | Version |
|-----------|-----------|---------|
| **Framework** | ASP.NET Core | 8.0 |
| **Language** | C# | Latest |
| **ORM** | Entity Framework Core | 8.0.0 |
| **Database** | SQL Server | 2019+ |
| **API Documentation** | Swagger/OpenAPI | 6.5.0 |
| **Mapping** | AutoMapper | 12.0.1 |
| **Validation** | FluentValidation | 11.8.0 |

### Key NuGet Packages
- `Swashbuckle.AspNetCore` - API documentation and Swagger UI
- `Microsoft.EntityFrameworkCore.SqlServer` - SQL Server provider for EF Core
- `Microsoft.EntityFrameworkCore.Tools` - EF Core CLI tools
- `AutoMapper.Extensions.Microsoft.DependencyInjection` - Object mapping for DTOs
- `FluentValidation.DependencyInjectionExtensions` - Request validation

### Frontend (Optional)

| Component | Technology | Version |
|-----------|-----------|---------|
| **Framework** | React | 18.2.0 |
| **Build Tool** | Vite | 5.1.0 |
| **Language** | TypeScript | 5.4.2 |
| **UI Library** | Material-UI (MUI) | 5.14.0 |
| **HTTP Client** | Axios | 1.4.0 |
| **Data Fetching** | React Query | 4.29.12 |
| **Routing** | React Router | 7.9.6 |
| **Schema Validation** | Zod | 3.22.2 |

The frontend provides an optional interactive interface to test the API endpoints. It is styled with Material-UI for a professional appearance and includes real-time data fetching with React Query.

---

## 📁 Project Structure

```
simple_erp/
├── backend/                          # ASP.NET Core API
│   ├── Controllers/                  # API endpoints
│   ├── Services/                     # Business logic
│   ├── Repositories/                 # Data access layer
│   ├── DTOs/                         # Request/Response contracts
│   ├── Validators/                   # FluentValidation rules
│   ├── Models/                       # Entity models
│   ├── Infrastructure/               # EF Core DbContext & migrations
│   ├── Middleware/                   # Exception handling & validation
│   ├── Constants/                    # Centralized constants
│   ├── Mapping/                      # AutoMapper profiles
│   ├── Tests/                        # Unit & Integration tests
│   ├── Program.cs                    # Application setup
│   ├── appsettings.json              # Configuration
│   └── Dockerfile                    # Container image
│
├── frontend/                         # React + Vite UI (optional)
│   ├── src/
│   │   ├── components/               # Reusable React components
│   │   ├── pages/                    # Page components
│   │   ├── App.tsx                   # Main app component
│   │   └── main.tsx                  # Entry point
│   ├── package.json                  # Dependencies
│   ├── vite.config.ts                # Vite configuration
│   ├── tsconfig.json                 # TypeScript configuration
│   └── Dockerfile                    # Container image
│
├── docker-compose.yml                # Multi-container orchestration
└── README.md                         # This file
```

---

## 🧪 Testing

The project includes comprehensive test suites:

**Unit Tests** (`backend/Tests/UnitTests/`)
- Service layer logic testing
- Commission calculation scenarios
- Fee calculation edge cases

**Integration Tests** (`backend/Tests/IntegrationTests/`)
- Full endpoint testing with TestWebApplicationFactory
- Database integration scenarios
- HTTP response validation

Run tests with:
```bash
cd backend
dotnet test
```

---

## 🔐 Security & Best Practices

- ✅ **Input Validation**: All inputs validated using FluentValidation
- ✅ **SQL Injection Prevention**: Parameterized queries via Entity Framework Core
- ✅ **CORS Configuration**: Frontend CORS properly configured for localhost:3000
- ✅ **Error Messages**: Sensitive information not exposed in error responses
- ✅ **Type Safety**: Strong typing with C# and TypeScript throughout
- ✅ **Centralized Configuration**: Sensitive data via `user-secrets` in development

---

## 📝 Initial Data

The database is automatically seeded with initial products on first run:

```
Product 101: Caneta Azul - 150 units
Product 102: Caderno Universitário - 75 units
Product 103: Borracha Branca - 200 units
Product 104: Lápis Preto HB - 320 units
Product 105: Marcador de Texto Amarelo - 90 units
```

These correspond to the original specification and can be used immediately for testing inventory movements.

---

## 🐛 Troubleshooting

### Docker Issues

**SQL Server connection timeout**
```bash
# Increase wait time for SQL Server to start
MSSQL_SA_PASSWORD=Your_password123 docker compose up --build
```

**Port already in use**
```bash
# Use custom ports
BACKEND_PORT=5001 FRONTEND_PORT=3001 MSSQL_PORT=1434 \
docker compose up --build
```

### Local Development Issues

**Database connection failed**
- Verify SQL Server is running: `sqlcmd -S . -U sa -P your_password -Q "SELECT 1"`
- Check connection string in `appsettings.json`
- Run migrations: `dotnet ef database update`

**Port conflicts**
- Backend: Change port in `launchSettings.json`
- Frontend: Vite uses port 3000 by default; configure in `vite.config.ts`

---

## 📞 Support & Notes

This is a technical assignment solution demonstrating:
- RESTful API design with proper HTTP semantics
- Clean architecture and separation of concerns
- Comprehensive validation and error handling
- Database-driven inventory management
- Production-ready code practices

For questions or issues, refer to the Swagger documentation at `/swagger/ui` or review the inline code comments throughout the project.

---

**Built with:** .NET 8, C#, Entity Framework Core, React, TypeScript, and Docker
