# ResumeAI

ResumeAI is a backend service for resume-related AI features. The solution is organized as a modular .NET application with clean layering (API, Application, Domain, Infrastructure, Shared) and contains unit and integration tests.

## Key information
- Target framework: .NET 10
- Solution file: ResumeAI.slnx
- Main API project: ResumeAI.Api

## Prerequisites
- .NET 10 SDK (install from https://dotnet.microsoft.com)
- Optional: Docker (if you want to containerize the API)

## Getting started
From the repository root (where ResumeAI.slnx is located):

1. Restore and build

   dotnet restore
   dotnet build --configuration Release

2. Run the API locally

   dotnet run --project ResumeAI.Api --configuration Debug

   The API project uses appsettings.json and environment variables for configuration. Provide any required secrets or connection strings through appsettings.*.json or environment variables (for example, ASPNETCORE_ENVIRONMENT or service-specific keys).

3. Run tests

   dotnet test

## Project structure
- ResumeAI.Api — Web API project (entry point)
- ResumeAI.Application — Application layer, use cases and orchestration
- ResumeAI.Domain — Core domain models and business logic
- ResumeAI.Infrastructure — Integrations, data access and external services
- ResumeAI.Shared — Shared utilities and common types
- ResumeAI.UnitTests — Unit tests
- ResumeAI.IntegrationTests — Integration tests

## Configuration
Keep sensitive configuration out of source control. Use user secrets or environment variables for local development. Typical values to configure:
- Connection strings for databases
- API keys and secrets for external services
- Logging and telemetry settings

## Development notes
- Follow SOLID and layered architecture patterns used across the solution.
- Add tests for any new behavior and keep the public API changes backwards compatible when possible.

## Contributing
1. Fork the repository and create a feature branch
2. Add tests and documentation for your changes
3. Open a pull request describing the change and rationale

## License
Specify license information here (e.g., MIT) or consult the project owner.

## Contact
For questions about the project structure or setup, open an issue or contact the repository maintainers.

