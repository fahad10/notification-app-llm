Notification Application with LLM-Based Alerting

Overview

This project is a C# ASP.NET Core Web API application that receives notifications through an HTTP endpoint and forwards important alerts to Discord.

The application uses an LLM API (OpenAI GPT model) to:

* Analyze incoming logs and notifications
* Determine the type and severity of warning/error
* Generate a human-readable alert message

The system also includes:

* Rate limiting (maximum 10 forwarded alerts per minute)
* Unit tests
* Integration tests
* Logging
* Discord webhook integration

⸻

Features

HTTP Notification API

The application exposes an HTTP endpoint:

POST /notifications

It accepts JSON payloads containing:

* Source
* Level
* Message
* Timestamp

⸻

Severity Filtering

Notifications with level:

* Info → ignored
* Warning or higher → processed and forwarded

⸻

LLM Integration (OpenAI)

The application integrates with the OpenAI API.

The LLM:

1. Analyzes the incoming log
2. Determines the warning/error severity
3. Generates a contextual alert message

Example:

Input:

{
  "source": "payment-service",
  "level": 1,
  "message": "Payment gateway latency detected",
  "timestamp": "2026-05-15T16:31:00Z"
}

LLM Response:

{
  "severity": "WARNING",
  "message": "Payment latency detected"
}

Discord Output:

⚠️ WARNING: Payment latency detected

⸻

Discord Integration

Notifications with severity Warning or higher are automatically forwarded to Discord using a webhook.

Example:

🚨 CRITICAL: Multiple failed login attempts detected

⸻

Rate Limiting

The application enforces a limit of:

Maximum 10 forwarded messages per minute

If the limit is exceeded, the API returns an error.

⸻

Technologies Used

* ASP.NET Core Web API (.NET 9)
* C#
* OpenAI API
* Discord Webhooks
* xUnit
* Moq
* FluentAssertions

⸻

Project Structure

NotificationApp_FahadHassan/
│
├── src/
│   └── NotificationApi/
│       ├── Controllers/
│       ├── Services/
│       ├── Interfaces/
│       ├── Dtos/
│       ├── RateLimiting/
│       └── Program.cs
│
├── tests/
│   └── NotificationApi.Tests/
│       ├── NotificationProcessorTests.cs
│       ├── NotificationApiIntegrationTests.cs
│       └── RateLimiterTests.cs
│
└── README.md

⸻

API Endpoint

POST /notifications

Request Payload

{
  "source": "payment-service",
  "level": 1,
  "message": "Payment gateway latency detected",
  "timestamp": "2026-05-15T16:31:00Z"
}

⸻

Notification Levels

Level	Meaning
0	Info
1	Warning
2	Error
3	Critical

⸻

Setup Instructions

Prerequisites

Install:

* .NET 
* Visual Studio / VS Code

⸻

Clone Repository

git clone <repository-url>
cd NotificationApp_FahadHassan

⸻

Configure appsettings.json

{
  "OpenAI": {
    "ApiKey": "YOUR_OPENAI_API_KEY"
  },
  "Discord": {
    "WebhookUrl": "YOUR_DISCORD_WEBHOOK"
  }
}

⸻

Run Application

dotnet clean
dotnet build
dotnet run --project src/NotificationApi/NotificationApi.csproj

Swagger UI:

http://localhost:5264/swagger

⸻

Running Tests

Run all tests:

dotnet test

⸻

Testing Strategy

Unit Tests

Unit tests verify:

* Info notifications are ignored
* Warning notifications are forwarded
* Rate limiting behavior
* Discord forwarding logic

Mocks are used for:

* OpenAI service
* Discord notifier
* Rate limiter

⸻

Integration Tests

Integration tests verify:

* Full HTTP pipeline
* API endpoint behavior
* Request/response handling

⸻

Architecture Overview

Flow

Client Request
    ↓
NotificationsController
    ↓
NotificationProcessor
    ↓
OpenAIService
    ↓
DiscordNotifier

⸻

Logging

The application uses ASP.NET Core logging for:

* Notification processing
* External API calls
* Errors
* Rate limiting

⸻

Example Test Payloads

Warning

{
  "source": "payment-service",
  "level": 1,
  "message": "Payment gateway latency detected",
  "timestamp": "2026-05-15T16:31:00Z"
}

Error

{
  "source": "database-service",
  "level": 2,
  "message": "Database connection timeout",
  "timestamp": "2026-05-15T16:35:00Z"
}

Critical

{
  "source": "security-service",
  "level": 3,
  "message": "Multiple failed login attempts detected",
  "timestamp": "2026-05-15T16:40:00Z"
}

⸻

Notes

* The OpenAI API key requires billing/credits enabled.
* Discord webhook URL must be valid.
* The project was designed with clean architecture principles and dependency injection.
* External services are mocked during unit testing.

⸻

Future Improvements

Possible future enhancements:

* Persistent storage/database
* Retry mechanism for external APIs
* Background queue processing
* Authentication/authorization
* Docker support
* Kubernetes deployment
* Structured observability/monitoring
* Multiple notification channels (Slack, Teams, Email)

⸻

Conclusion

This application demonstrates:

* HTTP API development in ASP.NET Core
* External API integration
* LLM-powered log analysis
* Automated alert forwarding
* Unit and integration testing
* Rate limiting
* Clean software architecture