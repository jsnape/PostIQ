using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace TinMonkey.PostIQ;

public class CosmosService
{
    private readonly CosmosClient cosmosClient;
    private Database database;
    private Container container;

    public CosmosService(string endpoint, string key)
    {
        CosmosSerializationOptions options = new()
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        };

        this.cosmosClient = new CosmosClientBuilder(endpoint, key)
            .WithSerializerOptions(options)
            .WithBulkExecution(true)
            .WithThrottlingRetryOptions(TimeSpan.FromHours(30), 5000)
            .Build();

        this.database = cosmosClient.GetDatabase("postiq");
        this.container = this.database.GetContainer("addresses");
    }

    public async Task LoadAsync(IEnumerable<Address> addresses)
    {
        List<Task> tasks = new(addresses.Count());

        foreach (var address in addresses)
        {
            var task = this.container
                .UpsertItemAsync(address, new PartitionKey(address.Postcode))
                .ContinueWith(r =>
                {
                    if (r.IsCompletedSuccessfully)
                    {
                        Console.WriteLine($"Created item {r.Result.Resource.Uprn} with ETag {r.Result.ETag}");
                    }
                    else
                    {
                        AggregateException innerExceptions = r?.Exception?.Flatten() ?? new AggregateException();
                        if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                        {
                            Console.WriteLine($"Received {address.Uprn}: {cosmosException.StatusCode} ({cosmosException.Message})");
                        }
                        else
                        {
                            Console.WriteLine($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                        }

                    }
                });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    public async Task<IEnumerable<AddressWithDistance>> QueryAsync(float[] vectors)
    {
        List<AddressWithDistance> addresses = new();

        try
        {
            var sqlQueryText =
                $"""
                SELECT TOP 5 
                    c.uprn,
                    c.classificationCode,
                    c.singleLineAddress,
                    c.organisation,
                    c.subBuilding,
                    c.buildingName,
                    c.buildingNumber,
                    c.streetName,
                    c.locality,
                    c.townName,
                    c.postcode,
                    c.vectors,
                    VectorDistance(c.vectors, @embedding) AS similarityScore
                FROM c
                ORDER BY VectorDistance(c.vectors,@embedding)
                """;

            var queryDefinition = new QueryDefinition(sqlQueryText)
                .WithParameter("@embedding", vectors);

            using var queryResultSetIterator = this.container.GetItemQueryIterator<AddressWithDistance>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                addresses.AddRange(currentResultSet);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return addresses;
    }
}
