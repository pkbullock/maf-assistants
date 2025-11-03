param accounts_az_open_ai_name string
param location string = resourceGroup().location

resource accounts_az_open_ai_name_resource 'Microsoft.CognitiveServices/accounts@2025-06-01' = {
  name: accounts_az_open_ai_name
  location: location
  sku: {
    name: 'S0'
  }
  kind: 'OpenAI'
  properties: {
    customSubDomainName: accounts_az_open_ai_name
    networkAcls: {
      defaultAction: 'Allow'
      virtualNetworkRules: []
      ipRules: []
    }
    publicNetworkAccess: 'Enabled'
  }
}

resource accounts_az_open_ai_name_gpt_4_1_mini 'Microsoft.CognitiveServices/accounts/deployments@2025-06-01' = {
  parent: accounts_az_open_ai_name_resource
  name: 'gpt-4.1-mini'
  sku: {
    name: 'GlobalStandard'
    capacity: 100
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4.1-mini'
      version: '2025-04-14'
    }
    versionUpgradeOption: 'OnceNewDefaultVersionAvailable'
    currentCapacity: 100
    raiPolicyName: 'Microsoft.DefaultV2'
  }
}

resource accounts_az_open_ai_name_gpt_4o_mini 'Microsoft.CognitiveServices/accounts/deployments@2025-06-01' = {
  parent: accounts_az_open_ai_name_resource
  name: 'gpt-4o-mini'
  sku: {
    name: 'GlobalStandard'
    capacity: 100
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4o-mini'
      version: '2024-07-18'
    }
    versionUpgradeOption: 'OnceNewDefaultVersionAvailable'
    currentCapacity: 100
    raiPolicyName: 'Microsoft.DefaultV2'
  }
}

resource accounts_az_open_ai_name_text_embedding_3_small 'Microsoft.CognitiveServices/accounts/deployments@2025-06-01' = {
  parent: accounts_az_open_ai_name_resource
  name: 'text-embedding-3-small'
  sku: {
    name: 'GlobalStandard'
    capacity: 150
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: 'text-embedding-3-small'
      version: '1'
    }
    versionUpgradeOption: 'NoAutoUpgrade'
    currentCapacity: 150
    raiPolicyName: 'Microsoft.DefaultV2'
  }
}

// Uncomment to enable Text-Embeddings-Ada-002 model deployment

// resource accounts_az_open_ai_name_Text_Embeddings_Ada_002 'Microsoft.CognitiveServices/accounts/deployments@2025-06-01' = {
//   parent: accounts_az_open_ai_name_resource
//   name: 'Text-Embeddings-Ada-002'
//   sku: {
//     name: 'Standard'
//     capacity: 120
//   }
//   properties: {
//     model: {
//       format: 'OpenAI'
//       name: 'text-embedding-ada-002'
//       version: '2'
//     }
//     versionUpgradeOption: 'OnceNewDefaultVersionAvailable'
//     currentCapacity: 120
//     raiPolicyName: 'Microsoft.Default'
//   }
// }

// Uncomment to enable Whisper model deployment

// resource accounts_az_open_ai_name_whisper 'Microsoft.CognitiveServices/accounts/deployments@2025-06-01' = {
//   parent: accounts_az_open_ai_name_resource
//   name: 'whisper'
//   sku: {
//     name: 'Standard'
//     capacity: 3
//   }
//   properties: {
//     model: {
//       format: 'OpenAI'
//       name: 'whisper'
//       version: '001'
//     }
//     versionUpgradeOption: 'OnceNewDefaultVersionAvailable'
//     currentCapacity: 3
//     raiPolicyName: 'Microsoft.DefaultV2'
//   }
// }

