using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BougeCameraRigQuentin : MonoBehaviour {

    public GameObject salleDepart;
    public GameObject cameraRig;
    public GameObject controllerLeft;
    public GameObject controllerRight;
    public float delaiRepositionnement;

    [HideInInspector]
    public Salle salleActive;

    private GameObject mainActuelle;

	private GameObject murActuel;
    private float dureeRepositionnement;
    private bool repositionnementEnCours = false;

    private GameObject poignee;
    private GameObject porteRotative;
	private Vector3 directionDepart;
	private Quaternion departRotationPoignee;
	private Quaternion finRotationPoignee;
	private float rotationDepartPorteY;
	private Quaternion rotationPorteRotative;
    private float decalZpoignee;
    private float delaiChoisi;
    private Quaternion rotationDepartPoignee;

    // Use this for initialization
    void Start () {
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
    }

    public void modifiePoignee(GameObject pg)
    {
        poignee = pg;
        porteRotative = poignee.transform.parent.gameObject;
        cameraRig.transform.SetParent(porteRotative.transform, true);
		directionDepart = mainActuelle.transform.position - poignee.transform.position;
		directionDepart = porteRotative.transform.InverseTransformVector(directionDepart);
		directionDepart.z = 0.0f;
		departRotationPoignee = poignee.transform.localRotation;
        rotationDepartPorteY = porteRotative.transform.rotation.eulerAngles.y;
    }

    public void tournePoignee()
    {
        Vector3 directionVoulue = mainActuelle.transform.position - poignee.transform.position;
        directionVoulue = porteRotative.transform.InverseTransformVector(directionVoulue);
        directionVoulue.z = 0.0f;
		Quaternion rotationVoulue = Quaternion.FromToRotation(directionDepart, directionVoulue);
        poignee.transform.localRotation = departRotationPoignee*rotationVoulue;
        Vector3 eulerPorte = new Vector3(0.0f, rotationDepartPorteY + poignee.transform.eulerAngles.z);
        porteRotative.transform.rotation = Quaternion.Euler(eulerPorte);
    }

	public void choisitPiece(GameObject ma)
    {
        murActuel = ma;
        repositionnementEnCours = true;
        dureeRepositionnement = 0.0f;
        calculeValeursPoignee();
    }

	void calculeValeursPoignee()
	{
		rotationDepartPoignee = poignee.transform.localRotation;
		/*Quaternion arrivee = Quaternion.identity;
		Quaternion decalage = Quaternion.Lerp(rotationDepartPoignee,arrivee,1.0f);
		decalZpoignee = decalage.eulerAngles.z;*/
		decalZpoignee = rotationDepartPoignee.eulerAngles.z;
		decalZpoignee = decalZpoignee % 180.0f;
		if (Mathf.Abs(decalZpoignee) > 90.0f)
		{
			decalZpoignee = decalZpoignee - (180.0f * Mathf.Sign(decalZpoignee));
		}
		delaiChoisi = delaiRepositionnement * (Mathf.Abs(decalZpoignee) / 180.0f);
		rotationPorteRotative = porteRotative.transform.rotation;
	}

    /*GameObject trouveSalleChoisie()
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
    }*/

    // Update is called once per frame
    void Update()
    {
        if (repositionnementEnCours)
        {
            repositionnePoignee();
        }
    }

    private void repositionnePoignee()
    {
		dureeRepositionnement += Time.deltaTime;
        if (dureeRepositionnement < delaiChoisi)
        {
            float decalageZ = (dureeRepositionnement / delaiChoisi);
			/*modifieZRotation(poignee, decalageZ);
            Vector3 eulerPorte = new Vector3(0.0f, 0.0f, rotationDepartPorteY + decalageZ);
            porteRotative.transform.rotation = Quaternion.Euler(eulerPorte);
			poignee.transform.localRotation = Quaternion.Lerp (Quaternion.identity, rotationDepartPoignee, decalageZ);*/
			porteRotative.transform.rotation = Quaternion.Lerp (murActuel.transform.rotation, rotationPorteRotative, decalageZ);
        }
        else
        {
            repositionnementEnCours = false;
			poignee.transform.localRotation = Quaternion.identity;
			porteRotative.transform.localRotation = Quaternion.identity;
            cameraRig.transform.SetParent(transform.root, true);
        }
    }

    private void modifieZRotation(GameObject objet, float valeur)
    {
        Vector3 eulerRotation = objet.transform.rotation.eulerAngles;
        objet.transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, valeur);
    }

}
