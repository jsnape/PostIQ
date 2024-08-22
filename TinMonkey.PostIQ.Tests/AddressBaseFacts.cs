namespace TinMonkey.PostIQ.Tests;

[Collection(nameof(AddressBaseCollection))]
public class AddressBaseFacts(AddressBaseFixture Fixture)
{
    private readonly AddressBaseFixture fixture = Fixture;

    [Fact]
    public void FindByMissingPostcodeReturnsEmptyList()
    {
        var candidates = fixture.AddressBase.FindByPostcode("AB12 3CD");
        Assert.Empty(candidates);
    }

    [Theory]
    [ClassData(typeof(BasicAddressExamples))]
    public void FindByPostcodeReturnsMatchingAddresses(string _, string postcode, string _1)
    {
        var candidates = fixture.AddressBase.FindByPostcode(postcode);
        Assert.NotEmpty(candidates);
    }
}
