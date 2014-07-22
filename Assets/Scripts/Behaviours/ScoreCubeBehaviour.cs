using UnityEngine;
using System.Collections.Generic;


public class ScoreCubeBehaviour : MonoBehaviour
{
	Stack<GameObject> _enabledFaces;
	Stack<GameObject> _disabledFaces;
	
	private void Awake()
	{
		_enabledFaces = new Stack<GameObject>();
		_disabledFaces = new Stack<GameObject>();
		
		_disabledFaces.Push(transform.FindChild("F0_Top").gameObject);
		_disabledFaces.Push(transform.FindChild("F1_Back").gameObject);
		_disabledFaces.Push(transform.FindChild("F2_Right").gameObject);
		_disabledFaces.Push(transform.FindChild("F3_Front").gameObject);
		_disabledFaces.Push(transform.FindChild("F4_Left").gameObject);
		_disabledFaces.Push(transform.FindChild("F5_Bottom").gameObject);
	}
	
	public void AddOne()
	{
		_enabledFaces.Push (_disabledFaces.Pop());
		_enabledFaces.Peek().GetComponent<MeshRenderer>().enabled = true;
	}
	
	public void SubtractOne()
	{
		_disabledFaces.Push(_enabledFaces.Pop());
		_disabledFaces.Peek().GetComponent<MeshRenderer>().enabled = false;
	}
	
	public void ChangeColor(Color newColor)
	{
		MeshRenderer[] mr = gameObject.GetComponentsInChildren<MeshRenderer>();
		for(int i=0;i<mr.Length;++i)
		{
			MeshRenderer m = mr[i];
			m.materials[0].color = newColor;
		}
	}
}