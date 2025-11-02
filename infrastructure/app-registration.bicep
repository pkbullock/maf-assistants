// Bicep template for creating an Entra ID App Registration with Microsoft Graph permissions
// for MAF Assistants Microsoft Graph integration

targetScope = 'subscription'

@description('The name of the application registration')
param applicationName string = 'MAF-Assistants-GraphAPI'

@description('The redirect URI for delegated authentication (default: http://localhost)')
param redirectUri string = 'http://localhost'

@description('Whether to create a client secret (set to false if using managed identity or certificate)')
param createClientSecret bool = true

// Microsoft Graph API Resource ID
var microsoftGraphAppId = '00000003-0000-0000-c000-000000000000'

// Microsoft Graph API Permissions
// Application permissions (app-only access)
var applicationPermissions = [
  {
    id: 'df021288-bdef-4463-88db-98f22de89214' // User.Read.All - Read all users' full profiles
    type: 'Role'
  }
  {
    id: '9a5d68dd-52b0-4cc2-bd40-abcf44ac3a30' // Application.Read.All - Read applications
    type: 'Role'
  }
  // SharePoint permissions
  {
    id: '9492366f-7969-46a4-8d15-ed1a20078fff' // Sites.Read.All - Read items in all site collections
    type: 'Role'
  }
  {
    id: '0c0bf378-bf22-4481-8f81-9e89a9b4960a' // Sites.ReadWrite.All - Read and write items in all site collections
    type: 'Role'
  }
  // Email permissions
  {
    id: '810c84a8-4a9e-49e6-bf7d-12d183f40d01' // Mail.Read - Read mail in all mailboxes
    type: 'Role'
  }
  {
    id: 'e2a3a72e-5f79-4c64-b1b1-878b674786c9' // Mail.Send - Send mail as any user
    type: 'Role'
  }
  // Teams permissions
  {
    id: '660b7406-55f1-41ca-a0ed-0b035e182f3e' // Team.ReadBasic.All - Read the names and descriptions of teams
    type: 'Role'
  }
  {
    id: '59a6b24b-4225-4393-8165-ebaec5f55d7a' // Channel.ReadBasic.All - Read the names and descriptions of channels
    type: 'Role'
  }
  {
    id: '7b2449af-6ccd-4f4d-9f78-e550c193f0d1' // ChannelMessage.Read.All - Read all channel messages
    type: 'Role'
  }
  {
    id: '4d02b0cc-d90b-441f-8d82-4fb55c34d6bb' // ChannelMessage.Send - Send messages to all channels
    type: 'Role'
  }
  // OneDrive/Files permissions
  {
    id: '01d4889c-1287-42c6-ac1f-5d1e02578ef6' // Files.Read.All - Read files in all site collections
    type: 'Role'
  }
  {
    id: '75359482-378d-4052-8f01-80520e7db3cd' // Files.ReadWrite.All - Read and write files in all site collections
    type: 'Role'
  }
  // Planner permissions
  {
    id: '913e9c1b-3c8a-4e47-a739-b66df0f6f2a6' // Tasks.Read - Read all tasks
    type: 'Role'
  }
  {
    id: 'b1b3e0c7-9a6f-4ce0-832e-8e7b0f2e3f0c' // Tasks.ReadWrite - Read and write all tasks
    type: 'Role'
  }
]

