using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using azure_exam.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        var builtConfig = config.Build();
        var keyVaultUrl = builtConfig["VaultUri"];

        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

            var cosmosDbConnectionString = secretClient.GetSecret("CosmosDb").Value.Value;
            var cosmosDbDatabaseName = secretClient.GetSecret("CosmosDb--DatabaseName").Value.Value;
            var cosmosDbContainerName = secretClient.GetSecret("CosmosDb--ContainerName").Value.Value;

            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "CosmosDb", cosmosDbConnectionString },
                { "CosmosDb:DatabaseName", cosmosDbDatabaseName },
                { "CosmosDb:ContainerName", cosmosDbContainerName }
            });
        }
    })
 .ConfigureServices((context, services) =>
 {
     var configuration = context.Configuration;
     var cosmosDbConnectionString = configuration["CosmosDb"];
     var cosmosDbDatabaseName = configuration["CosmosDb:DatabaseName"];
     var cosmosDbContainerName = configuration["CosmosDb:ContainerName"];

     services.AddSingleton(sp =>
     {
         var cosmosClient = new CosmosClient(cosmosDbConnectionString);
         return new CosmosDbService(cosmosClient, configuration);
     });
 })
    .Build();

host.Run();
