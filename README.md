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
├── PaymentGateway.Api.Tests/    # Unit and integration tests
```
