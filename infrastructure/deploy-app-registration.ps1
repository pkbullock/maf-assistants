# PowerShell script to create Entra ID App Registration for MAF Assistants
# Requires Az PowerShell module: Install-Module -Name Az -AllowClobber -Scope CurrentUser

param(
    [Parameter(Mandatory=$false)]
    [string]$ApplicationName = "MAF-Assistants-GraphAPI",
    
    [Parameter(Mandatory=$false)]
    [string]$RedirectUri = "http://localhost",
    
    [Parameter(Mandatory=$false)]
    [bool]$CreateClientSecret = $true
)

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "MAF Assistants - Entra ID App Registration" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if Az module is installed
if (-not (Get-Module -ListAvailable -Name Az.Resources)) {
    Write-Host "ERROR: Az.Resources module is not installed." -ForegroundColor Red
    Write-Host "Install with: Install-Module -Name Az -AllowClobber -Scope CurrentUser" -ForegroundColor Yellow
    exit 1
}

# Import required modules
Import-Module Az.Resources
Import-Module Az.Accounts

# Connect to Azure
Write-Host "Connecting to Azure..." -ForegroundColor Yellow
try {
    $context = Get-AzContext
    if (-not $context) {
        Connect-AzAccount
        $context = Get-AzContext
    }
    Write-Host "Connected to Azure subscription: $($context.Subscription.Name)" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "ERROR: Failed to connect to Azure: $_" -ForegroundColor Red
    exit 1
}

# Get tenant ID
$tenantId = $context.Tenant.Id
Write-Host "Tenant ID: $tenantId" -ForegroundColor Cyan

# Microsoft Graph API Application ID
$msGraphAppId = "00000003-0000-0000-c000-000000000000"

Write-Host ""
Write-Host "Creating app registration..." -ForegroundColor Yellow

# Define required resource access (Microsoft Graph permissions)
$requiredResourceAccess = @{
    ResourceAppId = $msGraphAppId
    ResourceAccess = @(
        # Application permissions
        @{ Id = "df021288-bdef-4463-88db-98f22de89214"; Type = "Role" }  # User.Read.All
        @{ Id = "9492366f-7969-46a4-8d15-ed1a20078fff"; Type = "Role" }  # Sites.Read.All
        @{ Id = "0c0bf378-bf22-4481-8f81-9e89a9b4960a"; Type = "Role" }  # Sites.ReadWrite.All
        @{ Id = "810c84a8-4a9e-49e6-bf7d-12d183f40d01"; Type = "Role" }  # Mail.Read
        @{ Id = "e2a3a72e-5f79-4c64-b1b1-878b674786c9"; Type = "Role" }  # Mail.Send
        @{ Id = "660b7406-55f1-41ca-a0ed-0b035e182f3e"; Type = "Role" }  # Team.ReadBasic.All
        @{ Id = "59a6b24b-4225-4393-8165-ebaec5f55d7a"; Type = "Role" }  # Channel.ReadBasic.All
        @{ Id = "7b2449af-6ccd-4f4d-9f78-e550c193f0d1"; Type = "Role" }  # ChannelMessage.Read.All
        @{ Id = "4d02b0cc-d90b-441f-8d82-4fb55c34d6bb"; Type = "Role" }  # ChannelMessage.Send
        @{ Id = "01d4889c-1287-42c6-ac1f-5d1e02578ef6"; Type = "Role" }  # Files.Read.All
        @{ Id = "75359482-378d-4052-8f01-80520e7db3cd"; Type = "Role" }  # Files.ReadWrite.All
        @{ Id = "913e9c1b-3c8a-4e47-a739-b66df0f6f2a6"; Type = "Role" }  # Tasks.Read
        @{ Id = "b1b3e0c7-9a6f-4ce0-832e-8e7b0f2e3f0c"; Type = "Role" }  # Tasks.ReadWrite
        # Delegated permissions
        @{ Id = "e1fe6dd8-ba31-4d61-89e7-88639da4683d"; Type = "Scope" } # User.Read
        @{ Id = "2cfdc887-d7b4-4798-9b33-3d98d6b95dd2"; Type = "Scope" } # Sites.Read.All
        @{ Id = "89fe6a52-be36-487e-b7d8-d061c450a026"; Type = "Scope" } # Sites.ReadWrite.All
        @{ Id = "570282fd-fa5c-430d-a7fd-fc8dc98a9dca"; Type = "Scope" } # Mail.Read
        @{ Id = "e383f46e-2787-4529-855e-0e479a3ffac0"; Type = "Scope" } # Mail.Send
        @{ Id = "767156cb-16ae-4d10-8f8b-41b657c8c8c8"; Type = "Scope" } # ChannelMessage.Send
        @{ Id = "df85f4d6-205c-4ac5-a5ea-6bf408dba283"; Type = "Scope" } # Files.Read.All
        @{ Id = "863451e7-0667-486c-a5d6-d135439485f0"; Type = "Scope" } # Files.ReadWrite.All
        @{ Id = "f45671fb-e0fe-4b4b-be20-3d3ce43f1bcb"; Type = "Scope" } # Tasks.Read
        @{ Id = "2219042f-cab5-40cc-b0d2-16b1540b4c5f"; Type = "Scope" } # Tasks.ReadWrite
    )
}

