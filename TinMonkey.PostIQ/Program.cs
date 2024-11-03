// Read from console until enter pressed twice and process data input so far.

using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Running;
using CommandLine.Text;
using TinMonkey.PostIQ;

// Add CTRL-C break handler
Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("Exiting...");
    Environment.Exit(0);
};

var cosmosEndpoint = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT");
var cosmosKey = Environment.GetEnvironmentVariable("COSMOS_KEY");

if (string.IsNullOrEmpty(cosmosEndpoint) || string.IsNullOrEmpty(cosmosKey))
{
    Console.WriteLine("Please set the COSMOS_ENDPOINT and COSMOS_KEY environment variables.");
    return;
}

var cosmosService = new CosmosService(cosmosEndpoint, cosmosKey);

var openAIEndpoint = Environment.GetEnvironmentVariable("OPENAI_ENDPOINT");
var openAIKey = Environment.GetEnvironmentVariable("OPENAI_KEY");

if (string.IsNullOrEmpty(openAIEndpoint) || string.IsNullOrEmpty(openAIKey))
{
    Console.WriteLine("Please set the OPENAI_ENDPOINT and OPENAI_KEY environment variables.");
    return;
}

var openAIService = new OpenAIService(openAIEndpoint, openAIKey);

if (args.Length > 0 && args[0] == "bench")
{
    _ = BenchmarkRunner.Run<Benchmarks>();
}
else if (args.Length > 0 && args[0] == "load")
{
    var addressGroups = AddressBaseLoader
        .Load(AddressBaseLoader.AddressBaseFile)
        .GroupBy(x => x.Postcode);

    foreach (var group in addressGroups)
    {
        var inputs = group.Select(x => x.SingleLineAddress).ToList();
        var embeddings = await openAIService.GetEmbeddingsAsync(inputs);

        var addresses = group.Zip(embeddings, (address, embedding) =>
        {
            Debug.Assert(address.SingleLineAddress == embedding.Item1);
            return address with { Vectors = embedding.Item2 };
        });

        await cosmosService.LoadAsync(addresses);

        // Sleep to avoid rate limiting
        await Task.Delay(10000);
    }
}
else
{
    VectorMatcher matcher = new(openAIService, cosmosService);

    var examples = BasicAddressExamples
        .GetBasicAddressExamples()
        .Select(x => (x.Item1, x.Item3))
        .ToArray();

    foreach (var (address, expectedUprn) in examples)
    {
        var actualUprn = await matcher.MatchAsync(address);

        if (expectedUprn != actualUprn)
        {
            throw new InvalidOperationException(
                $"Expected {expectedUprn} but got {actualUprn}");
        }
    }


    //var benchmark = new Benchmarks();
    //benchmark.FunctionalAddressMatching();
}
