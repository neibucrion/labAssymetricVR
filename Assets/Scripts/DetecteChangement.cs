using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetecteChangement : MonoBehaviour {  

    [HideInInspector]
    public BougeCameraRig gereMouvement;
    
    private bool mainActive = false;
    private bool mouvementEnCours = false;

    //Variable de controle statique de l'ensemble des objets de detection
    static private bool mouvementActif = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!mouvementEnCours && !mouvementActif && mainActive && (Input.GetButton("TriggerRight") || Input.GetButton("TriggerLeft")))
        {
            mouvementEnCours = true;
            mouvementActif = true;
            gereMouvement.choisitMainActuelle();
            /*GameObject murObjet = transform.parent.gameObject;
            Mur murPiece = CreeNiveau.renvoieMur(murObjet);
            gereMouvement.changePiece(murPiece);*/           
        }
        else if (mouvementEnCours && (!Input.GetButton("TriggerRight") && !Input.GetButton("TriggerLeft")))
        {
            mouvementEnCours = false;
            mouvementActif = false;
            GameObject murObjet = transform.parent.gameObject;
            Mur murPiece = CreeNiveau.renvoieMur(murObjet);
            gereMouvement.choisitPiece(murPiece);
        }
        else if (mouvementEnCours)
        {
            gereMouvement.recaleCameraRig();
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
