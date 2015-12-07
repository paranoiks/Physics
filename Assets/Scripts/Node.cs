using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {
    
    [SerializeField]
    private List<GameObject> Neighbours;

    public List<GameObject> NeighboursP
    {
        get
        {
            return Neighbours;
        }
    }

	// Use this for initialization
	void Start () {
	
	}

    public void AddNeighbour(GameObject neighbour)
    {
        Neighbours.Add(neighbour);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
