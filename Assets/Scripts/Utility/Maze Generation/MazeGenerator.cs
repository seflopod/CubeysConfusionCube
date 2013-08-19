using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator
{
	private readonly int ROWS = 8;
	private readonly int COLS = 8;
	private readonly int LAYS = 8;
	
	private Maze _maze;
	private Stack<MazeCell> _cellStack;
	private MazeCell _startCell;
	private MazeCell _endCell;
	private MazeData _data;
	private MazeCellData _mcData;
	
	public MazeGenerator(MazeData data, MazeCellData mcData)
	{
		_data = data;
		_mcData = mcData;
		ROWS = _data.numberOfRows;
		COLS = _data.numberOfColumns;
		LAYS = _data.numberOfLayers;
		
		
		_maze = new Maze(ROWS, COLS, LAYS, mcData);
		
		_cellStack = new Stack<MazeCell>();
	}
	
	public List<SpawnGroup> GenerateMaze()
	{
		CreateMaze();
		GameObject mazeGO = (GameObject)GameObject.Instantiate(_data.mazePrefab, Vector3.zero, Quaternion.identity);
		string frmtName = "L{0}/R{1}/C{2}";
		Dictionary<int, List<Transform>> cellByWalls = new Dictionary<int, List<Transform>>();
		for(int i=1;i<6;++i)
			cellByWalls.Add(i, (new List<Transform>()));
		List<SpawnGroup> spawnGroups = new List<SpawnGroup>(3);
		for(int i=0;i<3;++i)
			spawnGroups.Add(new SpawnGroup());
		
		//This uses Find heavily, so as a result it only makes sense to generate
		//the maze when loading.  It might not even make sense to do it this way
		//regardless
		for(int l=0;l<LAYS;++l)
		{
			for(int r=0;r<ROWS;++r)
			{
				for(int c=0;c<COLS;++c)
				{
					MazeCell cell = _maze.CellAt(r,c,l);
					GameObject cellGO = mazeGO.transform.FindChild(string.Format(frmtName,l,r,c)).gameObject;
					ApplyCellMaterials container = cellGO.GetComponent<ApplyCellMaterials>();
					MazeCellWallData tmp = new MazeCellWallData();
					tmp.materials = new List<Material>();
					tmp.materials.AddRange(_mcData.defaultMaterials);
					int n = 0;
					
					//Right
					tmp.active = cell.IsWallEnabled(MazeCell.WallSides.Right);
					if(tmp.active) { ++n; }
					container.rightWall = tmp;
					
					//Left
					tmp = new MazeCellWallData();
					tmp.materials = new List<Material>();
					tmp.materials.AddRange(_mcData.defaultMaterials);
					tmp.active = cell.IsWallEnabled(MazeCell.WallSides.Left);
					if(tmp.active) { ++n; }
					container.leftWall = tmp;
					
					//Top
					tmp = new MazeCellWallData();
					tmp.materials = new List<Material>();
					tmp.materials.AddRange(_mcData.defaultMaterials);
					tmp.active = cell.IsWallEnabled(MazeCell.WallSides.Top);
					if(tmp.active) { ++n; }
					container.topWall = tmp;
					
					//Bottom
					tmp = new MazeCellWallData();
					tmp.materials = new List<Material>();
					tmp.materials.AddRange(_mcData.defaultMaterials);
					tmp.active = cell.IsWallEnabled(MazeCell.WallSides.Bottom);
					if(tmp.active) { ++n; }
					container.bottomWall = tmp;
					
					//Front
					tmp = new MazeCellWallData();
					tmp.materials = new List<Material>();
					tmp.materials.AddRange(_mcData.defaultMaterials);
					tmp.active = cell.IsWallEnabled(MazeCell.WallSides.Front);
					if(tmp.active) { ++n; }
					container.frontWall = tmp;
					
					//Back
					tmp = new MazeCellWallData();
					tmp.materials = new List<Material>();
					tmp.materials.AddRange(_mcData.defaultMaterials);
					tmp.active = cell.IsWallEnabled(MazeCell.WallSides.Back);
					if(tmp.active) { ++n; }
					container.backWall = tmp;
					
					container.Apply();
					
					if(!container.topWall.active && container.bottomWall.active)
					{
						Transform btm = cellGO.transform.FindChild("Bottom");
						GameObject.Instantiate(_data.elevatorTriggerPrefab, btm.position, Quaternion.identity);
					}
					
					if(cell == _endCell)
					{
						AudioSource src = cellGO.AddComponent<AudioSource>();
						src.clip = _data.endCellSound;
						src.spread = 0.0f;
						src.rolloffMode = AudioRolloffMode.Linear;
						src.maxDistance = 32.0f;
						src.loop = true;
						src.Play();
					}
					cellByWalls[n].Add(cellGO.transform);
				}
			}
		}
		
		//this is just to disable any duplicate walls
		//I do it here for the sake of an easy fix rather than
		//mess with the loop above.
		for(int l=0;l<LAYS;++l)
		{
			for(int r=0;r<ROWS;++r)
			{
				for(int c=0;c<COLS;++c)
				{
					GameObject cellGO = mazeGO.transform.FindChild(string.Format(frmtName,l,r,c)).gameObject;
					ApplyCellMaterials container = cellGO.GetComponent<ApplyCellMaterials>();
					if(l != 0)
						container.bottomWall.active = false;
					if(r != 0)
						container.backWall.active = false;
					if(c != 0)
						container.leftWall.active = false;
					container.Apply();
				}
			}
		}
		
		
		//now that we've created the maze, we need to figure out where pickups will spawn
		int totalSpawns = 0;
		int nWalls = 5;
		int idxCntr = 0;
		while(totalSpawns < spawnGroups.Count*ROWS*COLS)
		{
			int nCells = cellByWalls[nWalls].Count;
			while(nCells == 0)
			{
				--nWalls;
				nCells = cellByWalls[nWalls].Count;
			}
			int cellIdx = Random.Range(0, nCells);
			spawnGroups[idxCntr%spawnGroups.Count].spawnPoints.Add(cellByWalls[nWalls][cellIdx].transform.position);
			cellByWalls[nWalls].RemoveAt(cellIdx);
			++idxCntr;
			++totalSpawns;
		}
		return spawnGroups;
	}	
	
	private void CreateMaze()
	{
		int r = Random.Range(0, 2) * (ROWS-1);
		int c = Random.Range(0, 2) * (COLS-1);
		int l = Random.Range(0, LAYS);
		_endCell = _maze.CellAt(r,c,l);
		_startCell = _endCell;
		while(!_endCell.Equals(_startCell))
		{
			r = Random.Range(0, 2) * (ROWS-1);
			c = Random.Range(0, 2) * (COLS-1);
			l = Random.Range(0, LAYS);
			_startCell = _maze.CellAt(r,c,l);
		}
		CreateMaze(_endCell);
	}
	
	private void CreateMaze(MazeCell cell)
	{
		cell.Visited = true;
		List<MazeCell> un = UnvisitedNeighbors(cell);
		
		if(un.Count == 0)
			return;
		
		MazeCell nxt = un[Random.Range(0,un.Count)];
		cell.DisableWall(cell.OnSide(nxt));
		nxt.DisableWall(nxt.OnSide(cell));
		_cellStack.Push(cell);
		CreateMaze(nxt);
		
		if(_cellStack.Count > 0)
			CreateMaze(_cellStack.Pop());
	}
	
	private List<MazeCell> UnvisitedNeighbors(MazeCell cell)
	{
		List<MazeCell> ret = new List<MazeCell>();
		MazeCell tmp;
		
		if(cell.Row < ROWS-1)
		{
			tmp = _maze.CellAt(cell.Row + 1, cell.Column, cell.Layer);
			if(!tmp.Visited)
				ret.Add(tmp);
		}
		
		if(cell.Row > 0)
		{
			tmp = _maze.CellAt(cell.Row - 1, cell.Column, cell.Layer);
			if(!tmp.Visited)
				ret.Add(tmp);
		}
		
		if(cell.Column < COLS-1)
		{
			tmp = _maze.CellAt(cell.Row, cell.Column + 1, cell.Layer);
			if(!tmp.Visited)
				ret.Add(tmp);
		}
		
		if(cell.Column > 0)
		{
			tmp = _maze.CellAt(cell.Row, cell.Column - 1, cell.Layer);
			if(!tmp.Visited)
				ret.Add(tmp);
		}
		
		if(cell.Layer < LAYS-1)
		{
			tmp = _maze.CellAt(cell.Row, cell.Column, cell.Layer + 1);
			if(!tmp.Visited)
				ret.Add(tmp);
		}
		
		if(cell.Layer > 0)
		{
			tmp = _maze.CellAt(cell.Row, cell.Column, cell.Layer - 1);
			if(!tmp.Visited)
				ret.Add(tmp);
		}
		
		return ret;
	}
	
	public MazeCell StartCell { get { return _startCell; } }
	public MazeCell EndCell { get { return _endCell; } }
}