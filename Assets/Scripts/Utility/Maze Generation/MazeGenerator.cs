using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator
{
	private readonly int ROWS = 8;
	private readonly int COLS = 8;
	private readonly int LAYS = 8;
	private readonly string FIND_CELL_STR = "L{0}/R{1}/C{2}";
	
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
		createLogicalMaze();
		GameObject mazeGO = (GameObject)GameObject.Instantiate(_data.mazePrefab, Vector3.zero, Quaternion.identity);

		Dictionary<int, List<Transform>> cellByWalls = new Dictionary<int, List<Transform>>();

		for(int i=1;i<6;++i)
		{
			cellByWalls.Add(i, (new List<Transform>()));
		}
				
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
					GameObject cellGO = mazeGO.transform.FindChild(string.Format(FIND_CELL_STR,l,r,c)).gameObject;
					int n = 0;
					bool needElevator = false;

					GameObject tmpGO = cellGO.transform.FindChild("Back").gameObject;
					tmpGO.SetActive(cell.IsWallEnabled(MazeCell.WallSides.Back));
					if(tmpGO.activeSelf)
					{
						++n;
						tmpGO.GetComponent<MeshRenderer>().materials = _mcData.defaultMaterials;
					}

					tmpGO = cellGO.transform.FindChild("Front").gameObject;
					tmpGO.SetActive(cell.IsWallEnabled(MazeCell.WallSides.Front));
					if(tmpGO.activeSelf)
					{
						++n;
						tmpGO.GetComponent<MeshRenderer>().materials = _mcData.defaultMaterials;
					}

					tmpGO = cellGO.transform.FindChild("Right").gameObject;
					tmpGO.SetActive(cell.IsWallEnabled(MazeCell.WallSides.Right));
					if(tmpGO.activeSelf)
					{
						++n;
						tmpGO.GetComponent<MeshRenderer>().materials = _mcData.defaultMaterials;
					}

					tmpGO = cellGO.transform.FindChild("Left").gameObject;
					tmpGO.SetActive(cell.IsWallEnabled(MazeCell.WallSides.Left));
					if(tmpGO.activeSelf)
					{
						++n;
						tmpGO.GetComponent<MeshRenderer>().materials = _mcData.defaultMaterials;
					}

					tmpGO = cellGO.transform.FindChild("Top").gameObject;
					tmpGO.SetActive(cell.IsWallEnabled(MazeCell.WallSides.Top));
					if(tmpGO.activeSelf)
					{
						++n;
						tmpGO.GetComponent<MeshRenderer>().materials = _mcData.defaultMaterials;
					}
					else
					{
						needElevator = true;
					}

					tmpGO = cellGO.transform.FindChild("Bottom").gameObject;
					tmpGO.SetActive(cell.IsWallEnabled(MazeCell.WallSides.Bottom));
					if(tmpGO.activeSelf)
					{
						++n;
						tmpGO.GetComponent<MeshRenderer>().materials = _mcData.defaultMaterials;
						if(needElevator)
						{
							Transform btm = tmpGO.transform;
							GameObject.Instantiate(_data.elevatorTriggerPrefab, btm.position, Quaternion.identity);
						}
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

		disableDuplicateWalls(mazeGO);

		return createSpawnGroups(cellByWalls);
	}

	//we need to figure out where pickups will spawn
	private List<SpawnGroup> createSpawnGroups(Dictionary<int, List<Transform>> cellByWalls)
	{
		List<SpawnGroup> spawnGroups = new List<SpawnGroup>(3);
		for(int i=0;i<3;++i)
		{
			spawnGroups.Add(new SpawnGroup());
		}

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

	//this is just to disable any duplicate walls
	//I do it here for the sake of an easy fix rather than
	//mess with the loop above.
	private void disableDuplicateWalls(GameObject mazeGO)
	{
		for(int l=0;l<LAYS;++l)
		{
			for(int r=0;r<ROWS;++r)
			{
				for(int c=0;c<COLS;++c)
				{
					GameObject cellGO = mazeGO.transform.FindChild(string.Format(FIND_CELL_STR,l,r,c)).gameObject;
					if(l != LAYS-1)
					{
						cellGO.transform.FindChild("Top").gameObject.SetActive(false);
					}
					if(r != ROWS-1)
					{
						cellGO.transform.FindChild("Front").gameObject.SetActive(false);
					}
					if(c != COLS-1)
					{
						cellGO.transform.FindChild("Right").gameObject.SetActive(false);
					}
				}
			}
		}
	}

	private void createLogicalMaze()
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
		createLogicalMaze(_endCell);
	}
	
	private void createLogicalMaze(MazeCell cell)
	{
		cell.Visited = true;
		List<MazeCell> un = unvisitedNeighbors(cell);
		
		if(un.Count == 0)
			return;
		
		MazeCell nxt = un[Random.Range(0,un.Count)];
		cell.DisableWall(cell.OnSide(nxt));
		nxt.DisableWall(nxt.OnSide(cell));
		_cellStack.Push(cell);
		createLogicalMaze(nxt);
		
		if(_cellStack.Count > 0)
			createLogicalMaze(_cellStack.Pop());
	}
	
	private List<MazeCell> unvisitedNeighbors(MazeCell cell)
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