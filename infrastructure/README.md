# Infrastructure - Entra ID App Registration

This directory contains infrastructure-as-code files for deploying the required Azure resources for MAF Assistants Microsoft Graph integration.

## Prerequisites

- Azure CLI installed ([Install Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli))
- Azure subscription with permissions to create Entra ID app registrations
- Global Administrator or Application Administrator role in Entra ID

## Quick Start

### Option 1: Using Azure CLI (Recommended)

```bash
# Login to Azure
az login

# Set your subscription (if you have multiple)
az account set --subscription "your-subscription-name"

# Deploy the app registration
az deployment sub create \
  --location "eastus" \
  --template-file app-registration.bicep \
  --parameters applicationName="MAF-Assistants-GraphAPI" \
              redirectUri="http://localhost" \
              createClientSecret=true
```

### Option 2: Using Azure Portal

1. Go to Azure Portal
2. Navigate to Subscriptions
3. Select your subscription
4. Click "Deployments" in the left menu
5. Click "Create"
6. Upload the `app-registration.bicep` file
7. Fill in the parameters
8. Review and create

### Option 3: Manual Azure CLI Commands

If you prefer not to use Bicep or encounter issues with the Microsoft.Graph provider, you can create the app registration manually:

```bash
# Create the app registration
az ad app create \
  --display-name "MAF-Assistants-GraphAPI" \
  --sign-in-audience "AzureADMyOrg" \
  --web-redirect-uris "http://localhost" \
  --enable-id-token-issuance true

# Get the app ID (replace with your actual app ID from the output above)
APP_ID="your-app-id-here"

# Add Microsoft Graph API permissions
# Sites (SharePoint)
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions 9492366f-7969-46a4-8d15-ed1a20078fff=Role  # Sites.Read.All
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions 0c0bf378-bf22-4481-8f81-9e89a9b4960a=Role  # Sites.ReadWrite.All

# Mail
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions 810c84a8-4a9e-49e6-bf7d-12d183f40d01=Role  # Mail.Read
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions e2a3a72e-5f79-4c64-b1b1-878b674786c9=Role  # Mail.Send

# Teams
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions 660b7406-55f1-41ca-a0ed-0b035e182f3e=Role  # Team.ReadBasic.All
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions 59a6b24b-4225-4393-8165-ebaec5f55d7a=Role  # Channel.ReadBasic.All
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions 7b2449af-6ccd-4f4d-9f78-e550c193f0d1=Role  # ChannelMessage.Read.All
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions 4d02b0cc-d90b-441f-8d82-4fb55c34d6bb=Role  # ChannelMessage.Send

# Files (OneDrive)
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions 01d4889c-1287-42c6-ac1f-5d1e02578ef6=Role  # Files.Read.All
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions 75359482-378d-4052-8f01-80520e7db3cd=Role  # Files.ReadWrite.All

# Tasks (Planner)
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions 913e9c1b-3c8a-4e47-a739-b66df0f6f2a6=Role  # Tasks.Read
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions b1b3e0c7-9a6f-4ce0-832e-8e7b0f2e3f0c=Role  # Tasks.ReadWrite

# User
az ad app permission add --id $APP_ID --api 00000003-0000-0000-c000-000000000000 --api-permissions df021288-bdef-4463-88db-98f22de89214=Role  # User.Read.All

# Grant admin consent (requires admin privileges)
az ad app permission admin-consent --id $APP_ID

# Create a client secret
az ad app credential reset --id $APP_ID --display-name "MAF-Assistants-Secret"
```

## Parameters

| Parameter | Description | Default | Required |
|-----------|-------------|---------|----------|
| `applicationName` | Name of the app registration | `MAF-Assistants-GraphAPI` | No |
| `redirectUri` | Redirect URI for delegated auth | `http://localhost` | No |
| `createClientSecret` | Whether to create a client secret | `true` | No |

## Post-Deployment Steps

### 1. Grant Admin Consent

