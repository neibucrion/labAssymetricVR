using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation2 : MonoBehaviour {

    public float vitesse;
    public float decalage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float rot = Input.GetAxis("Horizontal");
		float zDepart = transform.localRotation.eulerAngles.z;
		transform.localRotation = Quaternion.Euler(0.0f, 0.0f, zDepart+(rot*vitesse));
        transform.localPosition = Vector3.zero + (transform.localRotation * Vector3.up * decalage);
	}
}
