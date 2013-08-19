public class MazeCell
{
	public enum WallSides
	{
		Right = 0,
		Left = 1,
		Top = 2,
		Bottom = 3,
		Front = 4,
		Back = 5,
		Wtf = 6
	};
	
	private MazeCellData _data;
	private bool[] _sides;
		
	public MazeCell()
	{
		_sides = new bool[]{true, true, true, true, true, true};
		_data = null;
		Visited = false;
		Row = -1;
		Column = -1;
		Layer = -1;
	}
	
	public MazeCell(MazeCellData data, int row, int col, int lay)
	{
		_sides = new bool[]{true, true, true, true, true, true};
		_data = data;
		Row = row;
		Column = col;
		Layer = lay;
		Visited = false;
	}
	
	public void DisableWall(MazeCell.WallSides sideName)
	{
		_sides[(int)sideName] = false;
	}
	
	public void EnableWall(MazeCell.WallSides sideName)
	{
		_sides[(int)sideName] = true;
	}
	
	public bool IsWallEnabled(MazeCell.WallSides sideName)
	{
		return _sides[(int)sideName];
	}
	
	public WallSides OnSide(MazeCell neighbor)
	{
		int dR = Row - neighbor.Row;
		int dC = Column - neighbor.Column;
		int dL = Layer - neighbor.Layer;
		int testSum = dR + dC + dL;
		
		WallSides ret = WallSides.Wtf;
		
		if(testSum != -1 && testSum != 1)
		{
			ret = WallSides.Wtf;
		}
		else if(dR == 1)
		{
			ret = WallSides.Back;
		}
		else if(dR == -1)
		{
			ret = WallSides.Front;
		}
		else if(dC == 1)
		{
			ret = WallSides.Left;
		}
		else if(dC == -1)
		{
			ret = WallSides.Right;
		}
		else if(dL == 1)
		{
			ret = WallSides.Bottom;
		}
		else if(dL == -1)
		{
			ret = WallSides.Top;
		}
		
		return ret;
	}
	
	public bool Equals(MazeCell other)
	{
		return (Row==other.Row && Column==other.Column && Layer==other.Layer);
	}
	
	public override string ToString()
	{
		return string.Format("({0},{1},{2}):({3},{4}),({5},{6}),({7},{8})", Row, Column, Layer, _sides[0], _sides[1], _sides[2], _sides[3], _sides[4], _sides[5]);
	}
	
	public bool Visited { get; set; }
	public int Row { get; private set; }
	public int Column { get; private set; }
	public int Layer { get; private set; }
	public MazeCellData Data { get { return _data; } }
	
}