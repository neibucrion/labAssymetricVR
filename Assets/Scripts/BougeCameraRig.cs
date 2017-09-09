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

    private bool modeQuentin = false;
    private GameObject poignee;
    private GameObject porteRotative;
    private float rotationDepartPorteY;
    private float decalZpoignee;
    private float delaiChoisi;
    private Quaternion rotationDepartPoignee;

    // Use this for initialization
    void Start () {
        delaiChoisi = delaiRepositionnement;
        DetecteChangement[] portes = GameObject.FindObjectsOfType<DetecteChangement>();
        foreach (DetecteChangement porte in portes)
        {
            porte.gereMouvement = this;
        }
        GrabPoignee[] poignees = GameObject.FindObjectsOfType<GrabPoignee>();
        foreach (GrabPoignee poignee in poignees)
        {
            poignee.gereMouvement = this;
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

    public void modifiePoignee(GameObject pg)
    {
        poignee = pg;
        porteRotative = poignee.transform.parent.gameObject;
        rotationDepartPorteY = porteRotative.transform.rotation.eulerAngles.y;
        cameraRig.transform.SetParent(porteRotative.transform, true);
        modeQuentin = true;
    }

    public void memoriseValeurs()
    {
        memDepart = mainActuelle.transform.position;
        memDepart.y = 0.0f;
        memRotationY = mainActuelle.transform.rotation.eulerAngles.y;
        rotationDepart = cameraRig.transform.rotation;
    }

    public void tournePoignee()
    {
        Vector3 directionVoulue = mainActuelle.transform.position - poignee.transform.position;
        directionVoulue = poignee.transform.InverseTransformVector(directionVoulue);
        directionVoulue.z = 0.0f;
        directionVoulue = poignee.transform.TransformVector(directionVoulue);
        Quaternion rotationVoulue = Quaternion.LookRotation(directionVoulue);
        poignee.transform.rotation = rotationVoulue;
        Vector3 eulerPorte = new Vector3(0.0f, rotationDepartPorteY + poignee.transform.eulerAngles.z);
        porteRotative.transform.rotation = Quaternion.Euler(eulerPorte);
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
        if (modeQuentin)
        {
            calculeValeursPoignee();
        }
        else
        {
            calculeValeursRotation();
        }
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

    void calculeValeursPoignee()
    {
        rotationDepartPoignee = poignee.transform.rotation;
        decalZpoignee = rotationDepartPoignee.eulerAngles.z;
        decalZpoignee = decalZpoignee % 180.0f;
        if (Mathf.Abs(decalZpoignee) > 90.0f)
        {
            decalZpoignee = decalZpoignee - (180.0f * Mathf.Sign(decalZpoignee));
        }
        delaiChoisi = delaiRepositionnement * (Mathf.Abs(decalZpoignee) / 180.0f);
        rotationDepartPorteY = porteRotative.transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (repositionnementEnCours)
        {
            if (modeQuentin)
            {
                repositionnePoignee();
            }
            else
            {
                repositionnePiece();
            }
        }
    }

    private void repositionnePiece()
    {
        dureeRepositionnement += Time.deltaTime;
        if (dureeRepositionnement < delaiChoisi)
        {
            float rapport = dureeRepositionnement / delaiChoisi;
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

    private void repositionnePoignee()
    {
        if (dureeRepositionnement < delaiChoisi)
        {
            float decalageZ = decalZpoignee*(dureeRepositionnement / delaiChoisi);
            modifieZRotation(poignee, decalageZ);
            Vector3 eulerPorte = new Vector3(0.0f, rotationDepartPorteY + decalageZ);
            porteRotative.transform.rotation = Quaternion.Euler(eulerPorte);
        }
        else
        {
            repositionnementEnCours = false;
            modifieZRotation(poignee, 0.0f);
            porteRotative.transform.rotation = porteRotative.transform.parent.rotation;
            cameraRig.transform.SetParent(transform.root, true);
        }
    }

    private void modifieZRotation(GameObject objet, float valeur)
    {
        Vector3 eulerRotation = objet.transform.rotation.eulerAngles;
        objet.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, valeur);
    }

}
