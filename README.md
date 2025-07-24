# Store Management System

A comprehensive store management system built with .NET 8, following Test-Driven Development (TDD) principles and Clean Architecture patterns, with Azure KeyVault and Azure App Configuration integration.

## 📋 Overview

This project demonstrates a complete backend solution implementing:

- **RESTful API** for store management (Azure App Service)
- **Azure Functions** for product microservices
- **SQL Server database** with scalar functions and stored procedures
- **Azure KeyVault integration** for credential management
- **Azure App Configuration** for centralized configuration
- **Docker containerization** for Azure deployment
- **Automated testing** following TDD principles
- **Postman collection** for API testing

## 🏗️ Architecture

The project follows Clean Architecture principles with clear separation of concerns:

```
StoreManagement/
├── src/
│   ├── StoreManagement.API/          # RESTful API for Stores & Companies
│   ├── StoreManagement.Functions/    # Azure Functions for Products
│   ├── StoreManagement.Domain/       # Domain entities, DTOs, and interfaces
│   ├── StoreManagement.Infrastructure/ # Data access and external services
│   ├── StoreManagement.Database/     # FluentMigrator migrations
│   └── StoreManagement.Tests/        # Unit and integration tests
├── docker/                           # Docker configuration files
├── postman/                          # Postman collection for API testing
└── README.md                         # Project documentation
```

### Architecture Layers

- **Domain Layer**: Core business entities, DTOs, and service interfaces
- **Infrastructure Layer**: Data repositories, external service implementations
- **API Layer**: RESTful controllers and configuration
- **Functions Layer**: Azure Functions for microservices
- **Database Layer**: Migration scripts and database setup
- **Tests Layer**: Unit and integration tests

## 🚀 Technology Stack

- **.NET 8** - Main framework
- **ASP.NET Core Web API** - RESTful API
- **Azure Functions v4** - Serverless microservices
- **Entity Framework Core** - Object-Relational Mapping
- **FluentMigrator** - Database migrations
- **SQL Server** - Database engine
- **Azure KeyVault** - Secret management
- **Azure App Configuration** - Configuration management
- **AutoMapper** - Object mapping
- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **Docker** - Containerization
- **Swagger/OpenAPI** - API documentation

## 📊 Data Model

### Core Entities

- **Company**: Represents a company in the multi-tenant environment
- **Store**: Stores belonging to a company
- **Product**: Products managed by company/store

### Entity Relationships

- A **Company** can have multiple **Stores** (1:N)
- A **Company** can have multiple **Products** (1:N)
- A **Store** can have multiple **Products** (1:N)
- Cascade delete relationships maintain referential integrity

### Database Features

- **Scalar Functions**: Custom SQL functions for business logic
- **Stored Procedures**: Optimized data operations
- **Automatic Migrations**: Database schema versioning
- **Health Checks**: Database connectivity monitoring

## 🛠️ Prerequisites

- **.NET 8 SDK** or later
- **SQL Server** (LocalDB or full instance)
- **Docker Desktop** (optional, for containerized deployment)
- **Azure CLI** (for Azure deployment)
- **Postman** (for API testing)

## ⚡ Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/Lextokil/StoreManagement.git
cd StoreManagement
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Database Connection

Edit `src/StoreManagement.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StoreManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "KeyVaultEndpoint": "",
  "AppConfigEndpoint": ""
}
```

### 4. Run Database Migrations

The application automatically runs migrations on startup, but you can also run them manually:

```bash
dotnet run --project src/StoreManagement.Database
```

### 5. Start the Applications

**Option A: Local Development**

Start the API:
```bash
cd src/StoreManagement.API
dotnet run
```
Access: http://localhost:5000 (Swagger UI available at root)

Start Azure Functions:
```bash
cd src/StoreManagement.Functions
func start
```
Access: http://localhost:7071

**Option B: Docker Compose**

```bash
cd docker
docker-compose up -d
```

Services will be available at:
- **API**: http://localhost:8080
- **Functions**: http://localhost:7071
- **SQL Server**: localhost:1433
- **Azurite Storage**: localhost:10000-10002

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test src/StoreManagement.Tests/

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📡 API Endpoints

### Companies API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/companies` | List all companies |
| GET | `/api/companies/active` | List active companies |
| GET | `/api/companies/{code}` | Get company by code |
| GET | `/api/companies/{code}/with-stores` | Get company with stores |
| POST | `/api/companies` | Create new company |
| PUT | `/api/companies/{code}` | Update company (full) |
| PATCH | `/api/companies/{code}` | Update company (partial) |
| DELETE | `/api/companies/{code}` | Delete company |

### Stores API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/stores` | List all stores |
| GET | `/api/stores/{id}` | Get store by ID |
| GET | `/api/stores/company/{companyCode}` | List stores by company |
| POST | `/api/stores` | Create new store |
| PUT | `/api/stores/{id}` | Update store (full) |
| PATCH | `/api/stores/{id}` | Update store (partial) |
| DELETE | `/api/stores/{id}` | Delete store |

