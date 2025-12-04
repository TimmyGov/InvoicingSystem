# Invoice Management System

A production-grade invoice tracking system built with Clean Architecture principles, featuring a C# ASP.NET Core 8.0 backend and Angular 17 frontend.

## Features

- **Invoice Management**: Create, view, and track invoices with detailed line items
- **Customer Management**: Associate invoices with customer information
- **Status Tracking**: Track invoice status (Draft, Sent, Paid, Overdue, Cancelled)
- **Automated Notifications**: Daily background job to send overdue invoice notifications via Hangfire
- **RESTful API**: Well-documented API with Swagger/OpenAPI support
- **Modern UI**: Responsive Angular frontend with Bootstrap 5 styling
- **Clean Architecture**: Separation of concerns with Domain, Application, Infrastructure, and API layers
- **Repository Pattern**: Generic repository implementation for data access
- **Validation**: FluentValidation for comprehensive input validation
- **Object Mapping**: AutoMapper for DTO/Entity conversions
- **PostgreSQL Database**: Reliable data storage with Entity Framework Core
- **Docker Support**: Complete containerization with docker-compose

## Architecture

The system follows Clean Architecture principles with clear separation of concerns:

```
InvoiceSystem/
├── src/
│   ├── InvoiceSystem.Domain/          # Core business entities and interfaces
│   ├── InvoiceSystem.Application/      # Business logic, DTOs, services
│   ├── InvoiceSystem.Infrastructure/   # Data access, repositories, external services
│   ├── InvoiceSystem.API/             # REST API controllers and configuration
│   └── InvoiceSystem.Web/             # Angular frontend application
├── tests/
│   ├── InvoiceSystem.UnitTests/       # Unit tests with xUnit
│   └── InvoiceSystem.IntegrationTests/ # Integration tests
├── Dockerfile                          # API container configuration
└── docker-compose.yml                  # Multi-container orchestration
```

### Domain Layer
- **Entities**: User, Customer, Invoice, InvoiceItem, Notification
- **Enums**: InvoiceStatus
- **Interfaces**: IRepository<T>, IInvoiceRepository

### Application Layer
- **Services**: InvoiceService for business logic
- **DTOs**: CreateInvoiceDto, InvoiceResponseDto, CustomerDto, InvoiceItemResponseDto
- **Validators**: CreateInvoiceValidator with FluentValidation
- **Mappings**: AutoMapper profiles for entity/DTO conversion

### Infrastructure Layer
- **DbContext**: ApplicationDbContext with entity configurations
- **Repositories**: Generic Repository<T> and InvoiceRepository implementations
- **Background Jobs**: NotificationJob for Hangfire recurring tasks

### API Layer
- **Controllers**: InvoicesController with RESTful endpoints
- **Configuration**: Dependency injection, CORS, Swagger, Hangfire dashboard
- **Middleware**: Error handling and logging

## Prerequisites

### For Docker Deployment (Recommended)
- Docker 20.10+
- Docker Compose 2.0+

### For Manual Deployment
- .NET 8.0 SDK
- Node.js 18+ and npm
- PostgreSQL 16+

## Getting Started

### Option 1: Docker Deployment (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/TimmyGov/invoice-system.git
   cd invoice-system
   ```

2. **Start all services with Docker Compose**
   ```bash
   docker-compose up -d
   ```

3. **Apply database migrations**
   ```bash
   docker exec invoicesystem-api dotnet ef database update
   ```

4. **Access the application**
   - Angular Frontend: http://localhost:4200
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - Hangfire Dashboard: http://localhost:5000/hangfire

### Option 2: Manual Deployment

1. **Clone the repository**
   ```bash
   git clone https://github.com/TimmyGov/invoice-system.git
   cd invoice-system
   ```

2. **Set up PostgreSQL database**
   ```bash
   # Create database
   createdb invoicesystem
   ```

3. **Configure connection string**
   
   Update `src/InvoiceSystem.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=invoicesystem;Username=postgres;Password=yourpassword"
     }
   }
   ```

4. **Run database migrations**
   ```bash
   cd src/InvoiceSystem.API
   dotnet ef database update
   ```

5. **Start the API**
   ```bash
   dotnet run
   ```
   API will be available at http://localhost:5000

6. **Start the Angular frontend**
   ```bash
   cd ../../src/InvoiceSystem.Web
   npm install
   npm start
   ```
   Frontend will be available at http://localhost:4200

## API Endpoints

### Invoices

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/invoices` | Create a new invoice |
| GET | `/api/invoices/{id}` | Get invoice by ID |
| GET | `/api/invoices/user/{userId}` | Get all invoices for a user |

### Example: Create Invoice

