using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPoignee : MonoBehaviour {

    [HideInInspector]
    public BougeCameraRig gereMouvement;

    private bool mainActive = false;
    private bool grabEnCours = false;

    //Variable de controle statique de l'ensemble des objets de detection
    static private bool grabActif = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!grabEnCours && !grabActif && mainActive && (Input.GetButton("TriggerRight") || Input.GetButton("TriggerLeft")))
        {
            grabEnCours = true;
            grabActif = true;
            gereMouvement.choisitMainActuelle();
        }
        else if (grabEnCours && (!Input.GetButton("TriggerRight") && !Input.GetButton("TriggerLeft")))
        {
            grabEnCours = false;
            grabActif = false;
            GameObject murObjet = transform.parent.gameObject;
            Mur murPiece = CreeNiveau.renvoieMur(murObjet);
            gereMouvement.choisitPiece(murPiece);
        }
        else if (grabEnCours)
        {
            gereMouvement.tournePoignee(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "main")
        {
            mainActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "main")
        {
            mainActive = false;
        }
    }
}
