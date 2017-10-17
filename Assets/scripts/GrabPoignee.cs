using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPoignee : MonoBehaviour {

    [HideInInspector]
    public BougeCameraRigQuentin gereMouvement;

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
<<<<<<< HEAD
            gereMouvement.modifiePoignee(transform.parent.gameObject);
=======
            gereMouvement.modifiePoignee(gameObject.transform.parent.gameObject);
>>>>>>> 2098ac5620ae3ed459c09b0d3f5ca3f0fb6e3466
        }
        else if (grabEnCours && ((!Input.GetButton("TriggerRight") && !Input.GetButton("TriggerLeft"))||mainActive == false))
        {
            grabEnCours = false;
            grabActif = false;
            GameObject murObjet = transform.parent.gameObject.transform.parent.gameObject;
			gereMouvement.choisitPiece(murObjet);
        }
        else if (grabEnCours)
        {
            gereMouvement.tournePoignee();
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
