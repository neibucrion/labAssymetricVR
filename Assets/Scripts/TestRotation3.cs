using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation3 : MonoBehaviour {

	public GameObject point1;
	public GameObject point2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 direction = point2.transform.position - point1.transform.position;
		transform.position = point1.transform.position;//+ (direction / 2.0f);
		transform.rotation = Quaternion.LookRotation (direction);
	}
}
