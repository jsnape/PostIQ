namespace TinMonkey.PostIQ.Tests;

public class BasicAddressExamples : TheoryData<string, string, string?>
{
    public BasicAddressExamples()
    {
        foreach (var (address, postcode, uprn) in TinMonkey.PostIQ.BasicAddressExamples.GetBasicAddressExamples())
        {
            Add(address, postcode, uprn);
        }
    }
}
