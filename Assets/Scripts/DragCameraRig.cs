using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCameraRig : MonoBehaviour {
	public GameObject cameraRig;
	public GameObject manette;
	public float vitesse = 1.0f;
	public float inversionAxe = 1.0f;

	private bool mouvementActif = false;
	private bool rotationActive = false;
	private Vector3 departMain;
	private float memRotationY;
	
	// Update is called once per frame
	void Update () {
		if (!mouvementActif && Input.GetButton ("TriggerLeft")) {
			//if (!mouvementActif && (Input.GetButton("TriggerRight") || Input.GetButton("TriggerLeft"))) {
			activeMouvement ();
		} else if (mouvementActif && !Input.GetButton ("TriggerLeft")) {
			//} else if (mouvementActif && (!Input.GetButton("TriggerRight") && !Input.GetButton("TriggerLeft"))) {
			mouvementActif = false;
		}

		if (!rotationActive && mouvementActif && Input.GetButton ("TriggerRight")) {
			rotationActive = true;
		} else if (rotationActive && !Input.GetButton ("TriggerRight")) {
			rotationActive = false;
		}

		if (mouvementActif) {
			recaleCameraRig ();
		}
	}

	void activeMouvement()
	{
		mouvementActif = true;
		departMain = manette.transform.position;
		departMain.y = 0.0f;
		memRotationY = manette.transform.rotation.eulerAngles.y;
	}

	void recaleCameraRig()
	{
		Vector3 diffManetteCamera = cameraRig.transform.position - manette.transform.position;
		diffManetteCamera.y = 0.0f;
		cameraRig.transform.position = departMain + (diffManetteCamera*vitesse);
		if (rotationActive) {			
			float nouvY = cameraRig.transform.rotation.eulerAngles.y - manette.transform.rotation.eulerAngles.y;
			nouvY *= inversionAxe;
			nouvY += memRotationY;
			cameraRig.transform.rotation = Quaternion.Euler (0.0f, nouvY, 0.0f);
		}
	}
}
