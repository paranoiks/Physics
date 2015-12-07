using UnityEngine;
using System.Collections;

public class Debugger : MonoBehaviour {

    [SerializeField]
    private bool DebugMode;

	// Use this for initialization
	void Start () {
        ShowNodes();
	}

    private void ShowNodes()
    {
        var nodes = GameObject.FindGameObjectsWithTag("Node");
        foreach(var n in nodes)
        {
            n.GetComponent<MeshRenderer>().enabled = DebugMode;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        DrawNodes();
	}

    private void DrawNodes()
    {
        var nodes = (Node[])GameObject.FindObjectsOfType(typeof(Node));
        //var nodes = GameObject.FindGameObjectsWithTag("Node");
        foreach (var node in nodes)
        {
            foreach (var neighbour in node.NeighboursP)
            {
                if (neighbour != null)
                {
                    if (node.GetComponent<MeshRenderer>().enabled && neighbour.GetComponent<MeshRenderer>().enabled)
                    {
                        Debug.DrawLine(node.transform.position, neighbour.transform.position);
                    }
                }
            }
        }
    }
}
