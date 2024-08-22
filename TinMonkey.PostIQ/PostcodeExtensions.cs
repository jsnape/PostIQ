using System.Text.RegularExpressions;

namespace TinMonkey.PostIQ;

public static partial class PostcodeExtensions
{
    public static bool IsPostcode(this string postcode)
    {
        return PostcodeRegex().IsMatch(postcode);
    }

    public static (bool, string?, string?) TryParsePostcode(this string address)
    {
        var match = LikelyPostcodeRegex().Match(address);

        return match.Groups.Count switch
        {
            3 => (match.Success, match.Groups[1].Value, match.Groups[2].Value),
            _ => (false, null, null)
        };
    }

    [GeneratedRegex(@"^[A-Z]{1,2}[0-9R][0-9A-Z]? [0-9][A-Z]{2}$")]
    private static partial Regex PostcodeRegex();

    [GeneratedRegex(@"^.*?([A-Z]{1,2}[0-9R][0-9A-Z]?)[ ]*([0-9][A-Z]{2}).*$", RegexOptions.Multiline)]
    private static partial Regex LikelyPostcodeRegex();
}
