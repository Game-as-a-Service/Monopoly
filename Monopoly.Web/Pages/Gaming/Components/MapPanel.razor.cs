using Client.Pages.Gaming.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Gaming.Components;

public partial class MapPanel
{
	private static Block?[][] Blocks =>
	[
		[new StartPoint("Start"), new Land("A1", lot:"A"),    new Station("Station1"),    new Land("A2", lot:"A"),    new Land("A3", lot:"A"),    null,                       null],
		[new Land("F4", lot:"F"), null,                       null,                       null,                       new Land("A4", lot:"A"),    null,                       null],
		[new Station("Station4"), null,                       new Land("B5", lot:"B"),    new Land("B6", lot:"B"),    new ParkingLot("ParkingLot"),     new Land("C1", lot:"C"),    new Land("C2", lot:"C")],
		[new Land("F3", lot:"F"), null,                       new Land("B4", lot:"B"),    null,                       new Land("B1", lot:"B"),    null,                       new Land("C3", lot:"C")],
		[new Land("F2", lot:"F"), new Land("F1", lot:"F"),    new Jail("Jail"),           new Land("B3", lot:"B"),    new Land("B2", lot:"B"),    null,                       new Station("Station2")],
		[null,                    null,                       new Land("E3", lot:"E"),    null,                       null,                       null,                       new Land("D1", lot:"D")],
		[null,                    null,                       new Land("E2", lot:"E"),    new Land("E1", lot:"E"),    new Station("Station3"),    new Land("D3", lot:"D"),    new Land("D2", lot:"D")],
	];

	public static int GetMapRow()
	{
		return Blocks.Length;
	}

	public int GetMapCol()
	{
		return Blocks[0].Length;
	}

	public string GetBlockTag(int row, int col)
	{
        switch (Blocks[row][col])
        {
            case null:
				if (col == 0)
				{
					return "<div class=\"roadLeft\"></div>";
				}
				else
				{
                    return "<div class=\"empty\"></div>";
                }
            case StartPoint:
                return "<div class=\"start\"></div>";
            case ParkingLot:
                return "<div class=\"parking\"></div>";
            case Jail:
                return "<div class=\"prison\"></div>";
            case Station: //* 從可選擇方向判斷
                return "<div></div>";
            case Land: //* 從可選擇方向判斷
                return "<div></div>";
        }
        return "";
	}
}

