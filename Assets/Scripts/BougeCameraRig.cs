using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BougeCameraRig : MonoBehaviour {

    public GameObject salleDepart;
    public GameObject cameraRig;
    public GameObject controllerRight;
    public GameObject controllerLeft;

    [HideInInspector]
    public Salle salleActive;

    private Vector3 memPosManette;
    private float memRotManetteY;
    private Vector3 memDifferentiel;
    private Quaternion memRotDifferentiel;
    private GameObject mainActuelle;

    // Use this for initialization
    void Start () {
        DetecteChangement[] portes = GameObject.FindObjectsOfType<DetecteChangement>();
        foreach (DetecteChangement porte in portes)
        {
            porte.gereMouvement = this;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void changePiece(Mur porte)
    {
        Salle nouvelleSalle = null;
        if (porte.salle1 != salleActive)
        {
            nouvelleSalle = porte.salle1;
        }
        else
        {
            nouvelleSalle = porte.salle2;
        }
        cameraRig.transform.position = nouvelleSalle.tuile.transform.position;
        Vector3 eulerRig = cameraRig.transform.rotation.eulerAngles;
        eulerRig.y += 180.0f;
        cameraRig.transform.rotation = Quaternion.Euler(eulerRig);        
        salleActive = nouvelleSalle;
    }

    public void choisitMainActuelle(bool mainChoisie)
    {
        if (mainChoisie)
        {
            mainActuelle = controllerRight;
        }
        else
        {
            mainActuelle = controllerLeft;
        }
    }

    public void memoriseValeurs()
    {
        memPosManette = mainActuelle.transform.position;
        memRotManetteY = mainActuelle.transform.rotation.eulerAngles.y;
        memDifferentiel = cameraRig.transform.position - mainActuelle.transform.position;
        float diffY = cameraRig.transform.rotation.eulerAngles.y - mainActuelle.transform.rotation.eulerAngles.y;
        memRotDifferentiel = Quaternion.Euler(0.0f, diffY, 0.0f);
    }

    public void recaleCameraRig()
    {
        Vector3 posRelative = mainActuelle.transform.position - memPosManette;
        cameraRig.transform.position = memPosManette + posRelative + memDifferentiel;
        float diffY = memRotManetteY + (mainActuelle.transform.rotation.eulerAngles.y - memRotManetteY);
        cameraRig.transform.rotation = Quaternion.Euler(0.0f, diffY, 0.0f);

    }

}
