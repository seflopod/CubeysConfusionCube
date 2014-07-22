using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeManager
{
	private MazeData _data;
	private GameObject _mazeGameObject;
	private int _curSG;
	private int _prvSG;
	private int _curPO;
	private int _prvPO;
	
	private List<PickupBehaviour> _pickups;
	private List<SpawnGroup> _spawnGroups;
	private MazeCell _startCell;
	private MazeCell _endCell;
	
	public MazeManager()
	{
		_data = null;
		_mazeGameObject = null;
		_curSG = 0;
		_prvSG = 0;
		_curPO = 0;
		_prvPO = 0;
		_pickups = new List<PickupBehaviour>();
		CanDoPickup = false;
	}
	
	public void Init(MazeData data, MazeCellData mcData)
	{
		if(data == null)
			_data = new MazeData();
		else
			_data = data;
		
		MazeGenerator mg = new MazeGenerator(data, mcData);
		_spawnGroups = mg.GenerateMaze();
		_startCell = mg.StartCell;
		_endCell = mg.EndCell;
		CanDoPickup = false;
		_mazeGameObject = GameObject.FindGameObjectWithTag("MazeParent");
	}
	
	private void SelectNewSpawnGroup()
	{
		_prvSG = _curSG;
		while(_curSG == _prvSG || _spawnGroups[_curSG].spawnPoints.Count == 0)
		{
			_curSG = Random.Range(0, _spawnGroups.Count);
		}
		
		_prvPO = _curPO;
		while(_curPO == _prvPO)
		{
			_curPO = Random.Range(0, _spawnGroups.Count);
		}
	}
	
	private void SpawnNewPickups()
	{
		List<Vector3> points = _spawnGroups[_curSG].spawnPoints;
		int[] pointsIdx = new int[points.Count/2+1];
		for(int i=0;i<pointsIdx.Length;++i)
		{
			pointsIdx[i] = Random.Range(0, points.Count);
			bool canUse = true;
			for(int j=0;j<i;++j)
			{
				canUse = pointsIdx[j] != pointsIdx[i];
				if(!canUse)
				{
					break;
				}
			}
			if(!canUse)
			{
				i--;
			}
		}
		for(int i=0;i<pointsIdx.Length;++i)
		{
			GameObject tmpGO = ((GameObject) GameObject.Instantiate(
													_data.pickups[_curPO],
													points[i],
													Quaternion.identity));
			_pickups.Add(tmpGO.GetComponent<PickupBehaviour>());
		}
	}
	
	public Color SpawnPickups()
	{
		GameObject[] tmp = GameObject.FindGameObjectsWithTag("Pickup");
		if(tmp.Length > 0)
		{
			Debug.LogWarning("Not all pickups are being deleted.  Attempting to remove now");
			for(int i=0;i<tmp.Length;++i)
				GameObject.Destroy(tmp[i]);
		}
		SelectNewSpawnGroup();
		SpawnNewPickups();
		return _pickups[0].gameObject.GetComponentInChildren<MeshRenderer>().materials[0].color;
	}
	
	public Color RemovePickups(GameObject toRemove)
	{
		Color ret = Color.clear;
		if(toRemove.activeSelf)
		{
			ret = toRemove.GetComponentInChildren<MeshRenderer>().materials[0].GetColor("_Color");
			Vector3 spawnRemove = toRemove.transform.position;
			_spawnGroups[_curSG].spawnPoints.Remove(spawnRemove);
			
			toRemove.GetComponent<PickupBehaviour>().ToDie = true;
			
			foreach(PickupBehaviour pb in _pickups)
			{
				if(!pb.ToDie)
				{
					pb.ToDie = true;
					//deactivate the pickup to account for destruction lag
					pb.gameObject.SetActive(false);
					
					//destroy the pickup
					GameObject.Destroy(pb.gameObject);
				}
			}
			_pickups.Clear();
		}
		return ret;
	}
	
	public void ShowEnd()
	{
		//I just quit caring about doing this right.
		//right = generic in this case.  I don't know that I've been doing
		//it right this whole time.
		GameObject.Instantiate(_data.endPickupPrefab, EndPosition, Quaternion.identity);
	}
	
	public MazeCell StartCell { get { return _startCell; } }

	public MazeCell EndCell { get { return _endCell; } }

	public Vector3 StartPosition
	{
	
		get
		{
			Vector3 scale = _mazeGameObject.transform.localScale;
			return (new Vector3(_startCell.Column*scale.x, _startCell.Layer*scale.y, _startCell.Row*scale.z));
		}
	}

	public Vector3 EndPosition
	{
		get
		{
			Vector3 scale = _mazeGameObject.transform.localScale;
			return (new Vector3(_endCell.Column*scale.x, _endCell.Layer*scale.y, _endCell.Row*scale.z));
		}
	}

	public bool CanDoPickup { get; set; }
}