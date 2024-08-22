using System.Net;
using BenchmarkDotNet.Attributes;

namespace TinMonkey.PostIQ;

[MemoryDiagnoser]
public class Benchmarks
{
    private const int MaxTolerance = 50;

    public AddressBase AddressBase { get; }

    private readonly (string, string?)[] examples;

    public Benchmarks()
    {
        var addressBaseRecords = AddressBaseLoader.Load(AddressBaseLoader.AddressBaseFile);
        AddressBase = new AddressBase(addressBaseRecords);

        examples = BasicAddressExamples
            .GetBasicAddressExamples()
            .Select(x => (x.Item1, x.Item3))
            .ToArray();
    }

    [Benchmark]
    public void FunctionalAddressMatching()
    {
        AddressMatcher matcher = new(AddressBase, MaxTolerance);

        foreach (var (address, expectedUprn) in examples)
        {
            var actualUprn = matcher.Match(address);

            if (expectedUprn != actualUprn)
            {
                throw new InvalidOperationException(
                    $"Expected {expectedUprn} but got {actualUprn}");
            }
        }
    }
}

