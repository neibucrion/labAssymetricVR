using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BougeCameraRig : MonoBehaviour {

    public GameObject salleDepart;
    public GameObject cameraRig;
    public GameObject controllerRight;
    public GameObject controllerLeft;
    public float delaiRepositionnement;

    [HideInInspector]
    public Salle salleActive;

    private Vector3 memDifferentiel;
    private float memDiffRotationY;
    private GameObject mainActuelle;

    private Vector3 positionDepart;
    private Quaternion rotationDepart;
    private Vector3 positionSouhaitee;
    private Quaternion rotationSouhaitee;
    private float dureeRepositionnement;
    private bool repositionnementEnCours = false;

    // Use this for initialization
    void Start () {
        DetecteChangement[] portes = GameObject.FindObjectsOfType<DetecteChangement>();
        foreach (DetecteChangement porte in portes)
        {
            porte.gereMouvement = this;
        }
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

    public void choisitMainActuelle()
    {
        if (Input.GetButton("TriggerRight"))
        {
            mainActuelle = controllerRight;
        }
        else
        {
            mainActuelle = controllerLeft;
        }
        memoriseValeurs();
    }

    public void memoriseValeurs()
    {
        memDifferentiel = cameraRig.transform.position - mainActuelle.transform.position;
        memDiffRotationY = cameraRig.transform.rotation.eulerAngles.y - mainActuelle.transform.rotation.eulerAngles.y;       
    }

    public void recaleCameraRig()
    {
        cameraRig.transform.position = mainActuelle.transform.position + memDifferentiel;
        float nouvY = mainActuelle.transform.rotation.eulerAngles.y + memDiffRotationY;
        cameraRig.transform.rotation = Quaternion.Euler(0.0f, nouvY, 0.0f);
    }

    public void choisitPiece(Mur murActuel)
    {
        repositionnementEnCours = true;
        dureeRepositionnement = 0.0f;
        positionDepart = cameraRig.transform.position;
        rotationDepart = cameraRig.transform.rotation;
        Vector3 diffSalle1 = murActuel.salle1.tuile.transform.position - cameraRig.transform.position;
        Vector3 diffSalle2 = murActuel.salle2.tuile.transform.position - cameraRig.transform.position;
        if (diffSalle1.magnitude < diffSalle2.magnitude)
        {
            positionSouhaitee = murActuel.salle1.tuile.transform.position;

        }
        else
        {
            positionSouhaitee = murActuel.salle2.tuile.transform.position;
        }
        trouveRotationSouhaitee();        
    }

    private void trouveRotationSouhaitee()
    {
        Vector3 posManetteCameraRig = mainActuelle.transform.position - cameraRig.transform.position;
        posManetteCameraRig.y = 0.0f;
        if (Mathf.Abs(posManetteCameraRig.x) > Mathf.Abs(posManetteCameraRig.z))
        {
            posManetteCameraRig.z = 0.0f;
        }
        else
        {
            posManetteCameraRig.x = 0.0f;
        }
        rotationSouhaitee = Quaternion.LookRotation(posManetteCameraRig);
    }

    // Update is called once per frame
    void Update()
    {
        if (repositionnementEnCours)
        {
            repositionnePiece();
        }
    }

    private void repositionnePiece()
    {
        dureeRepositionnement += Time.deltaTime;
        if (dureeRepositionnement < delaiRepositionnement)
        {
            float rapport = dureeRepositionnement / delaiRepositionnement;
            cameraRig.transform.position = Vector3.Lerp(positionDepart, positionSouhaitee, rapport);
            cameraRig.transform.rotation = Quaternion.Lerp(rotationDepart, rotationSouhaitee, rapport);
        }
        else
        {
            repositionnementEnCours = false;
            cameraRig.transform.position = positionSouhaitee;
            cameraRig.transform.rotation = rotationSouhaitee;
        }
    }

}
