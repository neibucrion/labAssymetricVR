using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation4 : MonoBehaviour {
	public float dureeRotation;
	public Vector3 rotationVoulue;


	private float temps;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Jump"))
		{
			transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(rotationVoulue), 0.5f);
		}
		else
		{
			transform.rotation = Quaternion.identity;
		}
	}
}
