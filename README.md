# Payment Gateway API

## Overview

This project is a solution for the **Checkout.com Engineering Assessment**. The task involves implementing a **Payment Gateway API** that allows merchants to process payments and retrieve payment details. The API interacts with a simulated acquiring bank to authorize payments.

[Checkout.com Assessment Link](https://github.com/cko-recruitment/)

## Features

1. **Process Payments**:

   - Validates the provided payment details.
   - Sends payment requests to the simulated acquiring bank.
   - Returns the status of the payment (Authorized, Declined, or Rejected).

2. **Retrieve Payment Details**:

   - Allows merchants to retrieve details of previously processed payments.

3. **Integration with Acquiring Bank Simulator**:
   - Communicates with the bank simulator to process payment requests.

## Architectural Choices and Design Approach

This solution employs a Clean Architecture with a lightweight version of Domain-Driven Design (DDD) principles to achieve separation of concerns, maintainability, and scalability.

### Key Architectural Choices

1. **Clean Architecture:**

   - The project is divided into distinct layers: Api, Core, Infrastructure, and UseCases. Each layer has a clear responsibility and is loosely coupled to others.

   - Dependencies flow inward: outer layers depend on abstractions defined in the Core layer.

2. **Domain-Driven Design (DDD):**

   - The core domain logic resides in the PaymentGateway.Core layer. This includes entities, value objects, and domain services.

   - A small version of DDD is implemented, focusing on capturing the domain's core business logic while keeping the design practical for the scope of this project.

3. **Command-Handler Pattern:**

   - Business logic is encapsulated in command handlers, making the application logic reusable and testable.

   - Commands represent operations, and handlers process these commands, reducing coupling between the API and core logic.

4. **Mediator Pattern:**

   - The application uses MediatR to coordinate commands, queries, and events. This promotes loose coupling between layers and allows the API to delegate responsibilities to the appropriate handlers.

## Tech Stack

- **Backend**: .NET 8 (ASP.NET Core)
- **Containerization**: Docker & Docker Compose
- **Testing**: xUnit (for unit and integration tests)
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

---

## Prerequisites

- **.NET SDK 8.0**: [Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **Docker**: [Install Docker](https://docs.docker.com/get-docker/)
- **Docker Compose**: Comes with Docker Desktop

---

## Setup Instructions

### Clone the Repository

```bash
git clone https://github.com/fpessoto/payment-gateway-challenge-dotnet.git
cd payment-gateway-challenge-dotnet
```

## Running the Application

### Using Docker Compose

1. Build and start the services (API and bank simulator):

```bash
docker-compose up --build
```

## Testing

### Manually

- Create Payment - With Valid Card Info

```bash
curl --request POST \
  --url http://localhost:8082/api/Payments \
  --header 'Content-Type: application/json' \
  --data '{
  "cardNumber": 2222405343248877,
  "expiryMonth": 4,
  "expiryYear": 2025,
  "currency": "GBP",
  "amount": 100,
  "cvv": 123
}'
```

- Create Payment - With Invalid Card Info

```bash
curl --request POST \
  --url http://localhost:8082/api/Payments \
  --header 'Content-Type: application/json' \
  --data '{
  "cardNumber": 2222405343248112,
  "expiryMonth": 1,
  "expiryYear": 2026,
  "currency": "USD",
  "amount": 60000,
  "cvv": 456
}'
```

- Get Payment

```bash
curl --request GET --url http://localhost:8082/api/Payments/c7d68e63-5417-4550-ad15-1e21830c20a7
```

### Automated Tests

```bash
dotnet test
```

## Project Structure

```plaintext
src/
├── PaymentGateway.Api/          # Main API project
├── PaymentGateway.Core/         # Core domain logic
├── PaymentGateway.Infrastructure/ # Infrastructure services
├── PaymentGateway.UseCases/     # Business use cases
test/
├── PaymentGateway.Api.Tests/    #  Integration tests
├── PaymentGateway.Api.UnitTests/    # Unit tests
```

## Possible Improvements and Future Developments

While the current implementation satisfies the requirements of the Checkout.com Engineering
Assessment, several enhancements and future development ideas could further improve the project:

1. **Error Handling:**

- Implement a global error-handling middleware to standardize error responses.
- Provide detailed error codes and messages for client-side troubleshooting.

**2. Persistence Layer:**

- Integrate a database (e.g., PostgreSQL or MongoDB) to persist payment records instead of relying on in-memory storage.

**3. Advanced Validation:**

- Enhance validation for inputs such as card numbers.
- Add more comprehensive validation for currencies and amounts.

**4. Event-Driven Architecture:**

- Publish domain events (e.g., PaymentProcessed, PaymentFailed) to an event bus for asynchronous processing.
- Integrate with a message broker like RabbitMQ or Kafka to support event-driven workflows.

**5. Monitoring and Observability:**

- Add structured logging and monitoring using tools like DataDog, Seq, or Application Insights.
- Include distributed tracing to debug payment flows across services.

**6. Security Enhancements:**

- Implement token-based authentication (e.g., JWT or OAuth2).
- Encrypt sensitive information like card numbers and CVVs before storage or transmission.

**7. Scalability:**

- Deploy the application to a cloud environment (e.g., AWS, Azure) and leverage managed services like API Gateway, Lambda Functions, and DynamoDB.
- Use Kubernetes or Rancher for container orchestration in production environments.

**8. Documentation and Developer Experience:**

- Improve Swagger documentation with more detailed examples and descriptions.
- Add API versioning to allow backward compatibility for future updates.

**9. High Availability:**

- Add retries and fallback mechanisms for external services like the acquiring bank simulator.
- Use Polly to implement resiliency patterns like circuit breakers and retries.

**10. Extensibility:**

- Add support for multiple payment methods (e.g., PayPal, Apple Pay).
- Design an extensible plugin-based architecture to integrate new acquiring banks seamlessly.
- These improvements would enhance the robustness, scalability, and maintainability of the application, making it production-ready for real-world scenarios.
