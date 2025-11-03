# PowerShell script to create Entra ID App Registration for MAF Assistants
# Requires Az PowerShell module: Install-Module -Name Az -AllowClobber -Scope CurrentUser
# Created majority of the file with GitHub Copilot
# Validated and Refined by Paul Bullock 2nd Nov 2025

param(
    [string]$InstanceName = "open-ai-instance-486",
    [string]$ResourceGroupName = "rgAzureAI",
    [string]$Location = "westeurope",
    [string]$templateFile = ".\azure-open-ai.bicep"
)

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "MAF Assistants - Azure OpenAI Instance" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

#--------------------------------------------------------

# Check if Az module is installed
if (-not (Get-Module -ListAvailable -Name Az.Resources)) {
    Write-Host "ERROR: Az.Resources module is not installed." -ForegroundColor Red
    Write-Host "Install with: Install-Module -Name Az -AllowClobber -Scope CurrentUser" -ForegroundColor Yellow
    exit 1
}

# Import required modules
Import-Module Az.Resources

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

# Check if a resource group exists, create if not
$rg = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
if (-not $rg) {
    Write-Host "Creating resource group '$ResourceGroupName' in location '$Location'..." -ForegroundColor Yellow
    try {
        $rg = New-AzResourceGroup -Name $ResourceGroupName -Location $Location
        Write-Host "Resource group '$ResourceGroupName' created." -ForegroundColor Green
    } catch {
        Write-Host "ERROR: Failed to create resource group: $_" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Using existing resource group '$ResourceGroupName'." -ForegroundColor Green
}

# Deploy Azure OpenAI Instance
Write-Host ""
Write-Host "Deploying Resources '$InstanceName'..." -ForegroundColor Yellow

try {

    $deploymentTemplateParams = @{
        accounts_az_open_ai_name = $InstanceName
    }

    $deployment = New-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupName `
        -TemplateFile $templateFile `
        -TemplateParameterObject $deploymentTemplateParams `
        -Verbose

    if ($deployment.ProvisioningState -eq "Succeeded") {
        Write-Host "Resource '$InstanceName' deployed successfully." -ForegroundColor Green
    } else {
        Write-Host "ERROR: Deployment failed with state: $($deployment.ProvisioningState)" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "ERROR: Failed to deploy Resources in Specified Bicep File: $_" -ForegroundColor Red
    exit 1
}