# Try to use New-AzADApplication (newer approach)
try {
    Write-Host "Using Az.Resources module to create app registration..." -ForegroundColor Yellow
    
    $app = New-AzADApplication `
        -DisplayName $ApplicationName `
        -SignInAudience "AzureADMyOrg" `
        -Web @{ RedirectUris = @($RedirectUri); ImplicitGrantSettings = @{ EnableIdTokenIssuance = $true } } `
        -RequiredResourceAccess $requiredResourceAccess
    
    $appId = $app.AppId
    $objectId = $app.Id
    
    Write-Host "App registration created successfully!" -ForegroundColor Green
    Write-Host "Application ID: $appId" -ForegroundColor Cyan
    Write-Host "Object ID: $objectId" -ForegroundColor Cyan
    
} catch {
    Write-Host "ERROR: Failed to create app registration: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please use the Azure Portal or Azure CLI instead." -ForegroundColor Yellow
    Write-Host "See README.md for alternative deployment methods." -ForegroundColor Yellow
    exit 1
}

# Create service principal
Write-Host ""
Write-Host "Creating service principal..." -ForegroundColor Yellow
try {
    $sp = New-AzADServicePrincipal -ApplicationId $appId
    Write-Host "Service principal created successfully!" -ForegroundColor Green
} catch {
    Write-Host "WARNING: Failed to create service principal: $_" -ForegroundColor Yellow
    Write-Host "You may need to create it manually in Azure Portal." -ForegroundColor Yellow
}

# Create client secret if requested
$clientSecret = ""
if ($CreateClientSecret) {
    Write-Host ""
    Write-Host "Creating client secret..." -ForegroundColor Yellow
    try {
        $endDate = (Get-Date).AddYears(1)
        $passwordCred = New-AzADAppCredential -ApplicationId $appId -EndDate $endDate
        $clientSecret = $passwordCred.SecretText
        Write-Host "Client secret created successfully (valid for 1 year)!" -ForegroundColor Green
    } catch {
        Write-Host "WARNING: Failed to create client secret: $_" -ForegroundColor Yellow
        Write-Host "You can create it manually in Azure Portal under Certificates & secrets." -ForegroundColor Yellow
    }
}

# Display results
Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "Deployment Complete!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Grant Admin Consent:" -ForegroundColor Yellow
Write-Host "   - Go to Azure Portal > Entra ID > App registrations" -ForegroundColor White
Write-Host "   - Select '$ApplicationName'" -ForegroundColor White
Write-Host "   - Go to API permissions" -ForegroundColor White
Write-Host "   - Click 'Grant admin consent for [Your Organization]'" -ForegroundColor White
Write-Host ""
Write-Host "2. Configure User Secrets:" -ForegroundColor Yellow
Write-Host "   Run these commands in your project directory:" -ForegroundColor White
Write-Host ""
Write-Host "   cd MAF.Assistants.Runners" -ForegroundColor Gray
Write-Host "   dotnet user-secrets set `"MicrosoftGraphTenantId`" `"$tenantId`"" -ForegroundColor Gray
Write-Host "   dotnet user-secrets set `"MicrosoftGraphClientId`" `"$appId`"" -ForegroundColor Gray

if ($clientSecret) {
    Write-Host "   dotnet user-secrets set `"MicrosoftGraphClientSecret`" `"$clientSecret`"" -ForegroundColor Gray
    Write-Host ""
    Write-Host "   IMPORTANT: Save this client secret now! It won't be shown again:" -ForegroundColor Red
    Write-Host "   Client Secret: $clientSecret" -ForegroundColor Yellow
} else {
    Write-Host "   # Create client secret manually and then run:" -ForegroundColor Gray
    Write-Host "   dotnet user-secrets set `"MicrosoftGraphClientSecret`" `"your-secret-here`"" -ForegroundColor Gray
}

Write-Host ""
Write-Host "   dotnet user-secrets set `"MicrosoftGraphMaxItems`" `"100`"" -ForegroundColor Gray
Write-Host "   dotnet user-secrets set `"MicrosoftGraphUseDelegatedAuth`" `"false`"" -ForegroundColor Gray
Write-Host "   dotnet user-secrets set `"MicrosoftGraphRedirectUri`" `"$RedirectUri`"" -ForegroundColor Gray
Write-Host ""
Write-Host "Configuration Values:" -ForegroundColor Yellow
Write-Host "  Tenant ID: $tenantId" -ForegroundColor Cyan
Write-Host "  Client ID: $appId" -ForegroundColor Cyan
Write-Host "  Redirect URI: $RedirectUri" -ForegroundColor Cyan
Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
