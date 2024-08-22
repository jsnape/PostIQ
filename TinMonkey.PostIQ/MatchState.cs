using System.Collections.Immutable;

namespace TinMonkey.PostIQ;

internal partial record MatchState
{
    public required ImmutableArray<Address> Candidates { get; init; }

    public required ImmutableArray<string> UnmatchedParts { get; init; }

    public string? LastMatchedCandidate { get; init; }

    public string? LastMatchedPart { get; init; }

    public bool IsMatch => Candidates.Length > 0;
}
