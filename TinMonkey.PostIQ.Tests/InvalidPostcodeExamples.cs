namespace TinMonkey.PostIQ.Tests;

public class InvalidPostcodeExamples : TheoryData<string>
{
    public InvalidPostcodeExamples()
    {
        Add("2A NEWTOWN CLOSE, EXETER, EX1");
        Add("2A NEWTOWN CLOSE\nEXETER\n2EU");
        Add("FLAT 11, BARTS MEWS, EX4 BARTHOLOMEW STREET WEST, EXETER, 3AJ");
        Add("19, PEGASUS COURT, NORTH STREET, HEAVITREE, EXETER, 2RP EX1");
        Add("19, PEGASUS COURT, NORTH STREET, HEAVITREE, EXETER, EX1,2RP");
        Add("19, PEGASUS COURT, NORTH STREET, HEAVITREE, EXETER, EX1\n2RP");
        Add(@"FLAT 3AN, ANGEL PAVEMENT, 145-147 FORE STREET,
            ST DAVIDS, EXETER, EX4");
        Add(@"E X 2 8QW ENGINEERING, 1, GERALD DINNIS UNITS,
            COFTON ROAD, MARSH BARTON TRADING ESTATE
            EXETER");
    }
}
