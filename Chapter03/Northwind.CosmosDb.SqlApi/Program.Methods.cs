//CosmosClient, DatabaseResponse, Database, IndexingPolicy, and so on
using Microsoft.Azure.Cosmos;
using System.Net; //HttpStatusCode

// to use Azure Comsos DB in the local emulator
partial class Program
{
    private static string endpointUri = "https://localhost:8081/";
    private static string primaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    /* 
    // to use Azure Cosmos DB in the cloud
    private static string account = "apps-services-net7"; // use your account
    private static string endpointUri = 
    $"https://{account}.documents.azure.com:443/";
    private static string primaryKey = "LGrx7H...gZw=="; // use your key
    */
    static async Task CreateCosmosResources()
    {
        SectionTitle("Creating Comsos resources");
        try
        {
            using (CosmosClient client =
             new(accountEndpoint: endpointUri, authKeyOrResourceToken: primaryKey))
            {
                DatabaseResponse dbResponse =
                    await client.CreateDatabaseIfNotExistsAsync("Northwind", throughput: 400 /* RU/s */);
                string status = dbResponse.StatusCode switch
                {
                    HttpStatusCode.OK => "exists",
                    HttpStatusCode.Created => "created",
                    _ => "unknown"
                };
                WriteLine("Database Id: {0}, Status: {1}.", arg0: dbResponse.Database.Id, arg1: status);
                IndexingPolicy indexingPolicy = new()
                {
                    IndexingMode = IndexingMode.Consistent,
                    Automatic = true, //items are indexed unless explicitly excluded
                    IncludedPaths = { new IncludedPath { Path = "/*" } }
                };

                ContainerProperties containerProperties = new("Products", partitionKeyPath: "/productId")
                {
                    IndexingPolicy = indexingPolicy
                };

                ContainerResponse containerResponse =
                    await dbResponse.Database.CreateContainerIfNotExistsAsync(containerProperties, throughput: 1000 /* RU/s */);

                status = dbResponse.StatusCode switch
                {
                    HttpStatusCode.OK => "exists",
                    HttpStatusCode.Created => "created",
                    _ => "unknown"
                };
                WriteLine("Container Id: {0}, Status: {1}", arg0: containerResponse.Container.Id, arg1: status);
                Container container = containerResponse.Container;
                ContainerProperties properties = await container.ReadContainerAsync();
                WriteLine($"PartitionKeyPath: {properties.PartitionKeyPath}");
                WriteLine($"LastModified: {properties.LastModified}");
                WriteLine($"IndexingPolicy: {properties.IndexingPolicy}");

            }
        }
        catch (HttpRequestException ex)
        {
            WriteLine("Error: {0}", arg0: ex.Message);
            WriteLine("Hint: Make sure the Azure Cosmos Emulator is running.");

        }
        catch (Exception ex)
        {
            WriteLine("Error: {0} says {1}", arg0: ex.GetType(), arg1: ex.Message);
        }
    }

}
