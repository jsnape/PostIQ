using System.Diagnostics;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;

namespace TinMonkey.PostIQ;

[DebuggerDisplay("{SingleLineAddress}")]
public record Address
{
    public string Id => "address|" + Uprn;

    public required string Uprn { get; set; }

    public required string ClassificationCode { get; set; }

    public required string SingleLineAddress { get; set; }
    
    public string? Organisation { get; set; }
    
    public string? SubBuilding { get; set; }
    
    public string? BuildingName { get; set; }
    
    public string? BuildingNumber { get; set; }
    
    public string? StreetName { get; set; }
    
    public string? Locality { get; set; }
    
    public string? TownName { get; set; }
    
    public required string Postcode { get; set; }

    public float[] Vectors { get; set; } = [];
}

public record AddressWithDistance : Address
{
    public float SimilarityScore { get; set; }
}