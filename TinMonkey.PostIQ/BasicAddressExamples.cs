namespace TinMonkey.PostIQ;

public class BasicAddressExamples
{
    public static IEnumerable<(string, string, string?)> GetBasicAddressExamples()
    {
        yield return (
            "2A NEWTOWN CLOSE, EXETER, EX1 2EU",
            "EX1 2EU",
            "10013045896"
        );
        // Spelling errors
        yield return (
            "2A NEWTWN CLOSE, EXETER, EX1 2EU",
            "EX1 2EU",
            "10013045896"
        );
        yield return (
            "2A NEWTOWN CLOSE\nEXETER\nEX1 2EU",
            "EX1 2EU",
            "10013045896"
        );
        yield return (
            "2A NEWTOWN CLOSE\nEXETER EX12EU",
            "EX1 2EU",
            "10013045896"
        );
        yield return (
            "FLAT 6, GERBERA HOUSE, MEDLEY COURT, EXETER, EX4 2QF",
            "EX4 2QF",
            "10013045868"
        );
        yield return (
            "FLAT 11, BARTS MEWS, 53 BARTHOLOMEW STREET WEST, EXETER, EX4 3AJ",
            "EX4 3AJ",
            "10013036113"
        );
        // Abbreviations
        yield return (
            "FLAT 11, BARTS MEWS, 53 BARTSOLOMEW ST W, EXETER, EX4 3AJ",
            "EX4 3AJ",
            "10013036113"
        );
        // Abbreviations but out of order
        yield return (
            "FLAT 11, 53 BARTSOLOMEW ST W, BARTS MEWS, EXETER, EX4 3AJ",
            "EX4 3AJ",
            "10013036113"
        ); yield return (
            "ORCHARD HOUSE, IDE LANE, ALPHINGTON, EXETER, EX2 8UT",
            "EX2 8UT",
            "10013040942"
        );
        yield return (
            "19, PEGASUS COURT, NORTH STREET, HEAVITREE, EXETER, EX1 2RP",
            "EX1 2RP",
            "10013041164"
        );
        yield return (
            @"FLAT 2B, ANGEL PAVEMENT, 145-147 FORE STREET,
            ST DAVIDS, EXETER, EX4 3AN",
            "EX4 3AN",
            "10013042138"
        );
        yield return (
            @"D M A ENGINEERING, 1, GERALD DINNIS UNITS,
            COFTON ROAD, MARSH BARTON TRADING ESTATE
            EXETER, EX2 8QW",
            "EX2 8QW",
            "10013042292"
        );
        // Many streets per postcode (part 1)
        yield return (
            @"14A BLACKBOY ROAD
            EXETER, EX4 6SN",
            "EX4 6SN",
            "10013041607"
        );
        // Many streets per postcode (part 2)
        yield return (
            @"14A SPINNING PATH
            EXETER, EX4 6SN",
            "EX4 6SN",
            "10013046058"
        );
        // Incorrect street
        yield return (
            @"221B Baker Street
            EXETER, EX4 6SN",
            "EX4 6SN",
            null
        );
        // No street in valid address
        yield return (
            @"THE FLAT, PORT ROYAL INN
            EXETER, EX2 4DR",
            "EX2 4DR",
            "100041131456"
        );
    }
}
