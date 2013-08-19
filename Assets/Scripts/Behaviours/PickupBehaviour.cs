using UnityEngine;
using System.Collections;

public class PickupBehaviour : MonoBehaviour
{
	public float rotationSpeed = 0.88f;
	public float lightIntensityMin = 1.0f;
	public float lightIntensityMax = 3.0f;
	public float timeToCycle = 1.0f;
	
	private Light _light;
	private float _dIdT;
	private bool _incI;
	private bool _toDie;
	private void Start()
	{
		ToDie = false;
		_light = GetComponentInChildren<Light>();
		_light.intensity = lightIntensityMin;
		_incI = true;
		_dIdT = (lightIntensityMax - lightIntensityMin)/(2*timeToCycle);
		_toDie = false;
	}
	
	private void Update()
	{
		transform.RotateAround(Vector3.up, rotationSpeed * Time.deltaTime);
		if(_incI)
		{
			_light.intensity+=_dIdT*Time.deltaTime;
			_incI = (_light.intensity < lightIntensityMax);
		}
		else
		{
			_light.intensity-=_dIdT*Time.deltaTime;
			_incI = (_light.intensity > lightIntensityMin);
		}
	}
	
	public bool ToDie
	{
		get { return _toDie; }
		
		set
		{
			if(!value && value!=_toDie)
			{
				gameObject.GetComponent<BoxCollider>().enabled = false;
				MeshRenderer[] mrs = gameObject.GetComponentsInChildren<MeshRenderer>();
				foreach(MeshRenderer mr in mrs)
					mr.enabled = false;
				Light[] ls = gameObject.GetComponentsInChildren<Light>();
				foreach(Light l in ls)
					l.enabled = false;
			}
			_toDie = value;
		}
	}
}
