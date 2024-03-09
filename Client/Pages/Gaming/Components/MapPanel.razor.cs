using Client.Pages.Gaming.Entities;
using Microsoft.AspNetCore.Components;

namespace Client.Pages.Gaming.Components;

public partial class MapPanel
{
	//private static Block?[][] Blocks =>
	private static Block?[][] Blocks7x7 =>
	[
		[new StartPoint("Start"), new Land("A1", lot:"A"),    new Station("Station1"),    new Land("A2", lot:"A"),    new Land("A3", lot:"A"),    null,                       null],
		[new Land("F4", lot:"F"), null,                       null,                       null,                       new Land("A4", lot:"A"),    null,                       null],
		[new Station("Station4"), null,                       new Land("B5", lot:"B"),    new Land("B6", lot:"B"),    new ParkingLot("ParkingLot"),     new Land("C1", lot:"C"),    new Land("C2", lot:"C")],
		[new Land("F3", lot:"F"), null,                       new Land("B4", lot:"B"),    null,                       new Land("B1", lot:"B"),    null,                       new Land("C3", lot:"C")],
		[new Land("F2", lot:"F"), new Land("F1", lot:"F"),    new Jail("Jail"),           new Land("B3", lot:"B"),    new Land("B2", lot:"B"),    null,                       new Station("Station2")],
		[null,                    null,                       new Land("E3", lot:"E"),    null,                       null,                       null,                       new Land("D1", lot:"D")],
		[null,                    null,                       new Land("E2", lot:"E"),    new Land("E1", lot:"E"),    new Station("Station3"),    new Land("D3", lot:"D"),    new Land("D2", lot:"D")],
	];

	private static Block?[][] Blocks =>
	//private static Block?[][] Blocks5x9 =>
	[
		[new StartPoint("Start"), new Land("A1", lot:"A"),	  new Land("A2", lot:"A"),    new Station("Station1"),    new Land("A3", lot:"A"),    new Land("A4", lot:"A"),	  new Land("A5", lot:"A"),    	  null,         				  null],
		[new Land("D1", lot:"D"), null,                       null,                       null,    					  null,                   	  null,						  new Land("A6", lot:"A"),    	  null,                       	  null],
		[new Station("Station4"), null,       				  null,               		  new Land("B1", lot:"B"),    new Land("B2", lot:"B"),	  new Land("B3", lot:"B"),    new ParkingLot("ParkingLot"),   new Land("B4", lot:"B"),		  new Land("B5", lot:"B")],
		[new Land("D2", lot:"D"), new Land("D3", lot:"D"),    new Land("D4", lot:"D"),    new Jail("Jail"),   		  null,    					  null,						  null,							  null,     					  new Station("Station2")],
		[null, 					  null,						  null,						  new Land("C1", lot:"C"), 	  new Land("C2", lot:"C"),    new Station("Station3"),    new Land("C3", lot:"C"),    	  new Land("C4", lot:"C"),   	  new Land("C5", lot:"C")]
	];

	public static int GetMapRow()
	{
		return Blocks.Length;
	}

	public int GetMapCol()
	{
		return Blocks[0].Length;
	}

	public int GetRoadTop(int col)
	{
		for(int i = 0; i < Blocks.Length-1 ;i++)
		{
			if (Blocks[i][col] is not null)
			{
				return i;
			}
		}
		return 0;
	}

	public int GetRoadButton(int col)
	{
		for(int i = Blocks.Length-1; i >= 0 ;i--)
		{
			if (Blocks[i][col] is not null)
			{
				return i;
			}
		}
		return 0;
	}

	public int GetRoadLeft(int row)
	{
		for(int i = 0; i < Blocks[0].Length-1 ;i++)
		{
			if (Blocks[row][i] is not null)
			{
				return i;
			}
		}
		return 0;
	}
	public int GetRoadRight(int row)
	{
		for(int i = Blocks[0].Length-1; i >= 0 ;i--)
		{
			if (Blocks[row][i] is not null)
			{
				return i;
			}
		}
		return 0;
	}

