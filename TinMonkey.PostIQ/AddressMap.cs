using CsvHelper.Configuration;

namespace TinMonkey.PostIQ;

internal class AddressMap : ClassMap<Address>
{
    public AddressMap()
    {
        Map(m => m.Uprn).Name("UPRN");
        Map(m => m.ClassificationCode).Name("CLASSIFICATION_CODE");
        Map(m => m.SingleLineAddress).Name("SINGLE_LINE_ADDRESS");
        Map(m => m.Organisation).Name("ORGANISATION");
        Map(m => m.SubBuilding).Name("SUB_BUILDING");
        Map(m => m.BuildingName).Name("BUILDING_NAME");
        Map(m => m.BuildingNumber).Name("BUILDING_NUMBER");
        Map(m => m.StreetName).Name("STREET_NAME");
        Map(m => m.Locality).Name("LOCALITY");
        Map(m => m.TownName).Name("TOWN_NAME");
        Map(m => m.Postcode).Name("POSTCODE");
    }
}
