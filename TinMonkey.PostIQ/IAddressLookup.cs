namespace TinMonkey.PostIQ;

public interface IAddressLookup
{
    Address FindByUprn(string postcode);

    IEnumerable<Address> FindByPostcode(string postcode);
}
