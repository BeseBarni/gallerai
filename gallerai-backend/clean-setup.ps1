param (
    [Parameter(Mandatory=$true)]
    [string]$SolutionName
)

# 1. Setup Directory and Solution
Write-Host "Creating Solution: $SolutionName..." -ForegroundColor Green
dotnet new sln -n $SolutionName

# 2. Create Projects
Write-Host "Creating Projects..." -ForegroundColor Cyan
dotnet new classlib -n "$SolutionName.SharedKernel"
dotnet new classlib -n "$SolutionName.Domain"
dotnet new classlib -n "$SolutionName.Application"
dotnet new classlib -n "$SolutionName.Infrastructure"
dotnet new webapi   -n "$SolutionName.WebAPI" --no-openapi

# 3. Clean up default 'Class1.cs' and 'WeatherForecast.cs' files
Write-Host "Cleaning up default files..." -ForegroundColor Yellow
Remove-Item "$SolutionName.SharedKernel/Class1.cs" -ErrorAction SilentlyContinue
Remove-Item "$SolutionName.Domain/Class1.cs" -ErrorAction SilentlyContinue
Remove-Item "$SolutionName.Application/Class1.cs" -ErrorAction SilentlyContinue
Remove-Item "$SolutionName.Infrastructure/Class1.cs" -ErrorAction SilentlyContinue
# Optional: Keep WeatherForecast in API for testing, or delete it:
Remove-Item "$SolutionName.WebAPI/WeatherForecast.cs" -ErrorAction SilentlyContinue
Remove-Item "$SolutionName.WebAPI/Controllers/WeatherForecastController.cs" -ErrorAction SilentlyContinue

# 4. Add Projects to Solution
Write-Host "Adding projects to SLN..." -ForegroundColor Cyan
dotnet sln add "$SolutionName.SharedKernel/$SolutionName.SharedKernel.csproj"
dotnet sln add "$SolutionName.Domain/$SolutionName.Domain.csproj"
dotnet sln add "$SolutionName.Application/$SolutionName.Application.csproj"
dotnet sln add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj"
dotnet sln add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj"

# 5. Add Project References (The Onion Architecture)
Write-Host "Wiring up dependencies..." -ForegroundColor Magenta

# Domain -> SharedKernel
dotnet add "$SolutionName.Domain/$SolutionName.Domain.csproj" reference "$SolutionName.SharedKernel/$SolutionName.SharedKernel.csproj"

# Application -> Domain, SharedKernel
dotnet add "$SolutionName.Application/$SolutionName.Application.csproj" reference "$SolutionName.Domain/$SolutionName.Domain.csproj"
dotnet add "$SolutionName.Application/$SolutionName.Application.csproj" reference "$SolutionName.SharedKernel/$SolutionName.SharedKernel.csproj"

# Infrastructure -> Application, Domain, SharedKernel
dotnet add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj" reference "$SolutionName.Application/$SolutionName.Application.csproj"
dotnet add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj" reference "$SolutionName.Domain/$SolutionName.Domain.csproj"
dotnet add "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj" reference "$SolutionName.SharedKernel/$SolutionName.SharedKernel.csproj"

# API -> Application, Infrastructure, SharedKernel
dotnet add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj" reference "$SolutionName.Application/$SolutionName.Application.csproj"
dotnet add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj" reference "$SolutionName.Infrastructure/$SolutionName.Infrastructure.csproj"
dotnet add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj" reference "$SolutionName.SharedKernel/$SolutionName.SharedKernel.csproj"

# 6. Install Base Nuget Packages
Write-Host "Installing Base NuGet Packages..." -ForegroundColor Green

# Application Layer: MediatR (Command/Query Pattern), FluentValidation
dotnet add "$SolutionName.Application/$SolutionName.Application.csproj" package MediatR
dotnet add "$SolutionName.Application/$SolutionName.Application.csproj" package FluentValidation
dotnet add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj" package FastEndpoints
dotnet add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj" package FastEndpoints.Swagger
dotnet add "$SolutionName.WebAPI/$SolutionName.WebAPI.csproj" package Scalar.AspNetCore

Write-Host "Done! Your Clean Architecture solution '$SolutionName' is ready." -ForegroundColor Green