After deployment, you **must** grant admin consent for the API permissions:

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **Entra ID** > **App registrations**
3. Find and select your application (e.g., "MAF-Assistants-GraphAPI")
4. Click **API permissions** in the left menu
5. Click **Grant admin consent for [Your Organization]**
6. Confirm the action

### 2. Configure User Secrets

Update your application's user secrets with the values from the deployment:

```json
{
  "MicrosoftGraphTenantId": "your-tenant-id",
  "MicrosoftGraphClientId": "your-client-id",
  "MicrosoftGraphClientSecret": "your-client-secret",
  "MicrosoftGraphMaxItems": 100,
  "MicrosoftGraphUseDelegatedAuth": false,
  "MicrosoftGraphRedirectUri": "http://localhost"
}
```

To set user secrets in .NET:
```bash
cd MAF.Assistants.Runners
dotnet user-secrets set "MicrosoftGraphTenantId" "your-tenant-id"
dotnet user-secrets set "MicrosoftGraphClientId" "your-client-id"
dotnet user-secrets set "MicrosoftGraphClientSecret" "your-client-secret"
```

### 3. Choose Authentication Mode

**Application Authentication (App-Only):**
- Best for server-to-server scenarios
- Requires `MicrosoftGraphClientSecret`
- Set `MicrosoftGraphUseDelegatedAuth` to `false` (default)

**Delegated Authentication (User Context):**
- Best for user-specific operations
- Requires user to sign in via browser
- Set `MicrosoftGraphUseDelegatedAuth` to `true`
- User will be prompted to sign in on first use

## Permissions Included

The app registration includes both **Application** (app-only) and **Delegated** (user) permissions for:

### SharePoint
- `Sites.Read.All` - Read items in all site collections
- `Sites.ReadWrite.All` - Read and write items in all site collections

### Email
- `Mail.Read` - Read mail in all mailboxes / Read user mail
- `Mail.Send` - Send mail as any user / Send mail as a user

### Teams
- `Team.ReadBasic.All` - Read team names and descriptions
- `Channel.ReadBasic.All` - Read channel names and descriptions
- `ChannelMessage.Read.All` - Read all channel messages (app-only)
- `ChannelMessage.Send` - Send messages to channels

### OneDrive
- `Files.Read.All` - Read files in all site collections / Read all files user can access
- `Files.ReadWrite.All` - Read and write files / Full access to files user can access

### Planner
- `Tasks.Read` - Read all tasks / Read user tasks
- `Tasks.ReadWrite` - Read and write all tasks / Create, read, update, delete user tasks

### User
- `User.Read` - Sign in and read user profile (delegated)
- `User.Read.All` - Read all users' full profiles (app-only)

## Troubleshooting

### Issue: "Microsoft.Graph provider not registered"

The Bicep template uses the Microsoft.Graph resource provider which may not be available in all Azure subscriptions yet. Use the manual Azure CLI commands instead (Option 3 above).

### Issue: "Insufficient privileges to complete the operation"

You need at least one of these roles in Entra ID:
- Global Administrator
- Application Administrator
- Cloud Application Administrator

### Issue: "Admin consent required"

Some permissions require admin consent. Follow the "Grant Admin Consent" steps above.

### Issue: "Client secret not appearing in output"

For security reasons, the client secret is only shown once during creation. If you missed it:
1. Go to your app registration in Azure Portal
2. Navigate to **Certificates & secrets**
3. Create a new client secret

## Security Best Practices

1. **Rotate secrets regularly** - Client secrets should be rotated at least annually
2. **Use managed identities** when possible - For Azure-hosted services
3. **Limit permissions** - Only grant the minimum required permissions
4. **Monitor usage** - Enable logging and monitor API usage
5. **Secure secrets** - Never commit secrets to source control

## Additional Resources

- [Microsoft Graph Permissions Reference](https://docs.microsoft.com/en-us/graph/permissions-reference)
- [Entra ID App Registration Documentation](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
- [Azure Bicep Documentation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/)
- [MAF Assistants Graph Integration Documentation](../MAF.Assistants.Runners/GRAPH_INTEGRATION.md)
