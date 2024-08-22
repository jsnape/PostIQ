namespace TinMonkey.PostIQ.Tests;

[Collection(nameof(AddressBaseCollection))]
public class MatchSpecs(AddressBaseFixture Fixture)
{
    private readonly AddressMatcher target = new(Fixture.AddressBase, 50);

    [Theory]
    [ClassData(typeof(BasicAddressExamples))]
    public void BasicAddressMatching(string address, string _, string expectedUprn)
    {
        var actualUprn = target.Match(address);

        if (expectedUprn is null)
        {
            Assert.Null(actualUprn);
            return;
        }

        Assert.Equal(expectedUprn, actualUprn);

        var (digitConfidence, alphaConfidence) = target.MatchConfidence(address, actualUprn!);

        Assert.InRange(digitConfidence, 0, 2);
        Assert.InRange(alphaConfidence, 0, 20);
    }

    [Theory]
    [ClassData(typeof(BasicAddressExamples))]
    public void BasicAddressMatchingWithPostcode(string address, string postcode, string _)
    {
        var (success, outcode, incode) = address.TryParsePostcode();

        Assert.True(success);
        Assert.NotNull(outcode);
        Assert.NotNull(incode);
        Assert.Equal(postcode, $"{outcode} {incode}");
    }

    [Theory]
    [ClassData(typeof(InvalidPostcodeExamples))]
    public void BasicAddressMatchingWithInvalidPostcode(string address)
    {
        var (success, outcode, incode) = address.TryParsePostcode();

        Assert.False(success);
        Assert.Null(outcode);
        Assert.Null(incode);
    }
}