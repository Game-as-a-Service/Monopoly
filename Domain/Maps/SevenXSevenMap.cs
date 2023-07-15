namespace Domain.Maps;

public class SevenXSevenMap : Map
{
    public SevenXSevenMap() : base(Blocks)
    {
    }

    private static Block?[][] Blocks =>
        new Block?[][]
            {
                new Block?[] { new StartPoint("Start"), new Land("A1", lot:"A"),    new Land("Station1"),       new Land("A2", lot:"A"),    new Land("A3", lot:"A"),    null,                       null },
                new Block?[] { new Land("F4", lot:"F"), null,                       null,                       null,                       new Land("A4", lot:"A"),    null,                       null },
                new Block?[] { new Land("Station4"),    null,                       new Land("B5", lot:"B"),    new Land("B6", lot:"B"),    new ParkingLot("ParkingLot"),     new Land("C1", lot:"C"),    new Land("C2", lot:"C") },
                new Block?[] { new Land("F3", lot:"F"), null,                       new Land("B4", lot:"B"),    null,                       new Land("B1", lot:"B"),    null,                       new Land("C3", lot:"C") },
                new Block?[] { new Land("F2", lot:"F"), new Land("F1", lot:"F"),    new Jail("Jail"),           new Land("B3", lot:"B"),    new Land("B2", lot:"B"),    null,                       new Land("Station2") },
                new Block?[] { null,                    null,                       new Land("E3", lot:"E"),    null,                       null,                       null,                       new Land("D1", lot:"D") },
                new Block?[] { null,                    null,                       new Land("E2", lot:"E"),    new Land("E1", lot:"E"),    new Land("Station3"),       new Land("D3", lot:"D"),    new Land("D2", lot:"D") },
            };
}