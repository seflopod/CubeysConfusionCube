using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplyCellMaterials : MonoBehaviour
{
	public MazeCellWallData rightWall;
	public MazeCellWallData leftWall;
	public MazeCellWallData topWall;
	public MazeCellWallData bottomWall;
	public MazeCellWallData frontWall;
	public MazeCellWallData backWall;
	
	/*private void Awake()
	{
		MeshRenderer[] childMR = GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer mr in childMR)
		{
			switch(mr.gameObject.name)
			{
			case "Right":
				mr.gameObject.SetActive(rightWall.active);
				mr.materials = rightWall.materials.ToArray();
				break;
			case "Left":
				mr.gameObject.SetActive(leftWall.active);
				mr.materials = leftWall.materials.ToArray();
				break;
			case "Top":
				mr.gameObject.SetActive(topWall.active);
				mr.materials = topWall.materials.ToArray();
				break;
			case "Bottom":
				mr.gameObject.SetActive(bottomWall.active);
				mr.materials = bottomWall.materials.ToArray();
				break;
			case "Front":
				mr.gameObject.SetActive(frontWall.active);
				mr.materials = frontWall.materials.ToArray();
				break;
			case "Back":
				mr.gameObject.SetActive(backWall.active);
				mr.materials = backWall.materials.ToArray();
				break;
			default:
				break;
			}
		}
	}*/
	
	public void Apply()
	{
		MeshRenderer[] childMR = GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer mr in childMR)
		{
			switch(mr.gameObject.name)
			{
			case "Right":
				mr.gameObject.SetActive(rightWall.active);
				mr.materials = rightWall.materials.ToArray();
				break;
			case "Left":
				mr.gameObject.SetActive(leftWall.active);
				mr.materials = leftWall.materials.ToArray();
				break;
			case "Top":
				mr.gameObject.SetActive(topWall.active);
				mr.materials = topWall.materials.ToArray();
				break;
			case "Bottom":
				mr.gameObject.SetActive(bottomWall.active);
				mr.materials = bottomWall.materials.ToArray();
				break;
			case "Front":
				mr.gameObject.SetActive(frontWall.active);
				mr.materials = frontWall.materials.ToArray();
				break;
			case "Back":
				mr.gameObject.SetActive(backWall.active);
				mr.materials = backWall.materials.ToArray();
				break;
			default:
				break;
			}
		}
	}
}
