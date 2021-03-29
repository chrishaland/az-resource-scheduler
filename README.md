# az-resource-scheduler


## Adding database migrations

```
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef migrations add "<migration_name>" --startup-project Host --project Repository --context DatabaseContext
```

## User secrets for development

```
# Database connection
dotnet user-secrets set "ConnectionStrings:Database" "Server=localhost;Database=az-resource-scheduler;User ID=sa;Password=yourStrong(!)Password;MultipleActiveResultSets=true"

# Authentication
dotnet user-secrets set "oidc:clientId" "<clientId>"
dotnet user-secrets set "oidc:clientSecret" "<clientSecret>"
dotnet user-secrets set "oidc:authorityUri" "<authorityUri>"

# Feature flags
dotnet user-secrets set "unleash:apiUrl" "<apiUrl>"
dotnet user-secrets set "unleash:instanceTag" "<instanceTag>"
```