### Products Functions (Microservices)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products/store/{storeCode}` | List products by store |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update product (full) |
| PATCH | `/api/products/{id}` | Update product (partial) |
| DELETE | `/api/products/{id}` | Delete product |

### Health Checks

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Application and database health status |

## 🗃️ Database Management

### FluentMigrator

The project uses **FluentMigrator** for database version control:

- **Versioned migrations**: Each database change is a numbered migration
- **Rollback support**: Ability to revert changes
- **Automatic execution**: Migrations run automatically on application startup
- **Complete history**: Track all applied changes

### Migration Commands

```bash
# Apply all pending migrations
dotnet run --project src/StoreManagement.Database

# Rollback all migrations
dotnet run --project src/StoreManagement.Database -- --rollback
```

### Available Migrations

1. **Migration001_CreateCompaniesTable** - Creates Companies table with seed data
2. **Migration002_CreateStoresTable** - Creates Stores table with relationships
3. **Migration003_CreateProductsTable** - Creates Products table with relationships
4. **Migration004_CreateInsertProcedures** - Creates stored procedures
5. **Migration005_CreateProductJsonFunction** - Creates scalar functions

### SQL Server Functions

**Scalar Functions:**
- `dbo.GetProductsAsJson(@CompanyId)` - Returns products as JSON
- `dbo.GetProductCountByCompany(@CompanyId)` - Count products by company
- `dbo.GetStoreCountByCompany(@CompanyId)` - Count stores by company
- `dbo.GetTotalInventoryValueByCompany(@CompanyId)` - Total inventory value
- `dbo.GetLowStockProductsCount(@CompanyId)` - Low stock products count

**Stored Procedures:**
- `dbo.InsertProduct` - Insert new product
- `dbo.InsertStore` - Insert new store
- `dbo.InsertCompany` - Insert new company
- `dbo.UpdateProductStock` - Update product stock
- `dbo.GetCompanyDashboard` - Company dashboard data

## 🔐 Azure Configuration

### Azure KeyVault Setup

1. Create KeyVault in Azure Portal
2. Add secrets:
   - `ConnectionStrings--DefaultConnection`
   - Other sensitive credentials

3. Configure environment variable:
```bash
export KEYVAULT_ENDPOINT="https://your-keyvault.vault.azure.net/"
```

### Azure App Configuration Setup

1. Create App Configuration in Azure Portal
2. Configure connection string
3. Set environment variable:
```bash
export APPCONFIG_ENDPOINT="https://your-appconfig.azconfig.io"
```

### Managed Identity

The application supports Azure Managed Identity for secure access to Azure services without storing credentials.

## 🐳 Docker Deployment

### Local Development with Docker

```bash
cd docker
docker-compose up -d
```

This starts:
- **SQL Server 2022** with persistent storage
- **Store Management API** with health checks
- **Azure Functions** with Azurite storage emulator
- **Azurite** for local Azure Storage development

### Production Docker Images

Build production images:

```bash
# API Image
docker build -f docker/Dockerfile.api -t storemanagement-api .

# Functions Image
docker build -f docker/Dockerfile.functions -t storemanagement-functions .
```

### Azure Container Registry Deployment

```bash
# Login to Azure
az login

# Create Container Registry
az acr create --resource-group myResourceGroup --name myRegistry --sku Basic

# Push images
docker tag storemanagement-api myregistry.azurecr.io/storemanagement-api:latest
docker push myregistry.azurecr.io/storemanagement-api:latest

# Deploy to App Service
az webapp create --resource-group myResourceGroup --plan myAppServicePlan --name myStoreManagementAPI --deployment-container-image-name myregistry.azurecr.io/storemanagement-api:latest
```

## 📮 API Testing with Postman

### Import Collection

1. Import `postman/StoreManagement.postman_collection.json`
2. Import `postman/StoreManagement_Environment.postman_environment.json`
3. Select the "StoreManagement Environment"

### Environment Variables

Configure these variables in your Postman environment:
- `apiBaseUrl`: http://localhost:8080 (or your API URL)
- `functionsBaseUrl`: http://localhost:7071 (or your Functions URL)
- `companyCode`: Test company code
- `functionKey`: Azure Functions key (if required)

### Test Scenarios

The collection includes comprehensive tests for:
- **CRUD operations** for all entities
- **Error handling** and validation
- **Performance testing** (response times)
- **Data consistency** across operations
- **Business logic validation**

### Automated Testing

Each request includes automated tests that verify:
- HTTP status codes
- Response schema validation
- Data integrity
- Performance benchmarks
- Business rule compliance

## 🏗️ Development Patterns

