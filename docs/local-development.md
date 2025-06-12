# Local Development Setup

This guide will help you set up the Test1 project for local development.

## Prerequisites

### Required Software

- **.NET 8.0 SDK**: [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Git**: For version control
- **Docker**: For containerization (optional but recommended)

### Development Environment Options

#### Option 1: Visual Studio Code (Recommended)
- Install [Visual Studio Code](https://code.visualstudio.com/)
- Install the C# extension
- Install the Docker extension (if using Docker)

#### Option 2: Visual Studio
- Install [Visual Studio 2022](https://visualstudio.microsoft.com/) with .NET workload

#### Option 3: JetBrains Rider
- Install [JetBrains Rider](https://www.jetbrains.com/rider/)

## Setup Steps

### 1. Clone the Repository

```bash
git clone https://github.com/kvaes/test1.git
cd test1
```

### 2. Verify .NET Installation

```bash
dotnet --version
```

You should see version 8.0.x or later.

### 3. Restore Dependencies

```bash
cd agent
dotnet restore
```

### 4. Build the Project

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run
```

## Development Workflow

### Building and Testing

```bash
# Build the project
dotnet build

# Run tests (when available)
dotnet test

# Clean build artifacts
dotnet clean
```

### Docker Development

If you prefer to develop using Docker:

```bash
# Build the image
docker build -t test1-agent -f agent/Dockerfile .

# Run the container
docker run test1-agent

# Run with volume mounting for development
docker run -v $(pwd)/agent:/app/source test1-agent
```

## Project Structure

```
agent/
├── Functions/          # Semantic Kernel functions
├── Processes/          # Business processes
├── Services/           # Service layer implementations
├── Steps/              # Reusable process steps
├── Program.cs          # Application entry point
├── Agent.csproj        # Project configuration
└── Dockerfile          # Container definition
```

## Configuration

### Environment Variables

The application supports the following environment variables:

- `ASPNETCORE_ENVIRONMENT`: Set to `Development` for local development
- `Logging__LogLevel__Default`: Set logging level (Debug, Information, Warning, Error)

### appsettings.json

Create an `appsettings.Development.json` file in the agent folder for local configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Agent": "Debug"
    }
  }
}
```

## Debugging

### Visual Studio Code

1. Open the project folder in VS Code
2. Install the C# extension if not already installed
3. Press F5 to start debugging
4. Set breakpoints as needed

### Visual Studio

1. Open the `Agent.csproj` file
2. Set the startup project to Agent
3. Press F5 to start debugging

## Common Issues and Solutions

### Issue: "The framework 'Microsoft.NETCore.App', version '8.0.x' was not found"

**Solution**: Install the .NET 8.0 SDK from the official Microsoft website.

### Issue: Package restore fails

**Solution**: 
```bash
dotnet nuget locals all --clear
dotnet restore
```

### Issue: Build fails with dependency conflicts

**Solution**:
```bash
dotnet clean
dotnet restore --force
dotnet build
```

## Next Steps

Once you have the local environment working:

1. Review the [Architecture Documentation](architecture.md)
2. Check out the [BICS API Integration Guide](bics-api-integration.md)
3. Explore the [Plugin Development Guide](plugin-development.md)

## Getting Help

- Check the [GitHub Issues](https://github.com/kvaes/test1/issues) for known problems
- Review the [Security Policy](../.github/SECURITY.md) for security-related questions
- Contact the maintainers through GitHub