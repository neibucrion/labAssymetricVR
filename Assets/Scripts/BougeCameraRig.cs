using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BougeCameraRig : MonoBehaviour {

    public GameObject salleDepart;
    public GameObject cameraRig;
    public GameObject controllerLeft;
    public GameObject controllerRight;    
    public float delaiRepositionnement;

    [HideInInspector]
    public Salle salleActive;

    private Vector3 memDepart;
    private float memRotationY;
    private GameObject mainActuelle;

    private Mur murActuel;
    private GameObject objetSalle;
    private Vector3 positionDepart;
    private Vector3 positionSouhaitee;
    private Quaternion rotationDepart;
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
        memDepart = mainActuelle.transform.position;
        memDepart.y = 0.0f;
        memRotationY = mainActuelle.transform.rotation.eulerAngles.y;
        rotationDepart = cameraRig.transform.rotation;
    }

    public void recaleCameraRig()
    {
        Vector3 diffManetteCamera = cameraRig.transform.position - mainActuelle.transform.position;
        diffManetteCamera.y = 0.0f;
        cameraRig.transform.position = memDepart + diffManetteCamera;
        float nouvY = cameraRig.transform.rotation.eulerAngles.y - mainActuelle.transform.rotation.eulerAngles.y;
        nouvY += memRotationY;
        cameraRig.transform.rotation = Quaternion.Euler(0.0f, nouvY, 0.0f);
    }

    public void choisitPiece(Mur ma)
    {
        murActuel = ma;
        repositionnementEnCours = true;
        dureeRepositionnement = 0.0f;                
        objetSalle = trouveSalleChoisie();
        positionDepart = cameraRig.transform.position;
        positionSouhaitee = objetSalle.transform.position;        
        calculeValeursRotation();
    }

    GameObject trouveSalleChoisie()
    {
        GameObject retour = null;
        Vector3 diffSalle1 = murActuel.salle1.tuile.transform.position - cameraRig.transform.position;
        Vector3 diffSalle2 = murActuel.salle2.tuile.transform.position - cameraRig.transform.position;
        if (diffSalle1.magnitude < diffSalle2.magnitude)
        {
            retour = murActuel.salle1.tuile;
        }
        else
        {
            retour = murActuel.salle2.tuile;
        }
        return retour;
    }

    void calculeValeursRotation()
    {
        rotationDepart = cameraRig.transform.rotation;
        Vector3 directionSouhaitee = objetSalle.transform.position - murActuel.mur.transform.position;
        /*Quaternion directionDecalee = rotationDepart*Quaternion.LookRotation(directionSouhaitee);
        float decalY = directionDecalee.eulerAngles.y;*/
        Quaternion rotationFinale = Quaternion.LookRotation(directionSouhaitee);
        float decalY = rotationFinale.eulerAngles.y - rotationDepart.eulerAngles.y;
        decalY = decalY % 90.0f;
        if (Mathf.Abs(decalY) > 45.0f)
        {
            decalY = decalY - (90.0f*Mathf.Sign(decalY));
        }
        rotationSouhaitee = Quaternion.Euler(0.0f, decalY, 0.0f);        
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
            cameraRig.transform.rotation = rotationDepart*Quaternion.Lerp(Quaternion.identity, rotationSouhaitee, rapport);
        }
        else
        {
            repositionnementEnCours = false;
            cameraRig.transform.position = positionSouhaitee;
            cameraRig.transform.rotation = rotationDepart*rotationSouhaitee;
        }
    }
}