### Clean Architecture

- **Separation of Concerns**: Each layer has distinct responsibilities
- **Dependency Inversion**: Dependencies point inward toward the domain
- **Testability**: Easy to unit test business logic
- **Maintainability**: Changes in one layer don't affect others

### Test-Driven Development (TDD)

- **Red-Green-Refactor**: Write failing tests, make them pass, refactor
- **High Test Coverage**: Comprehensive unit and integration tests
- **Mocking**: Isolated testing with Moq framework
- **Integration Tests**: End-to-end testing with real database

### Repository Pattern

- **Data Access Abstraction**: Clean separation between business logic and data access
- **Unit of Work**: Consistent transaction management
- **Generic Repository**: Common CRUD operations
- **Specific Repositories**: Entity-specific operations

### Domain-Driven Design (DDD)

- **Rich Domain Models**: Business logic encapsulated in entities
- **Value Objects**: Immutable objects representing concepts
- **Domain Services**: Complex business operations
- **Aggregates**: Consistency boundaries

## 🔍 Monitoring and Health

### Health Checks

The application includes comprehensive health checks:
- **Database connectivity**: SQL Server connection status
- **External dependencies**: Azure services availability
- **Application status**: Memory usage, response times

Access health status at: `/health`

### Logging

- **Structured Logging**: JSON-formatted logs with ILogger
- **Application Insights**: Azure Functions telemetry
- **Error Tracking**: Comprehensive error logging
- **Audit Logging**: Track important business operations

### Performance Monitoring

- **Response Time Tracking**: Monitor API performance
- **Database Query Performance**: EF Core query optimization
- **Memory Usage**: Track application resource consumption
- **Throughput Metrics**: Request/response statistics

## 🚀 Deployment Strategies

### Development Environment

- **Local Development**: Run with `dotnet run`
- **Docker Development**: Use `docker-compose` for full stack
- **Hot Reload**: Automatic code reloading during development

### Staging Environment

- **Azure App Service**: Deploy API to staging slot
- **Azure Functions**: Deploy to staging environment
- **Database Migrations**: Automatic migration execution
- **Configuration Management**: Environment-specific settings

### Production Environment

- **Blue-Green Deployment**: Zero-downtime deployments
- **Auto-scaling**: Automatic scaling based on load
- **Load Balancing**: Distribute traffic across instances
- **Backup Strategy**: Automated database backups

## 🔧 Troubleshooting

### Common Issues

**Database Connection Issues:**
```bash
# Check SQL Server status
sqlcmd -S localhost -E -Q "SELECT @@VERSION"

# Verify connection string
dotnet run --project src/StoreManagement.Database
```

**Migration Failures:**
```bash
# Check migration status
dotnet run --project src/StoreManagement.Database -- --status

# Rollback and retry
dotnet run --project src/StoreManagement.Database -- --rollback
```

**Docker Issues:**
```bash
# Check container logs
docker-compose logs storemanagement-api

# Restart services
docker-compose restart
```

**Azure Functions Issues:**
```bash
# Check function logs
func logs

# Verify storage connection
func azure functionapp fetch-app-settings <function-app-name>
```

### Performance Optimization

- **Database Indexing**: Optimize query performance
- **Caching**: Implement Redis caching for frequently accessed data
- **Connection Pooling**: Optimize database connections
- **Async Operations**: Use async/await throughout the application

## 🤝 Contributing

### Development Workflow

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Write** tests for your changes (TDD approach)
4. **Implement** the feature
5. **Ensure** all tests pass (`dotnet test`)
6. **Commit** your changes (`git commit -m 'Add amazing feature'`)
7. **Push** to the branch (`git push origin feature/amazing-feature`)
8. **Open** a Pull Request

### Code Standards

- **C# Coding Conventions**: Follow Microsoft C# coding standards
- **Clean Code**: Write self-documenting, maintainable code
- **SOLID Principles**: Apply SOLID design principles
- **Test Coverage**: Maintain high test coverage (>80%)
- **Documentation**: Document public APIs and complex logic

### Pull Request Guidelines

- **Clear Description**: Explain what changes were made and why
- **Test Coverage**: Include tests for new functionality
- **Breaking Changes**: Clearly document any breaking changes
- **Performance Impact**: Consider performance implications
- **Security**: Ensure changes don't introduce security vulnerabilities


## 🎯 Project Goals

This Store Management System demonstrates:

- **Enterprise-grade Architecture**: Scalable, maintainable code structure
- **Modern .NET Development**: Latest .NET 8 features and best practices
- **Cloud-native Design**: Built for Azure deployment and scaling
- **Test-driven Development**: Comprehensive testing strategy
- **DevOps Integration**: Docker, CI/CD, and monitoring ready
- **Security Best Practices**: Secure credential management and data protection

**Built with ❤️ using .NET 8 and Azure services**