```bash
curl -X POST http://localhost:5000/api/invoices \
  -H "Content-Type: application/json" \
  -H "X-User-Id: 00000000-0000-0000-0000-000000000001" \
  -d '{
    "customerId": "customer-guid-here",
    "issueDate": "2024-01-01T00:00:00Z",
    "dueDate": "2024-01-31T00:00:00Z",
    "items": [
      {
        "description": "Web Development Services",
        "quantity": 40,
        "unitPrice": 100.00
      }
    ]
  }'
```

## Background Jobs

The system uses Hangfire to run scheduled background jobs:

- **Overdue Notifications**: Runs daily at 9:00 AM to send notifications for overdue invoices
- **Configuration**: Cron expression can be modified in `appsettings.json`

Access the Hangfire Dashboard at http://localhost:5000/hangfire to monitor jobs.

## Testing

### Run Unit Tests
```bash
dotnet test tests/InvoiceSystem.UnitTests/InvoiceSystem.UnitTests.csproj
```

### Run All Tests
```bash
dotnet test
```

The project includes:
- Unit tests for InvoiceService with Moq and FluentAssertions
- Test coverage for core business logic
- Integration test project structure

## Database Migrations

### Create a new migration
```bash
cd src/InvoiceSystem.API
dotnet ef migrations add MigrationName --project ../InvoiceSystem.Infrastructure
```

### Apply migrations
```bash
dotnet ef database update
```

### Remove last migration
```bash
dotnet ef migrations remove --project ../InvoiceSystem.Infrastructure
```

## Project Structure

```
InvoiceSystem/
├── src/
│   ├── InvoiceSystem.Domain/
│   │   ├── Entities/           # Domain entities
│   │   ├── Enums/             # Enumerations
│   │   └── Interfaces/        # Repository interfaces
│   ├── InvoiceSystem.Application/
│   │   ├── DTOs/              # Data Transfer Objects
│   │   ├── Services/          # Business logic services
│   │   ├── Validators/        # FluentValidation validators
│   │   └── Mappings/          # AutoMapper profiles
│   ├── InvoiceSystem.Infrastructure/
│   │   ├── Data/              # DbContext and configurations
│   │   ├── Repositories/      # Repository implementations
│   │   └── Jobs/              # Hangfire background jobs
│   ├── InvoiceSystem.API/
│   │   ├── Controllers/       # API endpoints
│   │   ├── Program.cs         # Application startup
│   │   └── appsettings.json   # Configuration
│   └── InvoiceSystem.Web/
│       └── src/
│           └── app/
│               ├── components/ # Angular components
│               ├── services/  # HTTP services
│               └── models/    # TypeScript models
├── tests/
│   ├── InvoiceSystem.UnitTests/
│   └── InvoiceSystem.IntegrationTests/
├── Dockerfile
├── docker-compose.yml
└── README.md
```

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: PostgreSQL 16
- **Background Jobs**: Hangfire 1.8.9
- **Validation**: FluentValidation 11.9.0
- **Mapping**: AutoMapper 13.0.1
- **API Documentation**: Swashbuckle (Swagger/OpenAPI)
- **Testing**: xUnit, Moq, FluentAssertions

### Frontend
- **Framework**: Angular 17
- **UI Library**: Bootstrap 5.3
- **HTTP Client**: Angular HttpClient
- **Language**: TypeScript 5.2

### DevOps
- **Containerization**: Docker
- **Orchestration**: Docker Compose
- **CI/CD**: GitHub Actions (ready)

## Configuration

### Environment Variables

The following environment variables can be configured in `.env` or `docker-compose.yml`:

- `DATABASE_HOST`: PostgreSQL host (default: postgres)
- `DATABASE_PORT`: PostgreSQL port (default: 5432)
- `DATABASE_NAME`: Database name (default: invoicesystem)
- `DATABASE_USER`: Database user (default: postgres)
- `DATABASE_PASSWORD`: Database password (default: postgres)
- `JWT_SECRET`: JWT secret key for authentication
- `JWT_EXPIRATION_HOURS`: Token expiration time (default: 24)
- `HANGFIRE_POLL_INTERVAL_HOURS`: Hangfire polling interval (default: 1)

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Style
- Follow C# coding conventions for backend code
- Follow Angular style guide for frontend code
- Write unit tests for new features
- Update documentation as needed

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues, questions, or contributions, please open an issue on GitHub.

## Roadmap

- [ ] User authentication with JWT
- [ ] Email notifications for overdue invoices
- [ ] PDF generation for invoices
- [ ] Payment tracking and integration
- [ ] Multi-currency support
- [ ] Invoice templates
- [ ] Reporting and analytics dashboard
- [ ] Multi-tenant support

---

Built with ❤️ using Clean Architecture principles