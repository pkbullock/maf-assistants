# Infrastructure 

## Entra ID App Registration

This directory contains scripts and deployment files for deploying the required Azure resources for MAF Assistants.

### Prerequisites

- Az PowerShell Module installed ```Install-Module -Name Az``` 
- Azure subscription with permissions to create Entra ID app registrations
- Global Administrator or Application Administrator role in Entra ID

### Quick Start

Run ```deploy-app-registration.ps1``` and capture the secret

Follow the instructions displayed by the script to set the user-secret values in the application

### Troubleshooting

#### Issue: "Insufficient privileges to complete the operation"

You need at least one of these roles in Entra ID:
- Global Administrator
- Application Administrator
- Cloud Application Administrator

#### Issue: "Admin consent required"

Some permissions require admin consent. Follow the "Grant Admin Consent" steps above.

#### Issue: "Client secret not appearing in output"

For security reasons, the client secret is only shown once during creation. If you missed it:
1. Go to your app registration in Azure Portal
2. Navigate to **Certificates & secrets**
3. Create a new client secret

### Security Best Practices

1. **Rotate secrets regularly** - Client secrets should be rotated at least annually
2. **Use managed identities** when possible - For Azure-hosted services
3. **Limit permissions** - Only grant the minimum required permissions
4. **Monitor usage** - Enable logging and monitor API usage
5. **Secure secrets** - Never commit secrets to source control


## Azure Open AI Setup

Sets up an instance of Azure Open AI

> Note: This is a public instance - protect use of your keys

### Option: Using PowerShell

This uses bicep, ensure this is installed and set the system variable PATH to ```%USERPROFILE%\.azure\bin```

Run ```deploy-app-registration.ps1``` and capture the secret

Follow the instructions displayed by the script (if any)



## Other User Secrets to set

```shell
  dotnet user-secrets set "AzureChatDeploymentName": "gpt-4.1-mini",
  dotnet user-secrets set "AzureEmbeddingModelName": "text-embedding-3-small",
  dotnet user-secrets set "AzureOpenAiEndpoint": "https://<your-instance>.openai.azure.com",
  dotnet user-secrets set "AzureOpenAiKey": "XXX",
  dotnet user-secrets set "FoundryLocalChatDeploymentName": "Phi-4-mini-instruct-generic-cpu:4",
  dotnet user-secrets set "OllamaLocalEmbeddingModelName": "embeddinggemma"
```


## Additional Resources

- [Microsoft Graph Permissions Reference](https://docs.microsoft.com/en-us/graph/permissions-reference)
- [Entra ID App Registration Documentation](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
- [Azure Bicep Documentation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/)
- [MAF Assistants Graph Integration Documentation](../MAF.Assistants.Runners/GRAPH_INTEGRATION.md)
