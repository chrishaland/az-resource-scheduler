# Client

The client is written in [Yarn 2](https://yarnpkg.com/).

* PnP
* Hooks
* Initiated from React Create App

## Development

Create a `oidc.json` file in `./public/_configuration/` with the format:

```
{
  "authority": "<oidc_provider_url>",
  "client_id": "<client_id>",
  "scope": "openid profile email",
  "roles": {
    "admin": "admin"
  }
}
```

The file is loaded by the client on initial render and its values used for authenticating the user using a public client from the authority (e.g. Azure AD, Keycloak). The client uses the `authorization code` flow.

## Hosting

We host the client as a static website using nginx, and use Istio to route api traffic to the API.

## Security

The nginx container runs as a non priviledged user and is configured with the most common security recommended by [OWASP](https://cheatsheetseries.owasp.org/cheatsheets/DotNet_Security_Cheat_Sheet.html#a3-sensitive-data-exposure).
