using Microsoft.Azure.Cosmos;

namespace TinMonkey.PostIQ;

public class VectorMatcher(OpenAIService OpenAIService, CosmosService CosmosService)
{
    public async Task<string?> MatchAsync(string address)
    {
        var embeddings = await OpenAIService.GetEmbeddingsAsync([address]);
        var candidates = await CosmosService.QueryAsync(embeddings.Single().Item2);
        return candidates.FirstOrDefault()?.Uprn;
    }

}
