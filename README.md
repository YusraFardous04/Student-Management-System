# Student Management System

School management system with a KPI dashboard and an AI assistant.
Built with ASP.NET Core, Entity Framework Core, SQL Server and Razor Pages.

## Setup
1. Create the database: `dotnet ef database update` (in the Api project)
2. Load the sample data: run `seed.sql` against the database
3. Add your Ollama key: `dotnet user-secrets set "Ollama:ApiKey" "<your key>"`
4. Run the Api project, then the Web project