	public string GetBlockTag(int row, int col)
	{
		string cssScope = "b-9s5izdb9cc";
		int roadTop = 0;
		int roadButton = 0;
		int roadLeft = 0;
		int roadRight = 0;
		string divRoad = string.Empty;
		string imgLand = string.Empty;
		string imgRoad = string.Empty;
        switch (Blocks[row][col])
        {
            case null:
				if (col == 0)
				{
					return string.Format("<div class='roadLeft' {0}></div>", cssScope);
				}
				else
				{
                    return string.Format("<div class='empty' {0}></div>", cssScope);
                }
            case StartPoint:
                return string.Format("<div class='start' {0}></div>", cssScope);
            case ParkingLot:
                return string.Format("<div class='parking' {0}></div>", cssScope);
            case Jail:
                return string.Format("<div class='prison' {0}></div>", cssScope);
            case Station: //* 從可選擇方向判斷
				
				if(col == 0)
				{
					divRoad = "roadLeft";
					imgLand = "landV";
					imgRoad = "railV";
				}
				else if (col == Blocks[0].Length-1)
				{
					divRoad = "roadRight";
					imgLand = "landV";
					imgRoad = "railV";
				}
				else
				{
					divRoad = "roadTop";
					imgLand = "landH";
					imgRoad = "railH";
				}
                return string.Format("<div class='{1}' {0}><img class='{2}' {0}><img class='{3}' {0}></div>", cssScope, divRoad, imgLand, imgRoad);
            case Land: //* 從可選擇方向判斷
				roadTop = GetRoadTop(col);
				roadButton = GetRoadButton(col);
				roadLeft = GetRoadLeft(row);
				roadRight = GetRoadRight(row);
				if (row == 0)
				{
					divRoad = "roadTop";
					imgLand = "landH";
					if (col == roadLeft)
					{
						imgRoad = "roadTopL";
					}
					else if (col == roadRight)
					{
						imgRoad = "roadTopR";
					}
					else
					{
						imgRoad = "roadH";
					}
				}
				else if (row == Blocks.Length - 1)
				{
					divRoad = "roadTop";
					if (col == roadLeft)
					{
						divRoad = "roadLeft";
						imgLand = "landV";
						imgRoad = "roadBtmL";
					}
					else if (col == roadRight)
					{
						return string.Format("<div class='roadTop' {0}><img class='roadBtmR' {0}></div>", cssScope);
					}
					else
					{
						imgLand = "landH";
						imgRoad = "roadH";
					}
				}
				else if (col == 0)
				{
					divRoad = "roadLeft";
					imgLand = "landV";
					// if (row == 0) 前面已判斷過
					if (row == roadButton)
					{
						imgRoad = "roadBtmL";
					}
					else
					{
						imgRoad = "roadV";
					}
				}
				else if (col == Blocks[0].Length - 1)
				{
					if (row == roadTop)
					{
						divRoad = "roadTop";
						imgLand = "landH";
						imgRoad = "roadTopR";
					}
					else
					{
						divRoad = "roadRight";
						imgLand = "landV";
						imgRoad = "roadV";
					}
				}
				else
				{
					if (Blocks[row][col-1] is not null && Blocks[row][col+1] is not null)
					{
						divRoad = "roadTop";
						imgLand = "landH";
						imgRoad = "roadH";
					}
					else if (Blocks[row+1][col] is not null && Blocks[row][col+1] is not null)
					{
						divRoad = "roadTop";
						imgLand = "landH";
						imgRoad = "roadTopL";
					}
					else if (Blocks[row-1][col] is not null && Blocks[row][col-1] is not null)
					{
						divRoad = "roadRight";
						imgLand = "landV";
						imgRoad = "roadBtmR";
					}
					else if (Blocks[row-1][col] is not null && Blocks[row+1][col] is not null)
					{
						imgLand = "landV";
						imgRoad = "roadV";
						if (Blocks[row+1][col-1] is not null && Blocks[row+1][col+1] is not null)
						{
							return string.Format("<div class='roadTop' {0}><img class='roadV' {0}></div>", cssScope);
						}
						else if (col == roadLeft)
						{
							divRoad = "roadLeft";
						}
						else
						{
							divRoad = "roadRight";
                        }
					}
				}
                return string.Format("<div class='{1}' {0}><img class='{2}' {0}><img class='{3}' {0}></div>", cssScope, divRoad, imgLand, imgRoad);
        }
        return "";
	}
}

