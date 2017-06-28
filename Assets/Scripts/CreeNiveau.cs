using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EtatPorte { EXTERIEUR, MITOYEN, PORTE };

public class Mur
{
    public EtatPorte porte = EtatPorte.EXTERIEUR;
    public Vector3 position;
    public Quaternion direction;
    public GameObject mur;
    public Salle salle1 = null;
    public Salle salle2 = null;
}

public class Salle
{
    public GameObject tuile = null;
    public Mur nord = null;
    public Mur est = null;
    public Mur sud = null;
    public Mur ouest = null;
}

public class CreeNiveau : MonoBehaviour {

    //Variables publiques pour l'integration
    public float distanceDeReference;
    public GameObject murmur;
    public GameObject murporte;

    //Variables privees
    private GameObject objetRef;
    private Mur contactActuel;
    private Vector3 difference;
    private GameObject[] tuiles;
    private GameObject[] portesRefs;
    private float etalon;

    static private List<Salle> salles;
    static private List<Mur> contacts;

	// Use this for initialization
	void Start () {
        tuiles = GameObject.FindGameObjectsWithTag("tuile");
        portesRefs = GameObject.FindGameObjectsWithTag("porte");
        salles = new List<Salle>();
        contacts = new List<Mur>();
        recaleElements();
        creeSalles();
        creeMurs();
        creeMursRestants();
        effaceBlocs();
        ajustePremiereSalle();
	}

    void recaleElements()
    {
        recaleTuiles();
        recalePortes();
    }

    void recaleTuiles()
    {
        etalon = distanceDeReference;
        foreach (GameObject tuile in tuiles)
        {
            objetRef = tuile;            
            ajustePositionObjet();
        }
    }   

    void recalePortes()
    {
        etalon = distanceDeReference / 2.0f;
        foreach (GameObject porte in portesRefs)
        {
            objetRef = porte;
            ajustePositionObjet();
        }
    }

    void ajustePositionObjet()
    {
        Vector3 positionObjet = objetRef.transform.position;
        positionObjet.x = rectifieValeur(positionObjet.x);
        positionObjet.z = rectifieValeur(positionObjet.z);
        objetRef.transform.position = positionObjet;
    }

    float rectifieValeur(float valeur)
    {
        float modulo = valeur % etalon;
        int partEntiere = (int)(valeur / etalon);
        float rapModulo = modulo / etalon;
        if (rapModulo > 0.5)
        {
            valeur = (partEntiere +1)*etalon;
        }
        else
        {
            valeur = partEntiere * etalon;
        }
        return valeur;
    }

    void creeSalles()
    {
        foreach (GameObject tuile in tuiles)
        {
            Salle salle = new Salle();
            salle.tuile = tuile;
            salle.nord = new Mur();
            salle.est = new Mur();
            salle.sud = new Mur();
            salle.ouest = new Mur();
            salles.Add(salle);            
        }
        trouveSallesVoisines();
    }

    void trouveSallesVoisines()
    {
        foreach (Salle salle in salles)
        {
            foreach (Salle salle2 in salles)
            {
                difference = salle2.tuile.transform.position - salle.tuile.transform.position;
                if (difference.magnitude == distanceDeReference)
                {
                    contactActuel = new Mur();
                    contactActuel.salle1 = salle;
                    contactActuel.salle2 = salle2;
                    contactActuel.porte = EtatPorte.MITOYEN;
                    contactActuel.position = salle.tuile.transform.position + (difference / 2.0f);
                    contactActuel.direction = Quaternion.LookRotation(difference);
                    verifieContactActuel();                    
                }
            }
        }
    }

    void verifieContactActuel()
    {
        Vector3 normal = difference.normalized;
        Salle salleDeReference = contactActuel.salle1;
        Salle autreSalle = contactActuel.salle2;
        if (normal.x == 1.0f && salleDeReference.est.porte == EtatPorte.EXTERIEUR)
        {
            salleDeReference.est = contactActuel;
            autreSalle.ouest = contactActuel;
            contacts.Add(contactActuel);
        }
        else if (normal.x == -1.0f && salleDeReference.ouest.porte == EtatPorte.EXTERIEUR)
        {
            salleDeReference.ouest = contactActuel;
            autreSalle.est = contactActuel;
            contacts.Add(contactActuel);
        }
        else if (normal.z == 1.0f && salleDeReference.nord.porte == EtatPorte.EXTERIEUR)
        {
            salleDeReference.nord = contactActuel;
            autreSalle.sud = contactActuel;
            contacts.Add(contactActuel);
        }
        else if (normal.z == -1.0f && salleDeReference.sud.porte == EtatPorte.EXTERIEUR)
        {
            salleDeReference.sud = contactActuel;
            autreSalle.nord = contactActuel;
            contacts.Add(contactActuel);
        }
        verifiePorte();
    }

    void verifiePorte()
    {
        foreach (GameObject porte in portesRefs)
        {
            if (contactActuel.position.x == porte.transform.position.x && contactActuel.position.z == porte.transform.position.z)
            {
                contactActuel.porte = EtatPorte.PORTE;                
            }
        }
    }

    void creeMurs()
    {
        foreach (Mur contact in contacts)
        {
            if (contact.porte == EtatPorte.MITOYEN)
            {
                Instantiate(murmur, contact.position, contact.direction);
            }
            else
            {
                contact.mur = Instantiate(murporte, contact.position, contact.direction);
            }
        }
    }

    void creeMursRestants()
    {
        float valeurRef = distanceDeReference / 2.0f;
        foreach (Salle salle in salles)
        {
            Vector3 direction = new Vector3(0.0f, 0.0f, valeurRef);
            testeUnMur(salle, salle.nord, direction);
            direction = new Vector3(-valeurRef, 0.0f, 0.0f);
            testeUnMur(salle, salle.ouest, direction);
            direction = new Vector3(0.0f, 0.0f, -valeurRef);
            testeUnMur(salle, salle.sud, direction);
            direction = new Vector3(valeurRef, 0.0f, 0.0f);
            testeUnMur(salle, salle.est, direction);
        }
    }

    void testeUnMur(Salle salle, Mur contact, Vector3 direction)
    {
        if (contact.porte == EtatPorte.EXTERIEUR)
        {
            Vector3 positionMur = salle.tuile.transform.position + direction;
            Quaternion directionMur = Quaternion.LookRotation(direction);
            Instantiate(murmur, positionMur, directionMur);
        }
    }

    void effaceBlocs()
    {
        foreach (GameObject porte in portesRefs)
        {
            porte.SetActive(false);
        }
    }

    void ajustePremiereSalle()
    {
        BougeCameraRig mouvement = GetComponent<BougeCameraRig>();
        mouvement.salleActive = renvoieSalle(mouvement.salleDepart);
    }

    static public Mur renvoieMur(GameObject objet)
    {
        Mur retour = null;
        foreach (Mur mur in contacts)
        {
            if (mur.mur == objet)
            {
                retour = mur;
                break;
            }
        }
        return retour;
    }

    static public Salle renvoieSalle(GameObject objet)
    {
        Salle retour = null;
        foreach (Salle salle in salles)
        {
            if (salle.tuile == objet)
            {
                retour = salle;
                break;
            }
        }
        return retour;
    }

    // Update is called once per frame
    void Update () {		
	}
}
