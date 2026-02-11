# Publish script for SalesTracker.Web.Client
# Publishes the Blazor WebAssembly project and then deploys to Firebase

Write-Host "Starting publish process..." -ForegroundColor Green

# Publish the Blazor WebAssembly project
Write-Host "Running dotnet publish..." -ForegroundColor Yellow
dotnet publish -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Publish completed successfully!" -ForegroundColor Green

# Deploy to Firebase
Write-Host "Deploying to Firebase..." -ForegroundColor Yellow
firebase deploy

if ($LASTEXITCODE -ne 0) {
    Write-Host "Firebase deploy failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Firebase deployment completed successfully!" -ForegroundColor Green
