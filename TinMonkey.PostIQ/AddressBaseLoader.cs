using CsvHelper;
using System.Globalization;

namespace TinMonkey.PostIQ;

public class AddressBaseLoader
{
    public const string AddressBaseFile = "addressbase-core-sample.csv";

    public static IEnumerable<Address> Load(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<AddressMap>();

        // Addresses with an 'R' classification are residential.
        return csv
            .GetRecords<Address>()
            .Where(r => r.ClassificationCode[0] == 'R')
            // Fix - sequence needs materializing before this function returns.
            .ToList();
    }
}
