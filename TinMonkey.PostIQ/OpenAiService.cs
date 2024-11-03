using System.ClientModel;
using Azure.AI.OpenAI;
using OpenAI.Embeddings;

namespace TinMonkey.PostIQ;

public class OpenAIService
{
    private const string EmbeddingDeploymentName = "text-embedding-ada-002";

    private readonly AzureOpenAIClient openAIClient;
    private readonly EmbeddingClient embeddingClient;

    public OpenAIService(string Endpoint, string Key)
    {
        this.openAIClient = new(new Uri(Endpoint), new ApiKeyCredential(Key));
        this.embeddingClient = openAIClient.GetEmbeddingClient(EmbeddingDeploymentName);
    }

    public async Task<(string, float[])[]> GetEmbeddingsAsync(IEnumerable<string> inputs)
    {
        var response = await this.embeddingClient.GenerateEmbeddingsAsync(inputs);
        var embeddings = response.Value;

        return inputs
            .Zip(embeddings, (input, embedding) => (input, embedding.ToFloats().ToArray()))
            .ToArray();
    }
}
