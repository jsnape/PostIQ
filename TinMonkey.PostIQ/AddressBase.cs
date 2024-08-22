namespace TinMonkey.PostIQ;

public class AddressBase(IEnumerable<Address> Addresses) : IAddressLookup
{
    private readonly ILookup<string, Address> postcodes = 
        Addresses.ToLookup(x => x.Postcode.ToUpperInvariant());

    private readonly IDictionary<string, Address> uprns =
        Addresses.ToDictionary(x => x.Uprn.ToUpperInvariant());

    public Address FindByUprn(string uprn)
    {
        return this.uprns[uprn.ToUpperInvariant()];
    }

    public IEnumerable<Address> FindByPostcode(string postcode)
    {
        return this.postcodes[postcode.ToUpperInvariant()];
    }
}
