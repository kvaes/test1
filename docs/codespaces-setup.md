# GitHub Codespaces Development

This guide explains how to develop the Test1 project using GitHub Codespaces, providing a complete development environment in the cloud.

## What is GitHub Codespaces?

GitHub Codespaces provides a cloud-based development environment that runs in your browser or VS Code. It comes pre-configured with all the tools you need to develop, build, and test the project.

## Quick Start

### 1. Create a Codespace

1. Navigate to the [Test1 repository](https://github.com/kvaes/test1)
2. Click the green "Code" button
3. Select the "Codespaces" tab
4. Click "Create codespace on main"

### 2. Wait for Setup

The codespace will automatically:
- Install .NET 8.0 SDK
- Restore project dependencies
- Configure the development environment
- Install required VS Code extensions

## Development Workflow in Codespaces

### Building and Running

Once your codespace is ready:

```bash
# Navigate to the agent directory
cd agent

# Build the project
dotnet build

# Run the application
dotnet run
```

### Testing

```bash
# Run tests
dotnet test

# Run with coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

### Container Development

Build and test the Docker container:

```bash
# Build the container
docker build -t test1-agent -f agent/Dockerfile .

# Run the container
docker run test1-agent
```

## End-to-End Development Setup

### Project Architecture in Codespaces

The project is designed to support full end-to-end development in Codespaces:

```
┌─────────────────────────────────────────┐
│           GitHub Codespace              │
├─────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────────┐   │
│  │   Agent     │  │   Future        │   │
│  │ (C# SK)     │  │   Frontend      │   │
│  │             │  │   (Vue.js)      │   │
│  └─────────────┘  └─────────────────┘   │
│         │                  │           │
│         └──────────────────┘           │
│              Internal API               │
├─────────────────────────────────────────┤
│              External APIs              │
│         (BICS API Developer Portal)     │
└─────────────────────────────────────────┘
```

### Setting Up End-to-End Testing

1. **Start the Agent Service**
   ```bash
   cd agent
   dotnet run
   ```

2. **Verify BICS API Connection**
   The agent includes plugins for connecting to the BICS API Developer Portal. Test the connection:
   ```bash
   # The agent will log connection status on startup
   # Check logs for successful BICS API initialization
   ```

3. **Test Plugin Functionality**
   The agent includes several built-in plugins:
   - `BicsApiPlugin`: For BICS API integration
   - `CorePlugin`: For core data processing

### Port Forwarding

Codespaces automatically handles port forwarding for web applications. When you run services:

- The agent service will be available on its configured port
- Any web interfaces will be accessible through Codespaces' port forwarding
- External API connections work seamlessly

### Environment Configuration

Create environment-specific configuration in Codespaces:

1. **Create a local configuration file**:
   ```bash
   cat > agent/appsettings.Development.json << EOF
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Agent": "Debug",
         "Agent.Services": "Debug"
       }
     },
     "BicsApi": {
       "BaseUrl": "https://developer.bics.com/api",
       "Timeout": "00:00:30"
     }
   }
   EOF
   ```

2. **Set environment variables** (if needed):
   ```bash
   export ASPNETCORE_ENVIRONMENT=Development
   export BICS_API_KEY=your-api-key-here
   ```

## Codespace Configuration

### Extensions

The following VS Code extensions are recommended and will be automatically installed:

- C# for Visual Studio Code
- Docker
- GitHub Copilot (if available)
- GitLens
- REST Client (for API testing)

### Debugging

To debug the application in Codespaces:

1. Open the project in the integrated VS Code
2. Set breakpoints in your code
3. Press `F5` or use the Debug panel
4. The debugger will attach to the running process

### Terminal Usage

Codespaces provides multiple terminal options:

```bash
# Main terminal for running the agent
cd agent && dotnet run

# Second terminal for testing
dotnet test

# Third terminal for Docker operations
docker build -t test1-agent -f agent/Dockerfile .
```

## Collaboration Features

### Live Share (if enabled)

Multiple developers can collaborate in the same codespace:

1. Share your codespace URL
2. Collaborators can join your session
3. Real-time editing and debugging

### Shared Debugging

When collaborating:
- Set breakpoints collaboratively
- Share terminal sessions
- Debug together in real-time

## Performance Tips

### Resource Management

- **CPU**: Codespaces provides adequate CPU for .NET development
- **Memory**: 4GB+ recommended for Semantic Kernel applications
- **Storage**: Project files are stored in the cloud

### Optimization

```bash
# Clean up build artifacts regularly
dotnet clean

# Clear NuGet cache if needed
dotnet nuget locals all --clear

# Use incremental builds
dotnet build --no-restore
```

## Troubleshooting

### Common Issues

1. **Slow builds**: Use `dotnet build --no-restore` after initial setup
2. **Port conflicts**: Codespaces automatically manages ports
3. **Environment variables**: Set them in the terminal or configuration files

### Getting Help

- Use the built-in VS Code help system
- Check GitHub Codespaces documentation
- Review project-specific documentation in the `docs/` folder

## Next Steps

Once your Codespace is set up:

1. Explore the [Plugin Development Guide](plugin-development.md)
2. Review the [BICS API Integration](bics-api-integration.md)
3. Check out the [Architecture Documentation](architecture.md)

## Codespace Lifecycle

### Saving Work

- Changes are automatically saved to the cloud
- Commit and push regularly to GitHub
- Codespaces automatically sync with the repository

### Stopping and Starting

- Codespaces automatically stop after inactivity
- Restart anytime from the GitHub interface
- All your work and configuration is preserved