// Delegated permissions (user context)
var delegatedPermissions = [
  {
    id: 'e1fe6dd8-ba31-4d61-89e7-88639da4683d' // User.Read - Sign in and read user profile
    type: 'Scope'
  }
  // SharePoint permissions
  {
    id: '2cfdc887-d7b4-4798-9b33-3d98d6b95dd2' // Sites.Read.All - Read items in all site collections
    type: 'Scope'
  }
  {
    id: '89fe6a52-be36-487e-b7d8-d061c450a026' // Sites.ReadWrite.All - Read and write items in all site collections
    type: 'Scope'
  }
  // Email permissions
  {
    id: '570282fd-fa5c-430d-a7fd-fc8dc98a9dca' // Mail.Read - Read user mail
    type: 'Scope'
  }
  {
    id: 'e383f46e-2787-4529-855e-0e479a3ffac0' // Mail.Send - Send mail as a user
    type: 'Scope'
  }
  // Teams permissions
  {
    id: '660b7406-55f1-41ca-a0ed-0b035e182f3e' // Team.ReadBasic.All - Read the names and descriptions of teams
    type: 'Scope'
  }
  {
    id: '3b55498e-47ec-484f-8136-9013221c06a9' // Channel.ReadBasic.All - Read the names and descriptions of channels
    type: 'Scope'
  }
  {
    id: '767156cb-16ae-4d10-8f8b-41b657c8c8c8' // ChannelMessage.Send - Send messages to channels
    type: 'Scope'
  }
  // OneDrive/Files permissions
  {
    id: 'df85f4d6-205c-4ac5-a5ea-6bf408dba283' // Files.Read.All - Read all files user can access
    type: 'Scope'
  }
  {
    id: '863451e7-0667-486c-a5d6-d135439485f0' // Files.ReadWrite.All - Have full access to all files user can access
    type: 'Scope'
  }
  // Planner permissions
  {
    id: 'f45671fb-e0fe-4b4b-be20-3d3ce43f1bcb' // Tasks.Read - Read user tasks
    type: 'Scope'
  }
  {
    id: '2219042f-cab5-40cc-b0d2-16b1540b4c5f' // Tasks.ReadWrite - Create, read, update, and delete user tasks
    type: 'Scope'
  }
]

resource appRegistration 'Microsoft.Graph/applications@v1.0' = {
  displayName: applicationName
  signInAudience: 'AzureADMyOrg'
  
  // Configure redirect URIs for delegated authentication
  web: {
    redirectUris: [
      redirectUri
    ]
    implicitGrantSettings: {
      enableAccessTokenIssuance: false
      enableIdTokenIssuance: true
    }
  }

  // Configure public client for interactive authentication
  publicClient: {
    redirectUris: [
      redirectUri
    ]
  }
  
  // API permissions for Microsoft Graph
  requiredResourceAccess: [
    {
      resourceAppId: microsoftGraphAppId
      resourceAccess: concat(applicationPermissions, delegatedPermissions)
    }
  ]

  // Optional: Add app roles or other configurations as needed
  notes: 'App registration for MAF Assistants Microsoft Graph integration. Supports both application (app-only) and delegated (user) authentication for SharePoint, Email, Teams, OneDrive, and Planner operations.'
}

// Create service principal for the application
resource servicePrincipal 'Microsoft.Graph/servicePrincipals@v1.0' = {
  appId: appRegistration.appId
}

// Optionally create a client secret
resource clientSecret 'Microsoft.Graph/applications/passwords@v1.0' = if (createClientSecret) {
  parent: appRegistration
  displayName: '${applicationName}-secret'
  endDateTime: dateTimeAdd(utcNow(), 'P1Y') // Valid for 1 year
}

// Outputs
output applicationId string = appRegistration.appId
output objectId string = appRegistration.id
output tenantId string = subscription().tenantId
output clientSecretValue string = createClientSecret ? clientSecret.secretText : ''

// Instructions for completing the setup
output instructions string = '''
===========================================
MAF Assistants App Registration Created
===========================================

Next Steps:
1. Grant admin consent for the API permissions:
   - Go to Azure Portal > Entra ID > App registrations
   - Select "${applicationName}"
   - Go to API permissions
   - Click "Grant admin consent for [Your Organization]"

2. Update your user secrets with these values:
   {
     "MicrosoftGraphTenantId": "${subscription().tenantId}",
     "MicrosoftGraphClientId": "${appRegistration.appId}",
     "MicrosoftGraphClientSecret": "${createClientSecret ? '[SECRET_VALUE_GENERATED]' : '[CREATE_MANUALLY]'}",
     "MicrosoftGraphMaxItems": 100,
     "MicrosoftGraphUseDelegatedAuth": false,
     "MicrosoftGraphRedirectUri": "${redirectUri}"
   }

3. For delegated authentication, set:
   "MicrosoftGraphUseDelegatedAuth": true

Permissions Granted:
- SharePoint: Sites.Read.All, Sites.ReadWrite.All
- Email: Mail.Read, Mail.Send
- Teams: Team.ReadBasic.All, Channel.ReadBasic.All, ChannelMessage.Read.All, ChannelMessage.Send
- OneDrive: Files.Read.All, Files.ReadWrite.All
- Planner: Tasks.Read, Tasks.ReadWrite
- User: User.Read, User.Read.All

Both Application (app-only) and Delegated (user) permissions are configured.
===========================================
'''
