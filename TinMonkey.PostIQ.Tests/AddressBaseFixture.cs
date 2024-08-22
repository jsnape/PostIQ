namespace TinMonkey.PostIQ.Tests;

public class AddressBaseFixture
{
    public AddressBaseFixture()
    {
        var addressBaseRecords = AddressBaseLoader.Load(AddressBaseLoader.AddressBaseFile);
        AddressBase = new AddressBase(addressBaseRecords);
    }

    public AddressBase AddressBase { get; }
}

[CollectionDefinition(nameof(AddressBaseCollection))]
public class AddressBaseCollection : ICollectionFixture<AddressBaseFixture>
{
}
