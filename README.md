# Test1 Project

This repository contains a C# based Semantic Kernel agent for ingesting metered usage for GitHub and recording that usage.

## Architecture

The project consists of:
- **Agent**: A C# Semantic Kernel-based application
- **BICS API Integration**: Plugins for interacting with the BICS API Developer Portal

## Project Structure

```
├── agent/                      # C# Semantic Kernel agent
│   ├── Functions/              # Kernel function definitions
│   ├── Processes/              # Defined processes
│   ├── Services/               # Service implementations
│   ├── Steps/                  # Reusable steps
│   ├── Dockerfile              # Container definition
│   └── Agent.csproj            # Project file
├── docs/                       # Documentation
├── .github/
│   ├── workflows/              # CI/CD workflows
│   ├── dependabot.yml          # Dependency management
│   └── SECURITY.md             # Security policy
└── README.md                   # This file
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Docker (for containerization)
- Visual Studio Code or Visual Studio

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/kvaes/test1.git
   cd test1
   ```

2. **Restore dependencies**
   ```bash
   cd agent
   dotnet restore
   ```

3. **Build the application**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

### Docker Development

1. **Build the Docker image**
   ```bash
   docker build -t test1-agent -f agent/Dockerfile .
   ```

2. **Run the container**
   ```bash
   docker run test1-agent
   ```

## Features

### Semantic Kernel Integration
- Native plugin architecture
- Error handling patterns
- Dependency injection

### BICS API Integration
- Multiple API endpoints supported
- Async/await patterns
- Comprehensive error handling

### Security
- Non-root container execution
- Dependency scanning via Dependabot
- Security policy and guidelines

## Contributing

Please read our [Security Policy](.github/SECURITY.md) before contributing.

### Development Workflow

1. Create a feature branch
2. Make your changes
3. Ensure tests pass
4. Submit a pull request

### Code Standards

- Follow C# coding conventions
- Include appropriate error handling
- Add unit tests for new functionality
- Update documentation as needed

## CI/CD

The project uses GitHub Actions for:
- Automated building and testing
- Container image creation and publishing
- Dependency updates via Dependabot

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.