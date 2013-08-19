public class Maze
{
	private int _rows;
	private int _cols;
	private int _layers;
	
	private MazeCell[][][] _cells;
	
	public Maze()
	{
		_rows = 0;
		_cols = 0;
		_layers = 0;
		_cells = null;
	}
	
	public Maze(int rows, int cols, int layers, MazeCellData mcData)
	{
		_rows = rows;
		_cols = cols;
		_layers = layers;
		_cells = new MazeCell[_rows][][];
		for(int i=0;i<_rows;++i)
		{
			_cells[i] = new MazeCell[_cols][];
			for(int j=0;j<_cols;++j)
			{
				_cells[i][j] = new MazeCell[_layers];
				for(int k=0;k<_layers;++k)
					_cells[i][j][k] = new MazeCell(mcData, i, j, k);
			}
		}
	}
	
	public MazeCell CellAt(int row, int col, int layer)
	{
		return _cells[row][col][layer];
	}
}