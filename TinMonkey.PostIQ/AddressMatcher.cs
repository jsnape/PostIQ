using System.Collections.Immutable;
using Fastenshtein;

namespace TinMonkey.PostIQ;

public class AddressMatcher(IAddressLookup Lookup, int Tolerance)
{
    private static readonly char[] AddressLineSeparators = [',', '\r', '\n'];

    private static readonly Func<Address, string?>[] FieldSelectors =
    [
        // Order is important because the match iteration operates in this sequence.
        c => c.StreetName,
        c => c.BuildingName,
        c => c.BuildingNumber,
        c => c.SubBuilding,
        c => c.Organisation,
    ];

    public static string? Match(string address, IAddressLookup lookup, int Tolerance)
    {
        var matcher = new AddressMatcher(lookup, Tolerance);
        return matcher.Match(address);
    }

    public string? Match(string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            return null;
        }

        var (foundPostcode, outcode, incode) = address.TryParsePostcode();

        if (!foundPostcode)
        {
            return null;
        }

        var state = new MatchState
        {
            Candidates = Lookup
                .FindByPostcode($"{outcode} {incode}")
                .ToImmutableArray(),

            UnmatchedParts = address
                .Split(AddressLineSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .Reverse()
                .ToImmutableArray(),
        };

        if (!state.IsMatch)
        {
            return null;
        }

        // Match the postcode to remove it from the unmatched parts
        state = MatchPart(state, c => c.Postcode, Tolerance);

        // But it may need splitting
        state = SplitMatchedField(state);

        if (state.Candidates.Any(c => c.StreetName == null))
        {
            // If any addresses are missing a street name
            // we should try and match town and locality first.
            state = MatchPart(state, c => c.TownName, Tolerance);
            state = MatchPart(state, c => c.Locality, Tolerance);
        }

        foreach (var selector in FieldSelectors)
        {
            state = MatchPart(state, selector, Tolerance);

            if (!state.IsMatch)
            {
                return null;
            }

            state = SplitMatchedField(state);
        }

        if (state.IsMatch)
        {
            // There can only be one candidate left.
            return state.Candidates.Single().Uprn;
        }

        return null;
    }

    public (int, int) MatchConfidence(string address, string uprn)
    {
        if (string.IsNullOrEmpty(address))
        {
            return (int.MaxValue, int.MaxValue);
        }

        var addressBase = Lookup.FindByUprn(uprn);
        if (addressBase == null)
        {
            return (int.MaxValue, int.MaxValue);
        }

        // extract the digits from the address
        var addressDigits = new string(address.Where(char.IsDigit).ToArray());
        var addressBaseDigits = new string(addressBase.SingleLineAddress.Where(char.IsDigit).ToArray());

        var digitDistance = Levenshtein.Distance(addressDigits, addressBaseDigits);

        var addressAlphaNum = new string(address.Where(char.IsLetterOrDigit).ToArray());
        var addressBaseAlphaNum = new string(addressBase.SingleLineAddress.Where(char.IsLetterOrDigit).ToArray());

        var alphaNumDistance = Levenshtein.Distance(addressAlphaNum, addressBaseAlphaNum);

        return (digitDistance, alphaNumDistance);
    }

    internal static MatchState MatchPart(
        MatchState state, Func<Address, string?> candidateSelector, int tolerance)
    {
        var candidateValues = state.Candidates
            .Select(candidateSelector)
            .Distinct()
            .Where(s => !string.IsNullOrEmpty(s))
            .Cast<string>()
            .ToArray();

        if (candidateValues.Length == 0)
        {
            // There were no candidates to match but this is OK
            // if the set of addresses has no value in that field.
            return state with
            {
                LastMatchedCandidate = null,
                LastMatchedPart = null,
            };
        }

        var matches = candidateValues
            .SelectMany(c => state.UnmatchedParts.Select(p => (c, p, d: MatchDistance(c, p))))
            .OrderBy(m => m.d);

        var (candidate, part, distance) = matches
            .Where(m => m.d * 100 / m.c.Length <= tolerance)
            .OrderBy(m => m.d)
            .FirstOrDefault();

        if (part == null)
        {
            // There were no unmatched parts that meet the tolerance criteria
            // If there are no candidates left then we have failed to match
            // and should stop.
            return state with
            {
                LastMatchedCandidate = candidate,
                LastMatchedPart = part,

                Candidates = state.Candidates
                    .RemoveAll(c => candidateSelector(c) != candidate),
            };
        }

        return state with
        {
            LastMatchedCandidate = candidate,
            LastMatchedPart = part,

            Candidates = state.Candidates
                .RemoveAll(c => candidateSelector(c) != candidate),

            UnmatchedParts = state.UnmatchedParts
                .Remove(part)
        };
    }

    internal static MatchState SplitMatchedField(MatchState state)
    {
        if (state.LastMatchedCandidate == null || state.LastMatchedPart == null)
        {
            // Nothing matched.
            return state;
        }

        // Fix: Converting to spans saves allocations when slicing below.
        var candidate = state.LastMatchedCandidate.AsSpan();
        var matched = state.LastMatchedPart.AsSpan();

        var firstSpaceIndex = matched.IndexOf(' ');

        if (firstSpaceIndex > 0)
        {
            var leftMatched = matched[..firstSpaceIndex];
            var leftCandidate = candidate[..firstSpaceIndex];

            if (!leftMatched.SequenceEqual(leftCandidate))
            {
                state = state with
                {
                    UnmatchedParts = state.UnmatchedParts.Add(leftMatched.ToString())
                };
            }
        }

        return state;
    }

    internal static int MatchDistance(string value, string match)
    {
        // Fix: if the match is longer than the value then we are looking for a suffix
        var part = match[Math.Max(0, match.Length - value.Length)..];
        return Levenshtein.Distance(value, part);
    }
}
