using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetecteChangement : MonoBehaviour {

    

    [HideInInspector]
    public BougeCameraRig gereMouvement;
    
    private bool mainActive = false;
    private bool mouvementEnCours = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!mouvementEnCours && mainActive && (Input.GetButton("TriggerRight") || Input.GetButton("TriggerLeft")))
        {
            mouvementEnCours = true;
            GameObject murObjet = transform.parent.gameObject;
            Mur murPiece = CreeNiveau.renvoieMur(murObjet);
            envoieMainActuelle();
            gereMouvement.memoriseValeurs();
            gereMouvement.changePiece(murPiece);
            
        }
        else if (mouvementEnCours && (!Input.GetButton("TriggerRight") && !Input.GetButton("TriggerLeft")))
        {
            mouvementEnCours = false;
        }
        else if (mouvementEnCours)
        {
            //gereMouvement.recaleCameraRig();
        }
	}

    private void envoieMainActuelle()
    {
        if (Input.GetButton("TriggerRight"))
        {
            gereMouvement.choisitMainActuelle(true);
        }
        else
        {
            gereMouvement.choisitMainActuelle(false);
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
