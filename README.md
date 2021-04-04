# Azure Resource Scheduler

## Configuration

### Configuring OpenId Connect Client
The following configuration describes creating a new OpenID Connect client in the IAM product, [Keycloak](https://www.keycloak.org/).

Create a client with the following properties:

| Type | Value |
| ---- | ----- |
| Client protocol | openid-connect |
| Access Type | confidential |
| Standard Flow Enabled | ON |
| Implicit Flow Enabled | OFF |
| Direct Access Grants Enabled | OFF |
| Service Accounts Enabled | OFF |
| Authorization Enabled | OFF |
| Valid redirects | https://\<hostname>, https://\<hostname>/api/account/signin-oidc |

Add the following roles to the client:

* user
* admin

Add a mapper, allowing the user info endpoint access to the roles assign to a user:

| Type | Value |
| ---- | ----- |
| Name | role mapping |
| Mapper Type | User Client Role |
| Client ID | \<client_id>
| Multivalued | ON |
| Token Claim Name | ${client_id}\\.roles |
| Claim JSON Type | String |
| Add to ID token | OFF |
| Add to access token | OFF |
| Add to userinfo | ON |

## Development

### Adding database migrations

```
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef migrations add "<migration_name>" --startup-project Host --project Repository --context DatabaseContext
```

### User secrets for development

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
