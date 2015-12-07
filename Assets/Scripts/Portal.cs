using UnityEngine;

public class Portal : MonoBehaviour {

    [SerializeField]
    private GameObject MOtherPortal;
    
    public GameObject OtherPortal
    {
        get
        {
            return MOtherPortal;
        }
        set
        {
            MOtherPortal = value;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
